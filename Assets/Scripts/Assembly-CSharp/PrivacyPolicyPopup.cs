using System;
using UnityEngine;

public class PrivacyPolicyPopup : BasePopup
{
	public PrivacyPolicyPopup(GameObject aRefObj)
		: base(aRefObj)
	{
		this.m_ErrorPopup = new GenericPopup(aRefObj);
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			this.mo_scrollableListRectInfo = new GUIDefines.RectInfo
			{
				leftRatio = 0.26f,
				topRatio = 0.3325f,
				widthRatio = 0.45f,
				heightRatio = 0.4609375f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true
				}
			};
			this.mo_scrollableListRectInfo.Init();
		}
		else
		{
			this.mo_scrollableListRectInfo = new GUIDefines.RectInfo
			{
				leftRatio = 0.25f,
				topRatio = 0.3f,
				widthRatio = 0.45f,
				heightRatio = 0.5546875f,
				IPad = new GUIDefines.RectIPadInfo
				{
					keepSizeRatio = true
				}
			};
			this.mo_scrollableListRectInfo.Init();
		}
		this.mo_termsOfUseList = new PrivacyPolicyList(aRefObj, this.mo_scrollableListRectInfo, ScrollableGUI.ScrollDirection.eVertical);
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			base.ButtonData = new GUIDefines.ButtonData[]
			{
				new GUIDefines.ButtonData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.72135156f,
						topRatio = 0.2268125f,
						widthRatio = 0.06933594f,
						heightRatio = 0.09244792f,
						IPad = new GUIDefines.RectIPadInfo()
					},
					detectZoneScale = 1.5f,
					content = new GUIDefines.ContentInfo
					{
						icon = new GUIDefines.TextureInfo
						{
							name = "GUI/CreateAccountNew/TermsOfUsePopup/close_button"
						}
					}
				}
			};
		}
		else
		{
			base.ButtonData = new GUIDefines.ButtonData[]
			{
				new GUIDefines.ButtonData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.7370825f,
						topRatio = 0.1616667f,
						widthRatio = 0.07395833f,
						heightRatio = 0.1109375f
					},
					detectZoneScale = 1.5f,
					content = new GUIDefines.ContentInfo
					{
						icon = new GUIDefines.TextureInfo
						{
							name = "GUI/CreateAccountNew/TermsOfUsePopup/close_button"
						}
					}
				}
			};
		}
		this.CreateLabels();
	}

	protected override void OnButtonSelect()
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		if (base.SelectedButton == 0)
		{
			global::UnityEngine.Object.DestroyImmediate(this.mo_background);
		}
		base.OnButtonSelect();
	}

	private void CreateLabels()
	{
		base.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.2604167f,
					topRatio = 0.1953125f,
					widthRatio = 0.4791667f,
					heightRatio = 0.075f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = 18f,
						topOffset = 34f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetString("TXT_PrivacyPolicy")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eMedium,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			}
		};
	}

	protected override void CreateLayouts()
	{
		base.WindowData = new GUIDefines.WindowData
		{
			pos = new GUIDefines.RectInfo
			{
				heightRatio = 1f,
				widthRatio = 1f
			},
			id = 10
		};
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			base.TextureData = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.18652344f,
						topRatio = 0.18619792f,
						widthRatio = 0.6269531f,
						heightRatio = 0.6796875f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/TermsOfUsePopup/eula_popup_bg"
					}
				}
			};
		}
		else
		{
			base.TextureData = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.165625f,
						topRatio = 0.1234375f,
						widthRatio = 0.66875f,
						heightRatio = 0.815625f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/TermsOfUsePopup/eula_popup_bg"
					}
				}
			};
		}
	}

	public override void Draw()
	{
		if (this.CanDraw())
		{
			base.Draw();
			this.m_ErrorPopup.Draw();
		}
	}

	protected override void DrawWindowContent(int aWindowId)
	{
		base.DrawWindowContent(aWindowId);
		this.mo_termsOfUseList.Draw();
	}

	public void Update()
	{
		this.mo_termsOfUseList.Update();
	}

	public override void Show(bool aShow)
	{
		base.Show(aShow);
		this.mo_termsOfUseList.ResetScrollPosition();
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
			this.m_Callback(0);
		}
	}

	private GenericPopup m_ErrorPopup;

	private int m_EmailButtonPressed;

	private PrivacyPolicyList mo_termsOfUseList;

	private GameObject mo_background;

	private GUIDefines.RectInfo mo_scrollableListRectInfo;

	public enum Button
	{
		eClose,
		eButton_COUNT
	}
}
