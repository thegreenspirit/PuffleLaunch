using System;
using UnityEngine;

public abstract class BasePopup : BaseGUI
{
	public BasePopup(GameObject aRefObj)
		: base(aRefObj)
	{
	}

	public GUIDefines.WindowData WindowData
	{
		get
		{
			return this.m_WindowData;
		}
		set
		{
			this.m_WindowData = value;
		}
	}

	public bool IsShowing
	{
		get
		{
			return this.m_IsShowing;
		}
	}

	protected void InitPopup()
	{
		if (!this.m_IsPopupInitialized)
		{
			base.InitLayouts();
			this.m_WindowData.Init();
			if (this.m_WindowBackground != null)
			{
				this.m_WindowBackground.Init();
			}
			this.m_IsPopupInitialized = true;
		}
	}

	public virtual void ClosePopup()
	{
	}

	protected override void InitLayouts()
	{
	}

	public override void Draw()
	{
		if (!this.CanDraw()) return;
		this.InitPopup();
		if (this.m_WindowBackground != null)
		{
			GUICompoundControls.Window(base.LocalTransform.position, this.m_WindowBackground, new GUI.WindowFunction(this.WindowContent));
		}
		GUICompoundControls.Window(base.LocalTransform.position, this.m_WindowData, new GUI.WindowFunction(this.WindowContent));
	}

	public override bool CanDraw()
	{
		return base.CanDraw() && this.m_IsShowing;
	}

	protected override void OnButtonSelect()
	{
		this.Show(false);
		if (this.m_Callback != null)
		{
			this.m_Callback(base.SelectedButton);
		}
	}

	protected void OnAutoSelect(int aSelection)
	{
		base.SelectedButton = aSelection;
		this.OnButtonSelect();
	}

	public virtual void Show(bool aShow)
	{
		if (aShow)
		{
			this.ResetButton();
		}
		this.m_IsShowing = aShow;
	}

	public void RegisterCallback(BasePopup.PopupCallback aCallback)
	{
		this.m_Callback = aCallback;
	}

	protected virtual void WindowContent(int aWindowId)
	{
		if (this.m_WindowBackground != null && aWindowId == this.m_WindowBackground.id)
		{
			// Green Spirit: What?
			// GUI.BringWindowToFront(aWindowId);
			GUIUtil.DrawSemiTransparentLayer();
		}
		else if (aWindowId == this.m_WindowData.id)
		{
			if (this.m_WindowBackground != null)
			{
				GUI.BringWindowToFront(aWindowId);
			}
			this.DrawWindowContent(aWindowId);
		}
	}

	protected virtual void DrawWindowContent(int aWindowId)
	{
		base.Draw();
	}

	public const int kPopupBackgroundId = 9;

	public const int kPopupWindowId = 10;

	public const int kCreateAccountPopupWindowId = 11;

	public const int kLoginPopupWindowId = 12;

	protected GUIDefines.WindowData m_WindowData;

	protected GUIDefines.WindowData m_WindowBackground = new GUIDefines.WindowData
	{
		pos = new GUIDefines.RectInfo
		{
			widthRatio = 1f,
			heightRatio = 1f
		},
		id = 9
	};

	protected bool m_IsShowing;

	protected BasePopup.PopupCallback m_Callback;

	protected bool m_IsPopupInitialized;

	public delegate void PopupCallback(int aSelectedButton);
}
