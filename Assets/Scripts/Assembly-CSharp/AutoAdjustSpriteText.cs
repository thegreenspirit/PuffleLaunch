using System;
using UnityEngine;

public class AutoAdjustSpriteText : MonoBehaviour
{
	public void Awake()
	{
		this.m_SpriteText = base.GetComponent<SpriteText>();
		Utilities.AssertMsg(this.m_SpriteText != null, "No sprite text is found in " + base.gameObject);
	}

	public void AutoAdjust()
	{
		if (this.m_SpriteText == null)
		{
			return;
		}
		this.AutoSize(ref this.m_SpriteText);
		this.SetColor(ref this.m_SpriteText);
	}

	private void AutoSize(ref SpriteText aSpriteText)
	{
		if (aSpriteText == null)
		{
			return;
		}
		AutoAdjustSpriteText.SpriteTextSize spriteTextSize = this.m_SpriteTextSize;
		if (spriteTextSize != AutoAdjustSpriteText.SpriteTextSize.eUseSizeGivenInSpriteText)
		{
			int spriteTextSize2 = (int)this.m_SpriteTextSize;
			if (spriteTextSize2 >= 0 && spriteTextSize2 < AutoAdjustSpriteText.sm_SpriteSizeData.Length)
			{
				aSpriteText.SetCharacterSize(AutoAdjustSpriteText.sm_SpriteSizeData[spriteTextSize2].characterSize);
				aSpriteText.SetLineSpacing(AutoAdjustSpriteText.sm_SpriteSizeData[spriteTextSize2].lineSpacing);
			}
		}
	}

	private void SetColor(ref SpriteText aSpriteText)
	{
		if (aSpriteText == null)
		{
			return;
		}
		AutoAdjustSpriteText.SpriteTextColor spriteTextColor = this.m_SpriteTextColor;
		if (spriteTextColor != AutoAdjustSpriteText.SpriteTextColor.eUseColorGivenInSpriteText)
		{
			aSpriteText.SetColor(AutoAdjustSpriteText.GetColor(this.m_SpriteTextColor));
		}
	}

	public static Color GetColor(AutoAdjustSpriteText.SpriteTextColor aColor)
	{
		if (aColor >= AutoAdjustSpriteText.SpriteTextColor.eWhite && aColor < (AutoAdjustSpriteText.SpriteTextColor)AutoAdjustSpriteText.sm_SpriteTextColorList.Length)
		{
			return AutoAdjustSpriteText.sm_SpriteTextColorList[(int)aColor];
		}
		Utilities.AssertMsg(false, "Color not found for: " + aColor);
		return Color.clear;
	}

	public static AutoAdjustSpriteText.SizeData[] sm_SpriteSizeData = new AutoAdjustSpriteText.SizeData[]
	{
		new AutoAdjustSpriteText.SizeData
		{
			characterSize = 0.5f,
			lineSpacing = 0.5f
		},
		new AutoAdjustSpriteText.SizeData
		{
			characterSize = 0.8f,
			lineSpacing = 0.6f
		},
		new AutoAdjustSpriteText.SizeData
		{
			characterSize = 1f,
			lineSpacing = 0.8f
		},
		new AutoAdjustSpriteText.SizeData
		{
			characterSize = 1.791534f,
			lineSpacing = 1.1f
		}
	};

	public static Color[] sm_SpriteTextColorList = new Color[]
	{
		Color.white,
		Color.black,
		new Color(0.4549f, 0.22353f, 0.08235f, 1f),
		new Color(0.45490196f, 0.36862746f, 0.3254902f, 1f),
		new Color(0.3019608f, 0.23921569f, 0.21568628f, 1f),
		new Color(0.6549f, 0.56863f, 0.52549f, 1f),
		new Color(0.17647f, 0.26667f, 0.46275f, 1f),
		new Color(0.72941f, 0.72941f, 0.72941f, 1f),
		new Color(0.609375f, 0.5546875f, 0.52734375f, 1f),
		new Color(0.97647f, 0.95686f, 0.41961f, 1f)
	};

	public AutoAdjustSpriteText.SpriteTextSize m_SpriteTextSize = AutoAdjustSpriteText.SpriteTextSize.eMedium;

	public AutoAdjustSpriteText.SpriteTextColor m_SpriteTextColor;

	private SpriteText m_SpriteText;

	public enum SpriteTextSize
	{
		eUseSizeGivenInSpriteText = -1,
		eMini,
		eSmall,
		eMedium,
		eLarge
	}

	public struct SizeData
	{
		public float characterSize;

		public float lineSpacing;
	}

	public enum SpriteTextColor
	{
		eUseColorGivenInSpriteText = -1,
		eWhite,
		eBlack,
		eOrange,
		eLightBrown,
		eDarkBrown,
		ePaintBrown,
		eBlue,
		eGrey,
		eGreyBrown,
		eYellow
	}
}
