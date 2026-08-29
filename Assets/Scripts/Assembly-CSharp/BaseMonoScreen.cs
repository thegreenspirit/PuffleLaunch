using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMonoScreen : MonoBehaviour
{
	public BaseScreen MainScreen
	{
		get
		{
			return this.m_MainScreen;
		}
	}

	protected abstract void CreateMainScreenLayouts();

	protected abstract void OnMainScreenButtonSelect();

	protected abstract void OnBack();

	protected virtual void Init(GameObject aRefObj)
	{
		string languageCode = LocalizationManager.GetLanguageCode();
		switch (languageCode)
		{
		case "pt":
			this.SetTextures(this.iPadTexturePT, this.normalTexturePT, this.lowresTexturePT);
			goto IL_014F;
		case "fr":
			this.SetTextures(this.iPadTextureFR, this.normalTextureFR, this.lowresTextureFR);
			goto IL_014F;
		case "es":
			this.SetTextures(this.iPadTextureES, this.normalTextureES, this.lowresTextureES);
			goto IL_014F;
		case "de":
			this.SetTextures(this.iPadTextureDE, this.normalTextureDE, this.lowresTextureDE);
			goto IL_014F;
		case "ja":
			this.SetTextures(this.iPadTextureJA, this.normalTextureJA, this.lowresTextureJA);
			goto IL_014F;
		}
		this.SetTextures(this.iPadTextureEN, this.normalTextureEN, this.lowresTextureEN);
		IL_014F:
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren != null)
		{
			if (!this.mAspectScaleApplied)
			{
				componentInChildren.transform.position = Vector3.Scale(componentInChildren.transform.position, this.mAspectScale);
				componentInChildren.transform.localScale = Vector3.Scale(componentInChildren.transform.localScale, this.mAspectScale);
				this.mAspectScaleApplied = true;
			}
			for (int i = 0; i < componentInChildren.materials.Length; i++)
			{
				if (this.mActiveTexture != string.Empty && this.mActiveTexture != null)
				{
					componentInChildren.materials[i].mainTexture = GUIUtil.LoadTexture(this.mActiveTexture);
				}
			}
		}
		this.m_MainScreen = new BaseScreen();
		this.CreateMainScreenLayouts();
		this.m_MainScreen.Init(aRefObj);
	}

	protected virtual void BlockControl(bool aBlockControl)
	{
		this.MainScreen.BlockControl(aBlockControl);
		if (this.MainScreen.ButtonData != null)
		{
			for (int i = 0; i < this.MainScreen.ButtonData.Length; i++)
			{
				this.MainScreen.ButtonData[i].isControlBlocked = aBlockControl;
			}
		}
	}

	protected virtual void HandleButtonSelect()
	{
		if (this.MainScreen.IsAnyButtonSelected())
		{
			this.OnMainScreenButtonSelect();
			this.MainScreen.ResetButton();
		}
		else if (!this.MainScreen.IsControlBlocked())
		{
			if (GameFlowManager.Instance.m_DoWindowBack)
			{
#if UNITY_ANDROID || UNITY_IOS
				if (!TouchScreenKeyboard.visible) this.OnBack();
#endif
				GameFlowManager.Instance.m_DoWindowBack = false;
			}
			else if (Input.GetKeyUp("menu"))
			{
				this.OnSettingsButton();
			}
		}
	}

	protected virtual void OnSettingsButton()
	{
	}

	public virtual void Update()
	{
		this.HandleButtonSelect();
	}

	private void OnDestroy()
	{
		this.MainScreen.StopGUI();
	}

	public void SetTopBarData(string asz_back, string asz_title)
	{
		this.SetTopBarData(asz_back, asz_title, null, null);
	}

	public void SetTopBarData(string asz_back, string asz_title, GUIDefines.ButtonData[] aCustomButtons, BaseMonoScreen.TopBarButtonCallback[] aCustomCallbacks)
	{
		this.msz_back = asz_back;
		this.msz_title = asz_title;
		if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eIPad)
		{
			this.mto_topBar = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						widthRatio = 1f,
						heightRatio = 0.10677083f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/Common/iPad/Create_Account_NavBar"
					}
				}
			};
		}
		else
		{
			this.mto_topBar = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						widthRatio = 1f,
						heightRatio = 0.125f
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/Common/Create_Account_NavBar"
					}
				}
			};
		}
		this.mto_title = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					topRatio = 0.04375f,
					widthRatio = 1f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = -10f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = this.msz_title
				}
			}
		};
		GUIDefines.ButtonData buttonData = null;
		if (asz_back != string.Empty)
		{
			if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eIPad)
			{
				buttonData = new GUIDefines.ButtonData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.02133333f,
						topRatio = 0.02589583f,
						widthRatio = 0.10839844f,
						heightRatio = 0.06640625f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true,
							topOffset = -4f
						}
					},
					detectZoneScale = 1.5f,
					content = new GUIDefines.ContentInfo(),
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/Common/iPad/Create_Account_BackBtn_iPad"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/Common/iPad/Create_Account_BackBtn_pressed_iPad"
						}
					}
				};
			}
			else
			{
				buttonData = new GUIDefines.ButtonData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.01875f,
						topRatio = 0.021875f,
						widthRatio = 0.109375f,
						heightRatio = 0.0796875f
					},
					detectZoneScale = 1.5f,
					content = new GUIDefines.ContentInfo(),
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/Common/Create_Account_BackBtn"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/Common/Create_Account_BackBtn_pressed"
						}
					}
				};
			}
		}
		List<GUIDefines.ButtonData> list = new List<GUIDefines.ButtonData>();
		if (buttonData != null)
		{
			list.Add(buttonData);
		}
		if (aCustomButtons != null && aCustomButtons.Length > 0)
		{
			Utilities.AssertMsg(aCustomButtons.Length == aCustomCallbacks.Length, "Custom top bar buttons and callbacks size mismatch");
			for (int i = 0; i < aCustomButtons.Length; i++)
			{
				aCustomButtons[i].buttonId = 1 + i;
			}
			list.AddRange(aCustomButtons);
			this.m_TopBarCustomCallbacks = aCustomCallbacks;
		}
		if (list.Count > 0)
		{
			this.mto_topBarButtons = list.ToArray();
		}
		if (this.mto_topBarButtons != null)
		{
			for (int j = 0; j < this.mto_topBarButtons.Length; j++)
			{
				this.mto_topBarButtons[j].Init();
			}
		}
		if (this.mto_topBar != null)
		{
			for (int k = 0; k < this.mto_topBar.Length; k++)
			{
				this.mto_topBar[k].Init();
			}
		}
		if (this.mto_title != null)
		{
			for (int l = 0; l < this.mto_title.Length; l++)
			{
				this.mto_title[l].Init();
			}
		}
	}

	public float GetTopBarHeightPixels()
	{
		return this.mto_topBar[0].pos.inPixel.height;
	}

	public float GetTopBarHeightRatio()
	{
		return this.mto_topBar[0].pos.heightRatio;
	}

	public void DrawTopBar()
	{
		GUICompoundControls.Textures(this.MainScreen.LocalTransform.position, this.mto_topBar);
		int num = 0;
		if (this.mto_topBarButtons != null)
		{
			num = GUICompoundControls.Buttons(this.MainScreen.LocalTransform.position, this.mto_topBarButtons);
		}
		if (!this.m_MainScreen.IsControlBlocked())
		{
			if (num == 0)
			{
				this.OnBack();
			}
			else if (num >= 1 && num < 2)
			{
				this.m_TopBarCustomCallbacks[num - 1]();
			}
		}
		GUICompoundControls.Labels(this.MainScreen.LocalTransform.position, this.mto_title);
	}

	protected void SetTextures(string aIPadTexture, string aNormalTexture, string aLowresTexture)
	{
		switch (ResolutionManager.Instance.AssetResolution)
		{
		case ResolutionManager.eAssetResolution.eLowres:
			if (aLowresTexture == null)
			{
				aLowresTexture = this.lowresTextureEN;
			}
			this.mActiveTexture = aLowresTexture;
			this.mAspectScale = Vector3.one;
			break;
		case ResolutionManager.eAssetResolution.eOriginal:
			if (aNormalTexture == null)
			{
				aNormalTexture = this.normalTextureEN;
			}
			this.mActiveTexture = aNormalTexture;
			this.mAspectScale = Vector3.one;
			break;
		case ResolutionManager.eAssetResolution.eIPad:
			if (aIPadTexture == null)
			{
				aIPadTexture = this.iPadTextureEN;
			}
			this.mActiveTexture = aIPadTexture;
			this.mAspectScale = Vector3.one;
			break;
		}
	}

	public string iPadTextureEN;

	public string normalTextureEN;

	public string lowresTextureEN;

	public string iPadTextureES;

	public string normalTextureES;

	public string lowresTextureES;

	public string iPadTextureFR;

	public string normalTextureFR;

	public string lowresTextureFR;

	public string iPadTexturePT;

	public string normalTexturePT;

	public string lowresTexturePT;

	public string iPadTextureDE;

	public string normalTextureDE;

	public string lowresTextureDE;

	public string iPadTextureJA;

	public string normalTextureJA;

	public string lowresTextureJA;

	protected string mActiveTexture;

	protected Vector3 mAspectScale;

	private bool mAspectScaleApplied;

	protected BaseScreen m_MainScreen;

	public string msz_back = "TXT_Back";

	public string msz_title = string.Empty;

	private GUIDefines.TextureData[] mto_topBar;

	private GUIDefines.LabelData[] mto_title;

	private GUIDefines.ButtonData[] mto_topBarButtons;

	private BaseMonoScreen.TopBarButtonCallback[] m_TopBarCustomCallbacks;

	public enum TopBarButton
	{
		eBack,
		eCustomButton_Start,
		eTopBarButton_COUNT
	}

	public delegate void TopBarButtonCallback();
}
