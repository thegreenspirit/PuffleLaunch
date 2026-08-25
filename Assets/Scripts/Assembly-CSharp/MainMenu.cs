using System;
using UnityEngine;

public class MainMenu : BaseMonoScreen
{
	protected override void CreateMainScreenLayouts()
	{
		base.MainScreen.TextureData = new GUIDefines.TextureData[]
		{
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					topRatio = 0.73125f,
					widthRatio = 1f,
					heightRatio = 0.26875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = 16f,
						heightScale = 19f
					}
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/MainMenu/Textures/lower_band"
				}
			}
		};
		base.MainScreen.ButtonData = new GUIDefines.ButtonData[]
		{
			new GUIDefines.ButtonData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.35f,
					topRatio = 0.775f,
					widthRatio = 0.3010417f,
					heightRatio = 0.1921875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 1f,
						topOffset = 12f,
						widthScale = 19f,
						heightScale = 6f
					}
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
					},
					customFontSize = GUIDefines.FontSize.eLarge,
					customFontType = GUIDefines.FontType.eInGame
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Play"
				}
			},
			new GUIDefines.ButtonData
			{
				buttonId = 1,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.0125f,
					topRatio = 0.8859375f,
					widthRatio = 0.06354167f,
					heightRatio = 0.0921875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 4f,
						topOffset = 6f
					}
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/info_button"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/info_button_pressed"
					}
				}
			},
			new GUIDefines.ButtonData
			{
				buttonId = 2,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.88125f,
					topRatio = 0.8671875f,
					widthRatio = 0.1041667f,
					heightRatio = 0.1234375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 6f,
						topOffset = 13f
					}
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/login_button"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/MainMenu/Textures/login_button_pressed"
					}
				}
			}
		};
	}

	private void Awake()
	{
		this.Init(base.gameObject);
		if (!PlayerPrefs.HasKey("RateMyApp"))
		{
			PlayerPrefs.SetInt("RateMyApp", 0);
		}
		if (!PlayerPrefs.HasKey("RateMyAppAtLevel5"))
		{
			PlayerPrefs.SetInt("RateMyAppAtLevel5", 1);
		}
		if (PlayerPrefs.GetInt("RateMyApp") < 5)
		{
			PlayerPrefs.SetInt("RateMyApp", PlayerPrefs.GetInt("RateMyApp") + 1);
			PlayerPrefs.Save();
		}
		if (PlayerPrefs.GetInt("RateMyApp") == 5)
		{
			GameFlowManager.Instance.GUIManager.ShowRateMyAppPopup(true);
		}
	}

	private void Start()
	{
		AudioManager.Instance.PlayMusic(AudioManager.MusicTrack.eMusic_Menu);
	}

	private void OnGUI()
	{
		if (!base.MainScreen.CanDraw() || GameFlowManager.Instance.GUIManager.IsLoginPopupShowing || GameFlowManager.Instance.GUIManager.IsCreateAccountPopupShowing)
		{
			return;
		}
		base.MainScreen.Draw();
		this.BlockControl(GameFlowManager.Instance.GUIManager.IsAppQuitPopupShowing);
	}

	protected override void OnMainScreenButtonSelect()
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		switch (base.MainScreen.SelectedButton)
		{
		case 0:
			base.MainScreen.StopGUI();
			GameFlowManager.Instance.LoadScene("LevelSelect", false);
			break;
		case 1:
			base.MainScreen.StopGUI();
			GameFlowManager.Instance.LoadScene("AboutCP", false);
			break;
		case 2:
			GameFlowManager.Instance.GUIManager.RegisterLoginBackTraceScene();
			GameFlowManager.Instance.GUIManager.ShowLoginPopup(true);
			break;
		}
	}

	protected override void OnBack()
	{
		if (GameFlowManager.Instance.GUIManager.IsLoginPopupShowing)
		{
			GameFlowManager.Instance.GUIManager.LoginPopupToBackTraceScene();
		}
		else if (GameFlowManager.Instance.GUIManager.IsCreateAccountPopupShowing)
		{
			GameFlowManager.Instance.GUIManager.ShowCreateAccountPopup(false);
			GameFlowManager.Instance.GUIManager.ShowLoginPopup(true);
		}
		else
		{
			GameFlowManager.Instance.GUIManager.ShowAppQuitPopup(true);
		}
	}

	private enum Button
	{
		ePlay,
		eInfo,
		eLogin,
		eButton_COUNT
	}
}
