using System;
using UnityEngine;

public class NetManager : MonoBehaviour
{
	public const float kNeverSync = -1f;
	public const float kRetryNow = -1f;

	private static NetManager m_cInstance;
	private string m_OnlineUsername;
	private int m_LastCoinTransferCount;

	private LoginRequest m_LoginRequest;
	private CreateAccountRequest m_CreateAccountRequest;
	private CoinTransferRequest m_CoinTransferRequest;

	private float m_CoinTransferTimeStart;

	private BaseNetRequest m_CurrentRequest;
	private NetManager.PopupType m_currentPopupType = NetManager.PopupType.eNone;
	private MessagePopup[] m_NetPopup;
	private ActivityIndicatorPopup m_ActivityIndicatorPopup;

	public enum Request { eLogin, eCreateAccount, eCoinTransfer, eRequest_COUNT }
	public enum PopupType { eGeneric, eCount, eCreateAccount, eNone }

	public static NetManager Instance
	{
		get { return NetManager.m_cInstance; }
	}

	public bool IsNetPopupShowing
	{
		get { return (this.m_ActivityIndicatorPopup != null && this.m_ActivityIndicatorPopup.IsShowing) || (this.GetCurrentPopup() != null && this.GetCurrentPopup().IsShowing); }
	}

	private void Awake()
	{
		NetManager.m_cInstance = this;
		this.m_NetPopup = new MessagePopup[1];
	}

	private void Start()
	{
		NetError.CreateErrorCodeDictionary();
		this.m_CoinTransferRequest = new CoinTransferRequest();
	}

	private void Update()
	{
		if (this.m_CurrentRequest != null && this.m_CurrentRequest.InProgress)
		{
			this.m_CurrentRequest.Update();
		}
	}

	public void Draw()
	{
		if (this.GetCurrentPopup() != null)
		{
			this.GetCurrentPopup().Draw();
		}
		else
		{
			this.SetCurrentPopupType(NetManager.PopupType.eGeneric);
		}
		if (this.m_ActivityIndicatorPopup == null)
		{
			if (ResolutionManager.Instance.ResolutionInfoSet)
			{
				this.m_ActivityIndicatorPopup = new ActivityIndicatorPopup(base.gameObject);
			}
		}
		else
		{
			this.m_ActivityIndicatorPopup.Draw();
		}
	}

	public void ShowProgressing(bool aShow, bool aSilent)
	{
		if (aSilent)
		{
			if (aShow)
			{
			}
			return;
		}
		if (this.m_ActivityIndicatorPopup != null)
		{
			this.m_ActivityIndicatorPopup.Show(aShow);
		}
	}

	public void ShowError(string aErrorMsg, bool aSilent)
	{
		if (this.m_ActivityIndicatorPopup != null)
		{
			this.m_ActivityIndicatorPopup.Show(false);
		}
		if (aSilent)
		{
			return;
		}
		if (this.GetCurrentPopup() != null)
		{
			this.GetCurrentPopup().ShowText(aErrorMsg);
		}
	}

	public void ShowErrorTextId(string aTextId, bool aSilent)
	{
		if (this.m_ActivityIndicatorPopup != null)
		{
			this.m_ActivityIndicatorPopup.Show(false);
		}
		if (aSilent)
		{
			return;
		}
		if (this.GetCurrentPopup() != null)
		{
			this.GetCurrentPopup().ShowTextId(aTextId);
		}
	}

	public void HideError()
	{
		if (this.GetCurrentPopup() != null)
		{
			this.GetCurrentPopup().Show(false);
		}
	}

	private MessagePopup GetCurrentPopup()
	{
		if (this.m_currentPopupType < NetManager.PopupType.eCount)
		{
			return this.m_NetPopup[(int)this.m_currentPopupType];
		}
		return null;
	}

	public void SetCurrentPopupType(NetManager.PopupType ae_popupType)
	{
		if (this.m_currentPopupType == ae_popupType)
		{
			return;
		}
		if (this.GetCurrentPopup() != null)
		{
			this.m_NetPopup[(int)this.m_currentPopupType] = null;
		}
		this.m_currentPopupType = ae_popupType;
		if (ae_popupType == NetManager.PopupType.eGeneric)
		{
			this.m_NetPopup[(int)this.m_currentPopupType] = new MessagePopup(base.gameObject);
		}
	}

	public string GetAuthToken()
	{
		return ProfileManager.Instance.CurrentProfile.AuthToken;
	}

	public void UpdateAuthToken(string aAuthToken)
	{
		if (aAuthToken != null)
		{
			ProfileManager.Instance.CurrentProfile.AuthToken = aAuthToken;
			ProfileManager.Instance.SaveCurrentProfile();
		}
	}

	public void ResetAuthToken()
	{
		this.UpdateAuthToken(string.Empty);
	}

	public bool IsPlayerLoggedIn()
	{
		return ProfileManager.Instance.CurrentProfile.HasAuthToken();
	}

	public void Login(string aUserName, string aPassword, BaseNetRequest.RequestCompleteCB aCallback)
	{
		if (this.m_LoginRequest == null)
		{
			this.m_LoginRequest = new LoginRequest();
		}
		this.m_LoginRequest.FeedbackMode = BaseNetRequest.Feedback.eProgressingOnly;
		this.m_LoginRequest.RegisterRequestCompleteCB(aCallback);
		BaseNetRequest.Message message = this.m_LoginRequest.BuildRequestMessage(aUserName, aPassword);
		this.m_OnlineUsername = aUserName;
		base.StartCoroutine(this.m_LoginRequest.SendRequest(message));
		this.m_CurrentRequest = this.m_LoginRequest;
	}

	public void TransferCoins(int aNumCoins, BaseNetRequest.RequestCompleteCB aCallback, bool aSilentMode)
	{
		if (aSilentMode)
		{
			this.m_CoinTransferRequest.FeedbackMode = BaseNetRequest.Feedback.eSilent;
		}
		else
		{
			this.m_CoinTransferRequest.FeedbackMode = BaseNetRequest.Feedback.eVerbose;
		}
		this.m_CoinTransferRequest.RegisterRequestCompleteCB(aCallback);
		this.m_CoinTransferRequest.RegisterRequestCompleteCB(new BaseNetRequest.RequestCompleteCB(this.OnTransferCoinsComplete));
		BaseNetRequest.Message message = this.m_CoinTransferRequest.BuildRequestMessage(this.GetAuthToken(), aNumCoins);
		this.m_LastCoinTransferCount = aNumCoins;
		base.StartCoroutine(this.m_CoinTransferRequest.SendRequest(message));
		this.m_CurrentRequest = this.m_CoinTransferRequest;
		this.m_CoinTransferTimeStart = Time.realtimeSinceStartup;
	}

	public void OnTransferCoinsComplete(bool aSuccess)
	{
		if (aSuccess)
		{
			int num = -1;
			for (int i = 0; i < 60; i++)
			{
				if (!ProfileManager.Instance.CurrentProfile.m_LevelData[i].LevelComplete)
				{
					break;
				}
				num = i;
			}

			BizIntel.ContextualEvent contextualEvent = new BizIntel.ContextualEvent("coin-transfer");
			contextualEvent.AddContextItem("player-id", ProfileManager.Instance.CurrentProfile.ProfileName);
			contextualEvent.AddContextItem("coin-count", this.m_LastCoinTransferCount);
			contextualEvent.AddContextItem("elapsed-time-msec", (int)((Time.realtimeSinceStartup - this.m_CoinTransferTimeStart) * 1000f));
			contextualEvent.AddContextItem("highest-level", num);
			contextualEvent.AddContextItem("most-recent-level", ProfileManager.Instance.CurrentProfile.LastLevelPlayed);
			contextualEvent.Log();
		}
	}

	public void OnAccountCreationComplete(bool aSuccess)
	{
		if (aSuccess)
		{
			BizIntel.ContextualEvent contextualEvent = new BizIntel.ContextualEvent("create-account");
			contextualEvent.AddContextItem("player-id", this.m_OnlineUsername);
			contextualEvent.AddContextItem("profile-id", ProfileManager.Instance.CurrentProfile.ProfileName);
			contextualEvent.Log();
		}
	}

	public bool IsAnyRequestInProgess()
	{
		return (this.m_LoginRequest != null && this.m_LoginRequest.InProgress) || (this.m_CoinTransferRequest != null && this.m_CoinTransferRequest.InProgress);
	}

	public void CreateCPAccount(string aUserName, string aPassword, string aPasswordConfirm, string aEmail, int aColor, BaseNetRequest.RequestCompleteCB aCallback)
	{
		if (this.m_CreateAccountRequest == null)
		{
			this.m_CreateAccountRequest = new CreateAccountRequest();
		}
		if (aPassword == aPasswordConfirm)
		{
			this.SetCurrentPopupType(NetManager.PopupType.eCreateAccount);
			this.m_OnlineUsername = aUserName;
			this.m_CreateAccountRequest.FeedbackMode = BaseNetRequest.Feedback.eProgressingOnly;
			this.m_CreateAccountRequest.RegisterRequestCompleteCB(aCallback);
			this.m_CreateAccountRequest.RegisterRequestCompleteCB(new BaseNetRequest.RequestCompleteCB(this.OnAccountCreationComplete));
			BaseNetRequest.Message message = this.m_CreateAccountRequest.BuildRequestMessage(aUserName, aPassword, aEmail, aColor);
			base.StartCoroutine(this.m_CreateAccountRequest.SendRequest(message));
			this.m_CurrentRequest = this.m_CreateAccountRequest;
		}
		else
		{
			this.m_CreateAccountRequest.SetNetError((aPasswordConfirm != null && aPasswordConfirm.Length != 0) ? 6 : 5);
			if (aCallback != null)
			{
				aCallback(false);
			}
		}
	}

	public bool HasCoinTransferError()
	{
		return this.m_CoinTransferRequest != null && this.m_CoinTransferRequest.LastErrorCode != 0;
	}

	public bool HasReachedCoinTransferLimitError()
	{
		return this.m_CoinTransferRequest != null && this.m_CoinTransferRequest.LastErrorCode == -32401;
	}

	public bool HasCoinTransferCompleted()
	{
		return this.m_CoinTransferRequest.LastResult != null;
	}

	public int GetLastErrorCode(NetManager.Request aRequest)
	{
		switch (aRequest)
		{
			case NetManager.Request.eLogin:
				if (this.m_LoginRequest != null)
				{
					return this.m_LoginRequest.LastErrorCode;
				}
				break;
			case NetManager.Request.eCreateAccount:
				if (this.m_CreateAccountRequest != null)
				{
					return this.m_CreateAccountRequest.LastErrorCode;
				}
				break;
			case NetManager.Request.eCoinTransfer:
				if (this.m_CoinTransferRequest != null)
				{
					return this.m_CoinTransferRequest.LastErrorCode;
				}
				break;
		}

		return 0;
	}

	public string GetLastErrorMsg(NetManager.Request aRequest)
	{
		switch (aRequest)
		{
			case NetManager.Request.eLogin:
				if (this.m_LoginRequest != null)
				{
					return this.m_LoginRequest.LastErrorMsg;
				}
				break;
			case NetManager.Request.eCreateAccount:
				if (this.m_CreateAccountRequest != null)
				{
					return this.m_CreateAccountRequest.LastErrorMsg;
				}
				break;
			case NetManager.Request.eCoinTransfer:
				if (this.m_CoinTransferRequest != null)
				{
					return this.m_CoinTransferRequest.LastErrorMsg;
				}
				break;
		}

		return string.Empty;
	}
}
