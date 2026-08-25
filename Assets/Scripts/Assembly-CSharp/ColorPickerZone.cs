using System;
using UnityEngine;

public class ColorPickerZone : BaseGUI
{
	public ColorPickerZone(GameObject aRefObj, GUIDefines.RectInfo ao_area, Vector2 av2_itemSizeRatio, string asz_colorBallResource, string asz_colorMaskResource, string asz_colorBgResource, string asz_colorBgHighlightedResource, int ai_elementsPerRow, int ai_elementsPerColumn)
		: base(aRefObj)
	{
		this.mto_colorBgHighlightedCurrent = new GUIDefines.TextureData[1];
		this.mto_colorBgHighlightedCurrent[0] = null;
		this.mo_area = ao_area;
		this.mo_area.Init();
		this.msz_colorBallResource = asz_colorBallResource;
		this.msz_colorMaskResource = asz_colorMaskResource;
		this.msz_colorBgResource = asz_colorBgResource;
		this.msz_colorBgHighlightedResource = asz_colorBgHighlightedResource;
		this.mi_elementsPerRow = ai_elementsPerRow;
		this.mi_elementsPerColumn = ai_elementsPerColumn;
		int num = this.mto_selectableColors.Length;
		this.mto_colorButtons = new GUIDefines.ButtonData[num];
		this.mto_colorMasks = new GUIDefines.TextureData[num];
		this.mto_colorBalls = new GUIDefines.TextureData[num];
		this.mto_colorBg = new GUIDefines.TextureData[num];
		this.mto_colorBgHighlighted = new GUIDefines.TextureData[num];
		GUIDefines.TextureInfo textureInfo = new GUIDefines.TextureInfo
		{
			name = this.msz_colorBgResource
		};
		textureInfo.Init();
		GUIDefines.TextureInfo textureInfo2 = new GUIDefines.TextureInfo
		{
			name = this.msz_colorBgHighlightedResource
		};
		textureInfo2.Init();
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < this.mto_selectableColors.Length; i++)
		{
			Color color = Utilities.m_cPenguinColors[(int)this.mto_selectableColors[i]];
			Vector2 vector = new Vector2(this.mo_area.leftRatio + (float)num3 * (this.mo_area.widthRatio / (float)this.mi_elementsPerRow), this.mo_area.topRatio + (float)num2 * (this.mo_area.heightRatio / (float)this.mi_elementsPerColumn));
			this.mto_colorBg[i] = new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = vector.x,
					topRatio = vector.y,
					widthRatio = av2_itemSizeRatio.x,
					heightRatio = av2_itemSizeRatio.y,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				icon = textureInfo
			};
			this.mto_colorBg[i].Init();
			this.mto_colorBgHighlighted[i] = new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = vector.x,
					topRatio = vector.y,
					widthRatio = av2_itemSizeRatio.x,
					heightRatio = av2_itemSizeRatio.y,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				icon = textureInfo2
			};
			this.mto_colorBgHighlighted[i].Init();
			this.mto_colorMasks[i] = new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = vector.x,
					topRatio = vector.y,
					widthRatio = av2_itemSizeRatio.x,
					heightRatio = av2_itemSizeRatio.y,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				icon = new GUIDefines.TextureInfo
				{
					name = this.msz_colorMaskResource
				},
				bgInfo = new GUIDefines.BackgroundInfo
				{
					useBgColor = true,
					bgColor = color
				}
			};
			this.mto_colorMasks[i].Init();
			this.mto_colorBalls[i] = new GUIDefines.TextureData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = vector.x,
					topRatio = vector.y,
					widthRatio = av2_itemSizeRatio.x,
					heightRatio = av2_itemSizeRatio.y,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				icon = new GUIDefines.TextureInfo
				{
					name = this.msz_colorBallResource
				}
			};
			this.mto_colorBalls[i].Init();
			this.mto_colorButtons[i] = new GUIDefines.ButtonData
			{
				pos = new GUIDefines.RectInfo
				{
					leftRatio = vector.x,
					topRatio = vector.y,
					widthRatio = av2_itemSizeRatio.x,
					heightRatio = av2_itemSizeRatio.y,
					IPad = new GUIDefines.RectIPadInfo
					{
						keepSizeRatio = true
					}
				},
				buttonId = i
			};
			this.mto_colorButtons[i].Init();
			num3++;
			if (num3 >= this.mi_elementsPerRow)
			{
				num2++;
				num3 = 0;
			}
		}
		this.OnColorSelect(0);
	}

	public ColorPickerZone()
	{
	}

	public Utilities.PenguinColors SelectedColor
	{
		get
		{
			return this.m_SelectedColor;
		}
	}

	protected override void CreateLayouts()
	{
	}

	public virtual void Update()
	{
	}

	public void RegisterCallback(ColorPickerZone.ColorPickerZoneCallback aCallback)
	{
		this.m_Callback = aCallback;
	}

	protected override void OnButtonSelect()
	{
		if (this.m_Callback != null)
		{
			this.m_Callback(base.SelectedButton);
		}
	}

	protected override void OnButtonSelect(int aSelectedButton)
	{
		if (this.m_Callback != null)
		{
			this.m_Callback(aSelectedButton);
		}
	}

	public override void Draw()
	{
		if (this.CanDraw())
		{
			base.Draw();
			int num = GUICompoundControls.Buttons(base.LocalTransform.position, this.mto_colorButtons);
			GUICompoundControls.Textures(base.LocalTransform.position, this.mto_colorBg);
			GUICompoundControls.Textures(base.LocalTransform.position, this.mto_colorBalls);
			GUICompoundControls.Textures(base.LocalTransform.position, this.mto_colorMasks);
			if (this.mto_colorBgHighlightedCurrent[0] != null)
			{
				GUICompoundControls.Textures(base.LocalTransform.position, this.mto_colorBgHighlightedCurrent);
			}
			this.OnColorSelect(num);
		}
	}

	private void OnColorSelect(int aSelectedButton)
	{
		if (this.IsValidButton(aSelectedButton))
		{
			this.m_SelectedColor = this.mto_selectableColors[aSelectedButton];
			this.mto_colorBgHighlightedCurrent[0] = this.mto_colorBgHighlighted[aSelectedButton];
			base.SelectedButton = aSelectedButton;
			this.OnButtonSelect();
		}
	}

	protected ColorPickerZone.ColorPickerZoneCallback m_Callback;

	private GUIDefines.RectInfo mo_area;

	private string msz_colorBallResource;

	private string msz_colorMaskResource;

	private string msz_colorBgResource;

	private string msz_colorBgHighlightedResource;

	private int mi_elementsPerRow;

	private int mi_elementsPerColumn;

	private GUIDefines.ButtonData[] mto_colorButtons;

	private GUIDefines.TextureData[] mto_colorMasks;

	private GUIDefines.TextureData[] mto_colorBalls;

	private GUIDefines.TextureData[] mto_colorBg;

	private GUIDefines.TextureData[] mto_colorBgHighlighted;

	private GUIDefines.TextureData[] mto_colorBgHighlightedCurrent;

	private Utilities.PenguinColors[] mto_selectableColors = new Utilities.PenguinColors[]
	{
		Utilities.PenguinColors.eBlue,
		Utilities.PenguinColors.eGreen,
		Utilities.PenguinColors.ePink,
		Utilities.PenguinColors.eBlack,
		Utilities.PenguinColors.eRed,
		Utilities.PenguinColors.eOrange,
		Utilities.PenguinColors.eYellowMustard,
		Utilities.PenguinColors.eDarkPurple,
		Utilities.PenguinColors.eBrown,
		Utilities.PenguinColors.ePeach,
		Utilities.PenguinColors.eDarkGreen,
		Utilities.PenguinColors.eLightBlue,
		Utilities.PenguinColors.eLimeGreen,
		Utilities.PenguinColors.eAqua
	};

	private Utilities.PenguinColors m_SelectedColor;

	public delegate void ColorPickerZoneCallback(int aSelectedButton);
}
