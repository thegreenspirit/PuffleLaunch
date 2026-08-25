using System;
using System.Collections.Generic;

public class UIControlHandler<T>
{
	public UIControlHandler(List<T> aControlList, List<string> aControlName)
	{
		this.m_ControlList = aControlList;
		this.m_ControlName = aControlName;
	}

	public virtual void EnableAll(bool aEnable)
	{
		for (int i = 0; i < this.m_ControlList.Count; i++)
		{
			this.Enable(i, aEnable);
		}
	}

	public virtual void Enable(string aControlName, bool aEnable)
	{
		int indexByName = this.GetIndexByName(aControlName);
		if (indexByName >= 0)
		{
			this.Enable(indexByName, aEnable);
		}
	}

	public virtual void Enable(int aControlIndex, bool aEnable)
	{
		IUIControlExtension iuicontrolExtension = this.m_ControlList[aControlIndex] as IUIControlExtension;
		Utilities.AssertMsg(iuicontrolExtension != null, "Cannot use UIController to handle a control that is not using IUIControlExtension: " + this.m_ControlList[aControlIndex]);
		if (iuicontrolExtension != null)
		{
			iuicontrolExtension.Enable(aEnable);
		}
	}

	public virtual void ShowAll(bool aShow)
	{
		for (int i = 0; i < this.m_ControlList.Count; i++)
		{
			this.Show(i, aShow);
		}
	}

	public virtual void Show(string aControlName, bool aShow)
	{
		int indexByName = this.GetIndexByName(aControlName);
		if (indexByName >= 0)
		{
			this.Show(indexByName, aShow);
		}
	}

	public virtual void Show(int aControlIndex, bool aShow)
	{
		IUIControlExtension iuicontrolExtension = this.m_ControlList[aControlIndex] as IUIControlExtension;
		Utilities.AssertMsg(iuicontrolExtension != null, "Cannot use generic controller to handle a control that is not using IUIControlExtension: " + this.m_ControlList[aControlIndex]);
		if (iuicontrolExtension != null)
		{
			iuicontrolExtension.Show(aShow);
		}
	}

	protected virtual int GetIndexByName(string aControlName)
	{
		int num = this.m_ControlName.FindIndex((string name) => name == aControlName);
		Utilities.AssertMsg(num >= 0, "Control name '" + aControlName + "' not found!");
		return num;
	}

	public virtual T GetControl(string aControlName)
	{
		int indexByName = this.GetIndexByName(aControlName);
		if (indexByName >= 0)
		{
			return this.m_ControlList[indexByName];
		}
		return default(T);
	}

	public List<T> m_ControlList;

	public List<string> m_ControlName;
}
