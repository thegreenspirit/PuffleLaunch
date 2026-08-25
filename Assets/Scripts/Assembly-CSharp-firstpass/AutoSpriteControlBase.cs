using System;
using UnityEngine;

public abstract class AutoSpriteControlBase : AutoSpriteBase, IEZDragDrop, IControl, IPackableControl, IUIObject, ISpritePackable
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
				this.spriteText.Persistent = this.persistent;
				this.spriteText.Parent = this;
				this.spriteText.anchor = this.defaultTextAnchor;
				this.spriteText.alignment = this.defaultTextAlignment;
				this.spriteText.pixelPerfect = true;
				this.spriteText.SetCamera(this.renderCamera);
				if (Application.isPlaying)
				{
					this.spriteText.Persistent = this.persistent;
				}
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

	protected override void Init()
	{
		this.nullCamera = this.renderCamera == null;
		base.Init();
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

	public override void Start()
	{
		base.Start();
		if (UIManager.Exists())
		{
			if (this.nullCamera && UIManager.instance.uiCameras.Length > 0)
			{
				this.SetCamera(UIManager.instance.uiCameras[0].camera);
			}
			if (Application.isPlaying)
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
		if (this.spriteText != null)
		{
			this.spriteText.Persistent = this.persistent;
			this.spriteText.Parent = this;
		}
	}

	public override void TruncateTop(float pct)
	{
		base.TruncateTop(pct);
		if (this.aggregateLayers != null)
		{
			for (int i = 0; i < this.aggregateLayers.Length; i++)
			{
				if (this.aggregateLayers[i] != null)
				{
					for (int j = 0; j < this.aggregateLayers[i].Length; j++)
					{
						this.aggregateLayers[i][j].TruncateTop(pct);
					}
				}
			}
		}
	}

	public override void TruncateBottom(float pct)
	{
		base.TruncateBottom(pct);
		if (this.aggregateLayers != null)
		{
			for (int i = 0; i < this.aggregateLayers.Length; i++)
			{
				if (this.aggregateLayers[i] != null)
				{
					for (int j = 0; j < this.aggregateLayers[i].Length; j++)
					{
						this.aggregateLayers[i][j].TruncateBottom(pct);
					}
				}
			}
		}
	}

	public override void TruncateLeft(float pct)
	{
		base.TruncateLeft(pct);
		if (this.aggregateLayers != null)
		{
			for (int i = 0; i < this.aggregateLayers.Length; i++)
			{
				if (this.aggregateLayers[i] != null)
				{
					for (int j = 0; j < this.aggregateLayers[i].Length; j++)
					{
						this.aggregateLayers[i][j].TruncateLeft(pct);
					}
				}
			}
		}
	}

	public override void TruncateRight(float pct)
	{
		base.TruncateRight(pct);
		if (this.aggregateLayers != null)
		{
			for (int i = 0; i < this.aggregateLayers.Length; i++)
			{
				if (this.aggregateLayers[i] != null)
				{
					for (int j = 0; j < this.aggregateLayers[i].Length; j++)
					{
						this.aggregateLayers[i][j].TruncateRight(pct);
					}
				}
			}
		}
	}

	public override void Untruncate()
	{
		base.Untruncate();
		if (this.aggregateLayers != null)
		{
			for (int i = 0; i < this.aggregateLayers.Length; i++)
			{
				if (this.aggregateLayers[i] != null)
				{
					for (int j = 0; j < this.aggregateLayers[i].Length; j++)
					{
						this.aggregateLayers[i][j].Untruncate();
					}
				}
			}
		}
	}

	public override void Unclip()
	{
		if (this.ignoreClipping)
		{
			return;
		}
		base.Unclip();
		if (this.spriteText != null)
		{
			this.spriteText.Unclip();
		}
		if (this.aggregateLayers != null)
		{
			for (int i = 0; i < this.aggregateLayers.Length; i++)
			{
				if (this.aggregateLayers[i] != null)
				{
					for (int j = 0; j < this.aggregateLayers[i].Length; j++)
					{
						this.aggregateLayers[i][j].Unclip();
					}
				}
			}
		}
		this.UpdateCollider();
	}

	public override bool Clipped
	{
		get
		{
			return base.Clipped;
		}
		set
		{
			if (this.ignoreClipping)
			{
				return;
			}
			base.Clipped = value;
			if (this.spriteText != null)
			{
				this.spriteText.Clipped = value;
			}
			if (this.aggregateLayers != null)
			{
				for (int i = 0; i < this.aggregateLayers.Length; i++)
				{
					if (this.aggregateLayers[i] != null)
					{
						for (int j = 0; j < this.aggregateLayers[i].Length; j++)
						{
							this.aggregateLayers[i][j].Clipped = value;
						}
					}
				}
			}
			this.UpdateCollider();
		}
	}

	public override Rect3D ClippingRect
	{
		get
		{
			return base.ClippingRect;
		}
		set
		{
			if (this.ignoreClipping)
			{
				return;
			}
			base.ClippingRect = value;
			if (this.spriteText != null)
			{
				this.spriteText.ClippingRect = value;
			}
			if (this.aggregateLayers != null)
			{
				for (int i = 0; i < this.aggregateLayers.Length; i++)
				{
					if (this.aggregateLayers[i] != null)
					{
						for (int j = 0; j < this.aggregateLayers[i].Length; j++)
						{
							this.aggregateLayers[i][j].ClippingRect = value;
						}
					}
				}
			}
			this.UpdateCollider();
		}
	}

	public override Camera RenderCamera
	{
		get
		{
			return base.RenderCamera;
		}
		set
		{
			base.RenderCamera = value;
			if (this.spriteText != null)
			{
				this.spriteText.RenderCamera = value;
			}
		}
	}

	public override void SetCamera(Camera c)
	{
		base.SetCamera(c);
		if (this.spriteText != null)
		{
			this.spriteText.SetCamera(c);
		}
		if (this.pixelPerfect)
		{
			this.UpdateCollider();
		}
	}

	public override void Hide(bool tf)
	{
		if (!this.m_started)
		{
			this.Start();
		}
		if (!base.IsHidden() && tf)
		{
			if (base.GetComponent<Collider>() is BoxCollider && Application.isPlaying)
			{
				this.savedColliderSize = ((BoxCollider)base.GetComponent<Collider>()).size;
				((BoxCollider)base.GetComponent<Collider>()).size = Vector3.zero;
			}
		}
		else if (base.IsHidden() && !tf && base.GetComponent<Collider>() is BoxCollider)
		{
			((BoxCollider)base.GetComponent<Collider>()).size = this.savedColliderSize;
		}
		base.Hide(tf);
		if (this.aggregateLayers != null)
		{
			for (int i = 0; i < this.aggregateLayers.Length; i++)
			{
				if (this.aggregateLayers[i] != null)
				{
					for (int j = 0; j < this.aggregateLayers[i].Length; j++)
					{
						this.aggregateLayers[i][j].Hide(tf);
					}
				}
			}
		}
		if (this.spriteText != null)
		{
			this.spriteText.Hide(tf);
		}
		if (!tf)
		{
			this.UpdateCollider();
		}
	}

	public void Copy(IControl c)
	{
		if (!(c is AutoSpriteControlBase))
		{
			return;
		}
		this.Copy((SpriteRoot)c);
	}

	public void Copy(IControl c, ControlCopyFlags flags)
	{
		if (!(c is AutoSpriteControlBase))
		{
			return;
		}
		this.Copy((SpriteRoot)c, flags);
	}

	public override void Copy(SpriteRoot s)
	{
		this.Copy(s, ControlCopyFlags.All);
	}

	public virtual void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			if (Application.isPlaying && s.Started)
			{
				base.Copy(s);
			}
			else
			{
				base.CopyAll(s);
			}
			if (!(s is AutoSpriteControlBase))
			{
				if (this.autoResize || this.pixelPerfect)
				{
					base.CalcSize();
				}
				else
				{
					this.SetSize(s.width, s.height);
				}
				base.SetBleedCompensation();
				return;
			}
		}
		AutoSpriteControlBase autoSpriteControlBase = (AutoSpriteControlBase)s;
		if ((flags & ControlCopyFlags.Transitions) == ControlCopyFlags.Transitions)
		{
			if (autoSpriteControlBase is UIStateToggleBtn || !Application.isPlaying)
			{
				if (autoSpriteControlBase.Transitions != null)
				{
					this.Transitions = new EZTransitionList[autoSpriteControlBase.Transitions.Length];
					for (int i = 0; i < this.Transitions.Length; i++)
					{
						this.Transitions[i] = new EZTransitionList();
						autoSpriteControlBase.Transitions[i].CopyToNew(this.Transitions[i], true);
					}
				}
			}
			else if (this.Transitions != null && autoSpriteControlBase.Transitions != null)
			{
				int num = 0;
				while (num < this.Transitions.Length && num < autoSpriteControlBase.Transitions.Length)
				{
					autoSpriteControlBase.Transitions[num].CopyTo(this.Transitions[num], true);
					num++;
				}
			}
		}
		if ((flags & ControlCopyFlags.Text) == ControlCopyFlags.Text)
		{
			if (this.spriteText == null && autoSpriteControlBase.spriteText != null)
			{
				GameObject gameObject = (GameObject)global::UnityEngine.Object.Instantiate(autoSpriteControlBase.spriteText.gameObject);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = autoSpriteControlBase.spriteText.transform.localPosition;
				gameObject.transform.localScale = autoSpriteControlBase.spriteText.transform.localScale;
				gameObject.transform.localRotation = autoSpriteControlBase.spriteText.transform.localRotation;
			}
			if (this.spriteText != null)
			{
				this.spriteText.Copy(autoSpriteControlBase.spriteText);
			}
			this.text = autoSpriteControlBase.text;
			this.textOffsetZ = autoSpriteControlBase.textOffsetZ;
			this.includeTextInAutoCollider = autoSpriteControlBase.includeTextInAutoCollider;
		}
		if ((flags & ControlCopyFlags.Data) == ControlCopyFlags.Data)
		{
			this.data = autoSpriteControlBase.data;
		}
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			if (autoSpriteControlBase.GetComponent<Collider>() != null)
			{
				if (base.GetComponent<Collider>().GetType() == autoSpriteControlBase.GetComponent<Collider>().GetType())
				{
					if (autoSpriteControlBase.GetComponent<Collider>() is BoxCollider)
					{
						if (base.GetComponent<Collider>() == null)
						{
							base.gameObject.AddComponent(typeof(BoxCollider));
						}
						BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
						BoxCollider boxCollider2 = (BoxCollider)autoSpriteControlBase.GetComponent<Collider>();
						boxCollider.center = boxCollider2.center;
						boxCollider.size = boxCollider2.size;
					}
					else if (autoSpriteControlBase.GetComponent<Collider>() is SphereCollider)
					{
						if (base.GetComponent<Collider>() == null)
						{
							base.gameObject.AddComponent(typeof(SphereCollider));
						}
						SphereCollider sphereCollider = (SphereCollider)base.GetComponent<Collider>();
						SphereCollider sphereCollider2 = (SphereCollider)autoSpriteControlBase.GetComponent<Collider>();
						sphereCollider.center = sphereCollider2.center;
						sphereCollider.radius = sphereCollider2.radius;
					}
					else if (autoSpriteControlBase.GetComponent<Collider>() is MeshCollider)
					{
						if (base.GetComponent<Collider>() == null)
						{
							base.gameObject.AddComponent(typeof(MeshCollider));
						}
						MeshCollider meshCollider = (MeshCollider)base.GetComponent<Collider>();
						MeshCollider meshCollider2 = (MeshCollider)autoSpriteControlBase.GetComponent<Collider>();
						meshCollider.smoothSphereCollisions = meshCollider2.smoothSphereCollisions;
						meshCollider.convex = meshCollider2.convex;
						meshCollider.sharedMesh = meshCollider2.sharedMesh;
					}
					else if (autoSpriteControlBase.GetComponent<Collider>() is CapsuleCollider)
					{
						if (base.GetComponent<Collider>() == null)
						{
							base.gameObject.AddComponent(typeof(CapsuleCollider));
						}
						CapsuleCollider capsuleCollider = (CapsuleCollider)base.GetComponent<Collider>();
						CapsuleCollider capsuleCollider2 = (CapsuleCollider)autoSpriteControlBase.GetComponent<Collider>();
						capsuleCollider.center = capsuleCollider2.center;
						capsuleCollider.radius = capsuleCollider2.radius;
						capsuleCollider.height = capsuleCollider2.height;
						capsuleCollider.direction = capsuleCollider2.direction;
					}
					if (base.GetComponent<Collider>() != null)
					{
						base.GetComponent<Collider>().isTrigger = autoSpriteControlBase.GetComponent<Collider>().isTrigger;
					}
				}
			}
			else if (Application.isPlaying)
			{
				if (base.GetComponent<Collider>() == null && this.width != 0f && this.height != 0f && !float.IsNaN(this.width) && !float.IsNaN(this.height))
				{
					BoxCollider boxCollider3 = (BoxCollider)base.gameObject.AddComponent(typeof(BoxCollider));
					boxCollider3.size = new Vector3(autoSpriteControlBase.width, autoSpriteControlBase.height, 0.001f);
					boxCollider3.center = autoSpriteControlBase.GetCenterPoint();
					boxCollider3.isTrigger = true;
				}
				else if (base.GetComponent<Collider>() is BoxCollider)
				{
					BoxCollider boxCollider4 = (BoxCollider)base.GetComponent<Collider>();
					boxCollider4.size = new Vector3(autoSpriteControlBase.width, autoSpriteControlBase.height, 0.001f);
					boxCollider4.center = autoSpriteControlBase.GetCenterPoint();
				}
				else if (base.GetComponent<Collider>() is SphereCollider)
				{
					SphereCollider sphereCollider3 = (SphereCollider)base.GetComponent<Collider>();
					sphereCollider3.radius = Mathf.Max(autoSpriteControlBase.width, autoSpriteControlBase.height);
					sphereCollider3.center = autoSpriteControlBase.GetCenterPoint();
				}
			}
		}
		if ((flags & ControlCopyFlags.DragDrop) == ControlCopyFlags.DragDrop)
		{
			this.isDraggable = autoSpriteControlBase.isDraggable;
			this.dragOffset = autoSpriteControlBase.dragOffset;
			this.cancelDragEasing = autoSpriteControlBase.cancelDragEasing;
			this.cancelDragDuration = autoSpriteControlBase.cancelDragDuration;
		}
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			this.detargetOnDisable = autoSpriteControlBase.detargetOnDisable;
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			this.changeDelegate = autoSpriteControlBase.changeDelegate;
			this.inputDelegate = autoSpriteControlBase.inputDelegate;
		}
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State || (flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance)
		{
			this.Container = autoSpriteControlBase.Container;
			if (Application.isPlaying)
			{
				this.controlIsEnabled = autoSpriteControlBase.controlIsEnabled;
				this.Hide(autoSpriteControlBase.IsHidden());
			}
			if (this.curAnim != null)
			{
				if (this.curAnim.index == -1)
				{
					if (autoSpriteControlBase.curAnim != null)
					{
						this.curAnim = autoSpriteControlBase.curAnim.Clone();
					}
					base.PlayAnim(this.curAnim);
				}
				else
				{
					this.SetState(this.curAnim.index);
				}
			}
			else
			{
				this.SetState(0);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (this.dragDropHelper == null)
		{
			this.dragDropHelper = new EZDragDropHelper(this);
		}
		else
		{
			this.dragDropHelper.host = this;
		}
		if (base.GetComponent<Collider>() != null)
		{
			this.customCollider = true;
		}
		this.Init();
		base.AddSpriteResizedDelegate(new SpriteRoot.SpriteResizedDelegate(this.OnResize));
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.managed && this.m_spriteMesh != null && this.m_hidden)
		{
			this.m_spriteMesh.Hide(true);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
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

	protected void OnResize(float newWidth, float newHeight, SpriteRoot sprite)
	{
		this.UpdateCollider();
	}

	protected virtual void AddCollider()
	{
		if (this.customCollider || !Application.isPlaying || !this.m_started)
		{
			return;
		}
		BoxCollider boxCollider = (BoxCollider)base.gameObject.AddComponent(typeof(BoxCollider));
		boxCollider.isTrigger = true;
		if (base.IsHidden())
		{
			boxCollider.size = Vector3.zero;
		}
		else
		{
			this.UpdateCollider();
		}
	}

	public virtual void UpdateCollider()
	{
		if (this.deleted || this.m_spriteMesh == null)
		{
			return;
		}
		if (!(base.GetComponent<Collider>() is BoxCollider) || base.IsHidden() || this.m_spriteMesh == null || this.customCollider)
		{
			return;
		}
		Vector3[] vertices = this.m_spriteMesh.vertices;
		Vector3 vector = vertices[1];
		Vector3 vector2 = vertices[3];
		if (this.includeTextInAutoCollider && this.spriteText != null)
		{
			Matrix4x4 localToWorldMatrix = this.spriteText.transform.localToWorldMatrix;
			Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
			Vector3 vector3 = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(this.spriteText.TopLeft));
			Vector3 vector4 = worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(this.spriteText.BottomRight));
			if (vector4.x - vector3.x > 0f && vector3.y - vector4.y > 0f)
			{
				vector.x = Mathf.Min(vector.x, vector3.x);
				vector.y = Mathf.Min(vector.y, vector4.y);
				vector2.x = Mathf.Max(vector2.x, vector4.x);
				vector2.y = Mathf.Max(vector2.y, vector3.y);
			}
		}
		BoxCollider boxCollider = (BoxCollider)base.GetComponent<Collider>();
		boxCollider.size = vector2 - vector;
		boxCollider.center = vector + boxCollider.size * 0.5f;
		boxCollider.isTrigger = true;
	}

	public virtual void FindOuterEdges()
	{
		if (this.deleted)
		{
			return;
		}
		if (!this.m_started)
		{
			this.Start();
		}
		this.topLeftEdge = this.unclippedTopLeft;
		this.bottomRightEdge = this.unclippedBottomRight;
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		if (this.spriteText != null)
		{
			Matrix4x4 matrix4x = this.spriteText.transform.localToWorldMatrix;
			Vector3 vector = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.spriteText.UnclippedTopLeft));
			Vector3 vector2 = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.spriteText.UnclippedBottomRight));
			this.topLeftEdge.x = Mathf.Min(this.topLeftEdge.x, vector.x);
			this.topLeftEdge.y = Mathf.Max(this.topLeftEdge.y, vector.y);
			this.bottomRightEdge.x = Mathf.Max(this.bottomRightEdge.x, vector2.x);
			this.bottomRightEdge.y = Mathf.Min(this.bottomRightEdge.y, vector2.y);
		}
		if (this.aggregateLayers != null)
		{
			for (int i = 0; i < this.aggregateLayers.Length; i++)
			{
				for (int j = 0; j < this.aggregateLayers[i].Length; j++)
				{
					if (!this.aggregateLayers[i][j].IsHidden() && this.aggregateLayers[i][j].gameObject.active)
					{
						Matrix4x4 matrix4x = this.aggregateLayers[i][j].transform.localToWorldMatrix;
						Vector3 vector = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.aggregateLayers[i][j].UnclippedTopLeft));
						Vector3 vector2 = worldToLocalMatrix.MultiplyPoint3x4(matrix4x.MultiplyPoint3x4(this.aggregateLayers[i][j].UnclippedBottomRight));
						this.topLeftEdge.x = Mathf.Min(this.topLeftEdge.x, vector.x);
						this.topLeftEdge.y = Mathf.Max(this.topLeftEdge.y, vector.y);
						this.bottomRightEdge.x = Mathf.Max(this.bottomRightEdge.x, vector2.x);
						this.bottomRightEdge.y = Mathf.Min(this.bottomRightEdge.y, vector2.y);
					}
				}
			}
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
			if (this.container != null)
			{
				if (this.aggregateLayers != null)
				{
					for (int i = 0; i < this.aggregateLayers.Length; i++)
					{
						if (this.aggregateLayers[i] != null)
						{
							for (int j = 0; j < this.aggregateLayers[i].Length; j++)
							{
								this.container.RemoveChild(this.aggregateLayers[i][j].gameObject);
							}
						}
					}
				}
				if (this.spriteText != null)
				{
					this.container.RemoveChild(this.spriteText.gameObject);
				}
			}
			if (value != null)
			{
				if (this.aggregateLayers != null)
				{
					for (int k = 0; k < this.aggregateLayers.Length; k++)
					{
						if (this.aggregateLayers[k] != null)
						{
							for (int l = 0; l < this.aggregateLayers[k].Length; l++)
							{
								value.AddChild(this.aggregateLayers[k][l].gameObject);
							}
						}
					}
				}
				if (this.spriteText != null)
				{
					value.AddChild(this.spriteText.gameObject);
				}
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
		string[] array = new string[this.States.Length];
		for (int i = 0; i < this.States.Length; i++)
		{
			array[i] = this.States[i].name;
		}
		return array;
	}

	public virtual EZTransitionList GetTransitions(int index)
	{
		return null;
	}

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
			stateObj = this.States[stateNum],
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

	public override void DoMirror()
	{
		if (Application.isPlaying)
		{
			return;
		}
		if (this.screenSize.x == 0f || this.screenSize.y == 0f)
		{
			this.Start();
		}
		if (this.mirror == null)
		{
			this.mirror = new AutoSpriteControlBaseMirror();
			this.mirror.Mirror(this);
		}
		this.mirror.Validate(this);
		if (this.mirror.DidChange(this))
		{
			this.Init();
			this.mirror.Mirror(this);
		}
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

	protected bool nullCamera;

	public string text;

	public SpriteText spriteText;

	public float textOffsetZ = -0.1f;

	public bool includeTextInAutoCollider = true;

	protected SpriteText.Anchor_Pos defaultTextAnchor = SpriteText.Anchor_Pos.Middle_Center;

	protected SpriteText.Alignment_Type defaultTextAlignment = SpriteText.Alignment_Type.Center;

	public bool detargetOnDisable;

	protected bool customCollider;

	protected Vector3 savedColliderSize;

	protected Vector2 topLeftEdge;

	protected Vector2 bottomRightEdge;

	[HideInInspector]
	public object data;

	protected SpriteRoot[][] aggregateLayers;

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
