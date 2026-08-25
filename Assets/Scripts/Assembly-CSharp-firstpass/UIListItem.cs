using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/List Item")]
public class UIListItem : UIButton, IUIListObject, IEZDragDrop, IUIObject
{
	public bool selected
	{
		get
		{
			return this.m_selected;
		}
		set
		{
			this.m_selected = value;
			if (this.m_selected)
			{
				this.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
			}
			else
			{
				this.SetControlState(UIButton.CONTROL_STATE.NORMAL);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (this.customCollider && base.GetComponent<Collider>() is BoxCollider)
		{
			BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
			this.colliderTL.x = boxCollider.center.x - boxCollider.size.x * 0.5f;
			this.colliderTL.y = boxCollider.center.y + boxCollider.size.y * 0.5f;
			this.colliderBR.x = boxCollider.center.x + boxCollider.size.x * 0.5f;
			this.colliderBR.y = boxCollider.center.y - boxCollider.size.y * 0.5f;
			this.colliderCenter = boxCollider.center;
		}
	}

	protected void DoNeccessaryInput(ref POINTER_INFO ptr)
	{
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			if (this.list != null && ptr.active)
			{
				this.list.ListDragged(ptr);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.TAP:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			if (this.list != null)
			{
				this.list.PointerReleased();
			}
			break;
		case POINTER_INFO.INPUT_EVENT.DRAG:
			if (this.list != null && !ptr.isTap)
			{
				this.list.ListDragged(ptr);
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
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (this.deleted)
		{
			return;
		}
		if (!this.m_controlIsEnabled)
		{
			this.DoNeccessaryInput(ref ptr);
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
			this.DoNeccessaryInput(ref ptr);
			return;
		}
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			if (this.list != null && ptr.active)
			{
				this.list.ListDragged(ptr);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.PRESS:
			if (!this.activeOnlyWhenSelected)
			{
				this.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			if (!this.selected)
			{
				this.SetControlState(UIButton.CONTROL_STATE.NORMAL);
			}
			if (this.list != null)
			{
				this.list.PointerReleased();
			}
			break;
		case POINTER_INFO.INPUT_EVENT.TAP:
			if (this.list != null)
			{
				this.list.DidSelect(this);
				this.list.PointerReleased();
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE:
			if (!this.selected)
			{
				if (this.soundOnOver != null && this.m_ctrlState != UIButton.CONTROL_STATE.OVER)
				{
					this.soundOnOver.PlayOneShot(this.soundOnOver.clip);
				}
				this.SetControlState(UIButton.CONTROL_STATE.OVER);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
			if (!this.selected)
			{
				this.SetControlState(UIButton.CONTROL_STATE.NORMAL);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.DRAG:
			if (!ptr.isTap)
			{
				if (!this.selected)
				{
					this.SetControlState(UIButton.CONTROL_STATE.NORMAL);
				}
				if (this.list != null)
				{
					this.list.ListDragged(ptr);
				}
			}
			else if (!this.activeOnlyWhenSelected)
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
				goto IL_02EC;
			}
		}
		else if (ptr.evt == this.whenToInvoke)
		{
			goto IL_02EC;
		}
		return;
		IL_02EC:
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

	protected override void OnEnable()
	{
		base.OnEnable();
		if (Application.isPlaying && this.m_started)
		{
			this.m_ctrlState = (UIButton.CONTROL_STATE)(-1);
			if (this.controlIsEnabled)
			{
				if (this.selected)
				{
					this.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
				}
				else
				{
					this.SetControlState(UIButton.CONTROL_STATE.NORMAL, true);
				}
			}
			else
			{
				this.SetControlState(UIButton.CONTROL_STATE.DISABLED, true);
			}
		}
	}

	protected override void OnDisable()
	{
		UIButton.CONTROL_STATE controlState = base.controlState;
		base.OnDisable();
		this.SetControlState(controlState);
	}

	public override void Copy(SpriteRoot s)
	{
		this.Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (!(s is UIListItem))
		{
			return;
		}
		UIListItem uilistItem = (UIListItem)s;
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			this.list = uilistItem.list;
		}
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			this.topLeftEdge = uilistItem.topLeftEdge;
			this.bottomRightEdge = uilistItem.bottomRightEdge;
			this.colliderTL = uilistItem.colliderTL;
			this.colliderBR = uilistItem.colliderBR;
			this.colliderCenter = uilistItem.colliderCenter;
			this.customCollider = uilistItem.customCollider;
		}
	}

	public override string Text
	{
		set
		{
			base.Text = value;
			this.FindOuterEdges();
			if (this.spriteText != null && this.spriteText.maxWidth > 0f && this.list != null)
			{
				this.list.PositionItems();
			}
		}
	}

	public override void FindOuterEdges()
	{
		base.FindOuterEdges();
		if (!this.customCollider)
		{
			this.colliderTL = this.topLeftEdge;
			this.colliderBR = this.bottomRightEdge;
		}
	}

	public override void TruncateRight(float pct)
	{
		base.TruncateRight(pct);
		if (base.GetComponent<Collider>() != null && base.GetComponent<Collider>() is BoxCollider)
		{
			if (this.customCollider)
			{
				BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
				Vector3 vector = boxCollider.center;
				vector.x = (1f - pct) * (this.colliderBR.x - this.colliderTL.x) * -0.5f;
				boxCollider.center = vector;
				vector = boxCollider.size;
				vector.x = pct * (this.colliderBR.x - this.colliderTL.x);
				boxCollider.size = vector;
			}
			else
			{
				this.UpdateCollider();
			}
		}
	}

	public override void TruncateLeft(float pct)
	{
		base.TruncateLeft(pct);
		if (base.GetComponent<Collider>() != null && base.GetComponent<Collider>() is BoxCollider)
		{
			if (this.customCollider)
			{
				BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
				Vector3 vector = boxCollider.center;
				vector.x = (1f - pct) * (this.colliderBR.x - this.colliderTL.x) * 0.5f;
				boxCollider.center = vector;
				vector = boxCollider.size;
				vector.x = pct * (this.colliderBR.x - this.colliderTL.x);
				boxCollider.size = vector;
			}
			else
			{
				this.UpdateCollider();
			}
		}
	}

	public override void TruncateTop(float pct)
	{
		base.TruncateTop(pct);
		if (base.GetComponent<Collider>() != null && base.GetComponent<Collider>() is BoxCollider)
		{
			if (this.customCollider)
			{
				BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
				Vector3 vector = boxCollider.center;
				vector.y = (1f - pct) * (this.colliderBR.y - this.colliderTL.y) * 0.5f;
				boxCollider.center = vector;
				vector = boxCollider.size;
				vector.y = pct * (this.colliderTL.y - this.colliderBR.y);
				boxCollider.size = vector;
			}
			else
			{
				this.UpdateCollider();
			}
		}
	}

	public override void TruncateBottom(float pct)
	{
		base.TruncateBottom(pct);
		if (base.GetComponent<Collider>() != null && base.GetComponent<Collider>() is BoxCollider)
		{
			if (this.customCollider)
			{
				BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
				Vector3 vector = boxCollider.center;
				vector.y = (1f - pct) * (this.colliderBR.y - this.colliderTL.y) * -0.5f;
				boxCollider.center = vector;
				vector = boxCollider.size;
				vector.y = pct * (this.colliderTL.y - this.colliderBR.y);
				boxCollider.size = vector;
			}
			else
			{
				this.UpdateCollider();
			}
		}
	}

	public override void Untruncate()
	{
		base.Untruncate();
		if (base.GetComponent<Collider>() != null && base.GetComponent<Collider>() is BoxCollider)
		{
			if (this.customCollider)
			{
				BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
				if (!this.customCollider)
				{
					boxCollider.center = Vector3.zero;
				}
				else
				{
					boxCollider.center = this.colliderCenter;
				}
				boxCollider.size = new Vector3(this.colliderBR.x - this.colliderTL.x, this.colliderTL.y - this.colliderBR.y, 0.001f);
			}
			else
			{
				this.UpdateCollider();
			}
		}
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);
		for (int i = 0; i < this.layers.Length; i++)
		{
			this.layers[i].Hide(tf);
		}
		if (this.spriteText != null)
		{
			if (tf)
			{
				this.spriteText.gameObject.active = false;
			}
			else
			{
				this.spriteText.gameObject.active = true;
			}
		}
	}

	public void SetList(UIScrollList c)
	{
		this.list = c;
	}

	public virtual UIScrollList GetScrollList()
	{
		return this.list;
	}

	public int Index
	{
		get
		{
			return this.m_index;
		}
		set
		{
			this.m_index = value;
		}
	}

	public void FindText()
	{
		if (this.spriteText == null)
		{
			this.spriteText = (SpriteText)base.GetComponentInChildren(typeof(SpriteText));
		}
		if (this.spriteText != null)
		{
			this.spriteText.gameObject.layer = base.gameObject.layer;
			this.spriteText.Parent = this;
		}
	}

	public SpriteText TextObj
	{
		get
		{
			return this.spriteText;
		}
	}

	public bool IsContainer()
	{
		return false;
	}

	public new static UIListItem Create(string name, Vector3 pos)
	{
		return (UIListItem)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIListItem));
	}

	public new static UIListItem Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UIListItem)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UIListItem));
	}

	Vector2 IUIListObject.TopLeftEdge
	{
		get { return base.TopLeftEdge; }
	}

	Vector2 IUIListObject.BottomRightEdge
	{
		get { return base.BottomRightEdge; }
	}

	bool IUIListObject.Managed
	{
		get { return base.Managed; }
	}

	[HideInInspector]
	public bool activeOnlyWhenSelected = true;

	protected int m_index;

	protected bool m_selected;

	protected UIScrollList list;

	protected Vector2 colliderTL;

	protected Vector2 colliderBR;

	protected Vector3 colliderCenter;
}
