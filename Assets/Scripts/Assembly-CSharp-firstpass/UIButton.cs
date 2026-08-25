using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Button")]
public class UIButton : AutoSpriteControlBase
{
	public UIButton.CONTROL_STATE controlState
	{
		get
		{
			return this.m_ctrlState;
		}
	}

	public override bool controlIsEnabled
	{
		get
		{
			return this.m_controlIsEnabled;
		}
		set
		{
			bool controlIsEnabled = this.m_controlIsEnabled;
			this.m_controlIsEnabled = value;
			if (!value)
			{
				this.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			}
			else if (!controlIsEnabled)
			{
				this.SetControlState(UIButton.CONTROL_STATE.NORMAL);
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

	public override string GetStateLabel(int index)
	{
		return this.stateLabels[index];
	}

	public override void SetStateLabel(int index, string label)
	{
		this.stateLabels[index] = label;
		if (index == (int)this.m_ctrlState)
		{
			base.UseStateLabel(index);
		}
	}

	public override void OnInput(ref POINTER_INFO ptr)
	{
		if (this.deleted)
		{
			return;
		}
		if (!this.m_controlIsEnabled || base.IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}
		if (this.inputDelegate != null)
		{
			this.inputDelegate(ref ptr);
		}
		if (!this.m_controlIsEnabled || base.IsHidden())
		{
			base.OnInput(ref ptr);
			return;
		}
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.PRESS:
		case POINTER_INFO.INPUT_EVENT.DRAG:
			this.SetControlState(UIButton.CONTROL_STATE.ACTIVE);
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.TAP:
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
			if (this.m_ctrlState != UIButton.CONTROL_STATE.OVER)
			{
				this.SetControlState(UIButton.CONTROL_STATE.OVER);
				if (this.soundOnOver != null)
				{
					this.soundOnOver.PlayOneShot(this.soundOnOver.clip);
				}
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			this.SetControlState(UIButton.CONTROL_STATE.NORMAL);
			break;
		}
		base.OnInput(ref ptr);
		if (this.repeat)
		{
			if (this.m_ctrlState == UIButton.CONTROL_STATE.ACTIVE)
			{
				goto IL_0164;
			}
		}
		else if (ptr.evt == this.whenToInvoke)
		{
			goto IL_0164;
		}
		return;
		IL_0164:
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

	public override void Start()
	{
		if (this.m_started)
		{
			return;
		}
		base.Start();
		if (Application.isPlaying)
		{
			this.aggregateLayers = new SpriteRoot[1][];
			this.aggregateLayers[0] = this.layers;
			for (int i = 0; i < this.transitions.Length; i++)
			{
				for (int j = 0; j < this.transitions[i].list.Length; j++)
				{
					this.transitions[i].list[j].MainSubject = base.gameObject;
					if (this.spriteText != null)
					{
						this.transitions[i].list[j].AddSubSubject(this.spriteText.gameObject);
					}
				}
			}
			this.stateIndices = new int[this.layers.Length, 4];
			for (int k = 0; k < this.layers.Length; k++)
			{
				if (this.layers[k] == null)
				{
					Debug.LogError("A null layer sprite was encountered on control \"" + base.name + "\". Please fill in the layer reference, or remove the empty element.");
				}
				else
				{
					this.stateIndices[k, 0] = this.layers[k].GetStateIndex("normal");
					this.stateIndices[k, 1] = this.layers[k].GetStateIndex("over");
					this.stateIndices[k, 2] = this.layers[k].GetStateIndex("active");
					this.stateIndices[k, 3] = this.layers[k].GetStateIndex("disabled");
					if (this.stateIndices[k, 0] != -1)
					{
						this.transitions[0].list[0].AddSubSubject(this.layers[k].gameObject);
						this.transitions[0].list[1].AddSubSubject(this.layers[k].gameObject);
						this.transitions[0].list[2].AddSubSubject(this.layers[k].gameObject);
					}
					if (this.stateIndices[k, 1] != -1)
					{
						this.transitions[1].list[0].AddSubSubject(this.layers[k].gameObject);
						this.transitions[1].list[1].AddSubSubject(this.layers[k].gameObject);
					}
					if (this.stateIndices[k, 2] != -1)
					{
						this.transitions[2].list[0].AddSubSubject(this.layers[k].gameObject);
						this.transitions[2].list[1].AddSubSubject(this.layers[k].gameObject);
					}
					if (this.stateIndices[k, 3] != -1)
					{
						this.transitions[3].list[0].AddSubSubject(this.layers[k].gameObject);
						this.transitions[3].list[1].AddSubSubject(this.layers[k].gameObject);
						this.transitions[3].list[2].AddSubSubject(this.layers[k].gameObject);
					}
					if (this.stateIndices[k, (int)this.m_ctrlState] != -1)
					{
						this.layers[k].SetState(this.stateIndices[k, (int)this.m_ctrlState]);
					}
					else
					{
						this.layers[k].Hide(true);
					}
				}
			}
			if (base.GetComponent<Collider>() == null)
			{
				this.AddCollider();
			}
			this.SetState((int)this.m_ctrlState);
		}
		if (this.managed && this.m_hidden)
		{
			this.Hide(true);
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
				this.SetControlState(UIButton.CONTROL_STATE.NORMAL, true);
			}
			else
			{
				this.SetControlState(UIButton.CONTROL_STATE.DISABLED, true);
			}
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.transitionQueued)
		{
			this.nextTransition.RemoveTransitionEndDelegate(new EZTransition.OnTransitionEndDelegate(this.RunFollowupTrans));
			this.transitionQueued = false;
		}
		if (EZAnimator.Exists() && !this.deleted)
		{
			bool flag = this.alwaysFinishActiveTransition;
			this.alwaysFinishActiveTransition = false;
			if (this.prevTransition != null && this.prevTransition.IsRunning())
			{
				this.prevTransition.End();
			}
			this.alwaysFinishActiveTransition = flag;
		}
		this.prevTransition = null;
	}

	public override void Copy(SpriteRoot s)
	{
		this.Copy(s, ControlCopyFlags.All);
	}

	public override void Copy(SpriteRoot s, ControlCopyFlags flags)
	{
		base.Copy(s, flags);
		if (!(s is UIButton))
		{
			return;
		}
		UIButton uibutton = (UIButton)s;
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			this.prevTransition = uibutton.prevTransition;
			if (Application.isPlaying)
			{
				this.SetControlState(uibutton.controlState);
			}
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			this.scriptWithMethodToInvoke = uibutton.scriptWithMethodToInvoke;
			this.methodToInvoke = uibutton.methodToInvoke;
			this.whenToInvoke = uibutton.whenToInvoke;
			this.delay = uibutton.delay;
		}
		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			this.soundOnOver = uibutton.soundOnOver;
			this.soundOnClick = uibutton.soundOnClick;
		}
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			this.repeat = uibutton.repeat;
		}
	}

	public virtual void SetControlState(UIButton.CONTROL_STATE s)
	{
		this.SetControlState(s, false);
	}

	public virtual void SetControlState(UIButton.CONTROL_STATE s, bool suppressTransitions)
	{
		if (this.m_ctrlState == s)
		{
			return;
		}
		if (!this.alwaysFinishActiveTransition || (this.prevTransition != this.transitions[2].list[0] && (this.prevTransition != this.transitions[2].list[1] || !this.prevTransition.IsRunning())))
		{
			int ctrlState = (int)this.m_ctrlState;
			this.m_ctrlState = s;
			if (this.animations[(int)s].GetFrameCount() > 0)
			{
				this.SetState((int)s);
			}
			base.UseStateLabel((int)s);
			if (s == UIButton.CONTROL_STATE.DISABLED)
			{
				this.m_controlIsEnabled = false;
			}
			else
			{
				this.m_controlIsEnabled = true;
			}
			this.UpdateCollider();
			for (int i = 0; i < this.layers.Length; i++)
			{
				if (this.stateIndices[i, (int)s] != -1)
				{
					this.layers[i].Hide(base.IsHidden());
					this.layers[i].SetState(this.stateIndices[i, (int)s]);
				}
				else
				{
					this.layers[i].Hide(true);
				}
			}
			if (suppressTransitions)
			{
				return;
			}
			if (this.prevTransition != null)
			{
				this.prevTransition.StopSafe();
			}
			this.StartTransition((int)s, ctrlState);
		}
		else
		{
			if (suppressTransitions)
			{
				return;
			}
			this.QueueTransition((int)s, 2);
		}
	}

	protected int DetermineNextTransition(int newState, int prevState)
	{
		int num = 0;
		switch (newState)
		{
		case 0:
			switch (prevState)
			{
			case 1:
				num = 0;
				break;
			case 2:
				num = 1;
				break;
			case 3:
				num = 2;
				break;
			}
			break;
		case 1:
			switch (prevState)
			{
			case 0:
				num = 0;
				break;
			case 2:
				num = 1;
				break;
			}
			break;
		case 2:
			if (prevState != 0)
			{
				if (prevState == 1)
				{
					num = 1;
				}
			}
			else
			{
				num = 0;
			}
			break;
		case 3:
			switch (prevState)
			{
			case 0:
				num = 0;
				break;
			case 1:
				num = 1;
				break;
			case 2:
				num = 2;
				break;
			}
			break;
		}
		return num;
	}

	protected void StartTransition(int newState, int prevState)
	{
		int num = this.DetermineNextTransition(newState, prevState);
		this.prevTransition = this.transitions[newState].list[num];
		if (this.prevTransition.animationTypes == null || this.prevTransition.animationTypes.Length < 1)
		{
			this.prevTransition = null;
		}
		else
		{
			this.prevTransition.Start();
		}
	}

	protected void QueueTransition(int newState, int prevState)
	{
		if (this.deleted)
		{
			return;
		}
		this.nextTransition = this.transitions[newState].list[this.DetermineNextTransition(newState, prevState)];
		this.nextState = (UIButton.CONTROL_STATE)newState;
		if (!this.transitionQueued)
		{
			this.prevTransition.AddTransitionEndDelegate(new EZTransition.OnTransitionEndDelegate(this.RunFollowupTrans));
		}
		this.transitionQueued = true;
	}

	protected void RunFollowupTrans(EZTransition trans)
	{
		if (this.deleted)
		{
			trans.RemoveTransitionEndDelegate(new EZTransition.OnTransitionEndDelegate(this.RunFollowupTrans));
			return;
		}
		this.prevTransition = null;
		this.nextTransition = null;
		trans.RemoveTransitionEndDelegate(new EZTransition.OnTransitionEndDelegate(this.RunFollowupTrans));
		this.transitionQueued = false;
		this.SetControlState(this.nextState);
	}

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			bool flag = this.spriteText == null;
			base.Text = value;
			if (flag && this.spriteText != null && Application.isPlaying)
			{
				for (int i = 0; i < this.transitions.Length; i++)
				{
					for (int j = 0; j < this.transitions[i].list.Length; j++)
					{
						this.transitions[i].list[j].AddSubSubject(this.spriteText.gameObject);
					}
				}
			}
		}
	}

	public override void InitUVs()
	{
		if (this.states[0].spriteFrames.Length != 0)
		{
			this.frameInfo.Copy(this.states[0].spriteFrames[0]);
		}
		base.InitUVs();
	}

	public static UIButton Create(string name, Vector3 pos)
	{
		return (UIButton)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIButton));
	}

	public static UIButton Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UIButton)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UIButton));
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		this.scriptWithMethodToInvoke = gui.DrawScriptSelection(this.scriptWithMethodToInvoke, ref this.methodToInvoke);
	}

	protected UIButton.CONTROL_STATE m_ctrlState;

	[HideInInspector]
	public TextureAnim[] states = new TextureAnim[]
	{
		new TextureAnim("Normal"),
		new TextureAnim("Over"),
		new TextureAnim("Active"),
		new TextureAnim("Disabled")
	};

	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
	{
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
		}),
		new EZTransitionList(new EZTransition[]
		{
			new EZTransition("From Normal"),
			new EZTransition("From Over"),
			new EZTransition("From Active")
		})
	};

	private EZTransition prevTransition;

	[HideInInspector]
	public string[] stateLabels = new string[] { "[\"]", "[\"]", "[\"]", "[\"]" };

	public SpriteRoot[] layers = new SpriteRoot[0];

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke = string.Empty;

	public POINTER_INFO.INPUT_EVENT whenToInvoke = POINTER_INFO.INPUT_EVENT.TAP;

	public float delay;

	public AudioSource soundOnOver;

	public AudioSource soundOnClick;

	public bool repeat;

	public bool alwaysFinishActiveTransition;

	protected bool transitionQueued;

	protected EZTransition nextTransition;

	protected UIButton.CONTROL_STATE nextState;

	protected int[,] stateIndices;

	public enum CONTROL_STATE
	{
		NORMAL,
		OVER,
		ACTIVE,
		DISABLED
	}
}
