using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/List Item Container")]
[Serializable]
public class UIListItemContainer : ControlBase, IUIListObject, IEZDragDrop, IUIContainer, IUIObject
{
	public override void Start()
	{
		if (this.m_started)
		{
			return;
		}
		this.m_started = true;
		this.ScanChildren();
	}

	public void ScanChildren()
	{
		this.uiObjs.Clear();
		Component[] array = base.transform.GetComponentsInChildren(typeof(SpriteRoot), true);
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == this))
			{
				if (base.gameObject.layer == UIManager.instance.gameObject.layer)
				{
					UIPanelManager.SetLayerRecursively(array[i].gameObject, base.gameObject.layer);
				}
				SpriteRoot spriteRoot = (SpriteRoot)array[i];
				if (spriteRoot is AutoSpriteControlBase)
				{
					if (((AutoSpriteControlBase)spriteRoot).RequestContainership(this))
					{
						this.uiObjs.Add(spriteRoot);
					}
				}
				else
				{
					this.uiObjs.Add(spriteRoot);
				}
				if (this.container != null)
				{
					this.container.AddSubject(spriteRoot.gameObject);
				}
				if (this.renderCamera != null)
				{
					spriteRoot.renderCamera = this.renderCamera;
				}
			}
		}
		array = base.transform.GetComponentsInChildren(typeof(ControlBase), true);
		for (int j = 0; j < array.Length; j++)
		{
			if (base.gameObject.layer == UIManager.instance.gameObject.layer)
			{
				UIPanelManager.SetLayerRecursively(array[j].gameObject, base.gameObject.layer);
			}
			((ControlBase)array[j]).RequestContainership(this);
			if (this.container != null)
			{
				this.container.AddSubject(array[j].gameObject);
			}
		}
		this.textObjs.Clear();
		Component[] componentsInChildren = base.transform.GetComponentsInChildren(typeof(SpriteText), true);
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			if (!(componentsInChildren[k] == this))
			{
				SpriteText spriteText = (SpriteText)componentsInChildren[k];
				if (spriteText.Parent == null)
				{
					if (base.gameObject.layer == UIManager.instance.gameObject.layer)
					{
						UIPanelManager.SetLayerRecursively(spriteText.gameObject, base.gameObject.layer);
					}
					this.textObjs.Add(spriteText);
					if (this.container != null)
					{
						this.container.AddSubject(spriteText.gameObject);
					}
					if (this.renderCamera != null)
					{
						spriteText.renderCamera = this.renderCamera;
					}
				}
			}
		}
	}

	public void AddChild(GameObject go)
	{
		SpriteRoot spriteRoot = (SpriteRoot)go.GetComponent(typeof(SpriteRoot));
		if (spriteRoot != null)
		{
			if (spriteRoot is AutoSpriteControlBase)
			{
				if (((AutoSpriteControlBase)spriteRoot).Container != this)
				{
					((AutoSpriteControlBase)spriteRoot).Container = this;
				}
				if (this.container != null)
				{
					this.container.AddSubject(go);
				}
			}
			this.uiObjs.Add(spriteRoot);
		}
		else
		{
			SpriteText spriteText = (SpriteText)go.GetComponent(typeof(SpriteText));
			if (spriteText != null)
			{
				this.textObjs.Add(spriteText);
				if (this.container != null)
				{
					this.container.AddSubject(go);
				}
			}
		}
	}

	public void RemoveChild(GameObject go)
	{
		SpriteRoot spriteRoot = (SpriteRoot)go.GetComponent(typeof(SpriteRoot));
		if (spriteRoot != null)
		{
			for (int i = 0; i < this.uiObjs.Count; i++)
			{
				if (this.uiObjs[i] == spriteRoot)
				{
					this.uiObjs.RemoveAt(i);
					break;
				}
			}
			if (spriteRoot is AutoSpriteControlBase && ((AutoSpriteControlBase)spriteRoot).Container == this)
			{
				((AutoSpriteControlBase)spriteRoot).Container = null;
			}
			if (this.container != null)
			{
				this.container.RemoveSubject(go);
			}
		}
		else
		{
			SpriteText spriteText = (SpriteText)go.GetComponent(typeof(SpriteText));
			if (spriteText != null)
			{
				for (int j = 0; j < this.textObjs.Count; j++)
				{
					if (this.textObjs[j] == spriteText)
					{
						this.textObjs.RemoveAt(j);
						break;
					}
				}
				if (this.container != null)
				{
					this.container.RemoveSubject(go);
				}
			}
		}
	}

	public void AddSubject(GameObject go)
	{
	}

	public void RemoveSubject(GameObject go)
	{
	}

	public override IUIContainer Container
	{
		get
		{
			return base.Container;
		}
		set
		{
			if (this.container != null)
			{
				for (int i = 0; i < this.uiObjs.Count; i++)
				{
					this.container.RemoveSubject(this.uiObjs[i].gameObject);
				}
				for (int j = 0; j < this.textObjs.Count; j++)
				{
					this.container.RemoveSubject(this.textObjs[j].gameObject);
				}
			}
			if (value != null)
			{
				for (int k = 0; k < this.uiObjs.Count; k++)
				{
					value.AddSubject(this.uiObjs[k].gameObject);
				}
				for (int l = 0; l < this.textObjs.Count; l++)
				{
					value.AddSubject(this.textObjs[l].gameObject);
				}
			}
			base.Container = value;
		}
	}

	public void MakeChild(GameObject go)
	{
		go.transform.parent = base.transform;
		this.AddChild(go);
	}

	public SpriteRoot GetElement(string elementName)
	{
		if (!this.m_started)
		{
			this.Start();
		}
		for (int i = 0; i < this.uiObjs.Count; i++)
		{
			if (this.uiObjs[i].name == elementName)
			{
				return this.uiObjs[i];
			}
		}
		return null;
	}

	public SpriteText GetTextElement(string elementName)
	{
		if (!this.m_started)
		{
			this.Start();
		}
		for (int i = 0; i < this.textObjs.Count; i++)
		{
			if (this.textObjs[i].name == elementName)
			{
				return this.textObjs[i];
			}
		}
		return null;
	}

	public override void OnInput(POINTER_INFO ptr)
	{
		if (this.deleted)
		{
			return;
		}
		if (!this.m_controlIsEnabled)
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
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			if (this.list != null && ptr.active)
			{
				this.list.ListDragged(ptr);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			if (this.list != null)
			{
				this.list.PointerReleased();
			}
			break;
		case POINTER_INFO.INPUT_EVENT.TAP:
			if (!(this.list == null))
			{
				if (ptr.callerIsControl)
				{
					this.list.DidClick(ptr.targetObj);
				}
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
		base.OnInput(ptr);
	}

	public override EZTransitionList GetTransitions(int index)
	{
		return null;
	}

	public override EZTransitionList[] Transitions
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public override string[] States
	{
		get
		{
			return null;
		}
	}

	public bool IsContainer()
	{
		return true;
	}

	public void FindOuterEdges()
	{
		if (!this.m_started)
		{
			this.Start();
		}
		this.topLeftEdge = Vector2.zero;
		this.bottomRightEdge = Vector2.zero;
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		if (this.spriteText != null)
		{
			this.spriteText.Start();
			Matrix4x4 matrix4x = this.spriteText.transform.localToWorldMatrix;
			Vector3 vector = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.spriteText.UnclippedTopLeft));
			Vector3 vector2 = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.spriteText.UnclippedBottomRight));
			this.topLeftEdge.x = Mathf.Min(this.topLeftEdge.x, vector.x);
			this.topLeftEdge.y = Mathf.Max(this.topLeftEdge.y, vector.y);
			this.bottomRightEdge.x = Mathf.Max(this.bottomRightEdge.x, vector2.x);
			this.bottomRightEdge.y = Mathf.Min(this.bottomRightEdge.y, vector2.y);
		}
		for (int i = 0; i < this.textObjs.Count; i++)
		{
			this.textObjs[i].Start();
			Matrix4x4 matrix4x = this.textObjs[i].transform.localToWorldMatrix;
			Vector3 vector = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.textObjs[i].UnclippedTopLeft));
			Vector3 vector2 = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.textObjs[i].UnclippedBottomRight));
			this.topLeftEdge.x = Mathf.Min(this.topLeftEdge.x, vector.x);
			this.topLeftEdge.y = Mathf.Max(this.topLeftEdge.y, vector.y);
			this.bottomRightEdge.x = Mathf.Max(this.bottomRightEdge.x, vector2.x);
			this.bottomRightEdge.y = Mathf.Min(this.bottomRightEdge.y, vector2.y);
		}
		for (int j = 0; j < this.uiObjs.Count; j++)
		{
			Matrix4x4 matrix4x = this.uiObjs[j].transform.localToWorldMatrix;
			Vector3 vector;
			Vector3 vector2;
			if (this.uiObjs[j] is AutoSpriteControlBase)
			{
				((AutoSpriteControlBase)this.uiObjs[j]).FindOuterEdges();
				vector = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(((AutoSpriteControlBase)this.uiObjs[j]).TopLeftEdge));
				vector2 = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(((AutoSpriteControlBase)this.uiObjs[j]).BottomRightEdge));
			}
			else
			{
				vector = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.uiObjs[j].UnclippedTopLeft));
				vector2 = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.uiObjs[j].UnclippedBottomRight));
			}
			this.topLeftEdge.x = Mathf.Min(this.topLeftEdge.x, vector.x);
			this.topLeftEdge.y = Mathf.Max(this.topLeftEdge.y, vector.y);
			this.bottomRightEdge.x = Mathf.Max(this.bottomRightEdge.x, vector2.x);
			this.bottomRightEdge.y = Mathf.Min(this.bottomRightEdge.y, vector2.y);
		}
	}

	public Vector2 TopLeftEdge
	{
		get
		{
			return this.topLeftEdge;
		}
	}

	public Vector2 BottomRightEdge
	{
		get
		{
			return this.bottomRightEdge;
		}
	}

	public void Hide(bool tf)
	{
		for (int i = 0; i < this.uiObjs.Count; i++)
		{
			this.uiObjs[i].Hide(tf);
		}
		for (int j = 0; j < this.textObjs.Count; j++)
		{
			this.textObjs[j].Hide(tf);
		}
		if (this.spriteText != null)
		{
			this.spriteText.Hide(tf);
		}
	}

	public bool Managed
	{
		get
		{
			return false;
		}
	}

	public Rect3D ClippingRect
	{
		get
		{
			return this.clippingRect;
		}
		set
		{
			this.clipped = true;
			this.clippingRect = value;
			for (int i = 0; i < this.uiObjs.Count; i++)
			{
				this.uiObjs[i].ClippingRect = value;
			}
			for (int j = 0; j < this.textObjs.Count; j++)
			{
				this.textObjs[j].ClippingRect = value;
			}
			if (this.spriteText != null)
			{
				this.spriteText.ClippingRect = value;
			}
		}
	}

	public bool Clipped
	{
		get
		{
			return this.clipped;
		}
		set
		{
			if (value && !this.clipped)
			{
				this.clipped = true;
				this.ClippingRect = this.clippingRect;
			}
			else if (this.clipped)
			{
				this.Unclip();
			}
			this.clipped = value;
		}
	}

	public void Unclip()
	{
		this.clipped = false;
		for (int i = 0; i < this.uiObjs.Count; i++)
		{
			this.uiObjs[i].Unclip();
		}
		for (int j = 0; j < this.textObjs.Count; j++)
		{
			this.textObjs[j].Unclip();
		}
		if (this.spriteText != null)
		{
			this.spriteText.Unclip();
		}
	}

	public override void UpdateCollider()
	{
		for (int i = 0; i < this.uiObjs.Count; i++)
		{
			if (this.uiObjs[i] is AutoSpriteControlBase)
			{
				((AutoSpriteControlBase)this.uiObjs[i]).UpdateCollider();
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
			return this.index;
		}
		set
		{
			this.index = value;
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

	public SpriteText TextObj
	{
		get
		{
			return this.spriteText;
		}
	}

	public bool selected
	{
		get
		{
			return this.m_selected;
		}
		set
		{
			this.m_selected = value;
		}
	}

	public void Delete()
	{
		for (int i = 0; i < this.uiObjs.Count; i++)
		{
			this.uiObjs[i].Delete();
		}
		for (int j = 0; j < this.textObjs.Count; j++)
		{
			this.textObjs[j].Delete();
		}
	}

	public Camera RenderCamera
	{
		get
		{
			return this.renderCamera;
		}
		set
		{
			this.renderCamera = value;
			for (int i = 0; i < this.uiObjs.Count; i++)
			{
				this.uiObjs[i].RenderCamera = value;
			}
			for (int j = 0; j < this.textObjs.Count; j++)
			{
				this.textObjs[j].RenderCamera = value;
			}
			if (this.spriteText != null)
			{
				this.spriteText.RenderCamera = value;
			}
		}
	}

	protected List<SpriteRoot> uiObjs = new List<SpriteRoot>();

	protected List<SpriteText> textObjs = new List<SpriteText>();

	protected bool m_started;

	protected Camera renderCamera;

	private Vector2 topLeftEdge;

	private Vector2 bottomRightEdge;

	private Rect3D clippingRect;

	private bool clipped;

	private UIScrollList list;

	protected int index;

	private bool m_selected;
}
