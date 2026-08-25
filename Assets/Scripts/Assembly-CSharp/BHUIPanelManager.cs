using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Management/Panel Manager")]
public class BHUIPanelManager : UIPanelManager
{
	public UIPanelBase GetPanel(int aPanelIndex)
	{
		for (int i = 0; i < this.panels.Count; i++)
		{
			if (this.panels[i].index == aPanelIndex)
			{
				return this.panels[i];
			}
		}
		Utilities.AssertMsg(false, "Panel (index = " + aPanelIndex + ") not found!");
		return null;
	}

	public UIPanelBase GetPanel(string aPanelName)
	{
		for (int i = 0; i < this.panels.Count; i++)
		{
			if (string.Equals(this.panels[i].name, aPanelName, StringComparison.CurrentCultureIgnoreCase))
			{
				return this.panels[i];
			}
		}
		Utilities.AssertMsg(false, "Panel (name = " + aPanelName + ") not found!");
		return null;
	}
}
