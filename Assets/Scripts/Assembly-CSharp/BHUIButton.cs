using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Button")]
[RequireComponent(typeof(UIControlExtension))]
public class BHUIButton : UIButton, IUIControlExtension
{
	protected override void Awake()
	{
		this.m_ControlExt = base.gameObject.GetComponent<UIControlExtension>();
		Utilities.AssertMsgCritical(this.m_ControlExt != null, "Fail to get UIControlExtension component!");
		this.m_ControlExt.SetMaterialTexture();
		this.UpdateSpriteFrameInfo();
		this.Text = string.Empty;
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
		this.AddValueChangedDelegate(new EZValueChangedDelegate(this.OnButtonPressed));
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		this.RemoveValueChangedDelegate(new EZValueChangedDelegate(this.OnButtonPressed));
	}

	protected virtual void OnButtonPressed(IUIObject obj)
	{
		if (this.m_PlayDefaultSFX)
		{
			GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		}
	}

	public void Enable(bool aEnable)
	{
		this.SetControlState((!aEnable) ? UIButton.CONTROL_STATE.DISABLED : UIButton.CONTROL_STATE.NORMAL);
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

	public bool m_PlayDefaultSFX = true;

	protected UIControlExtension m_ControlExt;
}
