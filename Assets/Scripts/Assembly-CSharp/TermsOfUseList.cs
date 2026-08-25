using System;
using UnityEngine;

public class TermsOfUseList : ScrollableGUI
{
	public TermsOfUseList(GameObject aRefObj, GUIDefines.RectInfo ao_scrollableArea, ScrollableGUI.ScrollDirection ae_ScrollDirection)
		: base(aRefObj)
	{
		base.InitScrollArea(ao_scrollableArea, ae_ScrollDirection, this.m_ScrollableDistance);
	}

	protected override void CreateLayouts()
	{
		string languageCode = LocalizationManager.GetLanguageCode();
		switch (languageCode)
		{
		case "fr":
			switch (ResolutionManager.Instance.LayoutSize)
			{
			case ResolutionManager.eLayoutSize.eLowres:
				this.m_ScrollableDistance = 119.75f;
				break;
			case ResolutionManager.eLayoutSize.eOriginal:
				this.m_ScrollableDistance = 124.75f;
				break;
			case ResolutionManager.eLayoutSize.eIPad:
				this.m_ScrollableDistance = 117.5f;
				break;
			}
			goto IL_024D;
		case "es":
			switch (ResolutionManager.Instance.LayoutSize)
			{
			case ResolutionManager.eLayoutSize.eLowres:
				this.m_ScrollableDistance = 117.85f;
				break;
			case ResolutionManager.eLayoutSize.eOriginal:
				this.m_ScrollableDistance = 122.85f;
				break;
			case ResolutionManager.eLayoutSize.eIPad:
				this.m_ScrollableDistance = 114.75f;
				break;
			}
			goto IL_024D;
		case "pt":
			switch (ResolutionManager.Instance.LayoutSize)
			{
			case ResolutionManager.eLayoutSize.eLowres:
				this.m_ScrollableDistance = 114.5f;
				break;
			case ResolutionManager.eLayoutSize.eOriginal:
				this.m_ScrollableDistance = 118.6f;
				break;
			case ResolutionManager.eLayoutSize.eIPad:
				this.m_ScrollableDistance = 111.75f;
				break;
			}
			goto IL_024D;
		case "de":
			switch (ResolutionManager.Instance.LayoutSize)
			{
			case ResolutionManager.eLayoutSize.eLowres:
				this.m_ScrollableDistance = 118f;
				break;
			case ResolutionManager.eLayoutSize.eOriginal:
				this.m_ScrollableDistance = 123f;
				break;
			case ResolutionManager.eLayoutSize.eIPad:
				this.m_ScrollableDistance = 115.2f;
				break;
			}
			goto IL_024D;
		}
		switch (ResolutionManager.Instance.LayoutSize)
		{
		case ResolutionManager.eLayoutSize.eLowres:
			this.m_ScrollableDistance = 103.4f;
			break;
		case ResolutionManager.eLayoutSize.eOriginal:
			this.m_ScrollableDistance = 106.5f;
			break;
		case ResolutionManager.eLayoutSize.eIPad:
			this.m_ScrollableDistance = 100.6f;
			break;
		}
		IL_024D:
		base.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_1")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_2")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_3")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_4")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_5")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_6")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_7")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_8")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_9")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_10")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					widthRatio = 0.4742233f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				content = new GUIDefines.ContentInfo
				{
					text = LocalizationManager.Instance.GetTOUString("TXT_TOU_11")
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eSmall,
					customNormalTextColor = GUIConstants.kWhiteColor,
					useCustomTextAlignment = true,
					customWordWrap = true
				}
			}
		};
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			this.mv2o_scrollBarStartPos = new GUIDefines.Vector2Info
			{
				xRatio = 0.73828125f,
				yRatio = 0.3359375f
			};
			this.mv2o_scrollBarStartPos.Init();
			this.mv2o_scrollBarEndPos = new GUIDefines.Vector2Info
			{
				xRatio = 0.73828125f,
				yRatio = 0.6791667f
			};
			this.mv2o_scrollBarEndPos.Init();
			this.mto_scrollBarTexture = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.5f,
						topRatio = 0.5f,
						widthRatio = 0.01367188f,
						heightRatio = 0.11979167f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/TermsOfUsePopup/scrollbar"
					}
				}
			};
		}
		else
		{
			this.mv2o_scrollBarStartPos = new GUIDefines.Vector2Info
			{
				xRatio = 0.75f,
				yRatio = 0.296875f
			};
			this.mv2o_scrollBarStartPos.Init();
			this.mv2o_scrollBarEndPos = new GUIDefines.Vector2Info
			{
				xRatio = 0.75f,
				yRatio = 0.709375f
			};
			this.mv2o_scrollBarEndPos.Init();
			this.mto_scrollBarTexture = new GUIDefines.TextureData[]
			{
				new GUIDefines.TextureData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.5f,
						topRatio = 0.5f,
						widthRatio = 0.01458333f,
						heightRatio = 0.14375f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					icon = new GUIDefines.TextureInfo
					{
						name = "GUI/CreateAccountNew/TermsOfUsePopup/scrollbar"
					}
				}
			};
		}
		for (int i = 0; i < this.mto_scrollBarTexture.Length; i++)
		{
			this.mto_scrollBarTexture[i].Init();
		}
	}

	public override void Init(GameObject aRefObj)
	{
		base.Init(aRefObj);
		for (int i = 1; i < base.LabelData.Length; i++)
		{
			GUIContent guicontent = GUIUtil.CreateGuiContent(base.LabelData[i - 1].content);
			GUIStyle guiStyle = GUIUtil.GetGuiStyle(base.LabelData[i - 1].style);
			Vector2 vector = new Vector2(0f, guiStyle.CalcHeight(guicontent, base.LabelData[i - 1].pos.inPixel.width * 0.95f));
			base.LabelData[i].style.customPadding = new GUIDefines.Vector2Info
			{
				inPixel = vector
			};
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void DrawScrollListContent()
	{
		GUILayout.BeginArea(this.mo_scrollAreaRect);
		foreach (GUIDefines.LabelData labelData2 in base.LabelData)
		{
			GUILayout.BeginArea(this.mo_scrollAreaInnerRect, labelData2.content.text, GUIUtil.GetGuiStyle(labelData2.style));
			GUILayout.EndArea();
		}
		GUILayout.EndArea();
	}

	public override void DrawBorders()
	{
	}

	public override void DrawScrollBar()
	{
		Vector2 vector = this.mv2o_scrollBarStartPos.inPixel + base.ScrollPercentage * (this.mv2o_scrollBarEndPos.inPixel - this.mv2o_scrollBarStartPos.inPixel);
		this.mto_scrollBarTexture[0].pos.inPixel.x = vector.x;
		this.mto_scrollBarTexture[0].pos.inPixel.y = vector.y;
		GUICompoundControls.Textures(base.LocalTransform.position, this.mto_scrollBarTexture);
	}

	private GUIDefines.RectInfo mo_scrollAreaRectInfo;

	private float m_ScrollableDistance;

	private GUIDefines.TextureData[] mto_scrollBarTexture;

	private GUIDefines.Vector2Info mv2o_scrollBarStartPos;

	private GUIDefines.Vector2Info mv2o_scrollBarEndPos;

	private int m_UnsentCodeCount;

	private int m_SentCodeCount;
}
