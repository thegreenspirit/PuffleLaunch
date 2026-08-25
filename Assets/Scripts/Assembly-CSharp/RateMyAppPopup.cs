using System;
using UnityEngine;

public class RateMyAppPopup : BasePopup
{
	public RateMyAppPopup(GameObject aRefObj)
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
				heightRatio = 1f
			},
			id = 10
		};
		base.TextureData = new GUIDefines.TextureData[]
		{
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2447916f,
					topRatio = 0.053125f,
					widthRatio = 0.5104167f,
					heightRatio = 0.8421875f
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/MainMenu/Textures/exit_popup"
				}
			}
		};
		base.ButtonData = new GUIDefines.ButtonData[]
		{
			new GUIDefines.ButtonData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.345833f,
					topRatio = 0.4765625f,
					widthRatio = 0.30625f,
					heightRatio = 0.0828125f
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/play_button"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/play_button_pressed"
					}
				}
			},
			new GUIDefines.ButtonData
			{
				buttonId = 1,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.345833f,
					topRatio = 0.571875f,
					widthRatio = 0.30625f,
					heightRatio = 0.0828125f
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/play_button"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/play_button_pressed"
					}
				}
			},
			new GUIDefines.ButtonData
			{
				buttonId = 2,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.345833f,
					topRatio = 0.665625f,
					widthRatio = 0.30625f,
					heightRatio = 0.0828125f
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/play_button"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/play_button_pressed"
					}
				}
			}
		};
		base.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.322917f,
					topRatio = 0.178125f,
					widthRatio = 0.353125f,
					heightRatio = 0.0921875f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_RateMyAppTitle"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eMedium,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.322917f,
					topRatio = 0.259375f,
					widthRatio = 0.353125f,
					heightRatio = 0.20625f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_RateMyAppMsgAndroid"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.4416666f,
					topRatio = 0.48125f,
					widthRatio = 0.109375f,
					heightRatio = 0.0796875f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_RateMyAppRateButton"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.4458333f,
					topRatio = 0.575f,
					widthRatio = 0.109375f,
					heightRatio = 0.0796875f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_RateMyAppRemindButton"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.449999f,
					topRatio = 0.66875f,
					widthRatio = 0.109375f,
					heightRatio = 0.0796875f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_RateMyAppNoButton"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall
				}
			}
		};
	}

	protected override void OnButtonSelect()
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		if (base.TextFieldData != null)
		{
			for (int i = 0; i < base.TextFieldData.Length; i++)
			{
				base.TextFieldData[i].isFocused = false;
			}
		}
		switch (base.SelectedButton)
		{
		case 0:
			PlayerPrefs.SetInt("RateMyApp", 10);
			PlayerPrefs.Save();
			Application.OpenURL("market://details?id=com.disney.PuffleLaunch");
			GameFlowManager.Instance.GUIManager.ShowRateMyAppPopup(false);
			break;
		case 1:
			PlayerPrefs.SetInt("RateMyApp", 0);
			PlayerPrefs.Save();
			GameFlowManager.Instance.GUIManager.ShowRateMyAppPopup(false);
			break;
		case 2:
			PlayerPrefs.SetInt("RateMyApp", 10);
			PlayerPrefs.Save();
			GameFlowManager.Instance.GUIManager.ShowRateMyAppPopup(false);
			break;
		}
	}

	public override void Show(bool aShow)
	{
		base.Show(aShow);
		if (aShow)
		{
			GameFlowManager.Instance.GUIManager.m_Popups.Add(this);
		}
		else
		{
			GameFlowManager.Instance.GUIManager.m_Popups.Remove(this);
		}
	}

	public override void ClosePopup()
	{
		this.Show(false);
		if (this.m_Callback != null)
		{
			this.m_Callback(1);
		}
	}

	private enum Button
	{
		eRateNow,
		eRemindMeLater,
		eNoRate,
		eButton_COUNT
	}

	private enum Label
	{
		eTitle,
		eLabel_COUNT
	}

	private enum Texture
	{
		eBackground,
		eTexture_COUNT
	}

	private enum ReturnCode
	{
		eSuccess,
		eFail,
		eReturnCode_COUNT
	}
}
