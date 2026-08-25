using System;
using UnityEngine;

public class UpsellPopup : BasePopup
{
	public UpsellPopup(GameObject aRefObj)
		: base(aRefObj)
	{
	}

	protected override void CreateLayouts()
	{
		base.WindowData = new GUIDefines.WindowData
		{
			pos = new GUIDefines.RectInfo
			{
				widthRatio = 1f,
				heightRatio = 1f,
				detatchFromRefObject = true
			},
			id = 10
		};
		base.TextureData = new GUIDefines.TextureData[]
		{
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 1f,
					heightRatio = 1f,
					detatchFromRefObject = true
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/Upsell/UpsellBackground"
				}
			}
		};
		string text = "androidMarket_button";
		base.ButtonData = new GUIDefines.ButtonData[]
		{
			new GUIDefines.ButtonData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.772375f,
					topRatio = 0.7830205f,
					widthRatio = 0.2136068f,
					heightRatio = 0.182291f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = -7f,
						topOffset = 18f
					}
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/Lite/" + text,
						isLocalized = true
					}
				}
			},
			new GUIDefines.ButtonData
			{
				buttonId = 1,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.922375f,
					topRatio = 0.00870833f,
					widthRatio = 0.0844401f,
					heightRatio = 0.1257813f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 4f
					}
				},
				detectZoneScale = 1.5f,
				content = new GUIDefines.ContentInfo(),
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/Upsell/upsell_close_button"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/Upsell/upsell_close_button_pressed"
					}
				}
			}
		};
		string text2 = string.Empty;
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eLowres)
		{
			text2 = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eMini);
		}
		else
		{
			text2 = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eSmall);
		}
		GUIDefines.FontSize fontSize = GUIDefines.FontSize.eSmall;
		if (LocalizationManager.GetLanguageCode() == "en" || LocalizationManager.GetLanguageCode() == "es")
		{
			fontSize = GUIDefines.FontSize.eMedium;
		}
		GUIDefines.RectInfo rectInfo;
		if (LocalizationManager.GetLanguageCode() == "de")
		{
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = 0.175f,
				topRatio = 0.8171875f,
				widthRatio = 0.8f,
				heightRatio = 0.04375f,
				IPad = new GUIDefines.RectIPadInfo
				{
					leftOffset = 21f,
					topOffset = 12f
				}
			};
		}
		else
		{
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = 0.225f,
				topRatio = 0.8015625f,
				widthRatio = 0.8f,
				heightRatio = 0.04375f,
				IPad = new GUIDefines.RectIPadInfo
				{
					leftOffset = 30f,
					topOffset = 6f
				}
			};
		}
		base.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					topRatio = 0.04375f,
					widthRatio = 1f,
					heightRatio = 0.04375f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_UPSELL_TITLE"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					useCustomDropShadowColor = true,
					customDropShadowColor = Color.black,
					useCustomDropShadowOffset = true,
					customDropShadowOffset = new Vector2(2f, 3f),
					customFontType = GUIDefines.FontType.eInGame,
					customFontSize = fontSize
				}
			},
			new GUIDefines.LabelData
			{
				pos = rectInfo,
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_UPSELL_POINTS"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					useCustomTextAlignment = true,
					useCustomDropShadowColor = true,
					customDropShadowColor = Color.black,
					useCustomDropShadowOffset = true,
					customDropShadowOffset = new Vector2(2f, 3f),
					customFontType = GUIDefines.FontType.eInGame,
					customFontSize = GUIDefines.FontSize.eSmall
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = -0.007291667f,
					topRatio = 0.9546875f,
					widthRatio = 1f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = -10f,
						topOffset = 6f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Disclaimer"
				},
				disableDropShadow = true,
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormalTextColor = Color.black,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.LowerRight,
					customFontType = GUIDefines.FontType.eOnDemand,
					customOnDemandFontName = text2
				}
			}
		};
	}

	protected override void OnButtonSelect()
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		UpsellPopup.Button selectedButton = (UpsellPopup.Button)base.SelectedButton;
		if (selectedButton != UpsellPopup.Button.eAppStore)
		{
			if (selectedButton == UpsellPopup.Button.eBack)
			{
				GameFlowManager.Instance.GUIManager.ShowUpsellPopup(false);
				if (LevelSelect.SelectedLevel - 1 == 5)
				{
					GameManager.Instance.StartLevel((GameManager.Level)(LevelSelect.SelectedLevel - 1));
					GameFlowManager.Instance.LoadScene("Gameplay", true);
				}
				else
				{
					GameFlowManager.Instance.LoadScene("LevelSelect_Lite", false);
				}
			}
		}
		else
		{
			Application.OpenURL("market://details?id=com.disney.PuffleLaunch");
		}
	}

	private enum Button
	{
		eAppStore,
		eBack,
		eButton_COUNT
	}
}
