using System;
using System.Text;
using UnityEngine;

public static class GUIUtil
{
	public static void ApplyBgColor(bool ab_Apply, Color ao_Color, bool ab_BgOnly)
	{
		if (ab_Apply)
		{
			if (ab_BgOnly)
			{
				GUIUtil.sm_BackupGUIColor = GUI.backgroundColor;
				GUI.backgroundColor = ao_Color;
			}
			else
			{
				GUIUtil.sm_BackupGUIColor = GUI.color;
				GUI.color = ao_Color;
			}
		}
	}

	public static void RestoreBgColor(bool ab_Restore, bool ab_BgOnly)
	{
		if (ab_Restore)
		{
			if (ab_BgOnly)
			{
				GUI.backgroundColor = GUIUtil.sm_BackupGUIColor;
			}
			else
			{
				GUI.color = GUIUtil.sm_BackupGUIColor;
			}
		}
	}

	public static Rect ConvertRatioToPixel(float aLeftRatio, float aTopRatio, float aWidthRatio, float aHeightRatio, bool aKeepSizeRatioOnIPad, bool aKeepWidthRatioOnIpad)
	{
		GUIUtil.sm_Rect.xMin = aLeftRatio * GUIConstants.kReferenceScreenWidth;
		GUIUtil.sm_Rect.yMin = aTopRatio * GUIConstants.kReferenceScreenHeight;
		GUIUtil.sm_Rect.width = aWidthRatio * GUIConstants.kReferenceScreenWidth;
		GUIUtil.sm_Rect.height = aHeightRatio * GUIConstants.kReferenceScreenHeight;
		if (Utilities.ReferenceAspectRatio != 1.5f)
		{
			if (!aKeepSizeRatioOnIPad && !aKeepWidthRatioOnIpad && aWidthRatio < 1f)
			{
				GUIUtil.sm_Rect.xMin = GUIUtil.sm_Rect.xMin;
				GUIUtil.sm_Rect.xMax = GUIUtil.sm_Rect.xMax;
				GUIUtil.sm_Rect.width = GUIUtil.sm_Rect.width * 0.9375f;
			}
			if (!aKeepSizeRatioOnIPad && aHeightRatio < 1f)
			{
				GUIUtil.sm_Rect.yMin = GUIUtil.sm_Rect.yMin;
				GUIUtil.sm_Rect.yMax = GUIUtil.sm_Rect.yMax;
				GUIUtil.sm_Rect.height = GUIUtil.sm_Rect.height * 0.8333333f;
			}
		}
		return GUIUtil.sm_Rect;
	}

	public static Vector2 ConvertRatioToPixel(float aXSpaceRatio, float aYSpaceRatio)
	{
		Rect rect = GUIUtil.ConvertRatioToPixel(aXSpaceRatio, aYSpaceRatio, 0f, 0f, true, true);
		GUIUtil.sm_Vector2.x = rect.xMin;
		GUIUtil.sm_Vector2.y = rect.yMin;
		return GUIUtil.sm_Vector2;
	}

	public static float CalculateIpadLeftOffset(float aTargetRatio, float aRefRatio)
	{
		return (aTargetRatio - aRefRatio) * -64f;
	}

	public static float CalculateIpadTopOffset(float aTargetRatio, float aRefRatio)
	{
		return (aTargetRatio - aRefRatio) * -128f;
	}

	public static Rect ApplyIPadAdjustment(Rect aRectInPixel, float aLeftRatio, float aTopRatio, GUIDefines.RectIPadInfo aIPad)
	{
		GUIUtil.sm_Rect = aRectInPixel;
		float num = aIPad.leftOffset;
		float num2 = aIPad.topOffset;
		if (aIPad.useLeftRefRatio)
		{
			num += GUIUtil.CalculateIpadLeftOffset(aLeftRatio, aIPad.leftRefRatio);
		}
		num += aIPad.leftRefOffset;
		if (num != 0f)
		{
			GUIUtil.sm_Rect.xMin = aLeftRatio * GUIConstants.kReferenceScreenWidth + num;
		}
		if (aIPad.useTopRefRatio)
		{
			num2 += GUIUtil.CalculateIpadTopOffset(aTopRatio, aIPad.topRefRatio);
		}
		num2 += aIPad.topRefOffset;
		if (num2 != 0f)
		{
			GUIUtil.sm_Rect.yMin = aTopRatio * GUIConstants.kReferenceScreenHeight + num2;
		}
		GUIUtil.sm_Rect.xMax = GUIUtil.sm_Rect.xMax + (num + aIPad.widthScale);
		GUIUtil.sm_Rect.yMax = GUIUtil.sm_Rect.yMax + (num2 + aIPad.heightScale);
		return GUIUtil.sm_Rect;
	}

	public static void ApplyIPadAdjustment(ref Vector2 aWorkVector, float aXRatio, float aYRatio, GUIDefines.Vector2IPadInfo aIPad)
	{
		if (aIPad.xOffset != 0f)
		{
			aWorkVector.x = aXRatio * GUIConstants.kReferenceScreenWidth + aIPad.xOffset;
		}
		if (aIPad.yOffset != 0f)
		{
			aWorkVector.y = aYRatio * GUIConstants.kReferenceScreenHeight + aIPad.yOffset;
		}
	}

	public static Rect ConvertToRelativePos(Vector3 aReferencePos, GUIDefines.RectInfo aAbsolutePos)
	{
		GUIUtil.sm_Rect = aAbsolutePos.inPixel;
		if (!aAbsolutePos.detatchFromRefObject)
		{
			GUIUtil.sm_Rect.xMin = aAbsolutePos.inPixel.xMin + aReferencePos.x * 30f;
			GUIUtil.sm_Rect.yMin = aAbsolutePos.inPixel.yMin - aReferencePos.y * 30f;
			GUIUtil.sm_Rect.width = aAbsolutePos.inPixel.width;
			GUIUtil.sm_Rect.height = aAbsolutePos.inPixel.height;
		}
		return GUIUtil.sm_Rect;
	}

	public static Rect FullScreenRect()
	{
		GUIUtil.sm_Rect.xMin = 0f;
		GUIUtil.sm_Rect.yMin = 0f;
		GUIUtil.sm_Rect.width = GUIConstants.kReferenceScreenWidth;
		GUIUtil.sm_Rect.height = GUIConstants.kReferenceScreenHeight;
		return GUIUtil.sm_Rect;
	}

	public static Rect ApplyStyleBgOriginalSize(GUIDefines.RectInfo aPos, GUIDefines.StyleInfo aStyle)
	{
		GUIUtil.sm_Rect = aPos.inPixel;
		GUIStyle guiStyle = GUIUtil.GetGuiStyle(aStyle);
		if (guiStyle != null)
		{
			bool flag = aPos.IPad != null && aPos.IPad.keepSizeRatio;
			bool flag2 = aPos.IPad != null && aPos.IPad.keepWidthRatio;
			if (aPos.useOriginalWidth && Utilities.AssertMsg(guiStyle.normal.background != null, "ApplyStyleBgOriginalSize: Invalid normal.background Texture2D in GUI Style"))
			{
				GUIUtil.sm_Rect.width = GUIUtil.GetDisplayImageWidth((float)guiStyle.normal.background.width, GUIUtil.GetImageRelativeScreenWidth(guiStyle.normal.background.name), flag, flag2);
			}
			if (aPos.useOriginalHeight && Utilities.AssertMsg(guiStyle.normal.background != null, "ApplyStyleBgOriginalSize: Invalid normal.background Texture2D in GUI Style"))
			{
				GUIUtil.sm_Rect.height = GUIUtil.GetDisplayImageHeight((float)guiStyle.normal.background.height, GUIUtil.GetImageRelativeScreenHeight(guiStyle.normal.background.name), flag);
			}
		}
		return GUIUtil.sm_Rect;
	}

	public static Rect ApplyTextureOriginalSize(GUIDefines.RectInfo aPos, GUIDefines.TextureInfo aTexture)
	{
		GUIUtil.sm_Rect = aPos.inPixel;
		if (aTexture != null && aTexture.image != null)
		{
			bool flag = aPos.IPad != null && aPos.IPad.keepSizeRatio;
			bool flag2 = aPos.IPad != null && aPos.IPad.keepWidthRatio;
			if (aPos.useOriginalWidth)
			{
				GUIUtil.sm_Rect.width = GUIUtil.GetDisplayImageWidth((float)aTexture.image.width, GUIUtil.GetImageRelativeScreenWidth(aTexture.image.name), flag, flag2);
			}
			if (aPos.useOriginalHeight)
			{
				GUIUtil.sm_Rect.height = GUIUtil.GetDisplayImageHeight((float)aTexture.image.height, GUIUtil.GetImageRelativeScreenHeight(aTexture.image.name), flag);
			}
		}
		return GUIUtil.sm_Rect;
	}

	public static Vector2 ApplyTextureOriginalSize(GUIDefines.Vector2Info aSize, GUIDefines.TextureInfo aTexture)
	{
		GUIUtil.sm_Vector2 = aSize.inPixel;
		if (aTexture.image != null)
		{
			if (aSize.useOriginalWidth)
			{
				GUIUtil.sm_Vector2.x = GUIUtil.GetDisplayImageWidth((float)aTexture.image.width, GUIUtil.GetImageRelativeScreenWidth(aTexture.image.name), false, false);
			}
			if (aSize.useOriginalHeight)
			{
				GUIUtil.sm_Vector2.y = GUIUtil.GetDisplayImageHeight((float)aTexture.image.height, GUIUtil.GetImageRelativeScreenHeight(aTexture.image.name), false);
			}
		}
		return GUIUtil.sm_Vector2;
	}

	public static float GetDisplayImageWidth(float aActualImageWidth, float aRelativeScreenWidth, bool aKeepSizeRatioOnIPad, bool aKeepWidthRatioOnIPad)
	{
		float num = aActualImageWidth;
		if (Utilities.ReferenceAspectRatio == 1.5f)
		{
			if (GUIConstants.kReferenceScreenWidth != aRelativeScreenWidth)
			{
				num = aActualImageWidth / aRelativeScreenWidth * GUIConstants.kReferenceScreenWidth;
			}
		}
		else
		{
			num = aActualImageWidth / aRelativeScreenWidth * GUIConstants.kReferenceScreenWidth;
			if (!aKeepSizeRatioOnIPad && !aKeepWidthRatioOnIPad)
			{
				num *= 0.9375f;
			}
		}
		Utilities.AssertMsg(num >= 0f, "Invalid display width: " + num.ToString());
		return num;
	}

	public static float GetDisplayImageHeight(float aActualImageHeight, float aRelativeScreenHeight, bool aKeepSizeRatioOnIPad)
	{
		float num = aActualImageHeight;
		if (Utilities.ReferenceAspectRatio == 1.5f)
		{
			if (GUIConstants.kReferenceScreenHeight != aRelativeScreenHeight)
			{
				num = aActualImageHeight / aRelativeScreenHeight * GUIConstants.kReferenceScreenHeight;
			}
		}
		else
		{
			num = aActualImageHeight / aRelativeScreenHeight * GUIConstants.kReferenceScreenHeight;
			if (!aKeepSizeRatioOnIPad)
			{
				num *= 0.8333333f;
			}
		}
		Utilities.AssertMsg(num >= 0f, "Invalid display height: " + num.ToString());
		return num;
	}

	public static float GetImageRelativeScreenWidth(string aImageName)
	{
		if (aImageName.Contains("_lowres"))
		{
			return 480f;
		}
		return 960f;
	}

	public static float GetImageRelativeScreenHeight(string aImageName)
	{
		if (aImageName.Contains("_lowres"))
		{
			return 320f;
		}
		return 640f;
	}

	public static GUIStyle GetGuiStyle(GUIDefines.StyleInfo aStyle)
	{
		GUIStyle guistyle;
		if (aStyle != null && aStyle.useCustomStyle)
		{
			guistyle = GUIUtil.CustomizeStyle(aStyle);
			Utilities.AssertMsg(guistyle != null, "Fail to customize GUI style!");
		}
		else if (aStyle != null && aStyle.styleName != null && aStyle.styleName.Length > 0)
		{
			guistyle = GUIStyleContainer.GetStyle(aStyle.styleName);
		}
		else
		{
			guistyle = null;
		}
		return guistyle;
	}

	public static GUIStyle CustomizeStyle(GUIDefines.StyleInfo aStyle)
	{
		GUIStyle customGUIStyle = GUIStyleContainer.CustomGUIStyle;
		if (aStyle.customNormal != null)
		{
			customGUIStyle.normal.background = aStyle.customNormal.image;
		}
		else
		{
			customGUIStyle.normal.background = null;
		}
		if (aStyle.customActive == null || aStyle.customActive.image == null)
		{
			customGUIStyle.active.background = customGUIStyle.normal.background;
			customGUIStyle.onActive.background = customGUIStyle.normal.background;
		}
		else
		{
			customGUIStyle.active.background = aStyle.customActive.image;
			customGUIStyle.onNormal.background = aStyle.customActive.image;
			customGUIStyle.onActive.background = aStyle.customNormal.image;
		}
		if (aStyle.customFontType == GUIDefines.FontType.eOnDemand)
		{
			customGUIStyle.font = GameFlowManager.Instance.GUIManager.GetOnDemandFont(aStyle.customOnDemandFontName);
		}
		else
		{
			customGUIStyle.font = GameFlowManager.Instance.GUIManager.GetFont(aStyle.customFontSize, aStyle.customFontType);
		}
		Vector2 vector;
		if (aStyle.customPadding != null)
		{
			vector = GUIUtil.GetSpace(aStyle.customPadding);
		}
		else
		{
			vector = Vector2.zero;
		}
		customGUIStyle.padding.left = (int)vector.x;
		customGUIStyle.padding.top = (int)vector.y;
		Vector2 vector2;
		if (aStyle.customPadding2 != null)
		{
			vector2 = GUIUtil.GetSpace(aStyle.customPadding2);
		}
		else
		{
			vector2 = Vector2.zero;
		}
		customGUIStyle.padding.right = (int)vector2.x;
		customGUIStyle.padding.bottom = (int)vector2.y;
		customGUIStyle.normal.textColor = aStyle.customNormalTextColor;
		customGUIStyle.active.textColor = aStyle.customActiveTextColor;
		customGUIStyle.focused.textColor = aStyle.customFocusedTextColor;
		customGUIStyle.alignment = aStyle.customTextAlignment;
		customGUIStyle.wordWrap = aStyle.customWordWrap;
		customGUIStyle.imagePosition = aStyle.customImagePosition;
		return customGUIStyle;
	}

	public static GUIStyle CreateDropShadowTextStyle(GUIStyle aGuiStyle)
	{
		Utilities.Assert(aGuiStyle != null);
		GUIStyle guistyle = new GUIStyle(aGuiStyle);
		GUIStyleState normal = guistyle.normal;
		Color color = GUIUtil.DetermineDropShadowColor(aGuiStyle);
		guistyle.active.textColor = color;
		normal.textColor = color;
		int dropShadowOffsetX = GameFlowManager.Instance.GUIManager.DropShadowOffsetX;
		int dropShadowOffsetY = GameFlowManager.Instance.GUIManager.DropShadowOffsetY;
		guistyle.contentOffset = new Vector2((float)dropShadowOffsetX, (float)dropShadowOffsetY);
		return guistyle;
	}

	public static GUIStyle CreateDropShadowTextStyleForLabel(GUIStyle aGuiStyle)
	{
		Utilities.Assert(aGuiStyle != null);
		GUIStyle guistyle = new GUIStyle(aGuiStyle);
		GUIStyleState normal = guistyle.normal;
		Color color = GUIUtil.DetermineDropShadowColor(aGuiStyle);
		guistyle.active.textColor = color;
		normal.textColor = color;
		return guistyle;
	}

	public static GUIStyle CreateCustomDropShadowTextStyleForLabel(GUIStyle aGuiStyle, Color aColor)
	{
		Utilities.Assert(aGuiStyle != null);
		GUIStyle guistyle = new GUIStyle(aGuiStyle);
		GUIStyleState normal = guistyle.normal;
		guistyle.active.textColor = aColor;
		normal.textColor = aColor;
		return guistyle;
	}

	private static Color DetermineDropShadowColor(GUIStyle aGuiStyle)
	{
		if (aGuiStyle.normal.textColor.Equals(GUIConstants.kWhiteColor) || aGuiStyle.normal.textColor.Equals(GUIConstants.kLevelSelectNewTextColor) || aGuiStyle.normal.textColor.Equals(GUIConstants.kLightGreyColor))
		{
			return GameFlowManager.Instance.GUIManager.DarkBrownDropShadowColor;
		}
		return GameFlowManager.Instance.GUIManager.WhiteDropShadowColor;
	}

	public static GUIStyle CreateFrontTextStyle(GUIStyle aGuiStyle)
	{
		Utilities.Assert(aGuiStyle != null);
		GUIStyle guistyle = new GUIStyle(aGuiStyle);
		GUIStyleState normal = guistyle.normal;
		Texture2D texture2D = null;
		guistyle.active.background = texture2D;
		normal.background = texture2D;
		return guistyle;
	}

	public static GUIStyle CreateFrontTextStyleWithNoDropShadow(GUIStyle aGuiStyle)
	{
		Utilities.Assert(aGuiStyle != null);
		return new GUIStyle(aGuiStyle);
	}

	public static GUIContent CreateGuiContent(GUIDefines.ContentInfo aContent)
	{
		GUIUtil.sm_Content.text = string.Empty;
		GUIUtil.sm_Content.image = null;
		if (aContent == null)
		{
			return GUIUtil.sm_Content;
		}
		if (aContent.text != null && aContent.text.Length > 0)
		{
			GUIUtil.sm_Content.text = aContent.text;
		}
		else if (aContent.textId != null && aContent.textId.Length > 0)
		{
			GUIUtil.sm_Content.text = LocalizationManager.Instance.GetString(aContent.textId);
		}
		if (aContent.prefixText != null && aContent.prefixText.Length > 0)
		{
			GUIUtil.sm_Content.text = aContent.prefixText + " " + GUIUtil.sm_Content.text;
		}
		else if (aContent.prefixTextId != null && aContent.prefixTextId.Length > 0)
		{
			GUIUtil.sm_Content.text = LocalizationManager.Instance.GetString(aContent.prefixTextId) + " " + GUIUtil.sm_Content.text;
		}
		if (aContent.suffixText != null && aContent.suffixText.Length > 0)
		{
			GUIUtil.sm_Content.text = GUIUtil.sm_Content.text + " " + aContent.suffixText;
		}
		else if (aContent.suffixTextId != null && aContent.suffixTextId.Length > 0)
		{
			GUIUtil.sm_Content.text = GUIUtil.sm_Content.text + " " + LocalizationManager.Instance.GetString(aContent.suffixTextId);
		}
		if (aContent.icon != null)
		{
			GUIUtil.sm_Content.image = aContent.icon.image;
		}
		return GUIUtil.sm_Content;
	}

	public static GUILayoutOption[] CreateGuiLayoutOptions(GUIDefines.Vector2Info aSize)
	{
		GUIUtil.sm_Vector2 = aSize.inPixel;
		GUIUtil.sm_LayoutOptions[0] = GUILayout.Width(GUIUtil.sm_Vector2.x);
		GUIUtil.sm_LayoutOptions[1] = GUILayout.Height(GUIUtil.sm_Vector2.y);
		return GUIUtil.sm_LayoutOptions;
	}

	public static Vector2 GetSpace(GUIDefines.Vector2Info aSpace)
	{
		if (aSpace == null)
		{
			return Vector2.zero;
		}
		GUIUtil.sm_Vector2 = aSpace.inPixel;
		return GUIUtil.sm_Vector2;
	}

	public static void SetControlName(string aControlName)
	{
		if (aControlName != null && aControlName.Length > 0)
		{
			GUI.SetNextControlName(aControlName);
		}
	}

	public static bool IsCurrentFocusControl(string aControlName)
	{
		return aControlName != null && aControlName.Length > 0 && GUI.GetNameOfFocusedControl() == aControlName;
	}

	public static int PrevPage(GUIDefines.GroupButtonData aGroupButtonData, int aFirstInPage)
	{
		int num = aFirstInPage - aGroupButtonData.multiPage.elementPerRow * aGroupButtonData.multiPage.elementPerCol;
		if (num < 0)
		{
			num = aFirstInPage;
		}
		return num;
	}

	public static int NextPage(GUIDefines.GroupButtonData aGroupButtonData, int aFirstInPage)
	{
		int num = aFirstInPage + aGroupButtonData.multiPage.elementPerRow * aGroupButtonData.multiPage.elementPerCol;
		if (num >= aGroupButtonData.elements.Length)
		{
			num = aFirstInPage;
		}
		return num;
	}

	public static string GetStringToDisplay(string aString, GUIStyle aStyle, float aWidth, bool aShowCursor)
	{
		string text = string.Format("{0}|", aString);
		int num = 0;
		for (float num2 = aStyle.CalcSize(new GUIContent(text)).x - aWidth; num2 > 0f; num2 = aStyle.CalcSize(new GUIContent(text.Substring(num, text.Length - num))).x - aWidth)
		{
			num++;
		}
		string text2 = string.Empty;
		if (aShowCursor && Time.time - (float)((int)(Time.time / 1.5f)) * 1.5f > 0.75f)
		{
			text2 = text;
		}
		else
		{
			text2 = aString;
		}
		return text2.Substring(num, text2.Length - num);
	}

	public static string MaskPassword(ref GUIDefines.TextFieldData aTextFieldData, string aPassword)
	{
		if (aTextFieldData.editedText == null || aPassword.Length > aTextFieldData.editedText.Length)
		{
			aTextFieldData.timeOfNukedPassword = Time.realtimeSinceStartup;
		}
		else if (aPassword.Length < aTextFieldData.editedText.Length)
		{
			aTextFieldData.timeOfNukedPassword = 0f;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('*', aPassword.Length - 1);
		if (Time.realtimeSinceStartup - aTextFieldData.timeOfNukedPassword < 2f)
		{
			stringBuilder.Append(aPassword.Substring(aPassword.Length - 1, 1));
		}
		else
		{
			stringBuilder.Append('*');
		}
		return stringBuilder.ToString();
	}

	public static bool AutoResizeAccordingToContent(GUIStyle aStyle, GUIContent aContent, GUIDefines.AutoResizeAllignment aResizeAllignment, ref Rect aPosition)
	{
		if (aStyle == null || aContent == null)
		{
			return false;
		}
		float x = aStyle.CalcSize(aContent).x;
		if (aPosition.width < x + 10f)
		{
			float num = x - aPosition.width + 10f;
			float num2 = aPosition.x / GUIConstants.kReferenceScreenWidth;
			float num3 = (aPosition.x + aPosition.width) / GUIConstants.kReferenceScreenWidth;
			GUIDefines.AutoResizeAllignment autoResizeAllignment;
			if (aResizeAllignment == GUIDefines.AutoResizeAllignment.eAuto)
			{
				if (num2 <= 0.1f)
				{
					autoResizeAllignment = GUIDefines.AutoResizeAllignment.eLeft;
				}
				else if (num3 >= 0.9f)
				{
					autoResizeAllignment = GUIDefines.AutoResizeAllignment.eRight;
				}
				else
				{
					autoResizeAllignment = GUIDefines.AutoResizeAllignment.eCenter;
				}
			}
			else
			{
				autoResizeAllignment = aResizeAllignment;
			}
			switch (autoResizeAllignment)
			{
			case GUIDefines.AutoResizeAllignment.eLeft:
				goto IL_00E7;
			case GUIDefines.AutoResizeAllignment.eRight:
				aPosition.x -= num;
				goto IL_00E7;
			}
			aPosition.x -= num / 2f;
			IL_00E7:
			aPosition.width += num;
			return true;
		}
		return false;
	}

	public static void DrawSemiTransparentLayer()
	{
		if (GUIUtil.sm_SemiTransparentLayer.image == null)
		{
			GUIUtil.sm_SemiTransparentLayer.Init();
		}
		GUICompoundControls.FullScreenTexture(GUIUtil.sm_SemiTransparentLayer);
	}

	public static void CleanUp()
	{
		GUIUtil.sm_SemiTransparentLayer.image = null;
	}

	public static float FindHorizontalPositionToAlign(float af_widthRatio, int ai_numDivisions, int ai_divisionIndex)
	{
		float num = (1f / (float)ai_numDivisions - af_widthRatio) / 2f;
		float num2 = 1f / (float)ai_numDivisions * (float)ai_divisionIndex;
		return num + num2;
	}

	public static Texture2D LoadTexture2D(string aTextureName)
	{
		GUITextureStatistics.MarkTextureInUse(aTextureName);
		return Resources.Load(aTextureName, typeof(Texture2D)) as Texture2D;
	}

	public static Texture LoadTexture(string aTextureName)
	{
		GUITextureStatistics.MarkTextureInUse(aTextureName);
		return Resources.Load(aTextureName, typeof(Texture)) as Texture;
	}

	private static GUIDefines.TextureInfo sm_SemiTransparentLayer = new GUIDefines.TextureInfo
	{
		name = "GUI/Common/semi_transparent"
	};

	private static Rect sm_Rect = default(Rect);

	private static GUIContent sm_Content = new GUIContent();

	private static Vector2 sm_Vector2 = default(Vector2);

	private static GUILayoutOption[] sm_LayoutOptions = new GUILayoutOption[2];

	private static Color sm_BackupGUIColor;
}
