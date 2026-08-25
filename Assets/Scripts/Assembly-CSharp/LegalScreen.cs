using System;
using UnityEngine;

public class LegalScreen : MonoBehaviour
{
	public float m_TimeToDisplay = 2f;

	public GameObject nextObject;
	public GameObject Logo;

	private static LegalScreen m_cInstance;
	private float m_Timer;
	private bool m_IsDone;

	private Transform m_LogoTransform;
	private Texture2D m_SplashScreenBg;

	private Rect m_LoadingBottomBarRect;
	private Texture2D m_LoadingBarFrameBg;
	private Rect m_LoadingBarFrameBgRect;
	private Texture2D m_LoadingBar;
	private Rect m_LoadingBarRect;
	private Texture2D m_LoadingBarFrame;
	private Rect m_LoadingBarFrameRect;
	private float m_LoadingBarTotalWidth;

	private GameObject m_GameFlowObject;
	private GameObject m_AssetLoaderObject;

	public static LegalScreen Instance
	{
		get { return LegalScreen.m_cInstance; }
	}

	public bool IsDone
	{
		get { return this.m_IsDone; }
	}

	private void Awake()
	{
		ResolutionManager.Instance.CheckDeviceOrientation();

		global::UnityEngine.Object.DestroyImmediate(GameObject.Find("LITE"));

		if (GameFlowManager.mLoadingDone)
		{
			this.nextObject.gameObject.SetActive(true);
			base.gameObject.SetActive(false);
		}
		else
		{
			this.m_GameFlowObject = Resources.Load("Prefabs/GameFlowManager", typeof(GameObject)) as GameObject;
			if (Utilities.AssertMsgCritical(this.m_GameFlowObject != null, "Danger, Will Robinson! Danger!\nCannot Load the GameFlowManger Object...!!"))
			{
				global::UnityEngine.Object.Instantiate(this.m_GameFlowObject);
			}
			this.m_AssetLoaderObject = Resources.Load("Prefabs/AssetLoader", typeof(GameObject)) as GameObject;
			if (this.m_AssetLoaderObject != null)
			{
				global::UnityEngine.Object.Instantiate(this.m_AssetLoaderObject);
			}
		}

		LegalScreen.m_cInstance = this;

		this.m_Timer = 0f;
		this.m_LogoTransform = this.Logo.transform;
		this.SetLogoScaling();

		string text = "GUI/MainMenu/Textures/Loading/BottomBar";
		switch (ResolutionManager.Instance.AssetResolution)
		{
			case ResolutionManager.eAssetResolution.eLowres:
				text += "_lowres";
				break;
			case ResolutionManager.eAssetResolution.eIPad:
				text += "_iPad";
				break;
		}

		this.m_SplashScreenBg = Resources.Load(text, typeof(Texture2D)) as Texture2D;
		if (this.m_SplashScreenBg == null)
		{
			this.m_SplashScreenBg = Resources.Load("GUI/MainMenu/Textures/Loading/BottomBar", typeof(Texture2D)) as Texture2D;
		}

		this.m_LoadingBarFrame = GUIUtil.LoadTexture2D("GUI/LoadingScreen/AnimatedLoadingScreen/bar_frame");
		this.m_LoadingBar = GUIUtil.LoadTexture2D("GUI/LoadingScreen/AnimatedLoadingScreen/bar_filler_slice");

		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			this.m_LoadingBarFrameRect = new Rect(0.34863f * (float)Screen.width, 0.89973f * (float)Screen.height, 0.33984f * (float)Screen.width, 0.05989f * (float)Screen.height);
			this.m_LoadingBarRect = new Rect(0.3623f * (float)Screen.width, 0.91147f * (float)Screen.height, 0f, 0.02864f * (float)Screen.height);
			this.m_LoadingBarTotalWidth = 0.31445f * (float)Screen.width;
			this.m_LoadingBottomBarRect = new Rect(0f, 0.8138f * (float)Screen.height, (float)Screen.width, 0.1862f * (float)Screen.height);
		}
		else
		{
			this.m_LoadingBarFrameRect = new Rect(0.34062f * (float)Screen.width, 0.89375f * (float)Screen.height, 0.3625f * (float)Screen.width, 0.07031f * (float)Screen.height);
			this.m_LoadingBarRect = new Rect(0.35521f * (float)Screen.width, 0.90469f * (float)Screen.height, 0f, 0.04844f * (float)Screen.height);
			this.m_LoadingBarTotalWidth = 0.33438f * (float)Screen.width;
			this.m_LoadingBottomBarRect = new Rect(0f, 0.79999f * (float)Screen.height, (float)Screen.width, 0.20313f * (float)Screen.height);
		}
	}

	private void Update()
	{
		if (this.m_Timer >= this.m_TimeToDisplay && !GameFlowManager.mLoadingDone)
		{
			this.m_Timer = 0f;
			this.m_IsDone = true;
			this.Logo.GetComponent<Dynamic3DBackground>().mb_forceToEnglish = false;
			this.Logo.GetComponent<Dynamic3DBackground>().LoadNewTexture("GUI/Logo/", "pl_logo");
			this.SetLogoScaling();
			GameFlowManager.mLoadingDone = true;
			this.nextObject.gameObject.SetActive(true);
			base.gameObject.SetActive(false);
		}
		if (this.m_LoadingBar != null && this.m_LoadingBarFrame != null)
		{
			this.m_LoadingBarRect.width = this.m_LoadingBarRect.width + this.m_LoadingBarTotalWidth / this.m_TimeToDisplay * Time.deltaTime;
			this.m_LoadingBarRect.width = Mathf.Clamp(this.m_LoadingBarRect.width, 0f, this.m_LoadingBarTotalWidth);
		}
		this.m_Timer += Time.deltaTime;
	}

	private void OnGUI()
	{
		if (this.m_SplashScreenBg != null)
		{
			GUI.DrawTexture(this.m_LoadingBottomBarRect, this.m_SplashScreenBg);
		}
		if (this.m_LoadingBar != null && this.m_LoadingBarFrame != null)
		{
			GUI.DrawTexture(this.m_LoadingBarRect, this.m_LoadingBar);
			GUI.DrawTexture(this.m_LoadingBarFrameRect, this.m_LoadingBarFrame);
		}
	}

	private void SetLogoScaling()
	{
		string text;
		switch (ResolutionManager.Instance.AssetResolution)
		{
		case ResolutionManager.eAssetResolution.eLowres:
			text = LocalizationManager.GetLanguageCode();
			switch (text)
			{
			case "en":
				this.m_LogoTransform.position = new Vector3(0.25f, 2.76f, -3f);
				this.m_LogoTransform.localScale = new Vector3(1.23f, 1f, 1.31f);
				break;
			case "fr":
				this.m_LogoTransform.position = new Vector3(0.25f, 2.4f, -3f);
				this.m_LogoTransform.localScale = new Vector3(1.42f, 1f, 1.47f);
				break;
			case "es":
				this.m_LogoTransform.position = new Vector3(-0.21f, 2.02f, -3f);
				this.m_LogoTransform.localScale = new Vector3(1.6f, 1f, 1.43f);
				break;
			case "pt":
				this.m_LogoTransform.position = new Vector3(-0.3f, 2.32f, -3f);
				this.m_LogoTransform.localScale = new Vector3(1.48f, 1f, 1.46f);
				break;
			}
			return;
		case ResolutionManager.eAssetResolution.eIPad:
			text = LocalizationManager.GetLanguageCode();
			switch (text)
			{
			case "en":
				this.m_LogoTransform.position = new Vector3(0.19f, 1.56f, -3f);
				this.m_LogoTransform.localScale = new Vector3(1.05f, 1f, 1.05f);
				break;
			case "fr":
				this.m_LogoTransform.position = new Vector3(0.42f, 1.35f, -3f);
				this.m_LogoTransform.localScale = new Vector3(1f, 1f, 1.16f);
				break;
			case "es":
				this.m_LogoTransform.position = new Vector3(0.42f, 1.48f, -3f);
				this.m_LogoTransform.localScale = new Vector3(1.11f, 1f, 1.12f);
				break;
			case "pt":
				this.m_LogoTransform.position = new Vector3(-0.25f, 1.45f, -3f);
				this.m_LogoTransform.localScale = new Vector3(1.08f, 1f, 1.18f);
				break;
			}
			return;
		}
		text = LocalizationManager.GetLanguageCode();
		switch (text)
		{
		case "en":
			this.m_LogoTransform.position = new Vector3(0.25f, 2.73f, -3f);
			this.m_LogoTransform.localScale = new Vector3(1.17f, 1f, 1.23f);
			break;
		case "fr":
			this.m_LogoTransform.position = new Vector3(0.25f, 2.56f, -3f);
			this.m_LogoTransform.localScale = new Vector3(1.2f, 1f, 1.4f);
			break;
		case "es":
			this.m_LogoTransform.position = new Vector3(0f, 2.49f, -3f);
			this.m_LogoTransform.localScale = new Vector3(1.33f, 1f, 1.32f);
			break;
		case "pt":
			this.m_LogoTransform.position = new Vector3(-0.37f, 2.84f, -3f);
			this.m_LogoTransform.localScale = new Vector3(1.28f, 1f, 1.43f);
			break;
		}
	}
}
