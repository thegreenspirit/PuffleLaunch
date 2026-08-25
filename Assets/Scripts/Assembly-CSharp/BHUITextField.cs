using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Text Field")]
[RequireComponent(typeof(UIControlExtension))]
public class BHUITextField : UITextField, IUIControlExtension
{
	protected override void Awake()
	{
		this.m_ControlExt = base.gameObject.GetComponent<UIControlExtension>();
		Utilities.AssertMsgCritical(this.m_ControlExt != null, "Fail to get UIControlExtension component!");
		this.m_ControlExt.SetMaterialTexture();
		this.UpdateSpriteFrameInfo();
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		this.Text = this.m_ControlExt.GetLocalizeText();
		this.spriteText.SetColor(AutoAdjustSpriteText.GetColor(this.m_UnfocusTextColor));
		base.AddFocusDelegate(new UITextField.FocusDelegate(this.OnTextFieldFocus));
		base.AddCommitDelegate(new EZKeyboardCommitDelegate(this.OnTextFieldCommit));
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		base.RemoveFocusDelegate(new UITextField.FocusDelegate(this.OnTextFieldFocus));
		base.RemoveCommitDelegate(new EZKeyboardCommitDelegate(this.OnTextFieldCommit));
	}

	protected virtual void OnTextFieldFocus(UITextField field)
	{
		this.spriteText.SetColor(AutoAdjustSpriteText.GetColor(this.m_FocusTextColor));
	}

	protected virtual void OnTextFieldCommit(IKeyFocusable control)
	{
		this.spriteText.SetColor(AutoAdjustSpriteText.GetColor(this.m_UnfocusTextColor));
	}

	public void Enable(bool aEnable)
	{
	}

	public void Show(bool aShow)
	{
		base.gameObject.SetActive(aShow);
		this.Hide(!aShow);
	}

	protected virtual void UpdateSpriteFrameInfo()
	{
		switch (this.m_ControlExt.AssetSizeCategoryId)
		{
		case SizeCategory.CategoryId.eSmall:
		{
			for (int i = 0; i < this.states.Length; i++)
			{
				for (int j = 0; j < this.states[i].spriteFrames.Length; j++)
				{
					this.states[i].spriteFrames[j].CopyFromSmall();
				}
			}
			break;
		}
		case SizeCategory.CategoryId.eLarge:
		case SizeCategory.CategoryId.eXLarge:
		{
			for (int k = 0; k < this.states.Length; k++)
			{
				for (int l = 0; l < this.states[k].spriteFrames.Length; l++)
				{
					this.states[k].spriteFrames[l].CopyFromLarge();
				}
			}
			break;
		}
		}
	}

	public AutoAdjustSpriteText.SpriteTextColor m_UnfocusTextColor = AutoAdjustSpriteText.SpriteTextColor.eGrey;

	public AutoAdjustSpriteText.SpriteTextColor m_FocusTextColor = AutoAdjustSpriteText.SpriteTextColor.eGrey;

	protected UIControlExtension m_ControlExt;
}
