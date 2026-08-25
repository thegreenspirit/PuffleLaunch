using System;
using UnityEngine;

public class ColorPickerPopup : BasePopup
{
	public ColorPickerPopup(GameObject aRefObj, GUIDefines.RectInfo ao_bgArea, GUIDefines.RectInfo ao_titleArea, Color ao_titleColor, GUIDefines.RectInfo ao_area, Vector2 av2_itemSizeRatio, string asz_bgResource, string asz_colorBallResource, string asz_colorMaskResource, string asz_colorBgResource, string asz_colorBgHighlightedResource, int ai_elementsPerRow, int ai_elementsPerColumn)
		: base(aRefObj)
	{
		this.mo_colorPickerZone = new ColorPickerZone(aRefObj, ao_area, av2_itemSizeRatio, asz_colorBallResource, asz_colorMaskResource, asz_colorBgResource, asz_colorBgHighlightedResource, ai_elementsPerRow, ai_elementsPerColumn);
		this.mo_colorPickerZone.RegisterCallback(new ColorPickerZone.ColorPickerZoneCallback(this.OnPickColor));
		this.msz_bgResource = asz_bgResource;
		base.WindowData = new GUIDefines.WindowData
		{
			pos = new GUIDefines.RectInfo
			{
				widthRatio = 1f,
				heightRatio = 1f
			},
			id = 10
		};
		base.TextureData = new GUIDefines.TextureData[]
		{
			new GUIDefines.TextureData
			{
				pos = ao_bgArea,
				icon = new GUIDefines.TextureInfo
				{
					name = this.msz_bgResource
				}
			}
		};
		base.LabelData = new GUIDefines.LabelData[]
		{
			new GUIDefines.LabelData
			{
				pos = ao_titleArea,
				content = new GUIDefines.ContentInfo
				{
					textId = "TXT_ChooseColor"
				},
				style = new GUIDefines.StyleInfo
				{
					useCustomStyle = true,
					customFontSize = GUIDefines.FontSize.eLarge,
					customNormalTextColor = ao_titleColor,
					useCustomTextAlignment = true,
					customTextAlignment = TextAnchor.MiddleCenter,
					customWordWrap = true
				}
			}
		};
	}

	private void OnPickColor(int aSelectedButton)
	{
		this.OnButtonSelect();
	}

	protected override void CreateLayouts()
	{
	}

	protected override void DrawWindowContent(int aWindowId)
	{
		base.DrawWindowContent(aWindowId);
		this.mo_colorPickerZone.Draw();
	}

	public Utilities.PenguinColors SelectedColor
	{
		get
		{
			return this.mo_colorPickerZone.SelectedColor;
		}
	}

	public override void Show(bool aShow)
	{
		base.Show(aShow);
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
		this.OnButtonSelect();
		this.Show(false);
	}

	private ColorPickerZone mo_colorPickerZone;

	private string msz_bgResource;
}
