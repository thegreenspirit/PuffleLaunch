using System;
using UnityEngine;

[RequireComponent(typeof(UIControlExtension))]
[RequireComponent(typeof(AutoAdjustSpriteText))]
[RequireComponent(typeof(DropShadow))]
[AddComponentMenu("EZ GUI/Controls/Label")]
public class BHUILabel : SpriteText, IUIControlExtension
{
	public UIControlExtension ControlExt
	{
		get
		{
			return this.m_ControlExt;
		}
	}

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
		this.m_AutoAdjust = base.gameObject.GetComponent<AutoAdjustSpriteText>();
		this.m_DropShadow = base.gameObject.GetComponent<DropShadow>();
		this.SetupSpriteText();
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		string localizeText = this.m_ControlExt.GetLocalizeText();
		if (!string.IsNullOrEmpty(localizeText))
		{
			base.Text = localizeText;
		}
		if (this.m_AutoAdjust)
		{
			this.m_AutoAdjust.AutoAdjust();
		}
		if (this.m_DropShadow)
		{
			this.m_DropShadow.CreateShadow();
		}
		this.m_IsReady = true;
	}

	public void Enable(bool aEnable)
	{
	}

	public void Show(bool aShow)
	{
		base.gameObject.SetActive(aShow);
		this.Hide(!aShow);
	}

	public void UpdateDropShadow()
	{
		if (this.m_DropShadow != null)
		{
			this.m_DropShadow.UpdateDropShadowText();
			this.m_DropShadow.UpdateDropShadowSize();
		}
	}

	public void HideDropShadow(bool aHide)
	{
		if (this.m_DropShadow != null)
		{
			this.m_DropShadow.HideDropShadowText(aHide);
		}
	}

	protected virtual void SetupSpriteText()
	{
		if (SizeCategory.Instance == null)
		{
			return;
		}
		switch (SizeCategory.Instance.CurCategoryId)
		{
		case SizeCategory.CategoryId.eSmall:
			if (this.m_MaxWidthSmall > 0f)
			{
				this.maxWidth = this.m_MaxWidthSmall;
			}
			break;
		case SizeCategory.CategoryId.eMedium:
			if (this.m_MaxWidthMedium > 0f)
			{
				this.maxWidth = this.m_MaxWidthMedium;
			}
			break;
		case SizeCategory.CategoryId.eLarge:
			if (this.m_MaxWidthLarge > 0f)
			{
				this.maxWidth = this.m_MaxWidthLarge;
			}
			break;
		}
	}

	public float m_MaxWidthSmall;

	public float m_MaxWidthMedium;

	public float m_MaxWidthLarge;

	protected UIControlExtension m_ControlExt;

	protected AutoAdjustSpriteText m_AutoAdjust;

	protected DropShadow m_DropShadow;

	protected bool m_IsReady;
}
