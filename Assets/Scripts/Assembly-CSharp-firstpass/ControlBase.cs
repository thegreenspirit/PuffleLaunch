using System;
using UnityEngine;

[ExecuteInEditMode]
public abstract class ControlBase : MonoBehaviour, IEZDragDrop, IControl, IUIObject
{
	public virtual string Text
	{
		get
		{
			return this.text;
		}
		set
		{
			this.text = value;
			if (this.spriteText == null)
			{
				if (this.text == string.Empty)
				{
					return;
				}
				if (UIManager.instance == null)
				{
					Debug.LogWarning("Warning: No UIManager exists in the scene. A UIManager with a default font is required to automatically add text to a control.");
					return;
				}
				if (UIManager.instance.defaultFont == null)
				{
					Debug.LogWarning("Warning: No default font defined.  A UIManager object with a default font is required to automatically add text to a control.");
					return;
				}
				GameObject gameObject = new GameObject();
				gameObject.layer = base.gameObject.layer;
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.name = "control_text";
				MeshRenderer meshRenderer = (MeshRenderer)gameObject.AddComponent(typeof(MeshRenderer));
				meshRenderer.material = UIManager.instance.defaultFontMaterial;
				this.spriteText = (SpriteText)gameObject.AddComponent(typeof(SpriteText));
				this.spriteText.font = UIManager.instance.defaultFont;
				this.spriteText.offsetZ = this.textOffsetZ;
				this.spriteText.Parent = this;
				this.spriteText.anchor = this.defaultTextAnchor;
				this.spriteText.alignment = this.defaultTextAlignment;
				this.spriteText.pixelPerfect = true;
				this.spriteText.Start();
			}
			this.spriteText.Text = this.text;
			this.text = this.spriteText.Text;
			if (this.includeTextInAutoCollider)
			{
				this.UpdateCollider();
			}
		}
	}

	public object Data
	{
		get
		{
			return this.data;
		}
		set
		{
			this.data = value;
		}
	}

	public virtual bool IncludeTextInAutoCollider
	{
		get
		{
			return this.includeTextInAutoCollider;
		}
		set
		{
			this.includeTextInAutoCollider = value;
			this.UpdateCollider();
		}
	}

	protected virtual void Awake()
	{
		if (base.GetComponent<Collider>() != null)
		{
			this.customCollider = true;
		}
		if (this.dragDropHelper == null)
		{
			this.dragDropHelper = new EZDragDropHelper(this);
		}
		else
		{
			this.dragDropHelper.host = this;
		}
	}

	public virtual void Start()
	{
		if (this.spriteText != null)
		{
			this.spriteText.Parent = this;
		}
		if (UIManager.Exists() && Application.isPlaying)
		{
			if (this.cancelDragEasing == EZAnimation.EASING_TYPE.Default)
			{
				this.cancelDragEasing = UIManager.instance.cancelDragEasing;
			}
			if (this.cancelDragDuration == -1f)
			{
				this.cancelDragDuration = UIManager.instance.cancelDragDuration;
			}
			if (float.IsNaN(this.dragOffset))
			{
				this.dragOffset = UIManager.instance.defaultDragOffset;
			}
		}
	}

	protected virtual void AddCollider()
	{
		if (this.customCollider)
		{
			return;
		}
		base.gameObject.AddComponent(typeof(BoxCollider));
		this.UpdateCollider();
	}

	public virtual void UpdateCollider()
	{
		if (this.customCollider || !(base.GetComponent<Collider>() is BoxCollider))
		{
			return;
		}
		BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
		if (this.includeTextInAutoCollider && this.spriteText != null)
		{
			Bounds bounds = new Bounds(boxCollider.center, boxCollider.size);
			Matrix4x4 localToWorldMatrix = this.spriteText.transform.localToWorldMatrix;
			Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
			Vector3 vector = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(this.spriteText.TopLeft)) * 2f;
			Vector3 vector2 = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(this.spriteText.BottomRight)) * 2f;
			bounds.Encapsulate(vector);
			bounds.Encapsulate(vector2);
			boxCollider.size = bounds.extents;
			boxCollider.center = bounds.center * 0.5f;
		}
		boxCollider.isTrigger = true;
	}

	public virtual bool controlIsEnabled
	{
		get
		{
			return this.m_controlIsEnabled;
		}
		set
		{
			this.m_controlIsEnabled = value;
		}
	}

	public virtual bool DetargetOnDisable
	{
		get
		{
			return this.DetargetOnDisable;
		}
		set
		{
			this.DetargetOnDisable = value;
		}
	}

	public IUIObject GetControl(ref POINTER_INFO ptr)
	{
		return this;
	}

	public virtual IUIContainer Container
	{
		get
		{
			return this.container;
		}
		set
		{
			if (this.container != null && this.spriteText != null)
			{
				this.container.RemoveChild(this.spriteText.gameObject);
			}
			if (value != null && this.spriteText != null)
			{
				value.AddChild(this.spriteText.gameObject);
			}
			this.container = value;
		}
	}

	public bool RequestContainership(IUIContainer cont)
	{
		Transform transform = base.transform.parent;
		Transform transform2 = ((Component)cont).transform;
		while (transform != null)
		{
			if (transform == transform2)
			{
				this.Container = cont;
				return true;
			}
			if (transform.gameObject.GetComponent("IUIContainer") != null)
			{
				return false;
			}
			transform = transform.parent;
		}
		return false;
	}

	public virtual bool GotFocus()
	{
		return false;
	}

	public virtual void SetInputDelegate(EZInputDelegate del)
	{
		this.inputDelegate = del;
	}

	public virtual void AddInputDelegate(EZInputDelegate del)
	{
		this.inputDelegate = (EZInputDelegate)Delegate.Combine(this.inputDelegate, del);
	}

	public virtual void RemoveInputDelegate(EZInputDelegate del)
	{
		this.inputDelegate = (EZInputDelegate)Delegate.Remove(this.inputDelegate, del);
	}

	public virtual void SetValueChangedDelegate(EZValueChangedDelegate del)
	{
		this.changeDelegate = del;
	}

	public virtual void AddValueChangedDelegate(EZValueChangedDelegate del)
	{
		this.changeDelegate = (EZValueChangedDelegate)Delegate.Combine(this.changeDelegate, del);
	}

	public virtual void RemoveValueChangedDelegate(EZValueChangedDelegate del)
	{
		this.changeDelegate = (EZValueChangedDelegate)Delegate.Remove(this.changeDelegate, del);
	}

	public virtual void OnInput(POINTER_INFO ptr)
	{
		this.OnInput(ref ptr);
	}

	public virtual void OnInput(ref POINTER_INFO ptr)
	{
		if (this.Container != null)
		{
			ptr.callerIsControl = true;
			this.Container.OnInput(ptr);
		}
	}

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
		if (Application.isPlaying)
		{
			if (EZAnimator.Exists())
			{
				EZAnimator.instance.Stop(base.gameObject);
				EZAnimator.instance.Stop(this);
			}
			if (this.detargetOnDisable && UIManager.Exists())
			{
				UIManager.instance.Detarget(this);
			}
		}
	}

	public virtual void OnDestroy()
	{
		this.deleted = true;
	}

	public virtual void Copy(IControl ctl)
	{
		this.Copy(ctl, ControlCopyFlags.All);
	}

	public virtual void Copy(IControl ctl, ControlCopyFlags flags)
	{
		if (!(ctl is ControlBase))
		{
			return;
		}
		ControlBase controlBase = (ControlBase)ctl;
		if ((flags & ControlCopyFlags.Transitions) == ControlCopyFlags.Transitions)
		{
			if (controlBase is UIStateToggleBtn3D)
			{
				if (controlBase.Transitions != null)
				{
					((UIStateToggleBtn3D)this).transitions = new EZTransitionList[controlBase.Transitions.Length];
					for (int i = 0; i < this.Transitions.Length; i++)
					{
						controlBase.Transitions[i].CopyToNew(this.Transitions[i], true);
					}
				}
			}
			else if (this.Transitions != null && controlBase.Transitions != null)
			{
				int num = 0;
				while (num < this.Transitions.Length && num < controlBase.Transitions.Length)
				{
					controlBase.Transitions[num].CopyTo(this.Transitions[num], true);
					num++;
				}
			}
		}
		if ((flags & ControlCopyFlags.Text) == ControlCopyFlags.Text)
		{
			if (this.spriteText == null && controlBase.spriteText != null)
			{
				GameObject gameObject = (GameObject)global::UnityEngine.Object.Instantiate(controlBase.spriteText.gameObject);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = controlBase.spriteText.transform.localPosition;
				gameObject.transform.localScale = controlBase.spriteText.transform.localScale;
				gameObject.transform.localRotation = controlBase.spriteText.transform.localRotation;
			}
			this.Text = controlBase.Text;
		}
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance && base.GetComponent<Collider>().GetType() == controlBase.GetComponent<Collider>().GetType())
		{
			if (base.GetComponent<Collider>() is BoxCollider)
			{
				BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
				BoxCollider boxCollider2 = (BoxCollider)controlBase.GetComponent<Collider>();
				boxCollider.center = boxCollider2.center;
				boxCollider.size = boxCollider2.size;
			}
			else if (base.GetComponent<Collider>() is SphereCollider)
			{
				SphereCollider sphereCollider = (SphereCollider)base.GetComponent<Collider>();
				SphereCollider sphereCollider2 = (SphereCollider)controlBase.GetComponent<Collider>();
				sphereCollider.center = sphereCollider2.center;
				sphereCollider.radius = sphereCollider2.radius;
			}
			else if (base.GetComponent<Collider>() is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = (CapsuleCollider)base.GetComponent<Collider>();
				CapsuleCollider capsuleCollider2 = (CapsuleCollider)controlBase.GetComponent<Collider>();
				capsuleCollider.center = capsuleCollider2.center;
				capsuleCollider.radius = capsuleCollider2.radius;
				capsuleCollider.height = capsuleCollider2.height;
				capsuleCollider.direction = capsuleCollider2.direction;
			}
			else if (base.GetComponent<Collider>() is MeshCollider)
			{
				MeshCollider meshCollider = (MeshCollider)base.GetComponent<Collider>();
				MeshCollider meshCollider2 = (MeshCollider)controlBase.GetComponent<Collider>();
				meshCollider.smoothSphereCollisions = meshCollider2.smoothSphereCollisions;
				meshCollider.convex = meshCollider2.convex;
				meshCollider.sharedMesh = meshCollider2.sharedMesh;
			}
			base.GetComponent<Collider>().isTrigger = controlBase.GetComponent<Collider>().isTrigger;
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			this.changeDelegate = controlBase.changeDelegate;
			this.inputDelegate = controlBase.inputDelegate;
		}
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			this.Container = controlBase.Container;
			if (Application.isPlaying)
			{
				this.controlIsEnabled = controlBase.controlIsEnabled;
			}
		}
	}

	public bool IsDraggable
	{
		get
		{
			return this.isDraggable;
		}
		set
		{
			this.isDraggable = value;
		}
	}

	public LayerMask DropMask
	{
		get
		{
			return this.dropMask;
		}
		set
		{
			this.dropMask = value;
		}
	}

	public float DragOffset
	{
		get
		{
			return this.dragOffset;
		}
		set
		{
			this.dragOffset = value;
			POINTER_INFO pointer_INFO;
			if (this.IsDragging && UIManager.Exists() && UIManager.instance.GetPointer(this, out pointer_INFO))
			{
				this.dragDropHelper.DragUpdatePosition(pointer_INFO);
			}
		}
	}

	public EZAnimation.EASING_TYPE CancelDragEasing
	{
		get
		{
			return this.cancelDragEasing;
		}
		set
		{
			this.cancelDragEasing = value;
		}
	}

	public float CancelDragDuration
	{
		get
		{
			return this.cancelDragDuration;
		}
		set
		{
			this.cancelDragDuration = value;
		}
	}

	public bool IsDragging
	{
		get
		{
			return this.dragDropHelper.IsDragging;
		}
		set
		{
			this.dragDropHelper.IsDragging = value;
		}
	}

	public GameObject DropTarget
	{
		get
		{
			return this.dragDropHelper.DropTarget;
		}
		set
		{
			this.dragDropHelper.DropTarget = value;
		}
	}

	public bool DropHandled
	{
		get
		{
			return this.dragDropHelper.DropHandled;
		}
		set
		{
			this.dragDropHelper.DropHandled = value;
		}
	}

	public void DragUpdatePosition(POINTER_INFO ptr)
	{
		this.dragDropHelper.DragUpdatePosition(ptr);
	}

	public void DefaultDragUpdatePosition(POINTER_INFO ptr)
	{
		this.dragDropHelper.DefaultDragUpdatePosition(ptr);
	}

	public void SetDragPosUpdater(EZDragDropHelper.UpdateDragPositionDelegate del)
	{
		this.dragDropHelper.SetDragPosUpdater(del);
	}

	public void CancelDrag()
	{
		this.dragDropHelper.CancelDrag();
	}

	public bool UseDefaultCancelDragAnim
	{
		get
		{
			return this.dragDropHelper.UseDefaultCancelDragAnim;
		}
		set
		{
			this.dragDropHelper.UseDefaultCancelDragAnim = value;
		}
	}

	public void CancelFinished()
	{
		this.dragDropHelper.CancelFinished();
	}

	public void DoDefaultCancelDrag()
	{
		this.dragDropHelper.DoDefaultCancelDrag();
	}

	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
		this.dragDropHelper.OnEZDragDrop_Internal(parms);
	}

	public void AddDragDropDelegate(EZDragDropDelegate del)
	{
		this.dragDropHelper.AddDragDropDelegate(del);
	}

	public void RemoveDragDropDelegate(EZDragDropDelegate del)
	{
		this.dragDropHelper.RemoveDragDropDelegate(del);
	}

	public void SetDragDropDelegate(EZDragDropDelegate del)
	{
		this.dragDropHelper.SetDragDropDelegate(del);
	}

	public abstract string[] States { get; }

	public virtual int DrawPreStateSelectGUI(int selState, bool inspector)
	{
		return 0;
	}

	public virtual int DrawPostStateSelectGUI(int selState)
	{
		return 0;
	}

	public virtual void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
	}

	public virtual string[] EnumStateElements()
	{
		return this.States;
	}

	public abstract EZTransitionList GetTransitions(int index);

	public abstract EZTransitionList[] Transitions { get; set; }

	public virtual string GetStateLabel(int index)
	{
		return null;
	}

	public virtual void SetStateLabel(int index, string label)
	{
	}

	public virtual ASCSEInfo GetStateElementInfo(int stateNum)
	{
		return new ASCSEInfo
		{
			transitions = this.GetTransitions(stateNum),
			stateLabel = this.GetStateLabel(stateNum)
		};
	}

	protected void UseStateLabel(int index)
	{
		string stateLabel = this.GetStateLabel(index);
		if (stateLabel == "[\"]")
		{
			return;
		}
		if (stateLabel == string.Empty && this.spriteText == null)
		{
			return;
		}
		this.Text = stateLabel;
	}

	public virtual void DoMirror()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.mirror == null)
		{
			this.mirror = new ControlBaseMirror();
			this.mirror.Mirror(this);
		}
		this.mirror.Validate(this);
		if (this.mirror.DidChange(this))
		{
			this.mirror.Mirror(this);
		}
	}

	public virtual void OnDrawGizmos()
	{
		this.DoMirror();
	}

	GameObject IUIObject.gameObject
	{
		get { return base.gameObject; }
	}

	Transform IUIObject.transform
	{
		get { return base.transform; }
	}

	string IUIObject.name
	{
		get { return base.name; }
	}

	public const string DittoString = "[\"]";

	protected ControlBaseMirror mirror;

	public string text;

	public SpriteText spriteText;

	public float textOffsetZ = -0.1f;

	public bool includeTextInAutoCollider;

	protected SpriteText.Anchor_Pos defaultTextAnchor = SpriteText.Anchor_Pos.Middle_Center;

	protected SpriteText.Alignment_Type defaultTextAlignment = SpriteText.Alignment_Type.Center;

	protected bool deleted;

	public bool detargetOnDisable;

	protected bool customCollider;

	[HideInInspector]
	public object data;

	protected bool m_controlIsEnabled = true;

	protected IUIContainer container;

	protected EZInputDelegate inputDelegate;

	protected EZValueChangedDelegate changeDelegate;

	protected EZDragDropHelper dragDropHelper = new EZDragDropHelper();

	public bool isDraggable;

	public LayerMask dropMask = -1;

	public float dragOffset = float.NaN;

	public EZAnimation.EASING_TYPE cancelDragEasing = EZAnimation.EASING_TYPE.Default;

	public float cancelDragDuration = -1f;
}
