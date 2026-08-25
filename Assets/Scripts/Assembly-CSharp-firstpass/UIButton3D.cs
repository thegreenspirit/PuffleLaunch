using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/3D Button")]
public class UIButton3D : ControlBase
{
	public UIButton3D.CONTROL_STATE controlState
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
			this.m_controlIsEnabled = value;
			if (!value)
			{
				this.SetControlState(UIButton3D.CONTROL_STATE.DISABLED);
			}
			else
			{
				this.SetControlState(UIButton3D.CONTROL_STATE.NORMAL);
			}
		}
	}

	public override string[] States
	{
		get
		{
			return this.states;
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
		if (!this.m_controlIsEnabled)
		{
			base.OnInput(ref ptr);
			return;
		}
		if (this.inputDelegate != null)
		{
			this.inputDelegate(ref ptr);
		}
		if (!this.m_controlIsEnabled)
		{
			base.OnInput(ref ptr);
			return;
		}
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.PRESS:
		case POINTER_INFO.INPUT_EVENT.DRAG:
			this.SetControlState(UIButton3D.CONTROL_STATE.ACTIVE);
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
		case POINTER_INFO.INPUT_EVENT.TAP:
			if (ptr.type != POINTER_INFO.POINTER_TYPE.TOUCHPAD && ptr.hitInfo.collider == base.GetComponent<Collider>())
			{
				this.SetControlState(UIButton3D.CONTROL_STATE.OVER);
			}
			else
			{
				this.SetControlState(UIButton3D.CONTROL_STATE.NORMAL);
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE:
			if (this.m_ctrlState != UIButton3D.CONTROL_STATE.OVER)
			{
				this.SetControlState(UIButton3D.CONTROL_STATE.OVER);
				if (this.soundOnOver != null)
				{
					this.soundOnOver.PlayOneShot(this.soundOnOver.clip);
				}
			}
			break;
		case POINTER_INFO.INPUT_EVENT.MOVE_OFF:
		case POINTER_INFO.INPUT_EVENT.RELEASE_OFF:
			this.SetControlState(UIButton3D.CONTROL_STATE.NORMAL);
			break;
		}
		base.OnInput(ref ptr);
		if (this.repeat)
		{
			if (this.m_ctrlState == UIButton3D.CONTROL_STATE.ACTIVE)
			{
				goto IL_014E;
			}
		}
		else if (ptr.evt == this.whenToInvoke)
		{
			goto IL_014E;
		}
		return;
		IL_014E:
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
		this.m_started = true;
		base.Start();
		if (Application.isPlaying)
		{
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
			if (base.GetComponent<Collider>() == null)
			{
				this.AddCollider();
			}
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (Application.isPlaying && this.m_started)
		{
			this.m_ctrlState = (UIButton3D.CONTROL_STATE)(-1);
			if (this.controlIsEnabled)
			{
				this.SetControlState(UIButton3D.CONTROL_STATE.NORMAL, true);
			}
			else
			{
				this.SetControlState(UIButton3D.CONTROL_STATE.DISABLED, true);
			}
		}
	}

	public override void OnDisable()
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

	public override void Copy(IControl c)
	{
		this.Copy(c, ControlCopyFlags.All);
	}

	public override void Copy(IControl c, ControlCopyFlags flags)
	{
		base.Copy(c, flags);
		if (!(c is UIButton3D))
		{
			return;
		}
		UIButton3D uibutton3D = (UIButton3D)c;
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			this.prevTransition = uibutton3D.prevTransition;
			if (Application.isPlaying)
			{
				this.SetControlState(uibutton3D.controlState);
			}
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			this.scriptWithMethodToInvoke = uibutton3D.scriptWithMethodToInvoke;
			this.methodToInvoke = uibutton3D.methodToInvoke;
			this.whenToInvoke = uibutton3D.whenToInvoke;
			this.delay = uibutton3D.delay;
		}
		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			this.soundOnOver = uibutton3D.soundOnOver;
			this.soundOnClick = uibutton3D.soundOnClick;
		}
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			this.repeat = uibutton3D.repeat;
		}
	}

	public virtual void SetControlState(UIButton3D.CONTROL_STATE s)
	{
		this.SetControlState(s, false);
	}

	public virtual void SetControlState(UIButton3D.CONTROL_STATE s, bool suppressTransitions)
	{
		if (this.m_ctrlState == s)
		{
			return;
		}
		if (!this.alwaysFinishActiveTransition || (this.prevTransition != this.transitions[2].list[0] && this.prevTransition != this.transitions[2].list[1]))
		{
			int ctrlState = (int)this.m_ctrlState;
			this.m_ctrlState = s;
			base.UseStateLabel((int)s);
			if (s == UIButton3D.CONTROL_STATE.DISABLED)
			{
				this.m_controlIsEnabled = false;
			}
			else
			{
				this.m_controlIsEnabled = true;
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
		this.prevTransition.Start();
	}

	protected void QueueTransition(int newState, int prevState)
	{
		if (this.deleted)
		{
			return;
		}
		this.nextTransition = this.transitions[newState].list[this.DetermineNextTransition(newState, prevState)];
		this.nextState = (UIButton3D.CONTROL_STATE)newState;
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
				this.transitions[0].list[0].AddSubSubject(this.spriteText.gameObject);
				this.transitions[0].list[1].AddSubSubject(this.spriteText.gameObject);
				this.transitions[0].list[2].AddSubSubject(this.spriteText.gameObject);
				this.transitions[1].list[0].AddSubSubject(this.spriteText.gameObject);
				this.transitions[1].list[1].AddSubSubject(this.spriteText.gameObject);
				this.transitions[2].list[0].AddSubSubject(this.spriteText.gameObject);
				this.transitions[2].list[1].AddSubSubject(this.spriteText.gameObject);
				this.transitions[3].list[0].AddSubSubject(this.spriteText.gameObject);
				this.transitions[3].list[1].AddSubSubject(this.spriteText.gameObject);
				this.transitions[3].list[2].AddSubSubject(this.spriteText.gameObject);
			}
		}
	}

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		this.scriptWithMethodToInvoke = gui.DrawScriptSelection(this.scriptWithMethodToInvoke, ref this.methodToInvoke);
	}

	public static UIButton3D Create(string name, Vector3 pos)
	{
		return (UIButton3D)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIButton3D));
	}

	public static UIButton3D Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UIButton3D)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UIButton3D));
	}

	protected UIButton3D.CONTROL_STATE m_ctrlState;

	protected string[] states = new string[] { "Normal", "Over", "Active", "Disabled" };

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

	protected UIButton3D.CONTROL_STATE nextState;

	protected bool m_started;

	public enum CONTROL_STATE
	{
		NORMAL,
		OVER,
		ACTIVE,
		DISABLED
	}
}
