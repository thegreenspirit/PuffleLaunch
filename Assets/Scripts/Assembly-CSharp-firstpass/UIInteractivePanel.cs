using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Panels/Interactive Panel")]
[Serializable]
public class UIInteractivePanel : UIPanelBase
{
	public UIInteractivePanel.STATE State
	{
		get
		{
			return this.m_panelState;
		}
	}

	public override EZTransitionList Transitions
	{
		get
		{
			return this.transitions;
		}
	}

	public override void OnInput(POINTER_INFO ptr)
	{
		if (!this.m_controlIsEnabled)
		{
			return;
		}
		if (this.inputDelegate != null)
		{
			this.inputDelegate(ref ptr);
		}
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.MOVE:
			if (this.m_panelState != UIInteractivePanel.STATE.OVER)
			{
				this.SetPanelState(UIInteractivePanel.STATE.OVER);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			if (base.GetComponent<Collider>() != null)
			{
				RaycastHit raycastHit;
				if (!base.GetComponent<Collider>().Raycast(ptr.ray, out raycastHit, ptr.rayDepth))
				{
					this.SetPanelState(UIInteractivePanel.STATE.NORMAL);
				}
				else if (ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
				{
					ptr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
				}
				else
				{
					ptr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
				}
			}
			break;
		case POINTER_INFO.INPUT_EVENT.DRAG:
			if (this.draggable && !ptr.callerIsControl)
			{
				if (ptr.inputDelta.sqrMagnitude != 0f)
				{
					Plane plane = default(Plane);
					plane.SetNormalAndPosition(base.transform.forward * -1f, base.transform.position);
					float num;
					plane.Raycast(ptr.ray, out num);
					Vector3 vector = ptr.ray.origin + ptr.ray.direction * num;
					plane.Raycast(ptr.prevRay, out num);
					Vector3 vector2 = ptr.prevRay.origin + ptr.prevRay.direction * num;
					vector = base.transform.position + (vector - vector2);
					if (this.constrainDragArea)
					{
						vector.x = Mathf.Clamp(vector.x, this.dragBoundaryMin.x, this.dragBoundaryMax.x);
						vector.y = Mathf.Clamp(vector.y, this.dragBoundaryMin.y, this.dragBoundaryMax.y);
						vector.z = Mathf.Clamp(vector.z, this.dragBoundaryMin.z, this.dragBoundaryMax.z);
					}
					base.transform.position = vector;
					this.SetPanelState(UIInteractivePanel.STATE.DRAGGING);
				}
			}
			break;
		}
		base.OnInput(ptr);
	}

	protected void SetPanelState(UIInteractivePanel.STATE s)
	{
		if (this.m_panelState == s)
		{
			return;
		}
		UIInteractivePanel.STATE panelState = this.m_panelState;
		this.m_panelState = s;
		if (this.prevTransition != null)
		{
			this.prevTransition.StopSafe();
		}
		this.StartTransition(s, panelState);
	}

	protected void StartTransition(UIInteractivePanel.STATE s, UIInteractivePanel.STATE prevState)
	{
		int num;
		switch (s)
		{
		case UIInteractivePanel.STATE.NORMAL:
			if (prevState == UIInteractivePanel.STATE.OVER)
			{
				num = 4;
			}
			else
			{
				num = 5;
			}
			break;
		case UIInteractivePanel.STATE.OVER:
			if (prevState == UIInteractivePanel.STATE.NORMAL)
			{
				num = 6;
			}
			else
			{
				num = 7;
			}
			break;
		case UIInteractivePanel.STATE.DRAGGING:
			num = 8;
			break;
		default:
			num = 4;
			break;
		}
		this.prevTransition = this.transitions.list[num];
		this.prevTransition.Start();
	}

	public void Hide()
	{
		this.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}

	public void Reveal()
	{
		this.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
	}

	public static UIInteractivePanel Create(string name, Vector3 pos)
	{
		return (UIInteractivePanel)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIInteractivePanel));
	}

	public static UIInteractivePanel Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UIInteractivePanel)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UIInteractivePanel));
	}

	protected UIInteractivePanel.STATE m_panelState;

	[HideInInspector]
	public EZTransitionList transitions = new EZTransitionList(new EZTransition[]
	{
		new EZTransition("Bring In Forward"),
		new EZTransition("Bring In Back"),
		new EZTransition("Dismiss Forward"),
		new EZTransition("Dismiss Back"),
		new EZTransition("Normal from Over"),
		new EZTransition("Normal from Dragging"),
		new EZTransition("Over from Normal"),
		new EZTransition("Over from Dragging"),
		new EZTransition("Dragging")
	});

	public bool draggable;

	public bool constrainDragArea;

	public Vector3 dragBoundaryMin;

	public Vector3 dragBoundaryMax;

	public enum STATE
	{
		NORMAL,
		OVER,
		DRAGGING
	}
}
