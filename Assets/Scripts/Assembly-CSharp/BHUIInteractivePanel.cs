using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("EZ GUI/Panels/Interactive Panel")]
public class BHUIInteractivePanel : UIInteractivePanel, IUIPanelExtension
{
	public event BHUIInteractivePanel.ActivatePanelHandler activatePanel;

	public event BHUIInteractivePanel.DeactivatePanelHandler deactivatePanel;

	public BHUIPanelManager PanelManager
	{
		get
		{
			return this.panelManager;
		}
		set
		{
			this.panelManager = value;
		}
	}

	public List<BHUIButton> ButtonList
	{
		get
		{
			return this.buttonList;
		}
		set
		{
			this.buttonList = value;
		}
	}

	public List<string> ButtonName
	{
		get
		{
			return this.buttonName;
		}
		set
		{
			this.buttonName = value;
		}
	}

	public List<BHUITexture> TextureList
	{
		get
		{
			return this.textureList;
		}
		set
		{
			this.textureList = value;
		}
	}

	public List<string> TextureName
	{
		get
		{
			return this.textureName;
		}
		set
		{
			this.textureName = value;
		}
	}

	public List<BHUILabel> LabelList
	{
		get
		{
			return this.labelList;
		}
		set
		{
			this.labelList = value;
		}
	}

	public List<string> LabelName
	{
		get
		{
			return this.labelName;
		}
		set
		{
			this.labelName = value;
		}
	}

	public List<BHUITextField> TextFieldList
	{
		get
		{
			return this.textFieldList;
		}
		set
		{
			this.textFieldList = value;
		}
	}

	public List<string> TextFieldName
	{
		get
		{
			return this.textFieldName;
		}
		set
		{
			this.textFieldName = value;
		}
	}

	public UIControlHandler<BHUIButton> ButtonHandler
	{
		get
		{
			return this.m_ButtonHandler;
		}
	}

	public UIControlHandler<BHUITexture> TextureHandler
	{
		get
		{
			return this.m_TextureHandler;
		}
	}

	public UIControlHandler<BHUILabel> LabelHandler
	{
		get
		{
			return this.m_LabelHandler;
		}
	}

	public UIControlHandler<BHUITextField> TextFieldHandler
	{
		get
		{
			return this.m_TextFieldHandler;
		}
	}

	public virtual void Awake()
	{
		Utilities.AssertMsg(this.panelManager != null, "Panel: " + base.gameObject + " doesn't have a valid panel manager!");
		if (this.panelManager != null)
		{
			this.panelManager.AddChild(base.gameObject);
		}
		this.m_ButtonHandler = new UIControlHandler<BHUIButton>(this.buttonList, this.buttonName);
		this.m_TextureHandler = new UIControlHandler<BHUITexture>(this.textureList, this.textureName);
		this.m_LabelHandler = new UIControlHandler<BHUILabel>(this.labelList, this.labelName);
		this.m_TextFieldHandler = new UIControlHandler<BHUITextField>(this.textFieldList, this.textFieldName);
		if (this.m_DismissOnStart)
		{
			base.StartCoroutine(this.DismissAfterControlsReady());
		}
	}

	private IEnumerator DismissAfterControlsReady()
	{
		if (this.deactivateAllOnDismiss && this.m_LabelHandler.m_ControlList.Count > 0)
		{
			foreach (BHUILabel label in this.m_LabelHandler.m_ControlList)
			{
				while (!label.IsReady)
				{
					yield return null;
				}
			}
		}
		this.Dismiss();
		yield break;
	}

	public virtual void Activate(bool aActivate)
	{
		if (Utilities.AssertMsg(this.panelManager != null, "Invalid panel manager, make sure you run 'Setup Panel' in edit mode!"))
		{
			for (int i = 0; i < this.m_DisableButtonOnPanelsWhenActive.Count; i++)
			{
				IUIPanelExtension iuipanelExtension = this.m_DisableButtonOnPanelsWhenActive[i] as IUIPanelExtension;
				if (Utilities.AssertMsg(iuipanelExtension != null, "Invalid or unknown type panel: " + this.m_DisableButtonOnPanelsWhenActive[i]))
				{
					iuipanelExtension.ButtonHandler.EnableAll(!aActivate);
				}
			}
		}
		if (aActivate)
		{
			this.BringIn();
			if (this.activatePanel != null)
			{
				this.activatePanel();
				this.activatePanel = null;
			}
		}
		else
		{
			this.Dismiss();
			if (this.deactivatePanel != null)
			{
				this.deactivatePanel();
				this.deactivatePanel = null;
			}
		}
	}

	public bool m_DismissOnStart;

	public List<UIPanelBase> m_DisableButtonOnPanelsWhenActive = new List<UIPanelBase>();

	public BHUIPanelManager panelManager;

	public List<BHUIButton> buttonList;

	public List<string> buttonName;

	public List<BHUITexture> textureList;

	public List<string> textureName;

	public List<BHUILabel> labelList;

	public List<string> labelName;

	public List<BHUITextField> textFieldList;

	public List<string> textFieldName;

	protected UIControlHandler<BHUIButton> m_ButtonHandler;

	protected UIControlHandler<BHUITexture> m_TextureHandler;

	protected UIControlHandler<BHUILabel> m_LabelHandler;

	protected UIControlHandler<BHUITextField> m_TextFieldHandler;

	public delegate void ActivatePanelHandler();

	public delegate void DeactivatePanelHandler();
}
