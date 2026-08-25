using System;
using UnityEngine;

public class TimeTrialPopup : BasePopup
{
	public TimeTrialPopup(GameObject aRefObj)
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
					leftRatio = 0.1453125f,
					topRatio = 0.02265625f,
					widthRatio = 0.709375f,
					heightRatio = 0.9546875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 19f,
						topOffset = 18f
					}
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/LevelSelect/Popups/Dialog_dropShadow"
				}
			},
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.40989584f,
					topRatio = 0.1484375f,
					widthRatio = 0.18020833f,
					heightRatio = 0.2703125f
				},
				icon = new GUIDefines.TextureInfo()
			}
		};
		base.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2416667f,
					topRatio = 0.43125f,
					widthRatio = 0.5208333f,
					heightRatio = 0.115625f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 15f,
						topOffset = -31f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_TimeTrial"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eLarge,
					customFontType = GUIDefines.FontType.eInGame,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.UpperCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2395833f,
					topRatio = 0.5546875f,
					widthRatio = 0.5208333f,
					heightRatio = 0.0921875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 15f,
						topOffset = -45f
					}
				},
				content = new GUIDefines.ContentInfo(),
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eMedium,
					customFontType = GUIDefines.FontType.eInGame,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.UpperCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2385416f,
					topRatio = 0.6796875f,
					widthRatio = 0.3385416f,
					heightRatio = 0.0921875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 22f,
						topOffset = -61f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_BestTime"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customFontType = GUIDefines.FontType.eInGame,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleLeft
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2385416f,
					topRatio = 0.7421875f,
					widthRatio = 0.3385416f,
					heightRatio = 0.0921875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 22f,
						topOffset = -68f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_NewGoal"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customFontType = GUIDefines.FontType.eInGame,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleLeft
				}
			}
		};
		this.m_LeftButtonPos.Init();
		this.m_CenterButtonPos.Init();
		base.ButtonData = new GUIDefines.ButtonData[]
		{
			new GUIDefines.ButtonData
			{
				pos = this.m_LeftButtonPos,
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_OKAllCaps"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = "GUI/LevelSelect/Popups/Button_OK"
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/LevelSelect/Popups/Button_OK_pressed"
					},
					customFontSize = GUIDefines.FontSize.eLarge,
					customFontType = GUIDefines.FontType.eInGame
				}
			}
		};
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
		float timeTrialBestTime = GameManager.GetTimeTrialBestTime(GameManager.Instance.CurrentWorld);
		float timeTrialNewGoal = GameManager.GetTimeTrialNewGoal(GameManager.Instance.CurrentWorld, timeTrialBestTime);
		if (GameManager.Instance.HasAchievedTimeTrialFire(GameManager.Instance.CurrentWorld))
		{
			base.LabelData[1].content.textId = "TXT_BestTime";
			base.LabelData[1].content.suffixText = GameManager.GetTimeFormatedString(timeTrialBestTime);
			base.LabelData[2].invisible = true;
			base.LabelData[3].invisible = true;
			base.ButtonData[0].pos = this.m_CenterButtonPos;
		}
		else
		{
			base.LabelData[1].content.textId = "TXT_BeatTheClock";
			base.LabelData[1].content.suffixText = string.Empty;
			base.LabelData[2].content.suffixText = GameManager.GetTimeFormatedString(timeTrialBestTime);
			base.LabelData[2].invisible = timeTrialBestTime <= 0f;
			base.LabelData[3].content.suffixText = GameManager.GetTimeFormatedString(timeTrialNewGoal);
			base.LabelData[3].invisible = timeTrialBestTime > 0f && timeTrialBestTime <= (float)GameManager.kTimeTrialTimes[(int)GameManager.Instance.CurrentWorld, 3];
			base.ButtonData[0].pos = this.m_LeftButtonPos;
		}
		this.UpdateClockTexture(GameManager.Instance.CurrentWorld, (int)timeTrialNewGoal);
	}

	protected override void OnButtonSelect()
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		if (GameObject.Find("LevelSelect") != null)
		{
			GameObject.Find("LevelSelect").GetComponent<LevelSelectManager>().RequestChangeButtonsState(true);
		}
		base.OnButtonSelect();
	}

	private void UpdateClockTexture(GameManager.World aWorld, int aNewGoal)
	{
		GUIDefines.RectInfo rectInfo = new GUIDefines.RectInfo
		{
			leftRatio = 0.40989584f,
			topRatio = 0.1484375f,
			widthRatio = 0.18020833f,
			heightRatio = 0.2703125f,
			IPad = new GUIDefines.RectIPadInfo
			{
				leftOffset = 6f
			}
		};
		string text;
		if (aNewGoal == GameManager.kTimeTrialTimes[(int)aWorld, 3])
		{
			text = "GUI/LevelSelect/Popups/Clock_Fire";
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = 0.4098958f,
				topRatio = 0.003125f,
				widthRatio = 0.1739584f,
				heightRatio = 0.3921875f,
				IPad = new GUIDefines.RectIPadInfo
				{
					leftOffset = 5f,
					topOffset = 24f
				}
			};
		}
		else if (aNewGoal == GameManager.kTimeTrialTimes[(int)aWorld, 2])
		{
			text = "GUI/LevelSelect/Popups/Clock_Gold";
		}
		else
		{
			text = "GUI/LevelSelect/Popups/Clock_Silver";
		}
		if (base.TextureData[1].icon.name != text)
		{
			base.TextureData[1].icon.name = text;
			base.TextureData[1].pos = rectInfo;
			base.TextureData[1].Init();
		}
	}

	public override void ClosePopup()
	{
		this.OnButtonSelect();
		this.Show(false);
	}

	private const string kBlueClockImageName = "GUI/LevelSelect/Popups/Clock_Blue";

	private const string kSilverClockImageName = "GUI/LevelSelect/Popups/Clock_Silver";

	private const string kGoldClockImageName = "GUI/LevelSelect/Popups/Clock_Gold";

	private const string kFireClockImageName = "GUI/LevelSelect/Popups/Clock_Fire";

	private GUIDefines.RectInfo m_LeftButtonPos = new GUIDefines.RectInfo
	{
		leftRatio = 0.5583333f,
		topRatio = 0.6541667f,
		widthRatio = 0.1958333f,
		heightRatio = 0.1859375f,
		IPad = new GUIDefines.RectIPadInfo
		{
			leftOffset = -3f,
			topOffset = -59f
		}
	};

	private GUIDefines.RectInfo m_CenterButtonPos = new GUIDefines.RectInfo
	{
		leftRatio = 0.4052083f,
		topRatio = 0.6541667f,
		widthRatio = 0.1958333f,
		heightRatio = 0.1859375f,
		IPad = new GUIDefines.RectIPadInfo
		{
			leftOffset = -3f,
			topOffset = -59f
		}
	};

	public enum Button
	{
		eOk,
		eButton_COUNT
	}

	public enum Texture
	{
		eBackground,
		eClock,
		eTexture_COUNT
	}

	public enum Label
	{
		eTitle,
		eBeatTheClock,
		eBestTime,
		eNewGoal,
		eLabel_COUNT
	}
}
