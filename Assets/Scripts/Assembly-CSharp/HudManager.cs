using System;
using UnityEngine;

public class HudManager
{
	public HudManager(GameObject aRefObj)
	{
		this.m_RefObj = aRefObj;
	}

	public InGameHud InGameHud
	{
		get
		{
			return this.m_InGameHud;
		}
	}

	public void Draw()
	{
		this.DrawInGameHud();
	}

	public void Update()
	{
		this.m_InGameHud.Update();
	}

	private void DrawInGameHud()
	{
		if (Utilities.Assert(this.m_InGameHud != null) && this.m_InGameHud.CanDraw())
		{
			this.m_InGameHud.Draw();
		}
	}

	public void ShowInGameHud(bool aShow)
	{
		if (aShow)
		{
			if (this.m_InGameHud == null)
			{
				this.m_InGameHud = new InGameHud(this.m_RefObj);
			}
		}
		else
		{
			this.m_InGameHud.SetVisible(false);
		}
	}

	public void CleanUp()
	{
		this.m_InGameHud = null;
	}

	private GameObject m_RefObj;

	private InGameHud m_InGameHud;
}
