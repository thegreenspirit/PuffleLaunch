using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Load Scene Button")]
public class UIBtnLoadScene : UIButton
{
	public void LoadSceneDelegate(UIPanelBase panel, EZTransition trans)
	{
		base.StartCoroutine(this.LoadScene());
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (this.deleted)
		{
			return;
		}
		base.OnInput(ref ptr);
		if (!this.m_controlIsEnabled || base.IsHidden())
		{
			return;
		}
		if (ptr.evt == this.whenToInvoke)
		{
			if (this.loadingPanel != null)
			{
				UIPanelManager uipanelManager = (UIPanelManager)this.loadingPanel.Container;
				this.loadingPanel.AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.LoadSceneDelegate));
				if (uipanelManager is UIPanelManager && uipanelManager != null)
				{
					uipanelManager.BringIn(this.loadingPanel);
				}
				else
				{
					this.loadingPanel.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
				}
			}
			else
			{
				base.Invoke("DoLoadScene", this.delay);
			}
		}
	}

	protected void DoLoadScene()
	{
		base.StartCoroutine(this.LoadScene());
	}

	protected IEnumerator LoadScene()
	{
		yield return null;
		Application.LoadLevel(this.scene);
		yield break;
	}

	public override void Copy(SpriteRoot s)
	{
		this.Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (!(s is UIBtnLoadScene))
		{
			return;
		}
		UIBtnLoadScene uibtnLoadScene = (UIBtnLoadScene)s;
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			this.scene = uibtnLoadScene.scene;
			this.loadingPanel = uibtnLoadScene.loadingPanel;
		}
	}

	public new static UIBtnLoadScene Create(string name, Vector3 pos)
	{
		return (UIBtnLoadScene)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIBtnLoadScene));
	}

	public new static UIBtnLoadScene Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UIBtnLoadScene)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UIBtnLoadScene));
	}

	public string scene;

	public UIPanelBase loadingPanel;
}
