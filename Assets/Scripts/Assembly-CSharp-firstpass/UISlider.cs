using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Slider")]
public class UISlider : AutoSpriteControlBase
{
	public override bool controlIsEnabled
	{
		get
		{
			return this.m_controlIsEnabled;
		}
		set
		{
			this.m_controlIsEnabled = value;
			if (this.knob != null)
			{
				this.knob.controlIsEnabled = value;
			}
		}
	}

	public float Value
	{
		get
		{
			return this.m_value;
		}
		set
		{
			float value2 = this.m_value;
			this.m_value = Mathf.Clamp01(value);
			if (this.m_value != value2)
			{
				this.UpdateValue();
			}
		}
	}

	public override TextureAnim[] States
	{
		get
		{
			return this.states;
		}
		set
		{
			this.states = value;
		}
	}

	public override EZTransitionList GetTransitions(int index)
	{
		if (index >= this.transitions.Length)
		{
			return null;
		}
		return this.transitions[index];
	}

	public override EZTransitionList[] Transitions
	{
		get
		{
			return this.transitions;
		}
		set
		{
			this.transitions = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.m_value = this.defaultValue;
	}

	public override void Start()
	{
		if (this.m_started)
		{
			return;
		}
		base.Start();
		this.aggregateLayers = new SpriteRoot[2][];
		this.aggregateLayers[0] = this.filledLayers;
		this.aggregateLayers[1] = this.emptyLayers;
		if (Application.isPlaying)
		{
			this.truncFloor = this.stopKnobFromEdge / this.width;
			this.truncRange = 1f - this.truncFloor * 2f;
			this.filledIndices = new int[this.filledLayers.Length];
			this.emptyIndices = new int[this.emptyLayers.Length];
			for (int i = 0; i < this.filledLayers.Length; i++)
			{
				if (this.filledLayers[i] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + base.name + "\". Please fill in the layer reference, or remove the empty element.");
				}
				else
				{
					this.filledIndices[i] = this.filledLayers[i].GetStateIndex("filled");
					if (this.filledIndices[i] != -1)
					{
						this.filledLayers[i].SetState(this.filledIndices[i]);
					}
				}
			}
			for (int j = 0; j < this.emptyLayers.Length; j++)
			{
				if (this.emptyLayers[j] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + base.name + "\". Please fill in the layer reference, or remove the empty element.");
				}
				else
				{
					this.emptyIndices[j] = this.emptyLayers[j].GetStateIndex("empty");
					if (this.emptyIndices[j] != -1)
					{
						this.emptyLayers[j].SetState(this.emptyIndices[j]);
					}
				}
			}
			this.knob = (UIScrollKnob)new GameObject
			{
				name = base.name + " - Knob",
				transform = 
				{
					parent = base.transform,
					localPosition = this.CalcKnobStartPos(),
					localRotation = Quaternion.identity,
					localScale = Vector3.one
				},
				layer = base.gameObject.layer
			}.AddComponent(typeof(UIScrollKnob));
			this.knob.plane = this.plane;
			this.knob.SetOffset(this.knobOffset);
			this.knob.persistent = this.persistent;
			this.knob.bleedCompensation = this.bleedCompensation;
			if (!this.managed)
			{
				if (this.knob.spriteMesh != null)
				{
					((SpriteMesh)this.knob.spriteMesh).material = base.GetComponent<Renderer>().sharedMaterial;
				}
			}
			else if (this.manager != null)
			{
				this.knob.Managed = this.managed;
				this.manager.AddSprite(this.knob);
				this.knob.SetDrawLayer(this.drawLayer + 1);
			}
			else
			{
				Debug.LogError("Sprite on object \"" + base.name + "\" not assigned to a SpriteManager!");
			}
			this.knob.autoResize = this.autoResize;
			if (this.pixelPerfect)
			{
				this.knob.pixelPerfect = true;
			}
			else
			{
				this.knob.SetSize(this.knobSize.x, this.knobSize.y);
			}
			this.knob.ignoreClipping = this.ignoreClipping;
			this.knob.color = this.color;
			this.knob.SetColliderSizeFactor(this.knobColliderSizeFactor);
			this.knob.SetSlider(this);
			this.knob.SetMaxScroll(this.width - this.stopKnobFromEdge * 2f);
			this.knob.SetInputDelegate(this.inputDelegate);
			this.knob.transitions[0] = this.transitions[2];
			this.knob.transitions[1] = this.transitions[3];
			this.knob.transitions[2] = this.transitions[4];
			this.knob.layers = this.knobLayers;
			for (int k = 0; k < this.knobLayers.Length; k++)
			{
				this.knobLayers[k].transform.parent = this.knob.transform;
			}
			this.knob.animations[0].SetAnim(this.states[2], 0);
			this.knob.animations[1].SetAnim(this.states[3], 1);
			this.knob.animations[2].SetAnim(this.states[4], 2);
			this.knob.SetupAppearance();
			this.knob.SetCamera(this.renderCamera);
			this.knob.Hide(base.IsHidden());
			this.emptySprite = (AutoSprite)new GameObject
			{
				name = base.name + " - Empty Bar",
				transform = 
				{
					parent = base.transform,
					localPosition = Vector3.zero,
					localRotation = Quaternion.identity,
					localScale = Vector3.one
				},
				layer = base.gameObject.layer
			}.AddComponent(typeof(AutoSprite));
			this.emptySprite.plane = this.plane;
			this.emptySprite.autoResize = this.autoResize;
			this.emptySprite.pixelPerfect = this.pixelPerfect;
			this.emptySprite.persistent = this.persistent;
			this.emptySprite.ignoreClipping = this.ignoreClipping;
			this.emptySprite.bleedCompensation = this.bleedCompensation;
			if (!this.managed)
			{
				this.emptySprite.GetComponent<Renderer>().sharedMaterial = base.GetComponent<Renderer>().sharedMaterial;
			}
			else if (this.manager != null)
			{
				this.emptySprite.Managed = this.managed;
				this.manager.AddSprite(this.emptySprite);
				this.emptySprite.SetDrawLayer(this.drawLayer);
			}
			else
			{
				Debug.LogError("Sprite on object \"" + base.name + "\" not assigned to a SpriteManager!");
			}
			this.emptySprite.color = this.color;
			this.emptySprite.SetAnchor(this.anchor);
			this.emptySprite.Setup(this.width, this.height, this.m_spriteMesh.material);
			if (this.states[1].spriteFrames.Length != 0)
			{
				this.emptySprite.animations = new UVAnimation[1];
				this.emptySprite.animations[0] = new UVAnimation();
				this.emptySprite.animations[0].SetAnim(this.states[1], 0);
				this.emptySprite.PlayAnim(0, 0);
			}
			this.emptySprite.renderCamera = this.renderCamera;
			this.emptySprite.Hide(base.IsHidden());
			if (this.container != null)
			{
				this.container.AddChild(this.knob.gameObject);
				this.container.AddChild(this.emptySprite.gameObject);
			}
			this.SetState(0);
			this.m_value = -1f;
			this.Value = this.defaultValue;
		}
		if (this.managed && this.m_hidden)
		{
			this.Hide(true);
		}
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		if (this.knob == null)
		{
			return;
		}
		this.knob.SetStartPos(this.CalcKnobStartPos());
		this.knob.SetMaxScroll(width - this.stopKnobFromEdge * 2f);
		this.knob.SetPosition(this.m_value);
		this.emptySprite.SetSize(width, height);
	}

	public override void Copy(SpriteRoot s)
	{
		this.Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (!(s is UISlider))
		{
			return;
		}
		UISlider uislider = (UISlider)s;
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			this.scriptWithMethodToInvoke = uislider.scriptWithMethodToInvoke;
			this.methodToInvoke = uislider.methodToInvoke;
		}
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			this.defaultValue = uislider.defaultValue;
			this.stopKnobFromEdge = uislider.stopKnobFromEdge;
			this.knobOffset = uislider.knobOffset;
			this.knobSize = uislider.knobSize;
			this.knobColliderSizeFactor = uislider.knobColliderSizeFactor;
		}
		if ((flags & ControlCopyFlags.Appearance) == ControlCopyFlags.Appearance && Application.isPlaying)
		{
			if (this.emptySprite != null)
			{
				this.emptySprite.Copy(uislider.emptySprite);
			}
			if (this.knob != null)
			{
				this.knob.Copy(uislider.knob);
			}
			this.truncFloor = uislider.truncFloor;
			this.truncRange = uislider.truncRange;
		}
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			this.CalcKnobStartPos();
			this.Value = uislider.Value;
		}
	}

	protected Vector3 CalcKnobStartPos()
	{
		Vector3 zero = Vector3.zero;
		switch (this.anchor)
		{
		case SpriteRoot.ANCHOR_METHOD.UPPER_LEFT:
			zero.x = this.stopKnobFromEdge;
			zero.y = this.height * -0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.UPPER_CENTER:
			zero.x = this.width * -0.5f + this.stopKnobFromEdge;
			zero.y = this.height * -0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT:
			zero.x = this.width * -1f + this.stopKnobFromEdge;
			zero.y = this.height * -0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT:
			zero.x = this.stopKnobFromEdge;
			break;
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER:
			zero.x = this.width * -0.5f + this.stopKnobFromEdge;
			break;
		case SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT:
			zero.x = this.width * -1f + this.stopKnobFromEdge;
			break;
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT:
			zero.x = this.stopKnobFromEdge;
			zero.y = this.height * 0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER:
			zero.x = this.width * -0.5f + this.stopKnobFromEdge;
			zero.y = this.height * 0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT:
			zero.x = this.width * -1f + this.stopKnobFromEdge;
			zero.y = this.height * 0.5f;
			break;
		case SpriteRoot.ANCHOR_METHOD.TEXTURE_OFFSET:
			zero.x = this.width * -0.5f + this.stopKnobFromEdge;
			break;
		}
		return zero;
	}

	public override void InitUVs()
	{
		if (this.states[0].spriteFrames.Length != 0)
		{
			this.frameInfo.Copy(this.states[0].spriteFrames[0]);
		}
		base.InitUVs();
	}

	protected void UpdateValue()
	{
		if (this.knob == null)
		{
			return;
		}
		float num = this.truncFloor + this.m_value * this.truncRange;
		this.UpdateAppearance(num);
		this.knob.SetPosition(this.m_value);
		if (this.scriptWithMethodToInvoke != null)
		{
			this.scriptWithMethodToInvoke.Invoke(this.methodToInvoke, 0f);
		}
		if (this.changeDelegate != null)
		{
			this.changeDelegate(this);
		}
	}

	public void ScrollKnobMoved(UIScrollKnob knob, float val)
	{
		this.m_value = val;
		float num = this.truncFloor + this.m_value * this.truncRange;
		this.UpdateAppearance(num);
		if (this.scriptWithMethodToInvoke != null)
		{
			this.scriptWithMethodToInvoke.Invoke(this.methodToInvoke, 0f);
		}
		if (this.changeDelegate != null)
		{
			this.changeDelegate(this);
		}
	}

	public override void SetInputDelegate(EZInputDelegate del)
	{
		if (this.knob != null)
		{
			this.knob.SetInputDelegate(del);
		}
		base.SetInputDelegate(del);
	}

	public override void AddInputDelegate(EZInputDelegate del)
	{
		if (this.knob != null)
		{
			this.knob.AddInputDelegate(del);
		}
		base.AddInputDelegate(del);
	}

	public override void RemoveInputDelegate(EZInputDelegate del)
	{
		if (this.knob != null)
		{
			this.knob.RemoveInputDelegate(del);
		}
		base.RemoveInputDelegate(del);
	}

	protected void UpdateAppearance(float truncVal)
	{
		this.TruncateRight(truncVal);
		if (this.emptySprite != null)
		{
			this.emptySprite.TruncateLeft(1f - truncVal);
		}
		for (int i = 0; i < this.filledLayers.Length; i++)
		{
			this.filledLayers[i].TruncateRight(truncVal);
		}
		for (int j = 0; j < this.emptyLayers.Length; j++)
		{
			this.emptyLayers[j].TruncateLeft(1f - truncVal);
		}
	}

	public UIScrollKnob GetKnob()
	{
		return this.knob;
	}

	public override IUIContainer Container
	{
		get
		{
			return base.Container;
		}
		set
		{
			if (value != this.container)
			{
				if (this.container != null)
				{
					this.container.RemoveChild(this.emptySprite.gameObject);
					this.container.RemoveChild(this.knob.gameObject);
				}
				if (value != null)
				{
					if (this.emptySprite != null)
					{
						value.AddChild(this.emptySprite.gameObject);
					}
					if (this.knob != null)
					{
						value.AddChild(this.knob.gameObject);
					}
				}
			}
			base.Container = value;
		}
	}

	public override void Unclip()
	{
		if (this.ignoreClipping)
		{
			return;
		}
		base.Unclip();
		this.emptySprite.Unclip();
		this.knob.Unclip();
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
			this.emptySprite.Clipped = value;
			this.knob.Clipped = value;
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
			this.emptySprite.ClippingRect = value;
			this.knob.ClippingRect = value;
		}
	}

	public static UISlider Create(string name, Vector3 pos)
	{
		return (UISlider)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UISlider));
	}

	public static UISlider Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UISlider)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UISlider));
	}

	public override void Hide(bool tf)
	{
		base.Hide(tf);
		if (this.emptySprite != null)
		{
			this.emptySprite.Hide(tf);
		}
		if (this.knob != null)
		{
			this.knob.Hide(tf);
		}
	}

	public override void SetColor(Color c)
	{
		base.SetColor(c);
		if (this.emptySprite != null)
		{
			this.emptySprite.SetColor(c);
		}
		if (this.knob != null)
		{
			this.knob.SetColor(c);
		}
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		this.scriptWithMethodToInvoke = gui.DrawScriptSelection(this.scriptWithMethodToInvoke, ref this.methodToInvoke);
	}

	protected float m_value;

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke = string.Empty;

	public float defaultValue;

	public float stopKnobFromEdge;

	public Vector3 knobOffset = new Vector3(0f, 0f, -0.1f);

	public Vector2 knobSize;

	public Vector2 knobColliderSizeFactor = new Vector2(1f, 1f);

	protected AutoSprite emptySprite;

	protected UIScrollKnob knob;

	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
	{
		new TextureAnim("Filled bar"),
		new TextureAnim("Empty bar"),
		new TextureAnim("Knob, Normal"),
		new TextureAnim("Knob, Over"),
		new TextureAnim("Knob, Active")
	};

	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
	{
		null,
		null,
		new EZTransitionList(new EZTransition[]
		{
			new EZTransition("From Over"),
			new EZTransition("From Active"),
			new EZTransition("From Disabled")
		}),
		new EZTransitionList(new EZTransition[]
		{
			new EZTransition("From Normal"),
			new EZTransition("From Active")
		}),
		new EZTransitionList(new EZTransition[]
		{
			new EZTransition("From Normal"),
			new EZTransition("From Over")
		})
	};

	public SpriteRoot[] filledLayers = new SpriteRoot[0];

	public SpriteRoot[] emptyLayers = new SpriteRoot[0];

	public SpriteRoot[] knobLayers = new SpriteRoot[0];

	protected float truncFloor;

	protected float truncRange;

	protected int[] filledIndices;

	protected int[] emptyIndices;
}
