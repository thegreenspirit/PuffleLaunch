using System;
using UnityEngine;

[RequireComponent(typeof(UIControlExtension))]
[AddComponentMenu("EZ GUI/Controls/Texture")]
public class BHUITexture : SimpleSprite, IUIControlExtension
{
	public bool IsReady
	{
		get
		{
			return this.m_IsReady;
		}
	}

	protected override void Awake()
	{
		this.m_ControlExt = base.gameObject.GetComponent<UIControlExtension>();
		Utilities.AssertMsgCritical(this.m_ControlExt != null, "Fail to get UIControlExtension component!");
		this.m_ControlExt.SetMaterialLocalizedTexture(this.m_Localized);
		this.SetupSimpleSprite();
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		this.m_IsReady = true;
	}

	public virtual void Update()
	{
		if (this.m_RotateDirection != BHUITexture.RotateDirection.eNone)
		{
			this.RotateTexture();
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
	}

	protected virtual void RotateTexture()
	{
	}

	public void Enable(bool aEnable)
	{
	}

	public void Show(bool aShow)
	{
		base.gameObject.SetActive(aShow);
		this.Hide(!aShow);
	}

	protected virtual void SetupSimpleSprite()
	{
		int assetLanguage = (int)this.m_ControlExt.AssetLanguage;
		if (assetLanguage < 0 || assetLanguage >= this.lowerLeftPixelSmall.Length || assetLanguage >= this.pixelDimensionsSmall.Length)
		{
			Utilities.AssertMsg(false, "Fail to set simple sprite uv due to invalid asset language: " + this.m_ControlExt.AssetLanguage);
			return;
		}
		Vector2 vector;
		Vector2 vector2;
		switch (this.m_ControlExt.AssetSizeCategoryId)
		{
		case SizeCategory.CategoryId.eSmall:
			vector = this.lowerLeftPixelSmall[assetLanguage];
			vector2 = this.pixelDimensionsSmall[assetLanguage];
			goto IL_00EF;
		case SizeCategory.CategoryId.eLarge:
			vector = this.lowerLeftPixelLarge[assetLanguage];
			vector2 = this.pixelDimensionsLarge[assetLanguage];
			goto IL_00EF;
		}
		vector = this.lowerLeftPixelMedium[assetLanguage];
		vector2 = this.pixelDimensionsMedium[assetLanguage];
		IL_00EF:
		if (this.m_FullScreenTile)
		{
			vector2.x = (float)Screen.width;
			vector2.y = (float)Screen.height;
		}
		if (this.m_HorizontalMirror)
		{
			vector.x += vector2.x;
			vector2.x *= -1f;
		}
		if (this.m_VericalMirror)
		{
			vector.y -= vector2.y;
			vector2.y *= -1f;
		}
		base.SetLowerLeftPixel(vector);
		base.SetPixelDimensions(vector2);
	}

	public Vector2[] lowerLeftPixelSmall = new Vector2[6];

	public Vector2[] pixelDimensionsSmall = new Vector2[6];

	public Vector2[] lowerLeftPixelMedium = new Vector2[6];

	public Vector2[] pixelDimensionsMedium = new Vector2[6];

	public Vector2[] lowerLeftPixelLarge = new Vector2[6];

	public Vector2[] pixelDimensionsLarge = new Vector2[6];

	public Texture2D defaultAtlasTexture;

	public string defaultAtlasTexturePath;

	public bool m_Localized;

	public bool m_FullScreenTile;

	public bool m_HorizontalMirror;

	public bool m_VericalMirror;

	public BHUITexture.RotateDirection m_RotateDirection = BHUITexture.RotateDirection.eNone;

	protected float m_CurrentRotateAngle;

	protected UIControlExtension m_ControlExt;

	private bool m_IsReady;

	public enum RotateDirection
	{
		eNone = -1,
		eClockwise,
		eCounterClockwise
	}
}
