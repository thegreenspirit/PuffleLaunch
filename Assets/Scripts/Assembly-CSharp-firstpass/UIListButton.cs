using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/List Button")]
public class UIListButton : UIListItem
{
	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (this.deleted)
		{
			return;
		}
		if (!this.m_controlIsEnabled)
		{
			base.DoNeccessaryInput(ref ptr);
			return;
		}
		if (this.list != null && Vector3.SqrMagnitude(ptr.origPos - ptr.devicePos) > this.list.dragThreshold * this.list.dragThreshold)
		{
			ptr.isTap = false;
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
			}
		}
		else
		{
			ptr.isTap = true;
		}
		if (this.inputDelegate != null)
		{
			this.inputDelegate(ref ptr);
		}
		if (!this.m_controlIsEnabled)
		{
			base.DoNeccessaryInput(ref ptr);
			return;
		}
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			if (ptr.active && this.list != null)
			{
				this.list.ListDragged(ptr);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.PRESS:
			this.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.TAP:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			if (this.list != null && ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				this.list.DidClick(this);
			}
			if (this.list != null)
			{
				this.list.PointerReleased();
			}
			if (ptr.type != POINTER_INFO.POINTER_TYPE.TOUCHPAD && ptr.hitInfo.collider == base.GetComponent<Collider>())
			{
				this.SetControlState(UIButton.CONTROL_STATE.OVER);
			}
			else
			{
				this.SetControlState(UIButton.CONTROL_STATE.NORMAL);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE:
			if (this.soundOnOver != null && this.m_ctrlState != UIButton.CONTROL_STATE.OVER)
			{
				this.soundOnOver.PlayOneShot(this.soundOnOver.clip);
			}
			this.SetControlState(UIButton.CONTROL_STATE.OVER);
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			this.SetControlState(UIButton.CONTROL_STATE.NORMAL);
			break;
		case POINTER_INFO.INPUT_EVENT.DRAG:
			if (!ptr.isTap)
			{
				this.SetControlState(UIButton.CONTROL_STATE.NORMAL);
				if (this.list != null)
				{
					this.list.ListDragged(ptr);
				}
			}
			else
			{
				this.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
			}
			break;
		}
		if (this.list != null && ptr.inputDelta.z != 0f && ptr.type != POINTER_INFO.POINTER_TYPE.RAY)
		{
			this.list.ScrollWheel(ptr.inputDelta.z);
		}
		if (this.Container != null)
		{
			ptr.callerIsControl = true;
			this.Container.OnInput(ptr);
		}
		if (this.repeat)
		{
			if (this.m_ctrlState == UIButton.CONTROL_STATE.ACTIVE)
			{
				goto IL_02D9;
			}
		}
		else if (ptr.evt == this.whenToInvoke)
		{
			goto IL_02D9;
		}
		return;
		IL_02D9:
		if (ptr.evt == this.whenToInvoke && this.soundOnClick != null)
		{
			this.soundOnClick.PlayOneShot(this.soundOnClick.clip);
		}
		if (this.scriptWithMethodToInvoke != null)
		{
			this.scriptWithMethodToInvoke.Invoke(this.methodToInvoke, this.delay);
		}
		if (this.changeDelegate != null)
		{
			this.changeDelegate(this);
		}
	}

	public new static UIListButton Create(string name, Vector3 pos)
	{
		return (UIListButton)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIListButton));
	}

	public new static UIListButton Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UIListButton)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UIListButton));
	}
}
