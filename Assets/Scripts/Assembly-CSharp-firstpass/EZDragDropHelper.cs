using System;
using UnityEngine;

public class EZDragDropHelper
{
	public EZDragDropHelper(IUIObject h)
	{
		this.host = h;
		this.dragPosUpdateDel = new EZDragDropHelper.UpdateDragPositionDelegate(this.DefaultDragUpdatePosition);
	}

	public EZDragDropHelper()
	{
		this.dragPosUpdateDel = new EZDragDropHelper.UpdateDragPositionDelegate(this.DefaultDragUpdatePosition);
	}

	private Plane DragPlane
	{
		get
		{
			return this.dragPlane;
		}
	}

	public bool UseDefaultCancelDragAnim
	{
		get
		{
			return this.useDefaultCancelDragAnim;
		}
		set
		{
			this.useDefaultCancelDragAnim = value;
		}
	}

	public bool IsDragging
	{
		get
		{
			return this.isDragging;
		}
		set
		{
			bool flag = this.isDragging;
			if (flag && !value)
			{
				this.CancelDrag();
			}
			this.isDragging = value;
		}
	}

	public bool IsCanceling
	{
		get
		{
			return this.isCanceling;
		}
	}

	public void CancelFinished()
	{
		this.isCanceling = false;
	}

	public GameObject DropTarget
	{
		get
		{
			return this.dropTarget;
		}
		set
		{
			if (value == this.host.gameObject)
			{
				return;
			}
			if (this.dropTarget != value)
			{
				if (this.dropTarget != null)
				{
					this.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.DragExit, this.host, default(POINTER_INFO)));
				}
				this.dropTarget = value;
				if (value != null)
				{
					this.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.DragEnter, this.host, default(POINTER_INFO)));
				}
			}
		}
	}

	public bool DropHandled
	{
		get
		{
			return this.dropHandled;
		}
		set
		{
			this.dropHandled = value;
		}
	}

	public void SetDragPosUpdater(EZDragDropHelper.UpdateDragPositionDelegate del)
	{
		this.dragPosUpdateDel = del;
	}

	public void DragUpdatePosition(POINTER_INFO ptr)
	{
		if (this.dragPosUpdateDel != null)
		{
			this.dragPosUpdateDel(ptr);
		}
	}

	public void DefaultDragUpdatePosition(POINTER_INFO ptr)
	{
		float num;
		this.dragPlane.Raycast(ptr.ray, out num);
		this.host.transform.position = this.touchCompensationOffset + ptr.ray.origin + ptr.ray.direction * (num - this.host.DragOffset);
	}

	public void CancelDrag()
	{
		if (!this.isDragging)
		{
			return;
		}
		EZDragDropParams ezdragDropParams = new EZDragDropParams(EZDragDropEvent.Cancelled, this.host, default(POINTER_INFO));
		this.OnEZDragDrop_Internal(ezdragDropParams);
		this.dropTarget = null;
		this.dropHandled = false;
		this.isDragging = false;
		this.isCanceling = true;
		if (this.useDefaultCancelDragAnim)
		{
			this.DoDefaultCancelDrag();
		}
		POINTER_INFO pointer_INFO = default(POINTER_INFO);
		pointer_INFO.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
		this.host.OnInput(pointer_INFO);
		if (UIManager.Exists())
		{
			UIManager.instance.Detarget(this.host);
		}
	}

	public void DoDefaultCancelDrag()
	{
		AnimatePosition.Do(this.host.gameObject, EZAnimation.ANIM_MODE.To, this.dragOriginOffset, EZAnimation.GetInterpolator(this.host.CancelDragEasing), this.host.CancelDragDuration, 0f, null, new EZAnimation.CompletionDelegate(this.FinishCancelDrag));
	}

	protected void FinishCancelDrag(EZAnimation anim)
	{
		if (this.host == null)
		{
			return;
		}
		this.host.transform.localPosition = this.dragOrigin;
		this.isCanceling = false;
		this.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.CancelDone, this.host, default(POINTER_INFO)));
	}

	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
		EZDragDropEvent evt = parms.evt;
		if (evt == EZDragDropEvent.Begin)
		{
			if (this.isCanceling)
			{
				return;
			}
			this.isDragging = true;
			this.dropHandled = false;
			Transform transform = this.host.transform;
			this.dragOrigin = transform.localPosition;
			this.dragPlane.SetNormalAndPosition(transform.TransformDirection(transform.forward * -1f), transform.position);
			Ray ray = parms.ptr.camera.ScreenPointToRay(parms.ptr.camera.WorldToScreenPoint(transform.position));
			float num;
			this.dragPlane.Raycast(ray, out num);
			this.dragOriginOffset = ray.origin + ray.direction * (num - this.host.DragOffset);
			if (transform.parent != null)
			{
				this.dragOriginOffset = transform.parent.InverseTransformPoint(this.dragOriginOffset);
			}
			this.dragPlane.Raycast(parms.ptr.ray, out num);
			this.touchCompensationOffset = transform.position - (parms.ptr.ray.origin + parms.ptr.ray.direction * num);
		}
		if (this.dragDropDelegate != null)
		{
			this.dragDropDelegate(parms);
		}
		if (this.dropTarget != null)
		{
			this.dropTarget.SendMessage("OnEZDragDrop", parms, SendMessageOptions.DontRequireReceiver);
		}
		this.host.gameObject.SendMessage("OnEZDragDrop", parms, SendMessageOptions.DontRequireReceiver);
		if (parms.evt == EZDragDropEvent.Dropped && parms.dragObj.Equals(this.host))
		{
			if (this.dropHandled)
			{
				this.isDragging = false;
				this.dropTarget = null;
			}
			else
			{
				this.CancelDrag();
			}
		}
	}

	public void AddDragDropDelegate(EZDragDropDelegate del)
	{
		this.dragDropDelegate = (EZDragDropDelegate)Delegate.Combine(this.dragDropDelegate, del);
	}

	public void RemoveDragDropDelegate(EZDragDropDelegate del)
	{
		this.dragDropDelegate = (EZDragDropDelegate)Delegate.Remove(this.dragDropDelegate, del);
	}

	public void SetDragDropDelegate(EZDragDropDelegate del)
	{
		this.dragDropDelegate = del;
	}

	public IUIObject host;

	protected EZDragDropHelper.UpdateDragPositionDelegate dragPosUpdateDel;

	private Vector3 touchCompensationOffset = Vector3.zero;

	protected Vector3 dragOrigin;

	protected Vector3 dragOriginOffset;

	protected Plane dragPlane;

	protected bool isDragging;

	protected bool isCanceling;

	protected bool useDefaultCancelDragAnim = true;

	protected GameObject dropTarget;

	protected bool dropHandled;

	protected EZDragDropDelegate dragDropDelegate;

	public delegate void UpdateDragPositionDelegate(POINTER_INFO ptr);
}
