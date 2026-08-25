using System;
using UnityEngine;

public class CreditsNew : BaseMonoScreen
{
	protected override void CreateMainScreenLayouts()
	{
		base.SetTopBarData("TXT_Back", "TXT_Credits");
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			base.MainScreen.TextureData = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.35253906f, 2, 0),
						topRatio = 0.2765625f,
						widthRatio = 0.35253906f,
						heightRatio = 0.28645834f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/CP_Logo_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.15429688f, 4, 0),
						topRatio = 0.6036458f,
						widthRatio = 0.15429688f,
						heightRatio = 0.17317708f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/Behaviour_Logo_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.253125f, 4, 1),
						topRatio = 0.6036458f,
						widthRatio = 0.23730469f,
						heightRatio = 0.17447917f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/DisneyMobile_Logo_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.13085938f, 1, 0),
						topRatio = 0.9505208f,
						widthRatio = 0.13085938f,
						heightRatio = 0.02994792f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/Disney_Copyright_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.00416667f, 1, 0) + 0.01041167f,
						topRatio = 0.240625f,
						widthRatio = 0.00416667f,
						heightRatio = 0.609375f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/Divider_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.2421875f, 2, 1),
						topRatio = 0.5078125f,
						widthRatio = 0.2421875f,
						heightRatio = 0.08723958f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/Version1_Box_HiRes"
					}
				}
			};
		}
		else
		{
			base.MainScreen.TextureData = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.37604168f, 2, 0),
						topRatio = 0.2375f,
						widthRatio = 0.37604168f,
						heightRatio = 0.34375f,
						IPad = new GUIDefines.RectIPadInfo()
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/CP_Logo_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.16458333f, 4, 0),
						topRatio = 0.6375f,
						widthRatio = 0.16458333f,
						heightRatio = 0.2078125f,
						IPad = new GUIDefines.RectIPadInfo()
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/Behaviour_Logo_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.253125f, 4, 1) - 0.01041167f,
						topRatio = 0.6375f,
						widthRatio = 0.253125f,
						heightRatio = 0.209375f,
						IPad = new GUIDefines.RectIPadInfo()
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/DisneyMobile_Logo_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.13958333f, 1, 0),
						topRatio = 0.9453125f,
						widthRatio = 0.13958333f,
						heightRatio = 0.0359375f,
						IPad = new GUIDefines.RectIPadInfo()
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/Disney_Copyright_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.00416667f, 1, 0) + 0.01041167f,
						topRatio = 0.240625f,
						widthRatio = 0.00416667f,
						heightRatio = 0.609375f,
						IPad = new GUIDefines.RectIPadInfo()
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/Divider_HiRes"
					}
				},
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.25833333f, 2, 1),
						topRatio = 0.5078125f,
						widthRatio = 0.25833333f,
						heightRatio = 0.1046875f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreditsNew/Version1_Box_HiRes"
					}
				}
			};
		}
		GUIDefines.RectInfo rectInfo = null;
		string text = string.Empty;
		string text2 = null;
		switch (ResolutionManager.Instance.LayoutSize)
		{
		case ResolutionManager.eLayoutSize.eLowres:
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = GUIUtil.FindHorizontalPositionToAlign(1f, 1, 0),
				topRatio = 0.9025f,
				widthRatio = 1f,
				heightRatio = 0.046875f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true,
					topOffset = -48f
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eMini);
			text2 = "TXT_TermsAndPrivacyTap";
			break;
		case ResolutionManager.eLayoutSize.eOriginal:
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = GUIUtil.FindHorizontalPositionToAlign(1f, 1, 0),
				topRatio = 0.9f,
				widthRatio = 1f,
				heightRatio = 0.046875f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true,
					topOffset = -48f
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eMedium);
			text2 = "TXT_TermsAndPrivacyTap";
			break;
		case ResolutionManager.eLayoutSize.eIPad:
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = GUIUtil.FindHorizontalPositionToAlign(1f, 1, 0),
				topRatio = 0.97f,
				widthRatio = 1f,
				heightRatio = 0.046875f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true,
					topOffset = -48f
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eMedium);
			text2 = "TXT_TermsAndPrivacyTap";
			break;
		}
		base.MainScreen.ButtonData = new GUIDefines.ButtonData[]
		{
			new GUIDefines.ButtonData
			{
				buttonId = 1,
				pos = CreditsNew.GetTermsOfUseButtonPos(rectInfo, CreditsNew.Button.eTOU),
				detectZoneScale = 1.1f,
				content = new GUIDefines.ContentInfo(),
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true
				}
			},
			new GUIDefines.ButtonData
			{
				buttonId = 2,
				pos = CreditsNew.GetTermsOfUseButtonPos(rectInfo, CreditsNew.Button.ePP),
				detectZoneScale = 1.1f,
				content = new GUIDefines.ContentInfo(),
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true
				}
			}
		};
		base.MainScreen.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.5f, 2, 0),
					topRatio = 0.2265625f,
					widthRatio = 0.5f,
					heightRatio = 0.046875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = 28f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Credits_1"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.25f, 4, 0),
					topRatio = 0.5625f,
					widthRatio = 0.25f,
					heightRatio = 0.046875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = -11f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Credits_2"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.25f, 4, 1),
					topRatio = 0.5625f,
					widthRatio = 0.25f,
					heightRatio = 0.046875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = -11f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Credits_3"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.5f, 2, 1),
					topRatio = 0.2265625f,
					widthRatio = 0.5f,
					heightRatio = 0.046875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = 30f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Credits_4"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.5f, 2, 1),
					topRatio = 0.29375f,
					widthRatio = 0.5f,
					heightRatio = 0.046875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = 20f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Credits_5"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eMedium,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.5f, 2, 1),
					topRatio = 0.359375f,
					widthRatio = 0.5f,
					heightRatio = 0.046875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = 10f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Credits_6"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eMedium,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.25833333f, 2, 1),
					topRatio = 0.5078125f,
					widthRatio = 0.25833333f,
					heightRatio = 0.1046875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = -6f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetString("TXT_Credits_7", Utilities.CurrentBuildString)
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.5f, 2, 1),
					topRatio = 0.7375f,
					widthRatio = 0.5f,
					heightRatio = 0.046875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = -38f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Credits_8"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.5f, 2, 1),
					topRatio = 0.80625f,
					widthRatio = 0.5f,
					heightRatio = 0.046875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = -48f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Credits_9"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontSize = GUIDefines.FontSize.eMedium,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = rectInfo,
				content = new GUIDefines.ContentInfo
				{
					textId = text2
				},
				disableDropShadow = true,
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontType = GUIDefines.FontType.eOnDemand,
					customOnDemandFontName = text,
					customNormalTextColor = GUIConstants.kTOULinkColorColor,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			}
		};
	}

	private void Awake()
	{
		this.Init(base.gameObject);
		this.mo_termsOfUsePopup = new TermsOfUsePopup(base.gameObject);
		this.mo_termsOfUsePopup.RegisterCallback(new BasePopup.PopupCallback(this.TermsOfUsePopupCallback));
		this.mo_privacyPolicyPopup = new PrivacyPolicyPopup(base.gameObject);
		this.mo_privacyPolicyPopup.RegisterCallback(new BasePopup.PopupCallback(this.PrivacyPolicyPopupCallback));
	}

	private void TermsOfUsePopupCallback(int aSelectedButton)
	{
	}

	private void PrivacyPolicyPopupCallback(int aSelectedButton)
	{
	}

	private void OnGUI()
	{
		if (!base.MainScreen.CanDraw())
		{
			return;
		}
		base.MainScreen.Draw();
		base.DrawTopBar();
		this.mo_termsOfUsePopup.Draw();
		this.mo_privacyPolicyPopup.Draw();
		this.BlockControl(this.mo_termsOfUsePopup.IsShowing || this.mo_privacyPolicyPopup.IsShowing);
	}

	private new void Update()
	{
		this.HandleButtonSelect();
		if (this.mo_termsOfUsePopup != null)
		{
			this.mo_termsOfUsePopup.Update();
		}
		if (this.mo_privacyPolicyPopup != null)
		{
			this.mo_privacyPolicyPopup.Update();
		}
	}

	protected override void OnMainScreenButtonSelect()
	{
		CreditsNew.Button selectedButton = (CreditsNew.Button)base.MainScreen.SelectedButton;
		if (selectedButton != CreditsNew.Button.eTOU)
		{
			if (selectedButton == CreditsNew.Button.ePP)
			{
				this.mo_privacyPolicyPopup.Show(true);
			}
		}
		else
		{
			this.mo_termsOfUsePopup.Show(true);
		}
	}

	protected override void OnBack()
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		base.MainScreen.StopGUI();
		GameFlowManager.Instance.LoadScene("AboutCP", false);
	}

	private static GUIDefines.RectInfo GetTermsOfUseButtonPos(GUIDefines.RectInfo o_textPos, CreditsNew.Button e_button)
	{
		int num = 0;
		string languageCode = LocalizationManager.GetLanguageCode();
		switch (languageCode)
		{
		case "en":
			num = 0;
			break;
		case "fr":
			num = 1;
			break;
		case "pt":
			num = 2;
			break;
		case "es":
			num = 3;
			break;
		}
		int layoutSize = (int)ResolutionManager.Instance.LayoutSize;
		float num3 = 0f;
		float num4;
		if (e_button == CreditsNew.Button.eTOU)
		{
			num4 = CreditsNew.mto_TOUButtonWidth[num][layoutSize];
			num3 = -num4;
		}
		else
		{
			num4 = CreditsNew.mto_PPButtonWidth[num][layoutSize];
		}
		return new GUIDefines.RectInfo
		{
			leftRatio = o_textPos.leftRatio + CreditsNew.mto_TOUButtonGroupHorizontalOffset[num][layoutSize].x + num3,
			topRatio = o_textPos.topRatio + CreditsNew.mto_TOUButtonGroupHorizontalOffset[num][layoutSize].y,
			widthRatio = num4,
			heightRatio = 0.1f,
			IPad = o_textPos.IPad
		};
	}

	private const float kf_TOUButtonHeight = 0.1f;

	private TermsOfUsePopup mo_termsOfUsePopup;

	private PrivacyPolicyPopup mo_privacyPolicyPopup;

	private static Vector2[][] mto_TOUButtonGroupHorizontalOffset = new Vector2[][]
	{
		new Vector2[]
		{
			new Vector2(0.5f, -0.025f),
			new Vector2(0.5f, -0.025f),
			new Vector2(0.5f, -0.025f)
		},
		new Vector2[]
		{
			new Vector2(0.66f, -0.025f),
			new Vector2(0.64f, -0.025f),
			new Vector2(0.635f, -0.025f)
		},
		new Vector2[]
		{
			new Vector2(0.53f, -0.025f),
			new Vector2(0.53f, -0.025f),
			new Vector2(0.53f, -0.025f)
		},
		new Vector2[]
		{
			new Vector2(0.525f, -0.025f),
			new Vector2(0.525f, -0.025f),
			new Vector2(0.525f, -0.025f)
		}
	};

	private static float[][] mto_TOUButtonWidth = new float[][]
	{
		new float[] { 0.1f, 0.1f, 0.1f },
		new float[] { 0.3f, 0.3f, 0.3f },
		new float[] { 0.2f, 0.2f, 0.2f },
		new float[] { 0.2f, 0.2f, 0.2f }
	};

	private static float[][] mto_PPButtonWidth = new float[][]
	{
		new float[] { 0.1f, 0.1f, 0.1f },
		new float[] { 0.2f, 0.175f, 0.175f },
		new float[] { 0.25f, 0.25f, 0.25f },
		new float[] { 0.25f, 0.25f, 0.25f }
	};

	private enum Button
	{
		eBack,
		eTOU,
		ePP,
		eButton_COUNT
	}
}
