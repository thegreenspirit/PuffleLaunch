using System;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
	private void Awake()
	{
		this.mb_isInitialized = false;
	}

	private void Init()
	{
		this.m_SplashScreenBgRect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		string text = "GUI/SplashScreens/CP_Splash2_1024x768";
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
			this.m_SplashScreenBg = Resources.Load("GUI/SplashScreens/CP_Splash2_1024x768", typeof(Texture2D)) as Texture2D;
		}
		this.mb_isInitialized = true;
	}

	private void OnGUI()
	{
		if (this.mb_isInitialized)
		{
			GUI.DrawTexture(this.m_SplashScreenBgRect, this.m_SplashScreenBg);
		}
	}

	private void Update()
	{
		if (!this.mb_isInitialized && GameFlowManager.Instance != null && ResolutionManager.Instance.ResolutionInfoSet)
		{
			this.Init();
		}
	}

	private Texture2D m_SplashScreenBg;

	private Rect m_SplashScreenBgRect;

	public bool mb_isInitialized;
}
