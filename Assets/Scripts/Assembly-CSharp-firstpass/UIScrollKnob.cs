using System;
using UnityEngine;

public class UIScrollKnob : UIButton
{
	protected override void Awake()
	{
		base.Awake();
		this.origPos = base.transform.localPosition;
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		base.OnInput(ref ptr);
		if (!this.m_controlIsEnabled)
		{
			return;
		}
		POINTER_INFO.INPUT_EVENT evt = ptr.evt;
		if (evt != POINTER_INFO.INPUT_EVENT.PRESS)
		{
			if (evt == POINTER_INFO.INPUT_EVENT.DRAG)
			{
				this.inputPoint = this.GetLocalInputPoint(ptr.ray);
				this.dist = this.inputPoint.x - this.prevPoint.x;
				this.prevPoint = this.inputPoint;
				this.newPos = base.transform.localPosition;
				this.newPos.x = Mathf.Clamp(this.newPos.x + this.dist, this.origPos.x, this.origPos.x + this.maxScrollPos);
				base.transform.localPosition = this.newPos;
				this.prevPoint.x = Mathf.Clamp(this.prevPoint.x, this.origPos.x - this.colliderExtent, this.origPos.x + this.colliderExtent + this.maxScrollPos);
				this.slider.ScrollKnobMoved(this, this.GetScrollPos());
			}
		}
		else
		{
			this.prevPoint = this.GetLocalInputPoint(ptr.ray);
		}
	}

	public void SetStartPos(Vector3 startPos)
	{
		this.origPos = startPos;
	}

	protected Vector3 GetLocalInputPoint(Ray ray)
	{
		this.ctrlPlane.SetNormalAndPosition(base.transform.forward * -1f, base.transform.position);
		this.ctrlPlane.Raycast(ray, out this.dist);
		return base.transform.parent.InverseTransformPoint(ray.origin + ray.direction * this.dist);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (!(s is UIScrollKnob))
		{
			return;
		}
		UIScrollKnob uiscrollKnob = (UIScrollKnob)s;
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			this.origPos = uiscrollKnob.origPos;
			this.ctrlPlane = uiscrollKnob.ctrlPlane;
			this.slider = uiscrollKnob.slider;
		}
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			this.maxScrollPos = uiscrollKnob.maxScrollPos;
			this.colliderSizeFactor = uiscrollKnob.colliderSizeFactor;
		}
	}

	public void SetColliderSizeFactor(Vector2 csf)
	{
		this.colliderSizeFactor = csf;
	}

	public override void UpdateCollider()
	{
		base.UpdateCollider();
		if (!(base.GetComponent<Collider>() is BoxCollider) || base.IsHidden())
		{
			return;
		}
		BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
		boxCollider.size = new Vector3(boxCollider.size.x * this.colliderSizeFactor.x, boxCollider.size.y * this.colliderSizeFactor.y, 0.001f);
		this.colliderExtent = boxCollider.size.x * 0.5f;
	}

	public float GetScrollPos()
	{
		return (base.transform.localPosition.x - this.origPos.x) / this.maxScrollPos;
	}

	public void SetPosition(float pos)
	{
		base.transform.localPosition = this.origPos + Vector3.right * this.maxScrollPos * pos;
	}

	public void SetSlider(UISlider s)
	{
		this.slider = s;
	}

	public UISlider GetSlider()
	{
		return this.slider;
	}

	public void SetMaxScroll(float max)
	{
		this.maxScrollPos = max;
	}

	public void SetupAppearance()
	{
		this.Start();
		this.InitUVs();
		this.UpdateUVs();
	}

	public new static UIScrollKnob Create(string name, Vector3 pos)
	{
		return (UIScrollKnob)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIScrollKnob));
	}

	public new static UIScrollKnob Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UIScrollKnob)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UIScrollKnob));
	}

	protected Vector3 origPos;

	protected UISlider slider;

	protected float maxScrollPos;

	protected Plane ctrlPlane;

	protected Vector2 colliderSizeFactor;

	protected float colliderExtent;

	private float dist;

	private Vector3 inputPoint;

	private Vector3 newPos;

	private Vector3 prevPoint;
}
