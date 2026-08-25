using System;
using UnityEngine;

public class GenericPopup : BasePopup
{
	public GenericPopup(GameObject aRefObj)
		: base(aRefObj)
	{
		base.Priority = BaseGUI.GUIPriority.eHigh;
	}

	protected override void CreateLayouts()
	{
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eIPad)
		{
			base.WindowData = new GUIDefines.WindowData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.150391f,
					topRatio = 0.22070312f,
					widthRatio = 0.69921875f,
					heightRatio = 0.558594f,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				id = 10,
				respectIpadSizeRatio = true,
				style = new GUIDefines.StyleInfo
				{
					styleName = "ErrorPopupWindow"
				}
			};
		}
		else
		{
			base.WindowData = new GUIDefines.WindowData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.127083f,
					topRatio = 0.16484375f,
					widthRatio = 0.74583f,
					heightRatio = 0.6703125f
				},
				id = 10,
				respectIpadSizeRatio = true,
				style = new GUIDefines.StyleInfo
				{
					styleName = "ErrorPopupWindow"
				}
			};
		}
		base.ButtonData = new GUIDefines.ButtonData[]
		{
			new GUIDefines.ButtonData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.235417f,
					topRatio = 0.396875f,
					widthRatio = 0.275f,
					heightRatio = 0.1557292f,
					IPad = new GUIDefines.RectIPadInfo
					{
						leftOffset = -16f,
						topOffset = -51f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Ok"
				},
				style = new GUIDefines.StyleInfo
				{
					styleName = "SmallButton"
				}
			}
		};
		base.TextureData = new GUIDefines.TextureData[]
		{
			new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.3208333f,
					topRatio = 0.2296875f,
					widthRatio = 0.03958333f,
					heightRatio = 0.0546875f
				},
				icon = new GUIDefines.TextureInfo
				{
					name = "GUI/BlueTooth/bluetooth_progress-indicator"
				},
				invisible = true,
				rotate = GUIDefines.RotateDirection.eClockwise
			}
		};
		base.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.08020833f,
					topRatio = 0.2046875f,
					widthRatio = 0.5749996f,
					heightRatio = 0.146875f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = -23f
					}
				},
				content = new GUIDefines.ContentInfo(),
				style = this.m_LabelStyle
			},
			new GUIDefines.LabelData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = 0.08020833f,
					topRatio = 0.0828125f,
					widthRatio = 0.5749996f,
					heightRatio = 0.1f,
					IPad = new GUIDefines.RectIPadInfo
					{
						topOffset = -9f
					}
				},
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_Error"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eLarge,
					customNormalTextColor = GUIConstants.kDarkBrownColor
				}
			}
		};
	}

	public void ShowProgressing(bool aShow)
	{
		if (aShow)
		{
			base.SetTextureInvisible(2, false);
			base.SetLabelTextId(0, "TXT_Waiting");
			base.SetButtonTextId(0, "TXT_Cancel");
		}
		else
		{
			base.SetTextureInvisible(2, true);
			base.SetLabelText(0, string.Empty);
		}
		this.Show(aShow);
	}

	public void ShowText(string aText)
	{
		base.SetTextureInvisible(2, true);
		base.SetLabelText(0, aText);
		base.SetButtonTextId(0, "TXT_Ok");
		this.Show(true);
	}

	public void ShowTextId(string aTextId)
	{
		base.SetTextureInvisible(2, true);
		base.SetLabelTextId(0, aTextId);
		base.SetButtonTextId(0, "TXT_Ok");
		this.Show(true);
	}

	protected GUIDefines.StyleInfo m_LabelStyle = new GUIDefines.StyleInfo
	{
		useCustomStyle = true,
		customFontSize = GUIDefines.FontSize.eMedium,
		customNormalTextColor = GUIConstants.kLightBrownColor,
		customWordWrap = true
	};

	public enum Button
	{
		eFirst,
		eButton_COUNT
	}

	public enum Label
	{
		eMessage,
		eLabel_COUNT
	}

	public enum Texture
	{
		eBackround,
		eSensei,
		eProgressIndicator,
		eTexture_COUNT
	}
}
