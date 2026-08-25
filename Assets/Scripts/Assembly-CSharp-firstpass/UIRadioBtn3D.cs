using System;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/3D Radio Button")]
public class UIRadioBtn3D : ControlBase, IRadioButton
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
			if (!value)
			{
				this.DisableMe();
			}
			else
			{
				this.SetButtonState();
			}
		}
	}

	public bool Value
	{
		get
		{
			return this.btnValue;
		}
		set
		{
			bool flag = this.btnValue;
			this.btnValue = value;
			if (this.btnValue)
			{
				this.PopOtherButtonsInGroup();
			}
			this.SetButtonState();
			if (flag != this.btnValue && this.changeDelegate != null)
			{
				this.changeDelegate(this);
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
		if (index == (int)this.state)
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
		if (ptr.evt == this.whenToInvoke)
		{
			this.Value = true;
			if (this.soundToPlay != null)
			{
				this.soundToPlay.PlayOneShot(this.soundToPlay.clip);
			}
			if (this.scriptWithMethodToInvoke != null)
			{
				this.scriptWithMethodToInvoke.Invoke(this.methodToInvoke, this.delay);
			}
		}
		base.OnInput(ref ptr);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (this.stateChangeWhileDeactivated)
		{
			this.SetButtonState();
			this.stateChangeWhileDeactivated = false;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (this.group == null)
		{
			return;
		}
		this.group.buttons.Remove(this);
		this.group = null;
	}

	public void SetGroup(GameObject parent)
	{
		this.SetGroup(parent.transform.GetHashCode());
	}

	public void SetGroup(int groupID)
	{
		if (this.group != null)
		{
			this.group.buttons.Remove(this);
			this.group = null;
		}
		this.radioGroup = groupID;
		this.group = RadioBtnGroup.GetGroup(groupID);
		this.group.buttons.Add(this);
		if (this.btnValue)
		{
			this.PopOtherButtonsInGroup();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.btnValue = this.defaultValue;
	}

	public override void Start()
	{
		base.Start();
		this.state = ((!this.controlIsEnabled) ? UIRadioBtn3D.CONTROL_STATE.Disabled : ((!this.btnValue) ? UIRadioBtn3D.CONTROL_STATE.False : UIRadioBtn3D.CONTROL_STATE.True));
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
			int num = ((!this.btnValue) ? 1 : 0);
			int num2 = ((!this.m_controlIsEnabled) ? 2 : num);
			if (base.GetComponent<Collider>() == null)
			{
				this.AddCollider();
			}
		}
		this.Value = this.btnValue;
		if (this.useParentForGrouping && base.transform.parent != null)
		{
			this.SetGroup(base.transform.parent.GetHashCode());
		}
		else
		{
			this.SetGroup(this.radioGroup);
		}
	}

	public override void Copy(IControl c)
	{
		this.Copy(c, ControlCopyFlags.All);
	}

	public override void Copy(IControl c, ControlCopyFlags flags)
	{
		if (!(c is UIRadioBtn3D))
		{
			return;
		}
		base.Copy(c);
		UIRadioBtn3D uiradioBtn3D = (UIRadioBtn3D)c;
		if ((flags & ControlCopyFlags.Settings) == ControlCopyFlags.Settings)
		{
			this.group = uiradioBtn3D.group;
			this.defaultValue = uiradioBtn3D.defaultValue;
		}
		if ((flags & ControlCopyFlags.State) == ControlCopyFlags.State)
		{
			this.prevTransition = uiradioBtn3D.prevTransition;
			if (Application.isPlaying)
			{
				this.Value = uiradioBtn3D.Value;
			}
		}
		if ((flags & ControlCopyFlags.Invocation) == ControlCopyFlags.Invocation)
		{
			this.scriptWithMethodToInvoke = uiradioBtn3D.scriptWithMethodToInvoke;
			this.methodToInvoke = uiradioBtn3D.methodToInvoke;
			this.whenToInvoke = uiradioBtn3D.whenToInvoke;
			this.delay = uiradioBtn3D.delay;
		}
		if ((flags & ControlCopyFlags.Sound) == ControlCopyFlags.Sound)
		{
			this.soundToPlay = uiradioBtn3D.soundToPlay;
		}
	}

	protected void PopOtherButtonsInGroup()
	{
		if (this.group == null)
		{
			return;
		}
		for (int i = 0; i < this.group.buttons.Count; i++)
		{
			if ((UIRadioBtn3D)this.group.buttons[i] != this)
			{
				((UIRadioBtn3D)this.group.buttons[i]).Value = false;
			}
		}
	}

	protected virtual void SetButtonState()
	{
		int num = (int)this.state;
		this.state = ((!this.controlIsEnabled) ? UIRadioBtn3D.CONTROL_STATE.Disabled : ((!this.btnValue) ? UIRadioBtn3D.CONTROL_STATE.False : UIRadioBtn3D.CONTROL_STATE.True));
		int num2 = (int)this.state;
		if (!base.gameObject.active)
		{
			this.stateChangeWhileDeactivated = true;
			return;
		}
		base.UseStateLabel(num2);
		if (this.prevTransition != null)
		{
			this.prevTransition.StopSafe();
		}
		this.StartTransition(num2, num);
	}

	protected void StartTransition(int newState, int prevState)
	{
		int num = 0;
		switch (newState)
		{
		case 0:
			if (prevState != 1)
			{
				if (prevState == 2)
				{
					num = 1;
				}
			}
			else
			{
				num = 0;
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
		}
		this.transitions[newState].list[num].Start();
		this.prevTransition = this.transitions[newState].list[num];
	}

	protected void DisableMe()
	{
		base.UseStateLabel(this.states.Length - 1);
		if (this.prevTransition != null)
		{
			this.prevTransition.StopSafe();
		}
		this.StartTransition(2, (int)this.state);
		this.state = UIRadioBtn3D.CONTROL_STATE.Disabled;
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

	public override void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
		this.scriptWithMethodToInvoke = gui.DrawScriptSelection(this.scriptWithMethodToInvoke, ref this.methodToInvoke);
	}

	public static UIRadioBtn3D Create(string name, Vector3 pos)
	{
		return (UIRadioBtn3D)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIRadioBtn3D));
	}

	public static UIRadioBtn3D Create(string name, Vector3 pos, Quaternion rotation)
	{
		return (UIRadioBtn3D)new GameObject(name)
		{
			transform = 
			{
				position = pos,
				rotation = rotation
			}
		}.AddComponent(typeof(UIRadioBtn3D));
	}

	string IRadioButton.name
	{
		get { return base.name; }
		set { base.name = value; }
	}

	private UIRadioBtn3D.CONTROL_STATE state;

	protected bool btnValue;

	public bool useParentForGrouping = true;

	public int radioGroup;

	protected RadioBtnGroup group;

	public bool defaultValue;

	protected bool stateChangeWhileDeactivated;

	protected string[] states = new string[] { "True", "False", "Disabled" };

	[HideInInspector]
	public EZTransitionList[] transitions = new EZTransitionList[]
	{
		new EZTransitionList(new EZTransition[]
		{
			new EZTransition("From False"),
			new EZTransition("From Disabled")
		}),
		new EZTransitionList(new EZTransition[]
		{
			new EZTransition("From True"),
			new EZTransition("From Disabled")
		}),
		new EZTransitionList(new EZTransition[]
		{
			new EZTransition("From True"),
			new EZTransition("From False")
		})
	};

	private EZTransition prevTransition;

	[HideInInspector]
	public string[] stateLabels = new string[] { "[\"]", "[\"]", "[\"]" };

	public MonoBehaviour scriptWithMethodToInvoke;

	public string methodToInvoke = string.Empty;

	public POINTER_INFO.INPUT_EVENT whenToInvoke = POINTER_INFO.INPUT_EVENT.TAP;

	public float delay;

	public AudioSource soundToPlay;

	protected enum CONTROL_STATE
	{
		True,
		False,
		Disabled
	}
}
