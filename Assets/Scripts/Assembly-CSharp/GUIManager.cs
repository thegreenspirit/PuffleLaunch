using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
	public bool IsPauseMenu
	{
		get { return this.m_IsPauseMenu; }
	}

	public GUISkin Skin
	{
		get
		{
			Utilities.AssertMsg(this.m_Skin != null, "Missing GUI Skin in GUIManager!");
			return this.m_Skin;
		}
	}

	public HudManager HudManager
	{
		get { return this.m_HudManager; }
	}

	public LoadingScreen LoadingScreen
	{
		get { return this.m_LoadingScreen; }
	}

	public GUIManager.Scene CurrentScene
	{
		get { return this.m_CurrentScene; }
		set { this.m_CurrentScene = value; }
	}

	public string CurrentSceneName
	{
		get { return this.m_CurrentSceneName; }
	}

	public Color WhiteDropShadowColor
	{
		get { return this.m_WhiteTextDropShadowColor; }
	}

	public Color DarkBrownDropShadowColor
	{
		get { return this.m_DarkBrownTextDropShadowColor; }
	}

	public Color DarkerBrownDropShadowColor
	{
		get { return this.m_DarkerBrownTextDropShadowColor; }
	}

	public Color GreyBrownDropShadowColor
	{
		get { return this.m_GreyBrownTextDropShadowColor; }
	}

	public Color LightBrownDropShadowColor
	{
		get { return this.m_LightBrownTextDropShadowColor; }
	}

	public int DropShadowOffsetX
	{
		get { return this.m_DropShadowOffsetX; }
	}

	public int DropShadowOffsetY
	{
		get { return this.m_DropShadowOffsetY; }
	}

	public bool IsLoginPopupShowing
	{
		get
		{
			return this.m_LoginPopup != null && this.m_LoginPopup.IsShowing;
		}
	}

	public bool IsUpsellPopupShowing
	{
		get
		{
			return this.m_UpsellPopup != null && this.m_UpsellPopup.IsShowing;
		}
	}

	public bool IsAppQuitPopupShowing
	{
		get
		{
			return this.m_AppQuitPopup != null && this.m_AppQuitPopup.IsShowing;
		}
	}

	public bool IsRateMyAppPopupShowing
	{
		get
		{
			return this.m_RateMyAppPopup != null && this.m_RateMyAppPopup.IsShowing;
		}
	}

	public bool IsCreateAccountPopupShowing
	{
		get
		{
			return this.mo_createAccountPopup != null && this.mo_createAccountPopup.IsShowing;
		}
	}

	public int AboutCPCurrentPage
	{
		get
		{
			return this.m_AboutCPCurrentPage;
		}
	}

	public bool EnableAutoResize
	{
		get
		{
			return this.m_EnableAutoResize;
		}
	}

	private void Awake()
	{
		global::UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		this.LanguageInitialization();
		this.SetGlobalFont();
		this.SetStyleFont();
		this.SetDropShadowOffset();
		this.m_LoadingScreen = new LoadingScreen(base.gameObject);
		this.m_HudManager = new HudManager(base.gameObject);
		GUIStyleContainer.Init();
		if (LocalizationManager.GetLanguageCode() == "de")
		{
			this.m_EnableAutoResize = true;
		}
		this.m_NewResMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.width / GUIConstants.kReferenceScreenWidth, (float)Screen.height / GUIConstants.kReferenceScreenHeight, 1f));
	}

	private void Update()
	{
		if (this.m_UpdateLoadingScreen)
		{
			this.m_LoadingScreen.Update();
		}
		else if (this.CurrentScene == GUIManager.Scene.eCreateAccountPopup)
		{
			if (this.mo_createAccountPopup != null)
			{
				this.mo_createAccountPopup.Update();
			}
		}
		else if (this.CurrentScene == GUIManager.Scene.eLoginPopup && this.m_LoginPopup != null)
		{
			this.m_LoginPopup.Update();
		}
		if (this.CurrentScene == GUIManager.Scene.eInGameHud || this.CurrentScene == GUIManager.Scene.ePauseMenu || this.CurrentScene == GUIManager.Scene.eUnlockPopup || this.CurrentScene == GUIManager.Scene.eTallyMenu)
		{
			this.m_HudManager.Update();
		}
	}

	private void OnGUI()
	{
		GUI.matrix = this.m_NewResMatrix;
		GUI.depth = 0;

		if (this.m_UpdateLoadingScreen)
		{
			this.m_LoadingScreen.Draw();
		}

		switch (this.CurrentScene)
		{
			case GUIManager.Scene.eInGameHud:
				if (StartOfGameDelay.Instance == null) {}
				break;
			case GUIManager.Scene.ePauseMenu:
				this.m_PauseMenu.Draw();
				break;
			case GUIManager.Scene.eTallyMenu:
				this.m_TallyMenu.Draw();
				break;
			case GUIManager.Scene.eUnlockPopup:
				this.mo_unlockPopup.Draw();
				break;
			case GUIManager.Scene.eLoginPopup:
				this.m_LoginPopup.Draw();
				break;
			case GUIManager.Scene.eCreateAccountPopup:
				if (this.mo_createAccountPopup != null)
				{
					this.mo_createAccountPopup.Draw();
				}
				break;
			case GUIManager.Scene.eUpsellPopup:
				this.m_UpsellPopup.Draw();
				break;
			case GUIManager.Scene.eAppQuitPopup:
				this.m_AppQuitPopup.Draw();
				break;
			case GUIManager.Scene.eRateMyApp:
				this.m_RateMyAppPopup.Draw();
				break;
		}
		if (NetManager.Instance)
		{
			NetManager.Instance.Draw();
		}
	}

	private void LanguageInitialization()
	{
		if (LocalizationManager.IsJapanese)
		{
			GUIConstants.kFontNames = GUIConstants.kFontNamesJA;
		}
		if (LocalizationManager.IsGerman || LocalizationManager.IsJapanese)
		{
			this.m_EnableAutoResize = true;
		}
	}

	private void SetGlobalFont()
	{
		this.m_InGameFonts = new Font[4];
		this.m_CPFonts = new Font[4];
		if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres)
		{
			for (int i = 0; i < 4; i++)
			{
				string text = string.Format("{0}{1}_{2}_{3}", new object[]
				{
					"Font/",
					"LowRes",
					GUIConstants.kFontNames[0],
					GUIConstants.kLowResFontSizes[i].ToString()
				});
				this.m_CPFonts[i] = Resources.Load(text, typeof(Font)) as Font;
				Utilities.AssertMsg(this.m_CPFonts[i] != null, "Fail to load Font: " + text);
			}
			for (int j = 0; j < 4; j++)
			{
				string text2 = string.Format("{0}{1}_{2}_{3}", new object[]
				{
					"Font/",
					"LowRes",
					GUIConstants.kFontNames[1],
					GUIConstants.kLowResFontSizes[j].ToString()
				});
				this.m_InGameFonts[j] = Resources.Load(text2, typeof(Font)) as Font;
				Utilities.AssertMsg(this.m_InGameFonts[j] != null, "Fail to load Font: " + text2);
			}
		}
		else
		{
			for (int k = 0; k < 4; k++)
			{
				string text3 = string.Format("{0}{1}_{2}", "Font/", GUIConstants.kFontNames[0], GUIConstants.kFontSizes[k].ToString());
				this.m_CPFonts[k] = Resources.Load(text3, typeof(Font)) as Font;
				Utilities.AssertMsg(this.m_CPFonts[k] != null, "Fail to load Font: " + text3);
			}
			for (int l = 0; l < 4; l++)
			{
				string text4 = string.Format("{0}{1}_{2}", "Font/", GUIConstants.kFontNames[1], GUIConstants.kFontSizes[l].ToString());
				this.m_InGameFonts[l] = Resources.Load(text4, typeof(Font)) as Font;
				Utilities.AssertMsg(this.m_InGameFonts[l] != null, "Fail to load Font: " + text4);
			}
		}
	}

	private void SetStyleFont()
	{
		if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres)
		{
			this.m_Skin.font = this.GetLowResFont(this.m_Skin.font);
			this.m_Skin.label.font = this.GetLowResFont(this.m_Skin.label.font);
			this.m_Skin.textField.font = this.GetLowResFont(this.m_Skin.label.font);
			this.m_Skin.textArea.font = this.GetLowResFont(this.m_Skin.label.font);
		}
		else
		{
			this.m_Skin.font = this.GetOriginalFont(this.m_Skin.font);
			this.m_Skin.label.font = this.GetOriginalFont(this.m_Skin.label.font);
			this.m_Skin.textField.font = this.GetOriginalFont(this.m_Skin.label.font);
			this.m_Skin.textArea.font = this.GetOriginalFont(this.m_Skin.label.font);
		}
	}

	private Font GetLowResFont(Font aFont)
	{
		string[] array = aFont.name.Split(new char[] { '_' });
		if (array.Length != 3 || !(array[0] == "LowRes"))
		{
			int num = int.Parse(array[1]);
			for (int i = 0; i < 4; i++)
			{
				if (num == GUIConstants.kFontSizes[i])
				{
					return this.m_CPFonts[i];
				}
			}
		}
		return aFont;
	}

	private Font GetOriginalFont(Font aFont)
	{
		string[] array = aFont.name.Split(new char[] { '_' });
		if (array.Length == 3)
		{
			int num = int.Parse(array[2]);
			for (int i = 0; i < 4; i++)
			{
				if (num == GUIConstants.kLowResFontSizes[i])
				{
					return this.m_CPFonts[i];
				}
			}
		}
		return aFont;
	}

	public Font GetFont(GUIDefines.FontSize aFontSize, GUIDefines.FontType aFontType)
	{
		if (aFontType == GUIDefines.FontType.eCPMenus)
		{
			return this.m_CPFonts[(int)aFontSize];
		}
		if (aFontType != GUIDefines.FontType.eInGame)
		{
			return this.m_CPFonts[(int)aFontSize];
		}
		return this.m_InGameFonts[(int)aFontSize];
	}

	public Font GetOnDemandFont(string aOnDemandFontName)
	{
		Font font;
		if (!this.m_OnDemandFonts.TryGetValue(aOnDemandFontName, out font))
		{
			font = Resources.Load("Font/" + aOnDemandFontName, typeof(Font)) as Font;
			if (Utilities.AssertMsg(font != null, "Fail to load on demand font: " + aOnDemandFontName))
			{
				this.m_OnDemandFonts.Add(aOnDemandFontName, font);
			}
		}
		return font;
	}

	public string GetLowResFontName(GUIDefines.FontType aFontType, GUIDefines.FontSize aFontSize)
	{
		if (aFontType < GUIDefines.FontType.eCPMenus || aFontType >= (GUIDefines.FontType)GUIConstants.kFontNames.Length || aFontSize < GUIDefines.FontSize.eMini || aFontSize >= (GUIDefines.FontSize)GUIConstants.kLowResFontSizes.Length)
		{
			Utilities.AssertMsg(false, string.Concat(new object[] { "Fail to Get lowres font name for type: ", aFontType, ", and size: ", aFontSize }));
			return string.Empty;
		}
		return string.Format("{0}_{1}_{2}", "LowRes", GUIConstants.kFontNames[(int)aFontType], GUIConstants.kLowResFontSizes[(int)aFontSize].ToString());
	}

	private void SetDropShadowOffset()
	{
		switch (ResolutionManager.Instance.LayoutSize)
		{
			case ResolutionManager.eLayoutSize.eLowres:
				this.m_DropShadowOffsetX = (int)this.m_DropShadowOffsetLowRes.x;
				this.m_DropShadowOffsetY = (int)this.m_DropShadowOffsetLowRes.y;
				break;
			case ResolutionManager.eLayoutSize.eOriginal:
				this.m_DropShadowOffsetX = (int)this.m_DropShadowOffsetOriginal.x;
				this.m_DropShadowOffsetY = (int)this.m_DropShadowOffsetOriginal.y;
				break;
			case ResolutionManager.eLayoutSize.eIPad:
				this.m_DropShadowOffsetX = (int)this.m_DropShadowOffsetIPad.x;
				this.m_DropShadowOffsetY = (int)this.m_DropShadowOffsetIPad.y;
				break;
		}
	}

	public void ChangeCurrentScene(string aSceneName)
	{
		this.CleanUp();
		this.m_HudManager.CleanUp();
		GUIUtil.CleanUp();
		switch (aSceneName)
		{
			case "!Loader_MainMenu":
				this.CurrentScene = GUIManager.Scene.eMainMenu;
				goto IL_00DB;
			case "Gameplay":
				this.m_HudManager.ShowInGameHud(true);
				this.m_PauseMenu = new PauseMenu(base.gameObject);
				this.CurrentScene = GUIManager.Scene.eInGameHud;
				goto IL_00DB;
			case "LevelSelect":
			case "LevelSelect_Lite":
				this.CurrentScene = GUIManager.Scene.eLevelSelect;
				goto IL_00DB;
		}
		this.CurrentScene = GUIManager.Scene.eNone;
		IL_00DB:
		this.m_CurrentSceneName = aSceneName;
	}

	private void CleanUp()
	{
		this.m_PauseMenu = null;
		this.m_TallyMenu = null;
		this.mo_unlockPopup = null;
		this.m_LoginPopup = null;
		this.m_AppQuitPopup = null;
		this.m_RateMyAppPopup = null;
		this.mo_createAccountPopup = null;
		this.m_OnDemandFonts.Clear();
	}

	public void ShowLoadingScreen()
	{
		this.CurrentScene = GUIManager.Scene.eLoadingScreen;
		if (this.m_LoadingScreen != null)
		{
			this.m_LoadingScreen.Start();
		}
		this.m_UpdateLoadingScreen = true;
	}

	public void HideLoadingScreen()
	{
		this.HideLoadingScreen(true);
	}

	public void HideLoadingScreen(bool aResetCurrentScreen)
	{
		if (this.m_LoadingScreen != null)
		{
			this.m_LoadingScreen.Stop();
		}
		if (aResetCurrentScreen)
		{
			this.CurrentScene = GUIManager.Scene.eNone;
		}
		this.m_UpdateLoadingScreen = false;
	}

	public void UpdateLoadingScreen()
	{
		if (this.m_LoadingScreen != null)
		{
			this.m_LoadingScreen.Update();
		}
	}

	public bool CanPause()
	{
		return !GameFlowManager.Instance.GUIManager.IsPauseMenu && GameFlowManager.Instance.GUIManager != null && GameFlowManager.Instance.GUIManager.CurrentScene == GUIManager.Scene.eInGameHud;
	}

	public void ShowPauseMenu(bool aShow)
	{
		if (aShow)
		{
			if (this.CanPause())
			{
				if (this.m_PauseMenu == null)
				{
					this.m_PauseMenu = new PauseMenu(base.gameObject);
				}
				this.CurrentScene = GUIManager.Scene.ePauseMenu;
				GameManager.Instance.Pause(true);
				AudioManager.Instance.Mute();
				this.m_IsPauseMenu = true;
			}
		}
		else
		{
			this.CurrentScene = GUIManager.Scene.eInGameHud;
			GameManager.Instance.Pause(false);
			AudioManager.Instance.Unmute();
			this.m_IsPauseMenu = false;
		}
	}

	public void ShowTallyMenu(bool aShow)
	{
		if (aShow)
		{
			this.m_HudManager.ShowInGameHud(false);
			if (this.m_TallyMenu == null)
			{
				this.m_TallyMenu = new TallyMenu(base.gameObject);
			}
			GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/TallyMenuCoinTransfer", typeof(GameObject))) as GameObject;
			this.m_TallyMenu.SetCoinTransfer3DObject(gameObject);
			if (Camera.main != null)
			{
				Camera.main.GetComponent<CameraFollow>().ZoomEnabled = false;
			}
			this.CurrentScene = GUIManager.Scene.eTallyMenu;
		}
		else
		{
			if (Camera.main != null)
			{
				Camera.main.GetComponent<CameraFollow>().ZoomEnabled = true;
			}
			this.m_HudManager.ShowInGameHud(true);
			this.CurrentScene = GUIManager.Scene.eInGameHud;
		}
	}

	public void ShowCreateAccountPopup(bool aShow)
	{
		if (aShow)
		{
			this.mo_createAccountPopup = new CreateAccountPopup(base.gameObject);
			this.mo_createAccountPopup.Show(true);
			this.CurrentScene = GUIManager.Scene.eCreateAccountPopup;
		}
		else
		{
			this.mo_createAccountPopup = null;
			this.CurrentScene = GUIManager.Scene.eNone;
		}
	}

	public void ShowUnlockPopups(bool aShow)
	{
		if (aShow)
		{
			this.mo_currentUnlock = GameManager.Instance.FindNextUnlock(GameManager.Unlock.eUnlock_None);
			if (this.mo_currentUnlock != GameManager.Unlock.eUnlock_None)
			{
				this.mo_unlockPopup = new LevelSelectPopup(base.gameObject);
				this.mo_unlockPopup.RegisterCallback(new BasePopup.PopupCallback(this.OnUnlockPopupDismissCallback));
				this.SetPopupPageID(this.mo_currentUnlock);
				this.mo_unlockPopup.Show(true);
			}
			this.CurrentScene = GUIManager.Scene.eUnlockPopup;
		}
		else
		{
			this.CurrentScene = GUIManager.Scene.eInGameHud;
		}
	}

	private void SetPopupPageID(GameManager.Unlock ae_unlock)
	{
		switch (ae_unlock)
		{
		case GameManager.Unlock.eUnlock_TimeTrial:
			this.mo_unlockPopup.SetPageID(LevelSelectPopup.PageID.TimeTrialUnlocked);
			break;
		case GameManager.Unlock.eUnlock_TimeTrialSilver:
			this.mo_unlockPopup.SetPageID(LevelSelectPopup.PageID.TimeTrialSilverAchieved);
			break;
		case GameManager.Unlock.eUnlock_TimeTrialGold:
			this.mo_unlockPopup.SetPageID(LevelSelectPopup.PageID.TimeTrialGoldAchieved);
			break;
		case GameManager.Unlock.eUnlock_TurboMode:
			this.mo_unlockPopup.SetPageID(LevelSelectPopup.PageID.TurboModeUnlocked);
			break;
		case GameManager.Unlock.eUnlock_SlowMotion:
			this.mo_unlockPopup.SetPageID(LevelSelectPopup.PageID.SlowMotionUnlocked);
			break;
		}
	}

	private void OnUnlockPopupDismissCallback(int aButtonSelected)
	{
		this.mo_currentUnlock = GameManager.Instance.FindNextUnlock(this.mo_currentUnlock);
		if (this.mo_currentUnlock != GameManager.Unlock.eUnlock_None)
		{
			this.SetPopupPageID(this.mo_currentUnlock);
			this.mo_unlockPopup.Show(true);
		}
		else
		{
			GameFlowManager.Instance.GUIManager.ShowTallyMenu(true);
			this.mo_unlockPopup = null;
		}
	}

	public void ShowLoginPopup(bool aShow)
	{
		if (aShow)
		{
			this.CurrentScene = GUIManager.Scene.eLoginPopup;
			this.m_LoginPopup = new LoginPopup(base.gameObject);
			this.m_LoginPopup.Show(true);
		}
		else
		{
			this.CurrentScene = GUIManager.Scene.eNone;
			this.m_LoginPopup = null;
		}
	}

	public void ShowUpsellPopup(bool aShow)
	{
		if (aShow)
		{
			this.CurrentScene = GUIManager.Scene.eUpsellPopup;
			this.m_UpsellPopup = new UpsellPopup(base.gameObject);
			this.m_UpsellPopup.Show(true);
		}
		else
		{
			this.CurrentScene = GUIManager.Scene.eNone;
			this.m_UpsellPopup = null;
		}
	}

	public void ShowAppQuitPopup(bool aShow)
	{
		if (aShow)
		{
			this.CurrentScene = GUIManager.Scene.eAppQuitPopup;
			this.m_AppQuitPopup = new AppQuitPopup(base.gameObject);
			this.m_AppQuitPopup.Show(true);
		}
		else
		{
			this.CurrentScene = GUIManager.Scene.eNone;
			this.m_AppQuitPopup.Show(false);
			this.m_AppQuitPopup = null;
		}
	}

	public void ShowRateMyAppPopup(bool aShow)
	{
		if (aShow)
		{
			this.m_PrevSceneRateMyApp = this.CurrentScene;
			this.CurrentScene = GUIManager.Scene.eRateMyApp;
			this.m_RateMyAppPopup = new RateMyAppPopup(base.gameObject);
			this.m_RateMyAppPopup.Show(true);
		}
		else
		{
			this.CurrentScene = this.m_PrevSceneRateMyApp;
			this.m_RateMyAppPopup.Show(false);
			this.m_RateMyAppPopup = null;
		}
	}

	public void LoginPopupToBackTraceScene()
	{
		this.ShowLoginPopup(false);
		this.CurrentScene = this.m_LoginBackTraceScene;
		this.m_LoginBackTraceScene = GUIManager.Scene.eNone;
	}

	public void CreateAccountPopupToBackTraceScene()
	{
		this.ShowCreateAccountPopup(false);
		this.CurrentScene = this.m_LoginBackTraceScene;
		this.m_LoginBackTraceScene = GUIManager.Scene.eNone;
	}

	public void RegisterLoginBackTraceScene()
	{
		this.m_LoginBackTraceScene = this.CurrentScene;
	}

	public void RegisterAboutCPCurrentPage(int aCurrentPage)
	{
		this.m_AboutCPCurrentPage = aCurrentPage;
	}

	public void UnregisterAboutCPCurrentPage()
	{
		this.m_AboutCPCurrentPage = 0;
	}

	public GUISkin m_Skin;

	public Color m_WhiteTextDropShadowColor;

	public Color m_DarkBrownTextDropShadowColor;

	public Color m_LightBrownTextDropShadowColor;

	public Color m_DarkerBrownTextDropShadowColor;

	public Color m_GreyBrownTextDropShadowColor;

	public Vector2 m_DropShadowOffsetOriginal;

	public Vector2 m_DropShadowOffsetLowRes;

	public Vector2 m_DropShadowOffsetIPad;

	public List<BasePopup> m_Popups = new List<BasePopup>();

	public Matrix4x4 m_NewResMatrix;

	private LoadingScreen m_LoadingScreen;

	private HudManager m_HudManager;

	private PauseMenu m_PauseMenu;

	private TallyMenu m_TallyMenu;

	private LevelSelectPopup mo_unlockPopup;

	private LoginPopup m_LoginPopup;

	private UpsellPopup m_UpsellPopup;

	private AppQuitPopup m_AppQuitPopup;

	private RateMyAppPopup m_RateMyAppPopup;

	private GUIManager.Scene m_PrevSceneRateMyApp;

	private CreateAccountPopup mo_createAccountPopup;

	private GUIManager.Scene m_CurrentScene;

	private string m_CurrentSceneName = string.Empty;

	private GUIManager.Scene m_LoginBackTraceScene;

	private int m_AboutCPCurrentPage;

	private Font[] m_InGameFonts;

	private Font[] m_CPFonts;

	private Dictionary<string, Font> m_OnDemandFonts = new Dictionary<string, Font>();

	private int m_DropShadowOffsetX;

	private int m_DropShadowOffsetY;

	private GameManager.Unlock mo_currentUnlock;

	private bool m_IsPauseMenu;

	private bool m_UpdateLoadingScreen;

	private bool m_EnableAutoResize;

	public enum Scene
	{
		eNone,
		eLoadingScreen,
		eSplashScene,
		eMainMenu,
		eLevelSelect,
		eInGameHud,
		ePauseMenu,
		eTallyMenu,
		eUnlockPopup,
		eLoginPopup,
		eCreateAccountPopup,
		eUpsellPopup,
		eAppQuitPopup,
		eRateMyApp,
		eScene_COUNT
	}
}
