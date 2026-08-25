using System;
using UnityEngine;

public class LoginPopup : BasePopup
{
	public LoginPopup(GameObject aRefObj)
		: base(aRefObj)
	{
		this.mo_termsOfUsePopup = new TermsOfUsePopup(aRefObj);
		this.mo_termsOfUsePopup.RegisterCallback(new BasePopup.PopupCallback(this.TermsOfUsePopupCallback));
		this.mo_privacyPolicyPopup = new PrivacyPolicyPopup(aRefObj);
		this.mo_privacyPolicyPopup.RegisterCallback(new BasePopup.PopupCallback(this.PrivacyPolicyPopupCallback));
	}

	protected void ResetLayouts()
	{
		this.CreateLayouts();
		this.m_IsPopupInitialized = false;
	}

	protected override void CreateLayouts()
	{
		this.CreateCommonScreenLayout();
		if (NetManager.Instance.IsPlayerLoggedIn())
		{
			this.CreateAlreadyLoggedInScreenLayout();
		}
		else
		{
			this.CreateLoginScreenLayout();
		}
	}

	private void CreateCommonScreenLayout()
	{
		this.m_WindowBackground = null;
		base.WindowData = new GUIDefines.WindowData
		{
			pos = new GUIDefines.RectInfo
			{
				widthRatio = 1f,
				heightRatio = 1f,
				detatchFromRefObject = true
			},
			id = 12
		};
		if (this.mBackgroundTexture == null)
		{
			this.mBackgroundTexture = new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 1f,
					heightRatio = 1f,
					detatchFromRefObject = true
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/CreditsNew/Credits_BG_BlueGradient"
				}
			};
		}
		if (this.mTopNavBarTexture == null)
		{
			if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eIPad)
			{
				this.mTopNavBarTexture = new GUIDefines.TextureData
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
				};
			}
			else
			{
				this.mTopNavBarTexture = new GUIDefines.TextureData
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
				};
			}
		}
		if (this.mBackButton == null)
		{
			if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
			{
				this.mBackButton = new GUIDefines.ButtonData
				{
					buttonId = 5,
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
				this.mBackButton = new GUIDefines.ButtonData
				{
					buttonId = 5,
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.01875f,
						topRatio = 0.021875f,
						widthRatio = 0.109375f,
						heightRatio = 0.0796875f,
						IPad = new GUIDefines.RectIPadInfo()
					},
					detectZoneScale = 1.5f,
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
		if (this.mTitleLabel == null)
		{
			if (LocalizationManager.GetLanguageCode() == "fr" && ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eOriginal)
			{
				this.mTitleLabel = new GUIDefines.LabelData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.015f,
						topRatio = 0.04375f,
						widthRatio = 1f,
						heightRatio = 0.04375f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true,
							topOffset = -10f
						}
					},
					content = new GUIDefines.ContentInfo(),
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customFontSize = GUIDefines.FontSize.eMedium
					}
				};
			}
			else
			{
				this.mTitleLabel = new GUIDefines.LabelData
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
					content = new GUIDefines.ContentInfo(),
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customFontSize = GUIDefines.FontSize.eMedium
					}
				};
			}
		}
	}

	private void CreateLoginScreenLayout()
	{
		GUIDefines.RectInfo rectInfo = null;
		string text = string.Empty;
		string text2 = string.Empty;
		switch (ResolutionManager.Instance.LayoutSize)
		{
		case ResolutionManager.eLayoutSize.eLowres:
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.5f, 2, 0),
				topRatio = 0.815f,
				widthRatio = 0.5f,
				heightRatio = 0.125f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true,
					topOffset = -48f
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eMini);
			text2 = "TXT_TermsAndPrivacyTap2lines";
			break;
		case ResolutionManager.eLayoutSize.eOriginal:
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.5f, 2, 0),
				topRatio = 0.89375f,
				widthRatio = 0.5f,
				heightRatio = 0.046875f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true,
					topOffset = -48f
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eSmall);
			text2 = "TXT_TermsAndPrivacyTap";
			break;
		case ResolutionManager.eLayoutSize.eIPad:
			rectInfo = new GUIDefines.RectInfo
			{
				leftRatio = GUIUtil.FindHorizontalPositionToAlign(0.5f, 2, 0),
				topRatio = 0.878125f,
				widthRatio = 0.5f,
				heightRatio = 0.046875f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true,
					topOffset = -48f
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eSmall);
			text2 = "TXT_TermsAndPrivacyTap";
			break;
		}
		base.TextFieldData = new GUIDefines.TextFieldData[]
		{
			new GUIDefines.TextFieldData
			{
				controlName = "Account",
				defaultTextId = "TXT_PenguinName",
				titleCase = true,
				maxLength = 20,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.53325f,
					topRatio = 0.221875f,
					widthRatio = 0.3645833f,
					heightRatio = 0.128125f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = 43f
					}
				},
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
					customFontSize = GUIDefines.FontSize.eMedium,
					customActiveTextColor = GUIConstants.kBlueColor,
					customFocusedTextColor = GUIConstants.kBlueColor
				}
			},
			new GUIDefines.TextFieldData
			{
				controlName = "Password",
				defaultTextId = "TXT_Password",
				isPassword = true,
				maxLength = 20,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.53325f,
					topRatio = 0.3671875f,
					widthRatio = 0.3645833f,
					heightRatio = 0.128125f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = 23f
					}
				},
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
					customFontSize = GUIDefines.FontSize.eMedium,
					customActiveTextColor = GUIConstants.kBlueColor,
					customFocusedTextColor = GUIConstants.kBlueColor
				}
			}
		};
		base.TextureData = new GUIDefines.TextureData[]
		{
			this.mBackgroundTexture,
			this.mTopNavBarTexture,
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.02395836f,
					topRatio = 0.3109375f,
					widthRatio = 0.4604167f,
					heightRatio = 0.421875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 14f,
						topOffset = 25f
					}
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/CreditsNew/CP_Logo_HiRes"
				}
			},
			new GUIDefines.TextureData
			{
				pos = base.TextFieldData[0].pos,
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/CreateAccountNew/Create_Account_FormBox_Error"
				},
				invisible = true
			},
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.5520833f,
					topRatio = 0.71875f,
					widthRatio = 0.3260417f,
					heightRatio = 0.00625f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = -23f
					}
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/CoinTransfer/Textures/Login_seperator"
				}
			}
		};
		base.ButtonData = new GUIDefines.ButtonData[]
		{
			this.mBackButton,
			new GUIDefines.ButtonData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.5262501f,
					topRatio = 0.5187525f,
					widthRatio = 0.3708333f,
					heightRatio = 0.165625f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = 6f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Login"
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
					customFontSize = GUIDefines.FontSize.eLarge
				},
				useAutoResizeGroup = true
			},
			new GUIDefines.ButtonData
			{
				buttonId = 1,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.5262501f,
					topRatio = 0.7562525f,
					widthRatio = 0.3708333f,
					heightRatio = 0.165625f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = -33f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_CreateAccount"
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
				useAutoResizeGroup = true
			},
			new GUIDefines.ButtonData
			{
				buttonId = 3,
				pos = LoginPopup.GetTermsOfUseButtonPos(rectInfo, LoginPopup.Button.eTOU),
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
				pos = LoginPopup.GetTermsOfUseButtonPos(rectInfo, LoginPopup.Button.ePP),
				detectZoneScale = 1.1f,
				content = new GUIDefines.ContentInfo(),
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true
				}
			}
		};
		base.LabelData = new GUIDefines.LabelData[]
		{
			this.mTitleLabel,
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.01041664f,
					topRatio = 0.2125f,
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
		base.SetLabelTextId(0, "TXT_TransferYourCoins");
	}

	private void CreateAlreadyLoggedInScreenLayout()
	{
		base.TextureData = new GUIDefines.TextureData[]
		{
			this.mBackgroundTexture,
			this.mTopNavBarTexture,
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2947917f,
					topRatio = 0.121875f,
					widthRatio = 0.4197917f,
					heightRatio = 0.3796875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 14f,
						topOffset = 25f
					}
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/CreditsNew/CP_Logo_HiRes"
				}
			}
		};
		base.ButtonData = new GUIDefines.ButtonData[]
		{
			this.mBackButton,
			new GUIDefines.ButtonData
			{
				buttonId = 2,
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2533334f,
					topRatio = 0.7375025f,
					widthRatio = 0.5104167f,
					heightRatio = 0.165625f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = -80f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_LogOut"
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
					customFontSize = GUIDefines.FontSize.eLarge
				}
			}
		};
		base.LabelData = new GUIDefines.LabelData[]
		{
			this.mTitleLabel,
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					topRatio = 0.5171875f,
					widthRatio = 1f,
					heightRatio = 0.125f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = -47f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetString("TXT_Transferring", ProfileManager.Instance.CurrentProfile.ProfileName)
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eMedium
				}
			}
		};
		base.TextFieldData = null;
		base.SetLabelTextId(0, "TXT_ClubPenguin");
	}

	public override void Draw()
	{
		if (!this.CanDraw())
		{
			return;
		}
		base.Draw();
		this.BlockControl(this.mo_termsOfUsePopup.IsShowing || this.mo_privacyPolicyPopup.IsShowing);
		this.mo_termsOfUsePopup.Draw();
		this.mo_privacyPolicyPopup.Draw();
	}

	public void Update()
	{
		this.mo_termsOfUsePopup.Update();
		this.mo_privacyPolicyPopup.Update();
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
				this.AttemptLogin();
				break;
			case 1:
				GameFlowManager.Instance.GUIManager.ShowLoginPopup(false);
				GameFlowManager.Instance.GUIManager.ShowCreateAccountPopup(true);
				break;
			case 2:
				NetManager.Instance.ResetAuthToken();
				this.ResetLayouts();
				break;
			case 3:
				this.mo_termsOfUsePopup.Show(true);
				break;
			case 4:
				this.mo_privacyPolicyPopup.Show(true);
				break;
			case 5:
				GameFlowManager.Instance.GUIManager.LoginPopupToBackTraceScene();
				break;
		}
	}

	private void TermsOfUsePopupCallback(int aSelectedButton) {}
	private void PrivacyPolicyPopupCallback(int aSelectedButton) {}

	private void AttemptLogin()
	{
		this.mUsername = base.TextFieldData[0].editedText;
		this.mPassword = base.TextFieldData[1].editedText;
		NetManager.Instance.Login(this.mUsername, this.mPassword, new BaseNetRequest.RequestCompleteCB(this.LoginCompleteCallback));
	}

	public void LoginCompleteCallback(bool aSuccess)
	{
		this.HideInLineError();
		if (aSuccess)
		{
			ProfileManager.Instance.CurrentProfile.ProfileName = this.mUsername;
			ProfileManager.Instance.SaveCurrentProfile();
			GameFlowManager.Instance.GUIManager.LoginPopupToBackTraceScene();
		}
		else if (NetError.IsUserNameRelatedError(NetManager.Instance.GetLastErrorCode(NetManager.Request.eLogin)))
		{
			this.ShowInLineError(LoginPopup.TextField.eAccount);
		}
		else if (NetError.IsPasswordRelatedError(NetManager.Instance.GetLastErrorCode(NetManager.Request.eLogin)))
		{
			this.ShowInLineError(LoginPopup.TextField.ePassword);
		}
		else
		{
			NetManager.Instance.ShowError(NetManager.Instance.GetLastErrorMsg(NetManager.Request.eLogin), false);
		}
	}

	private void ShowInLineError(LoginPopup.TextField aErrorField)
	{
		if (aErrorField != LoginPopup.TextField.eAccount)
		{
			if (aErrorField != LoginPopup.TextField.ePassword)
			{
				return;
			}
			base.LabelData[1].pos.topRatio = 0.35781252f;
		}
		else
		{
			base.LabelData[1].pos.topRatio = 0.2125f;
		}
		for (int i = 0; i < base.TextFieldData.Length; i++)
		{
			base.TextFieldData[i].isFocused = false;
		}
		base.TextureData[3].pos = base.TextFieldData[(int)aErrorField].pos;
		base.SetTextureInvisible(3, false);
		base.LabelData[1].pos.IPad.topOffset = base.TextFieldData[(int)aErrorField].pos.IPad.topOffset;
		base.LabelData[1].pos.Init();
		base.SetLabelInvisible(1, false);
		base.SetLabelText(1, NetManager.Instance.GetLastErrorMsg(NetManager.Request.eLogin));
	}

	private void HideInLineError()
	{
		base.SetTextureInvisible(3, true);
		base.SetLabelInvisible(1, true);
	}

	private static GUIDefines.RectInfo GetTermsOfUseButtonPos(GUIDefines.RectInfo o_textPos, LoginPopup.Button e_button)
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
		if (e_button == LoginPopup.Button.eTOU)
		{
			num4 = LoginPopup.mto_TOUButtonWidth[num][layoutSize];
			num3 = -num4;
		}
		else
		{
			num4 = LoginPopup.mto_PPButtonWidth[num][layoutSize];
		}
		return new GUIDefines.RectInfo
		{
			leftRatio = o_textPos.leftRatio + LoginPopup.mto_TOUButtonGroupHorizontalOffset[num][layoutSize].x + num3,
			topRatio = o_textPos.topRatio + LoginPopup.mto_TOUButtonGroupHorizontalOffset[num][layoutSize].y,
			widthRatio = num4,
			heightRatio = 0.1f,
			IPad = o_textPos.IPad
		};
	}

	private const float kFirstTextFieldPosY = 0.221875f;
	private const float kNextTextFieldOffsetY = 0.1453125f;
	private const float kFirstErrorBubblePosY = 0.2125f;
	private const float kf_TOUButtonHeight = 0.1f;

	private TermsOfUsePopup mo_termsOfUsePopup;
	private PrivacyPolicyPopup mo_privacyPolicyPopup;

	private string mPassword;
	private string mUsername;

	private GUIDefines.TextureData mBackgroundTexture;
	private GUIDefines.TextureData mTopNavBarTexture;
	private GUIDefines.ButtonData mBackButton;
	private GUIDefines.LabelData mTitleLabel;

	private static Vector2[][] mto_TOUButtonGroupHorizontalOffset = new Vector2[][]
	{
		new Vector2[]
		{
			new Vector2(0.19f, 0.05f),
			new Vector2(0.25f, -0.025f),
			new Vector2(0.25f, -0.025f)
		},
		new Vector2[]
		{
			new Vector2(0.29f, 0.025f),
			new Vector2(0.3575f, -0.025f),
			new Vector2(0.35f, -0.025f)
		},
		new Vector2[]
		{
			new Vector2(0.21f, 0.05f),
			new Vector2(0.275f, -0.025f),
			new Vector2(0.275f, -0.025f)
		},
		new Vector2[]
		{
			new Vector2(0.21f, 0.05f),
			new Vector2(0.275f, -0.025f),
			new Vector2(0.275f, -0.025f)
		}
	};

	private static float[][] mto_TOUButtonWidth = new float[][]
	{
		new float[] { 0.1f, 0.1f, 0.1f },
		new float[] { 0.25f, 0.2f, 0.2f },
		new float[] { 0.2f, 0.14f, 0.14f },
		new float[] { 0.2f, 0.14f, 0.14f }
	};

	private static float[][] mto_PPButtonWidth = new float[][]
	{
		new float[] { 0.1f, 0.1f, 0.1f },
		new float[] { 0.2f, 0.125f, 0.125f },
		new float[] { 0.275f, 0.175f, 0.175f },
		new float[] { 0.275f, 0.175f, 0.175f }
	};

	private enum Button
	{
		eLogin,
		eCreateAccount,
		eLogout,
		eTOU,
		ePP,
		eBack,
		eButton_COUNT
	}

	private enum Label
	{
		eTitle,
		eCommonLabel_COUNT,
		eLoginErrorBubble = 1,
		eLoginLabel_COUNT
	}

	private enum Texture
	{
		eBackground,
		eTopNavBar,
		eCommonTexture_COUNT,
		eLoginCPLogo = 2,
		eLoginErrorHighlight,
		eLoginSeperator,
		eLoginTexture_COUNT,
		eAlreadyLoggedInCPLogo = 2,
		eAlreadyLoggedInTexture_COUNT
	}

	private enum TextField
	{
		eAccount,
		ePassword,
		eTextField_COUNT
	}

	private enum ReturnCode
	{
		eSuccess,
		eFail,
		eReturnCode_COUNT
	}
}
