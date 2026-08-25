using System;
using UnityEngine;

public class ResolutionManager
{
	public static ResolutionManager Instance
	{
		get
		{
			if (ResolutionManager.m_cInstance == null)
			{
				ResolutionManager.m_cInstance = new ResolutionManager();
				ResolutionManager.m_cInstance.Initialize();
			}
			return ResolutionManager.m_cInstance;
		}
	}

	public bool ResolutionInfoSet
	{
		get { return this.mb_resolutionInfoSet; }
	}

	public ResolutionManager.eAssetResolution AssetResolution
	{
		get { return this.me_assetResolution; }
	}

	public ResolutionManager.eLayoutSize LayoutSize
	{
		get { return this.me_layoutSize; }
	}

	public void Initialize()
	{
		this.mb_resolutionInfoSet = false;
		this.SetResolutionInfo();

		// Green Spirit: change TouchScreenKeyboard to Screen
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;

		TouchScreenKeyboard.hideInput = true;
	}

	public void CheckDeviceOrientation()
	{
		DeviceOrientation deviceOrientation = Input.deviceOrientation;
		if (deviceOrientation != this.m_PrevOrientation && (deviceOrientation == DeviceOrientation.LandscapeLeft || deviceOrientation == DeviceOrientation.LandscapeRight))
		{
			this.m_PrevOrientation = deviceOrientation;
			Screen.orientation = (ScreenOrientation)deviceOrientation;
		}
	}

	public float GetAspectRatio()
	{
		if (this.m_AspectRatio == 0f)
		{
			this.m_AspectRatio = this.GetMaxValueOfScreenSize() / this.GetMinValueOfScreenSize();
		}
		return this.m_AspectRatio;
	}

	public float GetMaxValueOfScreenSize()
	{
		return (float)Mathf.Max(Screen.width, Screen.height);
	}

	public float GetMinValueOfScreenSize()
	{
		return (float)Mathf.Min(Screen.width, Screen.height);
	}

	private void SetResolutionInfo()
	{
		this.me_assetResolution = ResolutionManager.eAssetResolution.eOriginal;
		this.me_layoutSize = ResolutionManager.eLayoutSize.eOriginal;
		float num = 500f;
		float num2 = 850f;
		if ((float)Screen.width <= num)
		{
			this.me_assetResolution = ResolutionManager.eAssetResolution.eLowres;
			this.me_layoutSize = ResolutionManager.eLayoutSize.eLowres;
			GUIConstants.kReferenceScreenWidth = 480f;
			GUIConstants.kReferenceScreenHeight = 320f;
		}
		else
		{
			this.me_assetResolution = ResolutionManager.eAssetResolution.eOriginal;
			this.me_layoutSize = ResolutionManager.eLayoutSize.eOriginal;
			GUIConstants.kReferenceScreenWidth = 960f;
			GUIConstants.kReferenceScreenHeight = 640f;
			if ((float)Screen.width >= num2)
			{
				float num3 = (float)Screen.width / (float)Screen.height;
				float num4 = Mathf.Abs(num3 - 1.3333334f);
				float num5 = Mathf.Abs(num3 - 1.5f);
				if (num4 < num5)
				{
					this.me_layoutSize = ResolutionManager.eLayoutSize.eIPad;
					this.me_assetResolution = ResolutionManager.eAssetResolution.eIPad;
					GUIConstants.kReferenceScreenWidth = 1024f;
					GUIConstants.kReferenceScreenHeight = 768f;
				}
			}
		}
		this.mb_resolutionInfoSet = true;
	}

	private DeviceOrientation m_PrevOrientation = DeviceOrientation.LandscapeLeft;

	private ResolutionManager.eAssetResolution me_assetResolution;

	private ResolutionManager.eLayoutSize me_layoutSize;

	private bool mb_resolutionInfoSet;

	private float m_AspectRatio;

	private static ResolutionManager m_cInstance;

	public enum eAssetResolution
	{
		eLowres,
		eOriginal,
		eIPad
	}

	public enum eLayoutSize
	{
		eLowres,
		eOriginal,
		eIPad
	}
}
