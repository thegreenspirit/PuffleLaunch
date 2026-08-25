using System;
using UnityEngine;

public class AppQuitPopup : BasePopup
{
	public AppQuitPopup(GameObject aRefObj)
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
					leftRatio = 0.25f,
					topRatio = 0.1796875f,
					widthRatio = 0.5f,
					heightRatio = 0.5f
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
					leftRatio = 0.3260417f,
					topRatio = 0.5015625f,
					widthRatio = 0.109375f,
					heightRatio = 0.0796875f
				},
				detectZoneScale = 1.5f,
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
					leftRatio = 0.5635416f,
					topRatio = 0.5015625f,
					widthRatio = 0.109375f,
					heightRatio = 0.0796875f
				},
				detectZoneScale = 1.5f,
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
					leftRatio = 0.3125f,
					topRatio = 0.2484375f,
					widthRatio = 0.3708334f,
					heightRatio = 0.2078125f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_AndroidExit"
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
					leftRatio = 0.3239583f,
					topRatio = 0.5046875f,
					widthRatio = 0.109375f,
					heightRatio = 0.0796875f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Yes"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eMedium
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.5635416f,
					topRatio = 0.5046875f,
					widthRatio = 0.109375f,
					heightRatio = 0.0796875f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_No"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eMedium
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
		AppQuitPopup.Button selectedButton = (AppQuitPopup.Button)base.SelectedButton;
		if (selectedButton != AppQuitPopup.Button.eYes)
		{
			if (selectedButton == AppQuitPopup.Button.eNo)
			{
				GameFlowManager.Instance.GUIManager.ShowAppQuitPopup(false);
			}
		}
		else
		{
			Application.Quit();
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
		eYes,
		eNo,
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
