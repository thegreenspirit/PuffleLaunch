using System;
using UnityEngine;

public class CreateAccountPopup : BasePopup
{
	public CreateAccountPopup(GameObject aRefObj)
		: base(aRefObj)
	{
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			this.m_ColorPickerZone = new ColorPickerZone(aRefObj, new GUIDefines.RectInfo
			{
				leftRatio = 0.5614974f,
				topRatio = 0.64427084f,
				widthRatio = 0.416042f,
				heightRatio = 0.15760417f
			}, new Vector2(0.05371094f, 0.06901042f), null, "GUI/CreateAccountNew/IPad/ColorPicker/Create_Account_ColorBox", null, "GUI/CreateAccountNew/IPad/ColorPicker/Create_Account_ColorBox_HiLite", 7, 2);
			this.m_ColorPickerZone.RegisterCallback(new ColorPickerZone.ColorPickerZoneCallback(this.ColorPickupPopupCallback));
		}
		else
		{
			this.m_ColorPickerPopup = new ColorPickerPopup(aRefObj, new GUIDefines.RectInfo
			{
				leftRatio = 0.04166667f,
				topRatio = 0.1921875f,
				widthRatio = 0.9166667f,
				heightRatio = 0.615625f
			}, new GUIDefines.RectInfo
			{
				leftRatio = 0.09375f,
				topRatio = 0.2684375f,
				widthRatio = 0.8125f,
				heightRatio = 0.078125f
			}, GUIConstants.kWhiteColor, new GUIDefines.RectInfo
			{
				leftRatio = 0.09256133f,
				topRatio = 0.3921875f,
				widthRatio = 0.81562465f,
				heightRatio = 0.3625f
			}, new Vector2(0.10520833f, 0.1515625f), "GUI/CreateAccountNew/ColorPicker/Create_Account_ColorPopUpWindow", null, "GUI/CreateAccountNew/ColorPicker/Create_Account_ColorBox", null, "GUI/CreateAccountNew/ColorPicker/Create_Account_ColorBox_HiLite", 7, 2);
			this.m_ColorPickerPopup.RegisterCallback(new BasePopup.PopupCallback(this.ColorPickupPopupCallback));
		}
		this.mo_termsOfUsePopup = new TermsOfUsePopup(aRefObj);
		this.mo_termsOfUsePopup.RegisterCallback(new BasePopup.PopupCallback(this.TermsOfUsePopupCallback));
		this.mo_privacyPolicyPopup = new PrivacyPolicyPopup(aRefObj);
		this.mo_privacyPolicyPopup.RegisterCallback(new BasePopup.PopupCallback(this.PrivacyPolicyPopupCallback));
	}

	protected override void CreateLayouts()
	{
		GUIDefines.RectInfo rectInfo = null;
		string text = string.Empty;
		switch (ResolutionManager.Instance.LayoutSize)
		{
		case ResolutionManager.eLayoutSize.eLowres:
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = 0.51f,
				topRatio = 0.7265625f,
				widthRatio = 0.48f,
				heightRatio = 0.071875f
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eMini);
			break;
		case ResolutionManager.eLayoutSize.eOriginal:
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = 0.52f,
				topRatio = 0.7265625f,
				widthRatio = 0.46f,
				heightRatio = 0.071875f
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eMedium);
			break;
		case ResolutionManager.eLayoutSize.eIPad:
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = 0.5688281f,
				topRatio = 0.7994272f,
				widthRatio = 0.406f,
				heightRatio = 0.0703125f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eSmall);
			break;
		}
		base.WindowData = new GUIDefines.WindowData
		{
			pos = new GUIDefines.RectInfo
			{
				widthRatio = 1f,
				heightRatio = 1f,
				detatchFromRefObject = true
			},
			id = 11
		};
		this.m_WindowBackground = null;
		ResolutionManager.eLayoutSize layoutSize = ResolutionManager.Instance.LayoutSize;
		if (layoutSize != ResolutionManager.eLayoutSize.eIPad)
		{
			this.mto_textFieldPositions = new GUIDefines.RectInfo[]
			{
				new GUIDefines.RectInfo
				{
					leftRatio = 0.530208f,
					topRatio = 0.140625f,
					widthRatio = 0.4354167f,
					heightRatio = 0.1234375f
				},
				new GUIDefines.RectInfo
				{
					leftRatio = 0.530208f,
					topRatio = 0.296875f,
					widthRatio = 0.4354167f,
					heightRatio = 0.1234375f
				},
				new GUIDefines.RectInfo
				{
					leftRatio = 0.530208f,
					topRatio = 0.453125f,
					widthRatio = 0.4354167f,
					heightRatio = 0.1234375f
				},
				new GUIDefines.RectInfo
				{
					leftRatio = 0.530208f,
					topRatio = 0.609375f,
					widthRatio = 0.4354167f,
					heightRatio = 0.1234375f
				}
			};
			this.mto_errorBubblePositions = new GUIDefines.RectInfo[4];
			float num = 0.5145833f;
			float num2 = 0.1484375f;
			float num3 = 0.01041664f;
			float num4 = -0.015625f;
			for (int i = 0; i < 4; i++)
			{
				this.mto_errorBubblePositions[i] = new GUIDefines.RectInfo
				{
					leftRatio = num3,
					topRatio = this.mto_textFieldPositions[i].topRatio + num4,
					widthRatio = num,
					heightRatio = num2,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				};
			}
			this.mto_bgTexture = new GUIDefines.TextureData[]
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
						name = "GUI/CreateAccountNew/Create_Account_BG"
					}
				}
			};
			for (int j = 0; j < this.mto_bgTexture.Length; j++)
			{
				this.mto_bgTexture[j].Init();
			}
			base.ButtonData = new GUIDefines.ButtonData[]
			{
				new GUIDefines.ButtonData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.025f,
						topRatio = 0.15625f,
						widthRatio = 0.10833333f,
						heightRatio = 0.1625f
					},
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_ColorPicker"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_ColorPicker_pressed"
						}
					}
				},
				new GUIDefines.ButtonData
				{
					buttonId = 2,
					pos = CreateAccountPopup.GetTermsOfUseButtonPos(rectInfo, CreateAccountPopup.Button.eTOU),
					detectZoneScale = 1.1f,
					content = new GUIDefines.ContentInfo(),
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true
					}
				},
				new GUIDefines.ButtonData
				{
					buttonId = 3,
					pos = CreateAccountPopup.GetTermsOfUseButtonPos(rectInfo, CreateAccountPopup.Button.ePP),
					detectZoneScale = 1.1f,
					content = new GUIDefines.ContentInfo(),
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true
					}
				},
				new GUIDefines.ButtonData
				{
					buttonId = 1,
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.53333336f,
						topRatio = 0.8046875f,
						widthRatio = 0.425f,
						heightRatio = 0.165625f
					},
					content = new GUIDefines.ContentInfo
					{
						textId = "TXT_CreateYourPenguin"
					},
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/Common/button"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/Common/button_pressed"
						},
						customFontSize = GUIDefines.FontSize.eMedium
					},
					autoResizeAllignment = GUIDefines.AutoResizeAllignment.eCenter
				},
				new GUIDefines.ButtonData
				{
					buttonId = 4,
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
						},
						customFontSize = GUIDefines.FontSize.eMedium
					}
				}
			};
			GUIDefines.RectInfo rectInfo2 = new GUIDefines.RectInfo
			{
				leftRatio = 0.03645833f,
				topRatio = 0.196875f,
				widthRatio = 0.42916667f,
				heightRatio = 0.8f
			};
			base.TextureData = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = this.mto_textFieldPositions[2],
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Create_Account_FormBox_Error"
					},
					invisible = true
				},
				new GUIDefines.TextureData
				{
					pos = rectInfo2,
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Penguin/fat_penguin_grey_noeyes"
					},
					bgInfo = new GUIDefines.BackgroundInfo
					{
						useBgColor = true,
						bgColor = Utilities.m_cPenguinColors[0]
					}
				},
				new GUIDefines.TextureData
				{
					pos = rectInfo2,
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Penguin/fat_penguin_dark_shadow"
					},
					bgInfo = new GUIDefines.BackgroundInfo
					{
						useBgColor = true,
						bgColor = Utilities.m_cPenguinShadowColors[0]
					}
				},
				new GUIDefines.TextureData
				{
					pos = rectInfo2,
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Penguin/fat_penguin_highlight"
					},
					bgInfo = new GUIDefines.BackgroundInfo
					{
						useBgColor = true,
						bgColor = Utilities.m_cPenguinHightlightColors[0]
					}
				},
				new GUIDefines.TextureData
				{
					pos = rectInfo2,
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Penguin/fat_penguin_inside"
					}
				},
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
			base.LabelData = new GUIDefines.LabelData[]
			{
				new GUIDefines.LabelData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.01041664f,
						widthRatio = 0.5145833f,
						heightRatio = 0.1484375f,
						IPad = new GUIDefines.RectIPadInfo
						{
							leftOffset = 34f
						}
					},
					content = new GUIDefines.ContentInfo(),
					invisible = true,
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_ErrorBox"
						},
						customFontSize = GUIDefines.FontSize.eSmall,
						customNormalTextColor = GUIConstants.kRedColor,
						customWordWrap = true,
						customPadding = new GUIDefines.Vector2Info
						{
							xRatio = 0.009375f,
							yRatio = 0.0078125f
						},
						customPadding2 = new GUIDefines.Vector2Info
						{
							xRatio = 0.03645833f
						}
					}
				},
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
						textId = "TXT_CPAccount"
					}
				},
				new GUIDefines.LabelData
				{
					pos = rectInfo,
					content = new GUIDefines.ContentInfo
					{
						textId = "TXT_TOU_Link"
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
			base.TextFieldData = new GUIDefines.TextFieldData[]
			{
				new GUIDefines.TextFieldData
				{
					controlName = "Name",
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.530208f,
						topRatio = 0.1703125f,
						widthRatio = 0.4333333f,
						heightRatio = 0.1109375f
					},
					maxLength = 20,
					defaultTextId = "TXT_PenguinName",
					titleCase = true,
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox_Focus"
						},
						customActiveTextColor = GUIConstants.kBlueColor,
						customFocusedTextColor = GUIConstants.kBlueColor
					}
				},
				new GUIDefines.TextFieldData
				{
					controlName = "Email",
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.53020835f,
						topRatio = 0.3234375f,
						widthRatio = 0.4333333f,
						heightRatio = 0.1109375f
					},
					maxLength = 40,
					defaultTextId = "TXT_Email",
					keyboardType = TouchScreenKeyboardType.EmailAddress,
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox_Focus"
						},
						customActiveTextColor = GUIConstants.kBlueColor,
						customFocusedTextColor = GUIConstants.kBlueColor
					}
				},
				new GUIDefines.TextFieldData
				{
					controlName = "Password",
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.530208f,
						topRatio = 0.475f,
						widthRatio = 0.4333333f,
						heightRatio = 0.1109375f
					},
					isPassword = true,
					maxLength = 20,
					defaultTextId = "TXT_Password",
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox_Focus"
						},
						customActiveTextColor = GUIConstants.kBlueColor,
						customFocusedTextColor = GUIConstants.kBlueColor
					}
				},
				new GUIDefines.TextFieldData
				{
					controlName = "ReTypePassword",
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.530208f,
						topRatio = 0.621875f,
						widthRatio = 0.4333333f,
						heightRatio = 0.1109375f
					},
					isPassword = true,
					maxLength = 20,
					defaultTextId = "TXT_ReTypePassword",
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox_Focus"
						},
						customActiveTextColor = GUIConstants.kBlueColor,
						customFocusedTextColor = GUIConstants.kBlueColor
					}
				}
			};
			this.mto_textFieldErrorHighlights = new GUIDefines.TextureData[4];
			for (int k = 0; k < 4; k++)
			{
				this.mto_textFieldErrorHighlights[k] = new GUIDefines.TextureData
				{
					pos = this.mto_textFieldPositions[k],
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Create_Account_FormBox_Error"
					}
				};
				this.mto_textFieldErrorHighlights[k].Init();
				base.TextFieldData[k].pos = this.mto_textFieldPositions[k];
			}
		}
		else
		{
			this.mto_textFieldPositions = new GUIDefines.RectInfo[]
			{
				new GUIDefines.RectInfo
				{
					leftRatio = 0.5458984f,
					topRatio = 0.121875f,
					widthRatio = 0.42285156f,
					heightRatio = 0.10677083f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				new GUIDefines.RectInfo
				{
					leftRatio = 0.5458984f,
					topRatio = 0.2390625f,
					widthRatio = 0.42285156f,
					heightRatio = 0.10677083f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				new GUIDefines.RectInfo
				{
					leftRatio = 0.5458984f,
					topRatio = 0.35625f,
					widthRatio = 0.42285156f,
					heightRatio = 0.10677083f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				new GUIDefines.RectInfo
				{
					leftRatio = 0.5458984f,
					topRatio = 0.4734375f,
					widthRatio = 0.42285156f,
					heightRatio = 0.10677083f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				}
			};
			this.mto_errorBubblePositions = new GUIDefines.RectInfo[4];
			float num5 = 0.5145833f;
			float num6 = 0.1484375f;
			float num7 = 0.01041664f;
			float num8 = -0.02604167f;
			for (int l = 0; l < 4; l++)
			{
				this.mto_errorBubblePositions[l] = new GUIDefines.RectInfo
				{
					leftRatio = num7,
					topRatio = this.mto_textFieldPositions[l].topRatio + num8,
					widthRatio = num5,
					heightRatio = num6,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				};
			}
			this.mto_bgTexture = new GUIDefines.TextureData[]
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
						name = "GUI/CreateAccountNew/Create_Account_BG"
					}
				}
			};
			for (int m = 0; m < this.mto_bgTexture.Length; m++)
			{
				this.mto_bgTexture[m].Init();
			}
			base.ButtonData = new GUIDefines.ButtonData[]
			{
				new GUIDefines.ButtonData
				{
					buttonId = 1,
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.5489583f,
						topRatio = 0.8625f,
						widthRatio = 0.41308594f,
						heightRatio = 0.12369792f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					content = new GUIDefines.ContentInfo
					{
						textId = "TXT_CreateYourPenguin"
					},
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/Common/button"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/Common/button_pressed"
						},
						customFontSize = GUIDefines.FontSize.eMedium
					},
					autoResizeAllignment = GUIDefines.AutoResizeAllignment.eCenter
				},
				new GUIDefines.ButtonData
				{
					buttonId = 2,
					pos = CreateAccountPopup.GetTermsOfUseButtonPos(rectInfo, CreateAccountPopup.Button.eTOU),
					detectZoneScale = 1.1f,
					content = new GUIDefines.ContentInfo(),
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true
					}
				},
				new GUIDefines.ButtonData
				{
					buttonId = 3,
					pos = CreateAccountPopup.GetTermsOfUseButtonPos(rectInfo, CreateAccountPopup.Button.ePP),
					detectZoneScale = 1.1f,
					content = new GUIDefines.ContentInfo(),
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true
					}
				},
				new GUIDefines.ButtonData
				{
					buttonId = 4,
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
				}
			};
			GUIDefines.RectInfo rectInfo3 = new GUIDefines.RectInfo
			{
				leftRatio = 0.03645833f,
				topRatio = 0.196875f,
				widthRatio = 0.40039062f,
				heightRatio = 0.6666667f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true
				}
			};
			base.TextureData = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = this.mto_textFieldPositions[2],
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Create_Account_FormBox_Error"
					},
					invisible = true
				},
				new GUIDefines.TextureData
				{
					pos = rectInfo3,
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Penguin/fat_penguin_grey_noeyes"
					},
					bgInfo = new GUIDefines.BackgroundInfo
					{
						useBgColor = true,
						bgColor = Utilities.m_cPenguinColors[0]
					}
				},
				new GUIDefines.TextureData
				{
					pos = rectInfo3,
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Penguin/fat_penguin_dark_shadow"
					},
					bgInfo = new GUIDefines.BackgroundInfo
					{
						useBgColor = true,
						bgColor = Utilities.m_cPenguinShadowColors[0]
					}
				},
				new GUIDefines.TextureData
				{
					pos = rectInfo3,
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Penguin/fat_penguin_highlight"
					},
					bgInfo = new GUIDefines.BackgroundInfo
					{
						useBgColor = true,
						bgColor = Utilities.m_cPenguinHightlightColors[0]
					}
				},
				new GUIDefines.TextureData
				{
					pos = rectInfo3,
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Penguin/fat_penguin_inside"
					}
				},
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
			base.LabelData = new GUIDefines.LabelData[]
			{
				new GUIDefines.LabelData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.01041664f,
						widthRatio = 0.5145833f,
						heightRatio = 0.1484375f,
						IPad = new GUIDefines.RectIPadInfo
						{
							leftOffset = 34f
						}
					},
					content = new GUIDefines.ContentInfo(),
					invisible = true,
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_ErrorBox"
						},
						customFontSize = GUIDefines.FontSize.eSmall,
						customNormalTextColor = GUIConstants.kRedColor,
						customWordWrap = true,
						customPadding = new GUIDefines.Vector2Info
						{
							xRatio = 0.009375f,
							yRatio = 0.0078125f
						},
						customPadding2 = new GUIDefines.Vector2Info
						{
							xRatio = 0.03645833f
						}
					}
				},
				new GUIDefines.LabelData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.55334675f,
						topRatio = 0.5869793f,
						widthRatio = 0.5f,
						heightRatio = 0.04375f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					content = new GUIDefines.ContentInfo
					{
						textId = "TXT_ChooseColor"
					},
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customFontSize = GUIDefines.FontSize.eSmall,
						customNormalTextColor = GUIConstants.kWhiteColor,
						useCustomTextAlignment = true,
						customTextAlignment = TextAnchor.MiddleLeft,
						customWordWrap = true
					}
				},
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
						textId = "TXT_CPAccount"
					}
				},
				new GUIDefines.LabelData
				{
					pos = rectInfo,
					content = new GUIDefines.ContentInfo
					{
						textId = "TXT_TOU_Link"
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
			base.TextFieldData = new GUIDefines.TextFieldData[]
			{
				new GUIDefines.TextFieldData
				{
					controlName = "Name",
					pos = this.mto_textFieldPositions[0],
					maxLength = 20,
					defaultTextId = "TXT_PenguinName",
					titleCase = true,
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox_Focus"
						},
						customActiveTextColor = GUIConstants.kBlueColor,
						customFocusedTextColor = GUIConstants.kBlueColor
					}
				},
				new GUIDefines.TextFieldData
				{
					controlName = "Email",
					pos = this.mto_textFieldPositions[1],
					maxLength = 40,
					defaultTextId = "TXT_Email",
					keyboardType = TouchScreenKeyboardType.EmailAddress,
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox_Focus"
						},
						customActiveTextColor = GUIConstants.kBlueColor,
						customFocusedTextColor = GUIConstants.kBlueColor
					}
				},
				new GUIDefines.TextFieldData
				{
					controlName = "Password",
					pos = this.mto_textFieldPositions[2],
					isPassword = true,
					maxLength = 20,
					defaultTextId = "TXT_Password",
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox_Focus"
						},
						customActiveTextColor = GUIConstants.kBlueColor,
						customFocusedTextColor = GUIConstants.kBlueColor
					}
				},
				new GUIDefines.TextFieldData
				{
					controlName = "ReTypePassword",
					pos = this.mto_textFieldPositions[3],
					isPassword = true,
					maxLength = 20,
					defaultTextId = "TXT_ReTypePassword",
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/CreateAccountNew/Create_Account_FormBox_Focus"
						},
						customActiveTextColor = GUIConstants.kBlueColor,
						customFocusedTextColor = GUIConstants.kBlueColor
					}
				}
			};
			this.mto_textFieldErrorHighlights = new GUIDefines.TextureData[4];
			for (int n = 0; n < 4; n++)
			{
				this.mto_textFieldErrorHighlights[n] = new GUIDefines.TextureData
				{
					pos = this.mto_textFieldPositions[n],
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/Create_Account_FormBox_Error"
					}
				};
				this.mto_textFieldErrorHighlights[n].Init();
				base.TextFieldData[n].pos = this.mto_textFieldPositions[n];
			}
		}
	}

	protected void OnBack()
	{
		GameFlowManager.Instance.GUIManager.ShowCreateAccountPopup(false);
		GameFlowManager.Instance.GUIManager.ShowLoginPopup(true);
	}

	public override void Draw()
	{
		if (!this.CanDraw())
		{
			return;
		}
		base.Draw();
		if (ResolutionManager.Instance.LayoutSize != ResolutionManager.eLayoutSize.eIPad)
		{
			this.m_ColorPickerPopup.Draw();
			this.BlockControl(this.m_ColorPickerPopup.IsShowing || this.mo_termsOfUsePopup.IsShowing || this.mo_privacyPolicyPopup.IsShowing);
		}
		this.mo_termsOfUsePopup.Draw();
		this.mo_privacyPolicyPopup.Draw();
	}

	protected override void DrawWindowContent(int aWindowId)
	{
		GUICompoundControls.Textures(base.LocalTransform.position, this.mto_bgTexture);
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			this.m_ColorPickerZone.Draw();
			this.BlockControl(this.mo_termsOfUsePopup.IsShowing || this.mo_privacyPolicyPopup.IsShowing);
		}
		base.DrawWindowContent(aWindowId);
	}

	public void Update()
	{
		this.mo_termsOfUsePopup.Update();
		this.mo_privacyPolicyPopup.Update();
	}

	protected override void OnButtonSelect()
	{
		if (base.TextFieldData != null)
		{
			for (int i = 0; i < base.TextFieldData.Length; i++)
			{
				base.TextFieldData[i].isFocused = false;
			}
		}
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		switch (base.SelectedButton)
		{
		case 0:
			this.m_ColorPickerPopup.Show(true);
			break;
		case 1:
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			if (base.TextFieldData[0].editedText != null)
			{
				text = base.TextFieldData[0].editedText;
			}
			if (base.TextFieldData[0].editedText != null)
			{
				text2 = base.TextFieldData[1].editedText;
			}
			if (base.TextFieldData[0].editedText != null)
			{
				text3 = base.TextFieldData[2].editedText;
			}
			if (base.TextFieldData[0].editedText != null)
			{
				text4 = base.TextFieldData[3].editedText;
			}
			int num;
			if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
			{
				num = (int)this.m_ColorPickerZone.SelectedColor;
			}
			else
			{
				num = (int)this.m_ColorPickerPopup.SelectedColor;
			}
			NetManager.Instance.CreateCPAccount(text, text3, text4, text2, num, new BaseNetRequest.RequestCompleteCB(this.CreateAccountCompleteCallback));
			break;
		}
		case 2:
			this.mo_termsOfUsePopup.Show(true);
			break;
		case 3:
			this.mo_privacyPolicyPopup.Show(true);
			break;
		case 4:
			this.OnBack();
			break;
		}
	}

	private void UpdatePenguinColor(int aColorID)
	{
		if (aColorID < 16)
		{
			base.TextureData[2].bgInfo.bgColor = Utilities.m_cPenguinShadowColors[aColorID];
			base.TextureData[1].bgInfo.bgColor = Utilities.m_cPenguinColors[aColorID];
			base.TextureData[3].bgInfo.bgColor = Utilities.m_cPenguinHightlightColors[aColorID];
		}
	}

	private void GoToNextSceneAfterSuccess()
	{
		GameFlowManager.Instance.GUIManager.CreateAccountPopupToBackTraceScene();
	}

	private void ColorPickupPopupCallback(int aSelectedButton)
	{
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			this.UpdatePenguinColor((int)this.m_ColorPickerZone.SelectedColor);
		}
		else
		{
			this.UpdatePenguinColor((int)this.m_ColorPickerPopup.SelectedColor);
		}
	}

	private void TermsOfUsePopupCallback(int aSelectedButton)
	{
	}

	private void PrivacyPolicyPopupCallback(int aSelectedButton)
	{
	}

	private void CreateAccountCompleteCallback(bool aSuccess)
	{
		if (aSuccess)
		{
			ProfileManager.Instance.CurrentProfile.ProfileName = base.TextFieldData[0].editedText;
			ProfileManager.Instance.SaveCurrentProfile();
			this.GoToNextSceneAfterSuccess();
		}
		else if (NetError.IsUserNameRelatedError(NetManager.Instance.GetLastErrorCode(NetManager.Request.eCreateAccount)))
		{
			this.ShowInLineError(CreateAccountPopup.CreateAccountTextField.eName);
		}
		else if (NetError.IsEmailRelatedError(NetManager.Instance.GetLastErrorCode(NetManager.Request.eCreateAccount)))
		{
			this.ShowInLineError(CreateAccountPopup.CreateAccountTextField.eEmail);
		}
		else if (NetError.IsPasswordRelatedError(NetManager.Instance.GetLastErrorCode(NetManager.Request.eCreateAccount)))
		{
			this.ShowInLineError(CreateAccountPopup.CreateAccountTextField.ePassword);
		}
		else if (NetError.IsPasswordMismatchError(NetManager.Instance.GetLastErrorCode(NetManager.Request.eCreateAccount)))
		{
			this.ShowInLineError(CreateAccountPopup.CreateAccountTextField.eRetypePassword);
		}
		else
		{
			NetManager.Instance.ShowError(NetManager.Instance.GetLastErrorMsg(NetManager.Request.eCreateAccount), false);
		}
	}

	private void ShowInLineError(CreateAccountPopup.CreateAccountTextField aErrorField)
	{
		for (int i = 0; i < base.TextFieldData.Length; i++)
		{
			base.TextFieldData[i].isFocused = false;
		}
		base.TextureData[0].pos = base.TextFieldData[(int)aErrorField].pos;
		base.SetTextureInvisible(0, false);
		base.LabelData[0].pos = this.mto_errorBubblePositions[(int)aErrorField];
		base.LabelData[0].pos.Init();
		base.SetLabelInvisible(0, false);
		base.SetLabelText(0, NetManager.Instance.GetLastErrorMsg(NetManager.Request.eCreateAccount));
	}

	private void HideInLineError()
	{
		base.SetTextureInvisible(0, true);
		base.SetLabelInvisible(0, true);
	}

	private static GUIDefines.RectInfo GetTermsOfUseButtonPos(GUIDefines.RectInfo o_textPos, CreateAccountPopup.Button e_button)
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
		if (e_button == CreateAccountPopup.Button.eTOU)
		{
			num4 = CreateAccountPopup.mto_TOUButtonWidth[num][layoutSize];
			num3 = -num4;
		}
		else
		{
			num4 = CreateAccountPopup.mto_PPButtonWidth[num][layoutSize];
		}
		return new GUIDefines.RectInfo
		{
			leftRatio = o_textPos.leftRatio + CreateAccountPopup.mto_TOUButtonGroupHorizontalOffset[num][layoutSize].x + num3,
			topRatio = o_textPos.topRatio + CreateAccountPopup.mto_TOUButtonGroupHorizontalOffset[num][layoutSize].y,
			widthRatio = num4,
			heightRatio = CreateAccountPopup.mtf_TOUButtonHeight[layoutSize],
			IPad = o_textPos.IPad
		};
	}

	private GUIDefines.TextureData[] mto_textFieldErrorHighlights;

	private GUIDefines.TextureData[] mto_bgTexture;

	private ColorPickerPopup m_ColorPickerPopup;

	private ColorPickerZone m_ColorPickerZone;

	private TermsOfUsePopup mo_termsOfUsePopup;

	private PrivacyPolicyPopup mo_privacyPolicyPopup;

	private GUIDefines.RectInfo[] mto_textFieldPositions;

	private GUIDefines.RectInfo[] mto_errorBubblePositions;

	private static Vector2[][] mto_TOUButtonGroupHorizontalOffset = new Vector2[][]
	{
		new Vector2[]
		{
			new Vector2(0.175f, 0f),
			new Vector2(0.175f, -0.005f),
			new Vector2(0.165f, 0f)
		},
		new Vector2[]
		{
			new Vector2(0.285f, 0f),
			new Vector2(0.26f, -0.005f),
			new Vector2(0.225f, 0f)
		},
		new Vector2[]
		{
			new Vector2(0.195f, 0f),
			new Vector2(0.195f, -0.005f),
			new Vector2(0.185f, 0f)
		},
		new Vector2[]
		{
			new Vector2(0.185f, 0f),
			new Vector2(0.185f, -0.005f),
			new Vector2(0.18f, 0f)
		}
	};

	private static float[][] mto_TOUButtonWidth = new float[][]
	{
		new float[] { 0.15f, 0.15f, 0.075f },
		new float[] { 0.275f, 0.275f, 0.175f },
		new float[] { 0.175f, 0.175f, 0.125f },
		new float[] { 0.175f, 0.175f, 0.125f }
	};

	private static float[][] mto_PPButtonWidth = new float[][]
	{
		new float[] { 0.2f, 0.2f, 0.15f },
		new float[] { 0.2f, 0.2f, 0.15f },
		new float[] { 0.3f, 0.25f, 0.175f },
		new float[] { 0.3f, 0.25f, 0.175f }
	};

	private static float[] mtf_TOUButtonHeight = new float[] { 0.085f, 0.085f, 0.065f };

	public enum Button
	{
		eColorPicker,
		eCreatePenguin,
		eTOU,
		ePP,
		eBack,
		eButton_COUNT
	}

	public enum CreateAccountTextField
	{
		eName,
		eEmail,
		ePassword,
		eRetypePassword,
		eCount,
		eNone
	}

	public enum CreateAccountTexture
	{
		eErrorHighlight,
		ePenguinGreyNoEyes,
		ePenguinDarkShadow,
		ePenguinHighlight,
		ePenguinInside,
		eCount,
		eNone
	}

	public enum CreateAccountLabel
	{
		eErrorBubble,
		eChoseYourColor,
		eCount,
		eNone
	}
}
