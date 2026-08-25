using System;
using UnityEngine;

public class LevelButtonController : MonoBehaviour
{
	private void Start()
	{
		this.m_ezButton = base.GetComponent<BHUIButton>();
		this.Initialize();
		this.SetButtonElements();
	}

	private void Update()
	{
		if (!this.isPositionInitialized)
		{
			this.SetPosition();
			this.SetText();
			this.isPositionInitialized = true;
		}
	}

	private void Initialize()
	{
		if (this.mLevelNumberColor == null)
		{
			this.mLevelNumberColor = this.mo_LevelNumber.GetComponent<AutoAdjustSpriteText>();
		}
		if (GameManager.Instance.CurrentWorld == GameManager.World.eWorld_BlueSky)
		{
			this.mLevelNumberColor.m_SpriteTextColor = AutoAdjustSpriteText.SpriteTextColor.eBlue;
			this.materialName = "PadlockBlue";
		}
		else if (GameManager.Instance.CurrentWorld == GameManager.World.eWorld_SodaSunset)
		{
			this.mLevelNumberColor.m_SpriteTextColor = AutoAdjustSpriteText.SpriteTextColor.eOrange;
			this.materialName = "PadlockRed";
		}
		else
		{
			this.mLevelNumberColor.m_SpriteTextColor = AutoAdjustSpriteText.SpriteTextColor.eOrange;
			this.materialName = "PadlockYellow";
		}
		this.mLevelNumberColor.AutoAdjust();
		this.ChangeMaterial(this.mo_PadlockTexture, this.materialName);
	}

	private void SetButtonElements()
	{
		if (this.ShouldHighlight())
		{
			switch (GameManager.Instance.CurrentWorld)
			{
			default:
				this.buttonMaterialName = "LevelSelectButtonBlueHighlight";
				break;
			case GameManager.World.eWorld_SodaSunset:
				this.buttonMaterialName = "LevelSelectButtonRedHighlight";
				break;
			case GameManager.World.eWorld_BonusWorld:
				this.buttonMaterialName = "LevelSelectButtonYellowHighlight";
				break;
			}
			this.mo_PadlockTexture.GetComponent<Renderer>().enabled = false;
			this.mo_EmptyPuffleOTexture.GetComponent<Renderer>().enabled = false;
			this.mo_ClockTexture.GetComponent<Renderer>().enabled = false;
			this.mo_NewText.GetComponent<Renderer>().enabled = false;
			this.mo_Time.GetComponent<Renderer>().enabled = false;
			this.mo_PuffleORankTexture.GetComponent<Renderer>().enabled = true;
			this.mo_PuffleOCount.GetComponent<Renderer>().enabled = true;
			this.materialName = "PuffleORankFire";
			if (!ProfileManager.Instance.CurrentProfile.m_LevelData[this.mCurrentButtonID - 1].TurboLevelComplete)
			{
				float num = (float)ProfileManager.Instance.CurrentProfile.m_LevelData[this.mCurrentButtonID - 1].BestRingCount / (float)GameManager.smMaxRingInLevel[this.mCurrentButtonID - 1];
				if (num >= 1f)
				{
					this.materialName = "PuffleORankGold";
				}
				else if (num >= 0.5f)
				{
					this.materialName = "PuffleORankSilver";
				}
				else
				{
					this.materialName = "PuffleORankBronze";
				}
			}
			this.ChangeMaterial(this.mo_PuffleORankTexture, this.materialName);
			if (GameManager.HasCollectedAllRings(GameManager.Instance.CurrentWorld) && ProfileManager.Instance.CurrentProfile.m_LevelData[this.mCurrentButtonID - 1].BestTimeCount != float.MaxValue && GameManager.Instance.CurrentWorld != GameManager.World.eWorld_BonusWorld)
			{
				this.mo_ClockTexture.GetComponent<Renderer>().enabled = true;
				this.mo_Time.GetComponent<Renderer>().enabled = true;
				this.timeStr = GameManager.GetTimeFormatedString(ProfileManager.Instance.CurrentProfile.m_LevelData[this.mCurrentButtonID - 1].BestTimeCount).ToString();
			}
		}
		else
		{
			switch (GameManager.Instance.CurrentWorld)
			{
			default:
				this.buttonMaterialName = "LevelSelectButtonBlue";
				break;
			case GameManager.World.eWorld_SodaSunset:
				this.buttonMaterialName = "LevelSelectButtonRed";
				break;
			case GameManager.World.eWorld_BonusWorld:
				this.buttonMaterialName = "LevelSelectButtonYellow";
				break;
			}
			if (this.IsLevelUnlocked())
			{
				this.mo_PadlockTexture.GetComponent<Renderer>().enabled = false;
				this.mo_PuffleORankTexture.GetComponent<Renderer>().enabled = false;
				this.mo_ClockTexture.GetComponent<Renderer>().enabled = false;
				this.mo_PuffleOCount.GetComponent<Renderer>().enabled = false;
				this.mo_EmptyPuffleOTexture.GetComponent<Renderer>().enabled = true;
				this.mo_NewText.GetComponent<Renderer>().enabled = true;
				this.mo_Time.GetComponent<Renderer>().enabled = false;
			}
			else
			{
				this.mo_EmptyPuffleOTexture.GetComponent<Renderer>().enabled = false;
				this.mo_PuffleORankTexture.GetComponent<Renderer>().enabled = false;
				this.mo_ClockTexture.GetComponent<Renderer>().enabled = false;
				this.mo_NewText.GetComponent<Renderer>().enabled = false;
				this.mo_PuffleOCount.GetComponent<Renderer>().enabled = false;
				this.mo_Time.GetComponent<Renderer>().enabled = false;
				this.mo_PadlockTexture.GetComponent<Renderer>().enabled = true;
			}
		}
		this.ChangeMaterial(base.gameObject, this.buttonMaterialName);
	}

	public void LoadLevel()
	{
		if (this.IsLevelUnlocked())
		{
			AssetLoader.Instance.ScrollList.SetActive(false);
			if (this.mCurrentButtonID == 1 && CinematicManager.Instance != null)
			{
				LevelSelect.Instance.MainScreen.StopGUI();
				CinematicManager.Instance.ShowFullscreenBgWhenPlaying = true;
				CinematicManager.Instance.playCompleted += this.MoviePlayCompleted;
				CinematicManager.Instance.Play(CinematicManager.MovieId.eIntro);
			}
			else
			{
				this.StartSelectedLevel();
			}
		}
	}

	private void MoviePlayCompleted(bool aSuccess)
	{
		AudioManager.Instance.ResetMute();
		Resources.UnloadUnusedAssets();
		this.StartSelectedLevel();
	}

	private void StartSelectedLevel()
	{
		LevelSelect.Instance.MainScreen.StopGUI();
		LevelSelect.SelectedLevel = this.mCurrentButtonID;
		GameManager.Instance.StartLevel((GameManager.Level)(this.mCurrentButtonID - 1));
		GameFlowManager.Instance.LoadScene("Gameplay", true);
	}

	private void SetText()
	{
		if (base.transform.Find("LevelNumber") != null)
		{
			this.mo_LevelNumber.GetComponent<BHUILabel>().Text = this.mCurrentButtonID.ToString();
			this.mo_LevelNumber.GetComponent<BHUILabel>().UpdateDropShadow();
		}
		string text = string.Format("{0}/{1}", ProfileManager.Instance.CurrentProfile.m_LevelData[this.mCurrentButtonID - 1].BestRingCount, GameManager.smMaxRingInLevel[this.mCurrentButtonID - 1]);
		if (base.transform.Find("PuffleOCount") != null)
		{
			this.mo_PuffleOCount.GetComponent<BHUILabel>().Text = text;
			this.mo_PuffleOCount.GetComponent<BHUILabel>().UpdateDropShadow();
		}
		if (base.transform.Find("Time") != null && this.timeStr != null)
		{
			this.mo_Time.GetComponent<BHUILabel>().Text = this.timeStr;
			this.mo_Time.GetComponent<BHUILabel>().UpdateDropShadow();
		}
	}

	private void SetPosition()
	{
		if (this.mo_ClockTexture != null)
		{
			this.mo_ClockTexture.transform.localPosition = new Vector3(-0.15f * this.m_ezButton.width, -0.187f * this.m_ezButton.height, -1f);
		}
		if (this.mo_PuffleORankTexture != null)
		{
			this.mo_PuffleORankTexture.transform.localPosition = new Vector3(0f, 0.15f * this.m_ezButton.height, -1f);
		}
		if (this.mo_EmptyPuffleOTexture != null)
		{
			this.mo_EmptyPuffleOTexture.transform.localPosition = new Vector3(0f, 0.09f * this.m_ezButton.height, -1f);
		}
		if (this.mo_PadlockTexture != null)
		{
			this.mo_PadlockTexture.transform.localPosition = new Vector3(0f, 0f, -1f);
		}
		if (this.mo_LevelNumber != null)
		{
			this.mo_LevelNumber.transform.localPosition = new Vector3(-0.29f * this.m_ezButton.width, 0.32f * this.m_ezButton.height, -0.1f);
		}
		if (this.mo_NewText != null)
		{
			this.mo_NewText.transform.localPosition = new Vector3(0f, -0.15f * this.m_ezButton.height, -0.5f);
			if (LocalizationManager.GetLanguageCode() == "fr" && ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
			{
				this.mo_NewText.GetComponent<BHUILabel>().pixelPerfect = false;
				this.mo_NewText.GetComponent<BHUILabel>().SetCharacterSize(0.84f);
				this.mo_NewText.GetComponent<BHUILabel>().UpdateDropShadow();
			}
		}
		if (this.mo_PuffleOCount != null)
		{
			this.mo_PuffleOCount.transform.localPosition = new Vector3(0f, -0.033f * this.m_ezButton.height, -0.5f);
			if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eLowres)
			{
				this.mo_PuffleOCount.GetComponent<BHUILabel>().pixelPerfect = false;
				this.mo_PuffleOCount.GetComponent<BHUILabel>().SetCharacterSize(1f);
				this.mo_PuffleOCount.GetComponent<BHUILabel>().UpdateDropShadow();
			}
		}
		if (this.mo_Time != null)
		{
			this.mo_Time.transform.localPosition = new Vector3(-0.042f * this.m_ezButton.width, -0.123f * this.m_ezButton.height, -0.5f);
		}
	}

	private bool ShouldHighlight()
	{
		return ProfileManager.Instance.CurrentProfile.m_LevelData[this.mCurrentButtonID - 1].LevelComplete;
	}

	private bool IsLevelUnlocked()
	{
		return ProfileManager.Instance.CurrentProfile.m_LevelData[this.mCurrentButtonID - 1].LevelUnlocked;
	}

	public void ChangeList()
	{
		this.mCurrentButtonID = this.buttonID + 12 * (int)GameManager.Instance.CurrentWorld;
		this.Initialize();
		this.SetButtonElements();
		if (this.isPositionInitialized)
		{
			this.SetText();
		}
	}

	private void ChangeMaterial(GameObject aGameObject, string aMaterialName)
	{
		aGameObject.GetComponent<MeshRenderer>().material = Resources.Load("EZGUI/LevelSelect/" + aMaterialName, typeof(Material)) as Material;
		ResourceLoader.Instance.SetMaterialTexture(aGameObject, "EZGUI/LevelSelect/", false, out this.m_AssetSizeCategoryId, out this.m_AssetLanguage);
	}

	protected SizeCategory.CategoryId m_AssetSizeCategoryId = SizeCategory.CategoryId.eUnknown;

	protected LocalizationManager.Language m_AssetLanguage;

	public int buttonID;

	public GameObject mo_EmptyPuffleOTexture;

	public GameObject mo_PuffleORankTexture;

	public GameObject mo_ClockTexture;

	public GameObject mo_PadlockTexture;

	public GameObject mo_LevelNumber;

	public GameObject mo_NewText;

	public GameObject mo_PuffleOCount;

	public GameObject mo_Time;

	private BHUIButton m_ezButton;

	private int mCurrentButtonID;

	private string timeStr;

	private AutoAdjustSpriteText mLevelNumberColor;

	private string materialName = string.Empty;

	private string buttonMaterialName = string.Empty;

	private bool isPositionInitialized;

	public enum State
	{
		eLock,
		eUnlock,
		eCompleted,
		eState_COUNT
	}
}
