using System;
using System.Collections;
using System.Collections.Generic;
using Procurios.Public;
using UnityEngine;

public abstract class BaseNetRequest
{
	public BaseNetRequest()
	{
		this.m_RequestCompleteCB = new List<BaseNetRequest.RequestCompleteCB>();
		this.Init();
		this.CreateRequiredResultKeyList();
	}

	public bool InProgress
	{
		get { return this.m_InProgress; }
	}

	public BaseNetRequest.Feedback FeedbackMode
	{
		get { return this.m_FeedbackMode; }
		set { this.m_FeedbackMode = value; }
	}

	public bool SilentProgressingFeedback
	{
		get { return this.m_FeedbackMode == BaseNetRequest.Feedback.eSilent || this.m_FeedbackMode == BaseNetRequest.Feedback.eErrorOnly; }
	}

	public bool SilentErrorFeedback
	{
		get { return this.m_FeedbackMode == BaseNetRequest.Feedback.eSilent || this.m_FeedbackMode == BaseNetRequest.Feedback.eProgressingOnly; }
	}

	public Hashtable LastResult
	{
		get { return this.m_LastResult; }
	}

	public int LastErrorCode
	{
		get { return this.m_LastErrorCode; }
	}

	public string LastErrorMsg
	{
		get { return this.m_LastErrorMsg; }
	}

	protected virtual void Init() {}

	protected abstract void CreateRequiredResultKeyList();

	protected abstract void OnFail(Hashtable aResult);

	protected abstract void OnSuccess(Hashtable aResult);

	public virtual void Update()
	{
		if (this.m_InProgress && !this.m_IsRequestTimeOut && Time.realtimeSinceStartup - this.m_RequestStartTime >= 30f)
		{
			this.m_IsRequestTimeOut = true;
			this.CancelRequest();
		}
	}

	public virtual IEnumerator SendRequest(BaseNetRequest.Message aMessage)
	{
		this.m_LastResult = null;
		this.m_LastErrorCode = 0;
		this.m_LastErrorMsg = string.Empty;
		this.m_RequestMssage = aMessage;
		NetManager.Instance.ShowProgressing(true, this.SilentProgressingFeedback);
		this.m_InProgress = true;
		this.m_RequestStartTime = Time.realtimeSinceStartup;
		this.m_IsRequestTimeOut = false;
		this.m_IsRequestCancelled = false;
		this.m_WWW = aMessage.CreateConnection();
		yield return this.m_WWW;
		bool handleServerResult = this.m_WWW != null && this.m_WWW.isDone && !this.m_IsRequestCancelled;
		this.RequestDone(handleServerResult);
		yield break;
	}

	public virtual void CancelRequest()
	{
		if (!this.m_IsRequestCancelled && this.m_WWW != null && !this.m_WWW.isDone)
		{
			this.m_IsRequestCancelled = true;
			this.m_WWW.Dispose();
			bool isRequestTimeOut = this.m_IsRequestTimeOut;
			this.RequestDone(isRequestTimeOut);
		}
	}

	protected virtual void RequestDone(bool aHandleServerResult)
	{
		if (aHandleServerResult)
		{
			this.HandleServerResult(this.m_WWW);
		}
		this.m_InProgress = false;
		this.m_WWW = null;
	}

	protected void HandleServerResult(WWW aWww)
	{
		Hashtable serverResult = this.GetServerResult(aWww);
		this.m_LastResult = serverResult;
		bool flag;
		if (this.HandleError(serverResult))
		{
			flag = false;
			this.OnFail(serverResult);
		}
		else
		{
			NetManager.Instance.ShowProgressing(false, this.SilentProgressingFeedback);
			flag = true;
			this.OnSuccess(serverResult);
		}
		this.ExecuteRequestCompleteCBs(flag);
	}

	protected virtual Hashtable GetServerResult(WWW aWww)
	{
		Hashtable hashtable;
		if (this.m_IsRequestTimeOut)
		{
			hashtable = new Hashtable();
			hashtable["clientError"] = NetError.ClientError.eServerTimeOut;
		}
		else if (aWww == null)
		{
			hashtable = new Hashtable();
			hashtable["clientError"] = NetError.ClientError.eUnknown;
		}
		else if (aWww.error != null)
		{
			hashtable = new Hashtable();
			hashtable["clientError"] = NetError.ClientError.eFailToConnectToServer;
		}
		else
		{
			hashtable = JSON.JsonDecode(aWww.text) as Hashtable;
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				hashtable["clientError"] = NetError.ClientError.eServerError;
			}
		}
		return hashtable;
	}

	protected virtual bool HandleError(Hashtable aResult)
	{
		if (aResult.Contains("clientError"))
		{
			this.ShowNetError((int)aResult["clientError"]);
			return true;
		}
		if (!aResult.Contains("success"))
		{
			this.ShowNetError(4, "success");
			return true;
		}
		if ((bool)aResult["success"])
		{
			foreach (string text in this.m_RequiredResultKeys)
			{
				if (!aResult.Contains(text))
				{
					this.ShowNetError(4, text);
					return true;
				}
			}
			return false;
		}
		if (!aResult.Contains("errorCode"))
		{
			this.ShowNetError(4, "errorCode");
			return true;
		}
		int serverErrorCode = this.GetServerErrorCode(aResult);
		if (!this.HandleSpecialServerError(serverErrorCode, aResult))
		{
			this.ShowNetError(serverErrorCode);
		}
		return true;
	}

	private bool HandleSpecialServerError(int aErrorCode, Hashtable aResult)
	{
		bool flag = false;
		string text = string.Empty;
		if (aErrorCode != -32299)
		{
			if (aErrorCode != -32276)
			{
				if (aErrorCode == -32102 || aErrorCode == -32076)
				{
					NetManager.Instance.ResetAuthToken();
				}
			}
			else
			{
				if (aResult.Contains("userSuggestion"))
				{
					text = LocalizationManager.Instance.GetString(NetError.GetErrorMsgTextId(aErrorCode), aResult["userSuggestion"]);
					this.ShowNetError(aErrorCode, text, string.Empty);
				}
				else
				{
					text = LocalizationManager.Instance.GetString("TXT_UserNameTaken1");
					this.ShowNetError(aErrorCode, text, string.Empty);
				}
				flag = true;
			}
		}
		else
		{
			string empty = string.Empty;
			if (this.m_RequestMssage != null && this.m_RequestMssage.m_ParamterDict.TryGetValue("email", out empty))
			{
				string text2 = string.Empty;
				if (empty != null)
				{
					int num = empty.IndexOf('@');
					text2 = empty.Substring(num + 1);
					text = LocalizationManager.Instance.GetString(NetError.GetErrorMsgTextId(aErrorCode), text2);
					this.ShowNetError(aErrorCode, text, string.Empty);
					flag = true;
				}
			}
		}
		return flag;
	}

	public int GetErrorCode(Hashtable aResult)
	{
		if (aResult != null && aResult.Contains("clientError"))
		{
			NetError.ClientError clientError = (NetError.ClientError)((int)aResult["clientError"]);
			if (clientError != NetError.ClientError.eNone)
			{
				return (int)clientError;
			}
		}
		return this.GetServerErrorCode(aResult);
	}

	public int GetServerErrorCode(Hashtable aResult)
	{
		if (aResult != null && aResult.Contains("errorCode"))
		{
			return (int)((double)aResult["errorCode"]);
		}
		return 0;
	}

	private void ShowNetError(int aErrorCode)
	{
		this.ShowNetError(aErrorCode, NetError.GetErrorMsg(aErrorCode), string.Empty);
	}

	private void ShowNetError(int aErrorCode, string aExtraErrorInfo)
	{
		this.ShowNetError(aErrorCode, NetError.GetErrorMsg(aErrorCode), aExtraErrorInfo);
	}

	private void ShowNetError(int aErrorCode, string aErrorMsg, string aExtraErrorInfo)
	{
		this.SetNetError(aErrorCode, aErrorMsg + aExtraErrorInfo);
		NetManager.Instance.ShowError(this.m_LastErrorMsg, this.SilentErrorFeedback);
	}

	public void SetNetError(int aErrorCode)
	{
		this.SetNetError(aErrorCode, NetError.GetErrorMsg(aErrorCode));
	}

	public void SetNetError(int aErrorCode, string aErrorMsg)
	{
		this.m_LastErrorCode = aErrorCode;
		this.m_LastErrorMsg = aErrorMsg;
	}

	public virtual void RegisterRequestCompleteCB(BaseNetRequest.RequestCompleteCB aCallback)
	{
		this.m_RequestCompleteCB.Add(aCallback);
	}

	public virtual void UnRegisterAllRequestCompleteCBs()
	{
		this.m_RequestCompleteCB.Clear();
	}

	public virtual void ExecuteRequestCompleteCBs(bool aSuccess)
	{
		foreach (BaseNetRequest.RequestCompleteCB requestCompleteCB in this.m_RequestCompleteCB)
		{
			requestCompleteCB(aSuccess);
		}
		this.UnRegisterAllRequestCompleteCBs();
	}

	protected BaseNetRequest.Message m_RequestMssage;

	protected List<string> m_RequiredResultKeys = new List<string>();

	protected List<BaseNetRequest.RequestCompleteCB> m_RequestCompleteCB;

	protected bool m_InProgress;

	protected BaseNetRequest.Feedback m_FeedbackMode;

	protected WWW m_WWW;

	protected float m_RequestStartTime;

	protected bool m_IsRequestTimeOut;

	protected bool m_IsRequestCancelled;

	protected Hashtable m_LastResult;

	protected int m_LastErrorCode;

	protected string m_LastErrorMsg = string.Empty;

	public enum Separator
	{
		eNone,
		ePrefix,
		eSuffix,
		eSeparator_COUNT
	}

	public enum Feedback
	{
		eVerbose,
		eProgressingOnly,
		eErrorOnly,
		eSilent,
		eFeedbackMode_COUNT
	}

	public class Message
	{
		public Message(string aQueryString)
		{
			this.m_BaseURL = NetConstants.kHost + aQueryString;
			this.m_Parameters = new WWWForm();
			this.m_ParamterDict.Clear();
		}

		public void AddParameter(string aKey, string aValue)
		{
			string text = ((aValue == null) ? string.Empty : aValue);
			this.m_Parameters.AddField(aKey, text);
			this.m_ParamterDict.Add(aKey, aValue);
		}

		public WWW CreateConnection()
		{
			Hashtable headers = new Hashtable(this.m_Parameters.headers);
			headers["Content-Type"] = (string)headers["Content-Type"] + "; charset=UTF-8";
			byte[] data = this.m_Parameters.data;
			return new WWW(this.m_BaseURL, data, headers);
		}

		private string m_BaseURL;

		private WWWForm m_Parameters;

		public Dictionary<string, string> m_ParamterDict = new Dictionary<string, string>();
	}

	public delegate void RequestCompleteCB(bool aSuccess);
}
