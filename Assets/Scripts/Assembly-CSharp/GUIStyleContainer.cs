using System;
using System.Collections.Generic;
using UnityEngine;

public static class GUIStyleContainer
{
	public static void Init()
	{
		GUIStyleContainer.SetupCustomGUIStyle();
		GUIStyleContainer.GUIStyleTable = new Dictionary<string, GUIStyleContainer.TableData>();
		GUIStyleContainer.GUIStyleTable["LightGrayButton"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateLightGrayButtonGUIStyle));
		GUIStyleContainer.GUIStyleTable["CheatButton"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateCheatButtonGUIStyle));
		GUIStyleContainer.GUIStyleTable["SmallButton"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateSmallButtonGUIStyle));
		GUIStyleContainer.GUIStyleTable["SlowmoButton"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateSlowmoButtonGUIStyle));
		GUIStyleContainer.GUIStyleTable["TurboButton"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateTurboButtonGUIStyle));
		GUIStyleContainer.GUIStyleTable["ErrorPopupWindow"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateErrorPopupWindowGUIStyle));
		GUIStyleContainer.GUIStyleTable["InGameTextMini"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateMiniLabelGUIStyle));
		GUIStyleContainer.GUIStyleTable["InGameTextSmall"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateSmallLabelGUIStyle));
		GUIStyleContainer.GUIStyleTable["InGameTextMedium"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateMediumLabelGUIStyle));
		GUIStyleContainer.GUIStyleTable["InGameTextLarge"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateLargeLabelGUIStyle));
		GUIStyleContainer.GUIStyleTable["TallyScreenCounter"] = new GUIStyleContainer.TableData(new GUIStyleContainer.CreateGUIStyleFunc(GUIStyleContainer.CreateTallyScreenCounterGUIStyle));
	}

	public static void CleanUp()
	{
		// Green Spirit: there wasn't ifs for checking if it is null or not, so yeah
		if (GUIStyleContainer.GUIStyleTable != null)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, GUIStyleContainer.TableData> keyValuePair in GUIStyleContainer.GUIStyleTable)
			{
				if (keyValuePair.Value.guiStyle != null)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (string text in list)
			{
				GUIStyleContainer.TableData tableData = GUIStyleContainer.GUIStyleTable[text];
				tableData.guiStyle = null;
				GUIStyleContainer.GUIStyleTable[text] = tableData;
			}
		}

		if (GUIStyleContainer.CustomGUIStyle != null)
		{
			GUIStyleContainer.CustomGUIStyle.normal.background = null;
			GUIStyleContainer.CustomGUIStyle.hover.background = null;
			GUIStyleContainer.CustomGUIStyle.active.background = null;
			GUIStyleContainer.CustomGUIStyle.focused.background = null;
		}
	}

	public static GUIStyle GetStyle(string aStyleName)
	{
		// Green Spirit: fallback init if it isnt called before
		if (GUIStyleContainer.GUIStyleTable == null)
		{
			GUIStyleContainer.Init();
		}

		GUIStyleContainer.TableData tableData;
		if (GUIStyleContainer.GUIStyleTable.TryGetValue(aStyleName, out tableData))
		{
			if (tableData.guiStyle == null && Utilities.AssertMsg(tableData.createFunc != null, "Create function is not set for GUI style: " + aStyleName))
			{
				tableData.guiStyle = tableData.createFunc();
				GUIStyleContainer.GUIStyleTable[aStyleName] = tableData;
			}
			return tableData.guiStyle;
		}
		Utilities.AssertMsg(false, "GUI Style: " + aStyleName + " not found!");
		return null;
	}

	public static void SetupCustomGUIStyle()
	{
		GUIStyleContainer.CustomGUIStyle.clipping = TextClipping.Overflow;
	}

	public static GUIStyle CreateLightGrayButtonGUIStyle()
	{
		return GUIStyleContainer.CreateStandardButtonGUIStyle("GUI/LevelSelect/button_back", "GUI/LevelSelect/button_back_pressed", GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eSmall, GUIDefines.FontType.eCPMenus), GUIConstants.kBlackColor);
	}

	public static GUIStyle CreateCheatButtonGUIStyle()
	{
		return GUIStyleContainer.CreateStandardButtonGUIStyle("GUI/LevelSelect/cheat_button", "GUI/LevelSelect/cheat_button", GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eSmall, GUIDefines.FontType.eInGame), GUIConstants.kBlackColor);
	}

	public static GUIStyle CreateSmallButtonGUIStyle()
	{
		return GUIStyleContainer.CreateStandardButtonGUIStyle("GUI/Common/button", "GUI/Common/button_pressed", GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eMedium, GUIDefines.FontType.eCPMenus), GUIConstants.kWhiteColor);
	}

	public static GUIStyle CreateSlowmoButtonGUIStyle()
	{
		return GUIStyleContainer.CreateStandardToggleGUIStyle("GUI/InGame/SlowMoButton_Inactive", "GUI/InGame/SlowMoButton_Inactive", "GUI/InGame/SlowMoButton_Active", "GUI/InGame/SlowMoButton_Active", GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eSmall, GUIDefines.FontType.eInGame));
	}

	public static GUIStyle CreateTurboButtonGUIStyle()
	{
		return GUIStyleContainer.CreateStandardToggleGUIStyle("GUI/LevelSelect/Popups/TurboMode_TurboOnBox", "GUI/LevelSelect/Popups/TurboMode_TurboOnBox", "GUI/LevelSelect/Popups/TurboMode_TurboOnBox_Checked", "GUI/LevelSelect/Popups/TurboMode_TurboOnBox_Checked", GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eMedium, GUIDefines.FontType.eCPMenus));
	}

	public static GUIStyle CreateMiniLabelGUIStyle()
	{
		return GUIStyleContainer.CreateStandardLabelGUIStyle(GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eMini, GUIDefines.FontType.eInGame), TextAnchor.MiddleCenter, Color.white);
	}

	public static GUIStyle CreateSmallLabelGUIStyle()
	{
		return GUIStyleContainer.CreateStandardLabelGUIStyle(GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eSmall, GUIDefines.FontType.eInGame), TextAnchor.MiddleCenter, Color.white);
	}

	public static GUIStyle CreateMediumLabelGUIStyle()
	{
		return GUIStyleContainer.CreateStandardLabelGUIStyle(GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eMedium, GUIDefines.FontType.eInGame), TextAnchor.MiddleCenter, Color.white);
	}

	public static GUIStyle CreateLargeLabelGUIStyle()
	{
		return GUIStyleContainer.CreateStandardLabelGUIStyle(GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eLarge, GUIDefines.FontType.eInGame), TextAnchor.MiddleCenter, Color.white);
	}

	public static GUIStyle CreateTallyScreenCounterGUIStyle()
	{
		return GUIStyleContainer.CreateStandardLabelGUIStyle(GameFlowManager.Instance.GUIManager.GetFont(GUIDefines.FontSize.eMedium, GUIDefines.FontType.eInGame), TextAnchor.MiddleRight, Color.white);
	}

	public static GUIStyle CreateStandardButtonGUIStyle(string aNormalTextureName, string aActiveTextureName, Font aFont, Color aTextColor)
	{
		GUIDefines.Texture2DInfo texture2DInfo = new GUIDefines.Texture2DInfo
		{
			name = aNormalTextureName
		};
		texture2DInfo.Init();
		GUIDefines.Texture2DInfo texture2DInfo2 = new GUIDefines.Texture2DInfo
		{
			name = aActiveTextureName
		};
		texture2DInfo2.Init();
		return new GUIStyle
		{
			normal = 
			{
				background = texture2DInfo.image,
				textColor = aTextColor
			},
			active = 
			{
				background = texture2DInfo2.image,
				textColor = aTextColor
			},
			clipping = TextClipping.Overflow,
			font = aFont,
			alignment = TextAnchor.MiddleCenter
		};
	}

	public static GUIStyle CreateStandardToggleGUIStyle(string aNormalTextureName, string aActiveTextureName, string aOnNormalTextureName, string aOnActiveTextureName, Font aFont)
	{
		GUIDefines.Texture2DInfo texture2DInfo = new GUIDefines.Texture2DInfo
		{
			name = aNormalTextureName
		};
		texture2DInfo.Init();
		GUIDefines.Texture2DInfo texture2DInfo2 = new GUIDefines.Texture2DInfo
		{
			name = aActiveTextureName
		};
		texture2DInfo2.Init();
		GUIDefines.Texture2DInfo texture2DInfo3 = new GUIDefines.Texture2DInfo
		{
			name = aOnNormalTextureName
		};
		texture2DInfo3.Init();
		GUIDefines.Texture2DInfo texture2DInfo4 = new GUIDefines.Texture2DInfo
		{
			name = aOnActiveTextureName
		};
		texture2DInfo4.Init();
		return new GUIStyle
		{
			normal = 
			{
				background = texture2DInfo.image,
				textColor = GUIConstants.kWhiteColor
			},
			active = 
			{
				background = texture2DInfo2.image,
				textColor = GUIConstants.kWhiteColor
			},
			onNormal = 
			{
				background = texture2DInfo3.image,
				textColor = GUIConstants.kBlackColor
			},
			onActive = 
			{
				background = texture2DInfo4.image,
				textColor = GUIConstants.kBlackColor
			},
			clipping = TextClipping.Overflow,
			font = aFont,
			alignment = TextAnchor.MiddleCenter
		};
	}

	public static GUIStyle CreateStandardLabelGUIStyle(Font aFont, TextAnchor aAlignment, Color aTextColor)
	{
		return new GUIStyle
		{
			font = aFont,
			alignment = aAlignment,
			normal = 
			{
				textColor = aTextColor
			}
		};
	}

	public static GUIStyle CreateErrorPopupWindowGUIStyle()
	{
		return GUIStyleContainer.CreateStandardWindowGUIStyle("GUI/Common/error_popup");
	}

	public static GUIStyle CreateStandardWindowGUIStyle(string aBackgroundTextureName)
	{
		GUIDefines.Texture2DInfo texture2DInfo = new GUIDefines.Texture2DInfo
		{
			name = aBackgroundTextureName
		};
		texture2DInfo.Init();
		return new GUIStyle
		{
			normal = 
			{
				background = texture2DInfo.image
			},
			active = 
			{
				background = texture2DInfo.image
			}
		};
	}

	public static GUIStyle CustomGUIStyle = new GUIStyle();

	public static Dictionary<string, GUIStyleContainer.TableData> GUIStyleTable;

	public struct TableData
	{
		public TableData(GUIStyleContainer.CreateGUIStyleFunc aFunc)
		{
			this.guiStyle = null;
			this.createFunc = aFunc;
		}

		public GUIStyle guiStyle;

		public GUIStyleContainer.CreateGUIStyleFunc createFunc;
	}

	public delegate GUIStyle CreateGUIStyleFunc();
}
