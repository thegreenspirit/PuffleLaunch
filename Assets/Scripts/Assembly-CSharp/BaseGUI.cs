using System;
using UnityEngine;

public abstract class BaseGUI
{
	public BaseGUI(GameObject aRefObj)
	{
		this.Init(aRefObj);
	}

	public BaseGUI()
	{
	}

	public GUIDefines.ButtonData[] ButtonData
	{
		get
		{
			return this.m_ButtonData;
		}
		set
		{
			this.m_ButtonData = value;
		}
	}

	public GUIDefines.TextureData[] TextureData
	{
		get
		{
			return this.m_TextureData;
		}
		set
		{
			this.m_TextureData = value;
		}
	}

	public GUIDefines.LabelData[] LabelData
	{
		get
		{
			return this.m_LabelData;
		}
		set
		{
			this.m_LabelData = value;
		}
	}

	public GUIDefines.TextFieldData[] TextFieldData
	{
		get
		{
			return this.m_TextFieldData;
		}
		set
		{
			this.m_TextFieldData = value;
		}
	}

	public Transform LocalTransform
	{
		get
		{
			return this.m_LocalTransform;
		}
	}

	public int SelectedButton
	{
		get
		{
			return this.m_SelectedButton;
		}
		set
		{
			this.m_SelectedButton = value;
		}
	}

	public bool HasSelectedButton()
	{
		return this.m_SelectedButton != -1;
	}

	public GUIDefines.ButtonData SelectedButtonData
	{
		get
		{
			for (int i = 0; i < this.m_ButtonData.Length; i++)
			{
				if (this.m_SelectedButton == this.m_ButtonData[i].buttonId)
				{
					return this.m_ButtonData[i];
				}
			}
			Utilities.AssertMsg(false, "Tried to get selected button's data, but there is none! You can use HasSelectedButton() to avoid this assert");
			return new GUIDefines.ButtonData();
		}
	}

	public bool StopDraw
	{
		get
		{
			return this.m_StopDraw;
		}
		set
		{
			this.m_StopDraw = value;
		}
	}

	public BaseGUI.GUIPriority Priority
	{
		get
		{
			return this.m_Priority;
		}
		set
		{
			this.m_Priority = value;
		}
	}

	protected abstract void CreateLayouts();

	protected abstract void OnButtonSelect();

	public virtual void Init(GameObject aRefObj)
	{
		this.InitReference(aRefObj);
		this.CreateLayouts();
		this.InitLayouts();
	}

	protected virtual void InitLayouts()
	{
		if (this.m_ButtonData != null)
		{
			for (int i = 0; i < this.m_ButtonData.Length; i++)
			{
				this.m_ButtonData[i].Init();
			}
		}
		if (this.m_TextureData != null)
		{
			for (int j = 0; j < this.m_TextureData.Length; j++)
			{
				this.m_TextureData[j].Init();
			}
		}
		if (this.m_LabelData != null)
		{
			for (int k = 0; k < this.m_LabelData.Length; k++)
			{
				this.m_LabelData[k].Init();
			}
		}
		if (this.m_TextFieldData != null)
		{
			for (int l = 0; l < this.m_TextFieldData.Length; l++)
			{
				this.m_TextFieldData[l].Init();
			}
		}
	}

	protected virtual void InitReference(GameObject aRefObj)
	{
		if (Utilities.Assert(aRefObj != null))
		{
			this.m_RefObj = aRefObj;
			this.m_LocalTransform = this.m_RefObj.transform;
		}
	}

	public virtual void Draw()
	{
		GUI.matrix = GameFlowManager.Instance.GUIManager.m_NewResMatrix;
		if (this.m_TextureData != null)
		{
			GUICompoundControls.Textures(this.m_LocalTransform.position, this.m_TextureData);
		}
		if (this.m_TextFieldData != null)
		{
			GUICompoundControls.TextFields(this.m_LocalTransform.position, this.m_TextFieldData, this.IsControlBlocked());
			this.MoveScreenUpToShowHiddenTextField();
		}
		if (this.m_ButtonData != null)
		{
			int num = GUICompoundControls.Buttons(this.m_LocalTransform.position, this.m_ButtonData);
			if (num >= 0)
			{
				this.OnButtonSelect(num);
			}
		}
		if (this.m_LabelData != null)
		{
			GUICompoundControls.Labels(this.m_LocalTransform.position, this.m_LabelData);
		}
	}

	protected virtual void MoveScreenUpToShowHiddenTextField()
	{
		if (this.m_TextFieldData == null)
		{
			return;
		}
		for (int i = 0; i < this.m_TextFieldData.Length; i++)
		{
#if UNITY_ANDROID || UNITY_IOS
			if (TouchScreenKeyboard.visible)
			{
				if (this.m_TextFieldData[i].isFocused && this.m_LastFocusedTextField != i)
				{
					this.m_LastFocusedTextField = i;
					bool flag = false;
					if (this.m_TextFieldData[i].pos.inPixel.yMax >= TouchScreenKeyboard.area.yMin)
					{
						float num = 0f;
						if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eOriginal)
						{
							num = 170f;
						}
						float num2 = 30f;
						num2 *= 640f / GUIConstants.kReferenceScreenHeight;
						float yMin = TouchScreenKeyboard.area.yMin;
						float num3 = (this.m_TextFieldData[i].pos.inPixel.yMax - yMin - num) / num2;
						if (num3 > 0f)
						{
							this.m_LocalTransform.position = new Vector3(0f, num3, 0f);
							flag = true;
						}
					}
					if (!flag)
					{
						this.m_LocalTransform.position = Vector3.zero;
					}
				}
			}
			else if (this.m_WasKeyboardVisible)
			{
				this.m_LastFocusedTextField = -1;
				this.m_LocalTransform.position = Vector3.zero;
			}
			this.m_WasKeyboardVisible = TouchScreenKeyboard.visible;
#endif
		}
	}

	public virtual bool CanDraw()
	{
		GUI.skin = GameFlowManager.Instance.GUIManager.Skin;
		Utilities.Assert(this.m_LocalTransform != null);
		return GUI.skin != null && this.m_LocalTransform != null && !this.m_StopDraw;
	}

	protected virtual void OnButtonSelect(int aSelectedButton)
	{
		if (!this.IsControlBlocked() && this.IsValidButton(aSelectedButton))
		{
			this.m_SelectedButton = aSelectedButton;
			this.OnButtonSelect();
		}
	}

	public virtual void ResetButton()
	{
		this.m_SelectedButton = -1;
	}

	public virtual bool IsAnyButtonSelected()
	{
		return this.IsValidButton(this.m_SelectedButton);
	}

	public virtual bool IsValidButton(int aButtonId)
	{
		return aButtonId != -1;
	}

	public virtual void BlockControl(bool aBlockControl)
	{
		this.m_BlockControl = aBlockControl;
		if (this.ButtonData != null)
		{
			for (int i = 0; i < this.ButtonData.Length; i++)
			{
				this.ButtonData[i].isControlBlocked = aBlockControl;
			}
		}
	}

	public virtual bool IsControlBlocked()
	{
		if (this.m_Priority < BaseGUI.GUIPriority.eHigh)
		{
			return this.m_BlockControl || NetManager.Instance.IsNetPopupShowing || GameFlowManager.Instance.GUIManager.CurrentScene == GUIManager.Scene.eLoadingScreen;
		}
		return this.m_BlockControl;
	}

	public virtual void StopGUI()
	{
		this.m_StopDraw = true;
		this.m_BlockControl = true;
		GUIStyleContainer.CleanUp();
	}

	public virtual int GetButtonIndex(int aButtonId)
	{
		if (this.m_ButtonData != null)
		{
			for (int i = 0; i < this.m_ButtonData.Length; i++)
			{
				if (this.m_ButtonData[i].buttonId == aButtonId)
				{
					return i;
				}
			}
		}
		Utilities.Assert(false);
		return -1;
	}

	public void SetLabelTextId(int aLabelIndex, string aTextId)
	{
		if (this.LabelData.Length > aLabelIndex)
		{
			this.LabelData[aLabelIndex].content.text = string.Empty;
			this.LabelData[aLabelIndex].content.textId = aTextId;
		}
	}

	public void SetLabelText(int aLabelIndex, string aText)
	{
		if (this.LabelData.Length > aLabelIndex)
		{
			this.LabelData[aLabelIndex].content.text = aText;
			this.LabelData[aLabelIndex].content.textId = string.Empty;
		}
	}

	public void SetButtonTextId(int aButtonId, string aTextId)
	{
		int buttonIndex = this.GetButtonIndex(aButtonId);
		if (buttonIndex != -1)
		{
			this.ButtonData[buttonIndex].content.text = string.Empty;
			this.ButtonData[buttonIndex].content.textId = aTextId;
		}
	}

	public void SetButtonText(int aButtonId, string aText)
	{
		int buttonIndex = this.GetButtonIndex(aButtonId);
		if (buttonIndex != -1)
		{
			this.ButtonData[buttonIndex].content.text = aText;
			this.ButtonData[buttonIndex].content.textId = string.Empty;
		}
	}

	public void SetLabelInvisible(int aLabelIndex, bool aInvisible)
	{
		if (this.LabelData.Length > aLabelIndex)
		{
			this.LabelData[aLabelIndex].invisible = aInvisible;
		}
	}

	public void SetTextureInvisible(int aTextureIndex, bool aInvisible)
	{
		if (this.TextureData.Length > aTextureIndex)
		{
			this.TextureData[aTextureIndex].invisible = aInvisible;
		}
	}

	public void SetButtonInvisible(int aButtonId, bool aInvisible)
	{
		int buttonIndex = this.GetButtonIndex(aButtonId);
		if (buttonIndex != -1)
		{
			this.ButtonData[buttonIndex].invisible = aInvisible;
		}
	}

	private GUIDefines.ButtonData[] m_ButtonData;
	private GUIDefines.TextureData[] m_TextureData;
	private GUIDefines.LabelData[] m_LabelData;
	private GUIDefines.TextFieldData[] m_TextFieldData;

	private GameObject m_RefObj;
	private Transform m_LocalTransform;

	private int m_SelectedButton = -1;
	private bool m_StopDraw;
	private bool m_BlockControl;
	private BaseGUI.GUIPriority m_Priority = BaseGUI.GUIPriority.eNormal;
	private int m_LastFocusedTextField = -1;
	private bool m_WasKeyboardVisible;

	public enum GUIPriority
	{
		eLow,
		eNormal,
		eHigh,
		eGUIPriority_COUNT
	}
}
