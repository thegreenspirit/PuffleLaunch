using System;
using UnityEngine;

public class AboutCP : BaseMonoScreen
{
	protected override void OnBack()
	{
		GameFlowManager.Instance.GUIManager.UnregisterAboutCPCurrentPage();
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		base.MainScreen.StopGUI();
		GameFlowManager.Instance.LoadScene("!Loader_MainMenu", false);
	}

	protected override void CreateMainScreenLayouts()
	{
		GUIDefines.ButtonData[] array = null;
		string text = string.Empty;
		switch (ResolutionManager.Instance.LayoutSize)
		{
		case ResolutionManager.eLayoutSize.eLowres:
			array = new GUIDefines.ButtonData[]
			{
				new GUIDefines.ButtonData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.855625f,
						topRatio = 0.02120833f,
						widthRatio = 0.12291667f,
						heightRatio = 0.075f
					},
					detectZoneScale = 1.5f,
					content = new GUIDefines.ContentInfo
					{
						textId = "TXT_Credits"
					},
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/AboutCP/creditsbutton_iphone4_normal"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/AboutCP/creditsbutton_iphone4_pressed"
						},
						customFontSize = GUIDefines.FontSize.eSmall
					}
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eMini);
			break;
		case ResolutionManager.eLayoutSize.eOriginal:
			array = new GUIDefines.ButtonData[]
			{
				new GUIDefines.ButtonData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.855625f,
						topRatio = 0.02120833f,
						widthRatio = 0.12291667f,
						heightRatio = 0.075f
					},
					detectZoneScale = 1.5f,
					content = new GUIDefines.ContentInfo
					{
						textId = "TXT_Credits"
					},
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/AboutCP/creditsbutton_iphone4_normal"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/AboutCP/creditsbutton_iphone4_pressed"
						},
						customFontSize = GUIDefines.FontSize.eSmall
					}
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eSmall);
			break;
		case ResolutionManager.eLayoutSize.eIPad:
			array = new GUIDefines.ButtonData[]
			{
				new GUIDefines.ButtonData
				{
					pos = new GUIDefines.RectInfo
					{
						leftRatio = 0.855625f,
						topRatio = 0.02120833f,
						widthRatio = 0.11523438f,
						heightRatio = 0.0625f,
						IPad = new GUIDefines.RectIPadInfo
						{
							keepSizeRatio = true
						}
					},
					detectZoneScale = 1.5f,
					content = new GUIDefines.ContentInfo
					{
						textId = "TXT_Credits"
					},
					style = new GUIDefines.StyleInfo
					{
						useCustomStyle = true,
						customNormal = new GUIDefines.Texture2DInfo
						{
							name = "GUI/AboutCP/creditsbutton_iphone4_normal"
						},
						customActive = new GUIDefines.Texture2DInfo
						{
							name = "GUI/AboutCP/creditsbutton_iphone4_pressed"
						},
						customFontSize = GUIDefines.FontSize.eSmall
					}
				}
			};
			text = GameFlowManager.Instance.GUIManager.GetLowResFontName(GUIDefines.FontType.eCPMenus, GUIDefines.FontSize.eSmall);
			break;
		}
		BaseMonoScreen.TopBarButtonCallback[] array2 = new BaseMonoScreen.TopBarButtonCallback[]
		{
			new BaseMonoScreen.TopBarButtonCallback(this.CreditsCallback)
		};
		base.SetTopBarData("TXT_Back", "TXT_AboutCP", array, array2);
		base.MainScreen.TextureData = new GUIDefines.TextureData[]
		{
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					topRatio = base.GetTopBarHeightRatio() - 0.01f,
					widthRatio = 1f,
					heightRatio = 0.125f
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/AboutCP/AboutCP_Text_Transparency"
				}
			}
		};
		base.MainScreen.ButtonData = new GUIDefines.ButtonData[]
		{
			new GUIDefines.ButtonData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.915f,
					topRatio = 0.814f,
					widthRatio = 0.0708333f,
					heightRatio = 0.103125f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						leftOffset = -3f,
						topOffset = 16f,
						heightScale = -9f
					}
				},
				detectZoneScale = 1.5f,
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customNormal = new GUIDefines.Texture2DInfo
					{
						name = string.Empty
					},
					customActive = new GUIDefines.Texture2DInfo
					{
						name = "GUI/AboutCP/PlayAboutCPMovie_pressed"
					}
				}
			}
		};
		base.MainScreen.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					topRatio = base.GetTopBarHeightRatio() - 0.01f,
					widthRatio = 1f,
					heightRatio = 0.125f
				},
				content = new GUIDefines.ContentInfo
				{
					textId = this.m_PageTextsIds[0]
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
					leftRatio = 0.6f,
					topRatio = 0.925f,
					widthRatio = 0.4f,
					heightRatio = 0.05f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = 5f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_AboutCP_pg5b"
				},
				disableDropShadow = true,
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customWordWrap = true,
					customFontType = GUIDefines.FontType.eOnDemand,
					customOnDemandFontName = text,
					customNormalTextColor = GUIConstants.kLessDarkGreyColor,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter
				}
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.701042f,
					topRatio = 0.59375f,
					widthRatio = 0.286458f,
					heightRatio = 0.04375f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true,
						topOffset = 41f,
						heightScale = -9f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_AboutCP_Video"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eMedium
				}
			}
		};
	}

	private Texture2D LoadTexture(string path)
	{
		switch (ResolutionManager.Instance.AssetResolution)
		{
		case ResolutionManager.eAssetResolution.eLowres:
			path += "_lowres";
			break;
		case ResolutionManager.eAssetResolution.eIPad:
			path += "_iPad";
			break;
		}
		return GUIUtil.LoadTexture2D(path);
	}

	private void Awake()
	{
		this.Init(base.gameObject);
		string text = this.m_TexturePaths[GameFlowManager.Instance.GUIManager.AboutCPCurrentPage];
		this.mo_firstTexture = this.LoadTexture(text);
		for (int i = 0; i < this.m_AboutCPRefObj.Length; i++)
		{
			this.m_AboutCPRefObj[i].GetComponent<Renderer>().material.mainTexture = this.mo_firstTexture;
		}
		this.m_ScrollAreaDetectZone = new Rect(0f, base.GetTopBarHeightPixels(), (float)Screen.width, (float)Screen.height - base.GetTopBarHeightPixels());
		if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eIPad)
		{
			TextMesh component = GameObject.Find("Page1/Disclaimer").GetComponent<TextMesh>();
			component.characterSize = 0.24f;
			TextMesh component2 = GameObject.Find("Page2/Disclaimer").GetComponent<TextMesh>();
			component2.characterSize = 0.24f;
		}
	}

	private void Start()
	{
	}

	private new void Update()
	{
		base.HandleButtonSelect();
	}

	private void OnGUI()
	{
		if (!base.MainScreen.CanDraw())
		{
			return;
		}
		base.MainScreen.Draw();
		base.DrawTopBar();
	}

	private void OnDestroy()
	{
		string text = "Player Not Logged In";

		if (ProfileManager.Instance != null && ProfileManager.Instance.CurrentProfile != null && ProfileManager.Instance.CurrentProfile.ProfileName != null)
		{
			text = ProfileManager.Instance.CurrentProfile.ProfileName;
		}

		BizIntel.ContextualEvent contextualEvent = new BizIntel.ContextualEvent("view-aboutcp");
		contextualEvent.AddContextItem("player-id", text);
		contextualEvent.AddContextItem("elapsed-time", (int)Time.timeSinceLevelLoad);
		contextualEvent.Log();
	}

	protected override void OnMainScreenButtonSelect()
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		AboutCP.Button selectedButton = (AboutCP.Button)base.MainScreen.SelectedButton;
		if (selectedButton == AboutCP.Button.ePlayVideo)
		{
			if (CinematicManager.Instance != null)
			{
				CinematicManager.Instance.ShowFullscreenBgWhenPlaying = true;
				CinematicManager.Instance.Play(CinematicManager.MovieId.eAboutCP);
			}
		}
	}

	public void CreditsCallback()
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		base.MainScreen.StopGUI();
		GameFlowManager.Instance.LoadScene("CreditsNew", false);
	}

	public GameObject[] m_AboutCPRefObj;

	public string[] m_TexturePaths;

	public string[] m_PageTextsIds;

	public Vector2 m_TouchPosition = new Vector2(0f, 0f);

	public Vector2 m_PreviousTouchPosition = new Vector2(0f, 0f);

	public bool m_WasTouchDown;

	public Vector2 m_StartTouchPosition;

	public Rect m_ScrollAreaDetectZone;

	public bool m_ScrollAreaSelected;

	private Texture2D mo_firstTexture;

	private enum Button
	{
		ePlayVideo,
		eButton_COUNT
	}

	private enum Label
	{
		ePageText,
		eLabel_COUNT
	}
}
