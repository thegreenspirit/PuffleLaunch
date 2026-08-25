using System;
using UnityEngine;

public static class GUIDefines
{
	public enum FontSize
	{
		eMini,
		eSmall,
		eMedium,
		eLarge,
		eFontSize_COUNT
	}

	public enum FontType
	{
		eOnDemand = -1,
		eCPMenus,
		eInGame,
		eFontType_COUNT
	}

	public enum ScreenResolution
	{
		eOriginal,
		eLow,
		eIPad,
		eScreenResolution_COUNT
	}

	public enum RotateDirection
	{
		eNone,
		eClockwise,
		eCounterClockwise,
		eRotateDirection_COUNT
	}

	public enum AutoResizeAllignment
	{
		eAuto,
		eCenter,
		eLeft,
		eRight,
		eAutoResizeAllignment_COUNT
	}

	public class BackgroundInfo
	{
		public bool useBgColor;

		public Color bgColor;
	}

	public class RectIPadInfo
	{
		public RectIPadInfo()
		{
			this.Init();
		}

		public RectIPadInfo(GUIDefines.RectIPadInfo aIpad)
		{
			this.keepSizeRatio = aIpad.keepSizeRatio;
			this.keepWidthRatio = aIpad.keepWidthRatio;
			this.leftOffset = aIpad.leftOffset;
			this.topOffset = aIpad.topOffset;
			this.widthScale = aIpad.widthScale;
			this.heightScale = aIpad.heightScale;
			this.useLeftRefRatio = aIpad.useLeftRefRatio;
			this.leftRefRatio = aIpad.leftRefRatio;
			this.useTopRefRatio = aIpad.useTopRefRatio;
			this.topRefRatio = aIpad.topRefRatio;
			this.leftRefOffset = aIpad.leftRefOffset;
			this.topRefOffset = aIpad.topRefOffset;
			this.enableDebug = aIpad.enableDebug;
			this.Init();
		}

		public void Init()
		{
		}

		public bool keepSizeRatio;

		public bool keepWidthRatio;

		public float leftOffset;

		public float topOffset;

		public float widthScale;

		public float heightScale;

		public bool useLeftRefRatio;

		public float leftRefRatio;

		public bool useTopRefRatio;

		public float topRefRatio;

		public float leftRefOffset;

		public float topRefOffset;

		public bool enableDebug;
	}

	public class RectInfo
	{
		public RectInfo()
		{
			this.Init();
		}

		public RectInfo(GUIDefines.RectInfo aInfo)
		{
			this.leftRatio = aInfo.leftRatio;
			this.topRatio = aInfo.topRatio;
			this.widthRatio = aInfo.widthRatio;
			this.heightRatio = aInfo.heightRatio;
			this.useOriginalWidth = aInfo.useOriginalWidth;
			this.useOriginalHeight = aInfo.useOriginalHeight;
			if (aInfo.IPad != null)
			{
				this.IPad = new GUIDefines.RectIPadInfo(aInfo.IPad);
			}
			this.enableDebug = aInfo.enableDebug;
			this.Init();
		}

		public void Init()
		{
			bool flag = false;
			bool flag2 = false;
			if (this.IPad != null)
			{
				this.IPad.Init();
				flag = this.IPad.keepSizeRatio;
				flag2 = this.IPad.keepWidthRatio;
			}
			if (this.useAnchor)
			{
				this.anchor.Init();
				this.inPixel = GUIUtil.ConvertRatioToPixel(this.leftRatio, this.topRatio, this.widthRatio, this.heightRatio, flag, flag2);
				this.inPixel.x = this.inPixel.x + this.anchor.inPixel.x;
				this.inPixel.y = this.inPixel.y + this.anchor.inPixel.y;
				float x = GUIUtil.GetGuiStyle(this.anchorStyle).CalcSize(GUIUtil.CreateGuiContent(this.anchorContent)).x;
				this.inPixel.x = this.inPixel.x + x;
				float num = 0f;
				if (this.anchorStyle.customTextAlignment == TextAnchor.LowerCenter || this.anchorStyle.customTextAlignment == TextAnchor.MiddleCenter || this.anchorStyle.customTextAlignment == TextAnchor.UpperCenter)
				{
					num = (this.anchor.inPixel.xMax - this.anchor.inPixel.xMin - x) * 0.5f;
				}
				else if (this.anchorStyle.customTextAlignment == TextAnchor.LowerRight || this.anchorStyle.customTextAlignment == TextAnchor.MiddleRight || this.anchorStyle.customTextAlignment == TextAnchor.UpperRight)
				{
					num = this.anchor.inPixel.xMax - this.anchor.inPixel.xMin - x;
				}
				this.inPixel.x = this.inPixel.x + num;
			}
			else
			{
				this.inPixel = GUIUtil.ConvertRatioToPixel(this.leftRatio, this.topRatio, this.widthRatio, this.heightRatio, flag, flag2);
			}
			if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
			{
				if (this.IPad == null)
				{
					this.IPad = new GUIDefines.RectIPadInfo();
				}
				this.inPixel = GUIUtil.ApplyIPadAdjustment(this.inPixel, this.leftRatio, this.topRatio, this.IPad);
			}
		}

		public float leftRatio;

		public float topRatio;

		public float widthRatio;

		public float heightRatio;

		public bool useOriginalWidth;

		public bool useOriginalHeight;

		public GUIDefines.RectIPadInfo IPad;

		public Rect inPixel;

		public bool detatchFromRefObject;

		public bool useAnchor;

		public GUIDefines.RectInfo anchor;

		public GUIDefines.ContentInfo anchorContent;

		public GUIDefines.StyleInfo anchorStyle;

		public bool enableDebug;
	}

	public class Vector2IPadInfo
	{
		public void Init()
		{
		}

		public float xOffset;

		public float yOffset;

		public bool enableDebug;
	}

	public class Vector2Info
	{
		public void Init()
		{
			if (!this.setPixelsDirectly)
			{
				if (this.IPad != null)
				{
					this.IPad.Init();
				}
				this.inPixel = GUIUtil.ConvertRatioToPixel(this.xRatio, this.yRatio);
				if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
				{
					if (this.IPad == null)
					{
						this.IPad = new GUIDefines.Vector2IPadInfo();
					}
					GUIUtil.ApplyIPadAdjustment(ref this.inPixel, this.xRatio, this.yRatio, this.IPad);
				}
			}
		}

		public float xRatio;

		public float yRatio;

		public bool useOriginalWidth;

		public bool useOriginalHeight;

		public GUIDefines.Vector2IPadInfo IPad;

		public Vector2 inPixel;

		public bool setPixelsDirectly;

		public bool enableDebug;
	}

	public class ButtonElementInfo
	{
		public void Init()
		{
			if (this.content != null)
			{
				this.content.Init();
			}
			if (this.style != null)
			{
				this.style.Init();
			}
		}

		public int buttonId;

		public GUIDefines.ContentInfo content;

		public GUIDefines.StyleInfo style;
	}

	public class GenerateElementInfo
	{
		public void Init()
		{
		}

		public bool enable;

		public int elementCount;

		public string iconNamePrefix;

		public int iconIndexStartAt;
	}

	public class MultiPageInfo
	{
		public void Init()
		{
		}

		public int elementPerRow;

		public int elementPerCol;

		public int totalPage;
	}

	public class Texture2DInfo
	{
		public void Init()
		{
			string text = string.Empty;
			if (this.name != null && this.name.Length > 0)
			{
				string text2 = this.name;
				switch (ResolutionManager.Instance.AssetResolution)
				{
				case ResolutionManager.eAssetResolution.eLowres:
					text2 = this.name + "_lowres";
					break;
				case ResolutionManager.eAssetResolution.eIPad:
					text2 = this.name + "_iPad";
					break;
				}
				if (this.isLocalized)
				{
					string languageCode = LocalizationManager.GetLanguageCode();
					switch (languageCode)
					{
					case "fr":
						text = "_fr";
						goto IL_0135;
					case "es":
						text = "_es";
						goto IL_0135;
					case "pt":
						text = "_pt";
						goto IL_0135;
					}
					text = "_en";
				}
				IL_0135:
				text2 += text;
				this.image = GUIUtil.LoadTexture2D(text2);
				if (this.image == null && ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres)
				{
					Debug.Log("Fail to load alternate Texture2D: " + text2 + ". Loading original Texture2D: " + this.name);
				}
				if (this.image != null)
				{
					return;
				}
				if (this.isLocalized)
				{
					this.image = GUIUtil.LoadTexture2D(this.name + text);
				}
				else
				{
					this.image = GUIUtil.LoadTexture2D(this.name);
				}
				Utilities.AssertMsg(this.image != null, "Fail to load Texture2D: " + this.name);
			}
		}

		public string name;

		public Texture2D image;

		public bool isLocalized;
	}

	public class TextureInfo
	{
		public void Init()
		{
			if (this.name != null && this.name.Length > 0)
			{
				string text = this.name;
				switch (ResolutionManager.Instance.AssetResolution)
				{
				case ResolutionManager.eAssetResolution.eLowres:
					text = this.name + "_lowres";
					break;
				case ResolutionManager.eAssetResolution.eIPad:
					text = this.name + "_iPad";
					break;
				}
				this.image = GUIUtil.LoadTexture(text);
				if (this.image == null && ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres)
				{
					Debug.Log("Fail to load alternate Texture2D: " + text + ". Loading original Texture2D: " + this.name);
				}
				if (this.image != null)
				{
					return;
				}
				this.image = GUIUtil.LoadTexture(this.name);
				Utilities.AssertMsg(this.image != null, "Fail to load Texture: " + this.name);
			}
		}

		public string name;

		public Texture image;
	}

	public class StyleInfo
	{
		public void Init()
		{
			if (this.customNormal != null)
			{
				this.customNormal.Init();
			}
			if (this.customActive != null)
			{
				this.customActive.Init();
			}
			if (this.customPadding != null)
			{
				this.customPadding.Init();
			}
			if (this.customPadding2 != null)
			{
				this.customPadding2.Init();
			}
			if (this.customNormalTextColor.a == 0f)
			{
				this.customNormalTextColor = GUIConstants.kWhiteColor;
			}
			if (this.customActiveTextColor.a == 0f)
			{
				this.customActiveTextColor = GUIConstants.kWhiteColor;
				this.customActiveTextColor.a = 0.5f;
			}
			if (this.customFocusedTextColor.a == 0f)
			{
				this.customFocusedTextColor = GUIConstants.kWhiteColor;
			}
			if (!this.useCustomTextAlignment)
			{
				this.customTextAlignment = TextAnchor.MiddleCenter;
			}
		}

		public string styleName;

		public GUIStyle defaultStyle;

		public bool useCustomStyle;

		public GUIDefines.Texture2DInfo customNormal;

		public GUIDefines.Texture2DInfo customActive;

		public GUIDefines.FontSize customFontSize;

		public GUIDefines.FontType customFontType;

		public string customOnDemandFontName;

		public GUIDefines.Vector2Info customPadding;

		public GUIDefines.Vector2Info customPadding2;

		public Color customNormalTextColor;

		public Color customActiveTextColor;

		public Color customFocusedTextColor;

		public bool useCustomTextAlignment;

		public TextAnchor customTextAlignment;

		public bool customWordWrap;

		public ImagePosition customImagePosition;

		public bool useCustomDropShadowColor;

		public Color customDropShadowColor;

		public bool useCustomDropShadowOffset;

		public Vector2 customDropShadowOffset;
	}

	public class ContentInfo
	{
		public void Init()
		{
			if (this.icon != null)
			{
				this.icon.Init();
			}
		}

		public string textId;

		public string text;

		public string prefixTextId;

		public string prefixText;

		public string suffixTextId;

		public string suffixText;

		public GUIDefines.TextureInfo icon;
	}

	public class AutoResizeData
	{
		public int groupId;

		public int index;

		public Rect pos;
	}

	public class ButtonData
	{
		public void Init()
		{
			this.pos.Init();
			if (this.content != null)
			{
				this.content.Init();
			}
			if (this.style != null)
			{
				this.style.Init();
			}
			this.pos.inPixel = GUIUtil.ApplyStyleBgOriginalSize(this.pos, this.style);
		}

		public int buttonId;

		public GUIDefines.RectInfo pos;

		public float detectZoneScale;

		public GUIDefines.ContentInfo content;

		public GUIDefines.StyleInfo style;

		public bool invisible;

		public bool isTogglable;

		public bool toggleState;

		public bool isControlBlocked;

		public bool isAutoResizeOff;

		public bool useAutoResizeGroup;

		public int autoResizeGroupId;

		public GUIDefines.AutoResizeAllignment autoResizeAllignment;
	}

	public class GroupButtonData
	{
		public void Init()
		{
			if (this.autoGenerate != null && this.autoGenerate.enable)
			{
				this.GenerateElements();
			}
			if (this.area.IPad == null)
			{
				this.area.IPad = new GUIDefines.RectIPadInfo();
			}
			this.area.IPad.keepSizeRatio = true;
			this.area.Init();
			this.size.Init();
			this.space.Init();
			if (this.style != null)
			{
				this.style.Init();
			}
			if (this.multiPage != null)
			{
				this.multiPage.Init();
			}
			for (int i = 0; i < this.elements.Length; i++)
			{
				this.elements[i].Init();
			}
			if (this.elements.Length > 0)
			{
				if (this.elements[0].content != null)
				{
					this.size.inPixel = GUIUtil.ApplyTextureOriginalSize(this.size, this.elements[0].content.icon);
				}
				this.multiPage.totalPage = Mathf.CeilToInt((float)this.elements.Length / (float)(this.multiPage.elementPerCol * this.multiPage.elementPerRow));
			}
		}

		private void GenerateElements()
		{
			if (this.autoGenerate.elementCount <= 0)
			{
				return;
			}
			this.elements = new GUIDefines.ButtonElementInfo[this.autoGenerate.elementCount];
			for (int i = 0; i < this.autoGenerate.elementCount; i++)
			{
				this.elements[i] = new GUIDefines.ButtonElementInfo();
				this.elements[i].buttonId = i + this.autoGenerate.iconIndexStartAt;
				if (this.autoGenerate.iconNamePrefix != null && this.autoGenerate.iconNamePrefix.Length > 0)
				{
					int num = i;
					this.elements[i].content = new GUIDefines.ContentInfo
					{
						icon = new GUIDefines.TextureInfo
						{
							name = this.autoGenerate.iconNamePrefix + num
						}
					};
				}
			}
		}

		public GUIDefines.RectInfo area;

		public GUIDefines.Vector2Info size;

		public GUIDefines.Vector2Info space;

		public GUIDefines.StyleInfo style;

		public GUIDefines.MultiPageInfo multiPage;

		public GUIDefines.ButtonElementInfo[] elements;

		public GUIDefines.GenerateElementInfo autoGenerate;
	}

	public class TextureData
	{
		public void Init()
		{
			this.pos.Init();
			if (this.tiled)
			{
				this.tileSize.Init();
			}
			if (this.icon != null)
			{
				this.icon.Init();
			}
			if (this.pivotPointOffset != null)
			{
				this.pivotPointOffset.Init();
			}
			this.pos.inPixel = GUIUtil.ApplyTextureOriginalSize(this.pos, this.icon);
		}

		public GUIDefines.RectInfo pos;

		public GUIDefines.TextureInfo icon;

		public GUIDefines.BackgroundInfo bgInfo;

		public bool invisible;

		public GUIDefines.RotateDirection rotate;

		public GUIDefines.Vector2Info pivotPointOffset;

		public float rotateAngle;

		public bool tiled;

		public GUIDefines.RectInfo tileSize;
	}

	public class LabelData
	{
		public void Init()
		{
			this.pos.Init();
			if (this.content != null)
			{
				this.content.Init();
			}
			if (this.style != null)
			{
				this.style.Init();
			}
			this.pos.inPixel = GUIUtil.ApplyStyleBgOriginalSize(this.pos, this.style);
		}

		public GUIDefines.RectInfo pos;

		public GUIDefines.ContentInfo content;

		public GUIDefines.StyleInfo style;

		public GUIDefines.BackgroundInfo bgInfo;

		public bool invisible;

		public bool disableDropShadow;
	}

	public class TextFieldData
	{
		public void Init()
		{
			this.pos.Init();
			if (this.style != null)
			{
				this.style.Init();
			}
			this.pos.inPixel = GUIUtil.ApplyStyleBgOriginalSize(this.pos, this.style);
			this.editedText = string.Empty;
			this.maskedPassword = string.Empty;
			this.isFocused = false;
		}

		public string controlName;

		public GUIDefines.RectInfo pos;

		public bool isPassword;

		public bool isReadOnly;

		public int maxLength;

		public GUIDefines.StyleInfo style;

		public string defaultTextId;

		public TouchScreenKeyboardType keyboardType;

		public bool titleCase;

		public string editedText;

		public float timeOfNukedPassword;

		public string maskedPassword;

		public bool isFocused;
	}

	public class RadioButtonData
	{
		public void Init()
		{
			if (this.area.IPad == null)
			{
				this.area.IPad = new GUIDefines.RectIPadInfo();
			}
			this.area.IPad.keepSizeRatio = true;
			this.area.Init();
			this.space.Init();
			if (this.style != null)
			{
				this.style.Init();
			}
			this.isOn = new bool[this.count];
			for (int i = 0; i < this.isOn.Length; i++)
			{
				if (i == this.defaultOn)
				{
					this.isOn[i] = true;
				}
				else
				{
					this.isOn[i] = false;
				}
			}
		}

		public GUIDefines.RectInfo area;

		public GUIDefines.Vector2Info space;

		public int count;

		public int defaultOn;

		public GUIDefines.StyleInfo style;

		public bool[] isOn;
	}

	public class UnClickableRadioButtonData
	{
		public void Init()
		{
			if (this.area.IPad == null)
			{
				this.area.IPad = new GUIDefines.RectIPadInfo();
			}
			this.area.IPad.keepSizeRatio = true;
			this.area.Init();
			this.space.Init();
			this.on.Init();
			if (this.onPadding != null)
			{
				this.onPadding.Init();
			}
			this.off.Init();
			if (this.offPadding != null)
			{
				this.offPadding.Init();
			}
		}

		public GUIDefines.RectInfo area;

		public GUIDefines.Vector2Info space;

		public GUIDefines.TextureInfo on;

		public GUIDefines.Vector2Info onPadding;

		public GUIDefines.TextureInfo off;

		public GUIDefines.Vector2Info offPadding;

		public int count;
	}

	public class WindowData
	{
		public void Init()
		{
			if (!this.respectIpadSizeRatio)
			{
				if (this.pos.IPad == null)
				{
					this.pos.IPad = new GUIDefines.RectIPadInfo();
				}
				this.pos.IPad.keepSizeRatio = true;
			}
			this.pos.Init();
			if (this.style != null)
			{
				this.style.Init();
			}
			this.pos.inPixel = GUIUtil.ApplyStyleBgOriginalSize(this.pos, this.style);
		}

		public GUIDefines.RectInfo pos;

		public int id;

		public GUIDefines.StyleInfo style;

		public bool respectIpadSizeRatio;
	}

	public class PageControlData
	{
		public GameObject refObj;

		public Transform refTransform;

		public int firstInPage;

		public int PageNumber;
	}
}
