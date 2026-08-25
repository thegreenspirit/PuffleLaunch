using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("EZ GUI/Management/Panel Manager")]
[Serializable]
public class UIPanelManager : MonoBehaviour, IEZDragDrop, IUIContainer, IUIObject
{
	public static UIPanelManager instance
	{
		get
		{
			return UIPanelManager.m_instance;
		}
	}

	public void OnDestroy()
	{
		UIPanelManager.m_instance = null;
	}

	public UIPanelBase CurrentPanel
	{
		get
		{
			return this.curPanel;
		}
	}

	public void AddChild(GameObject go)
	{
		UIPanelBase uipanelBase = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));
		if (uipanelBase == null)
		{
			return;
		}
		if (this.panels.IndexOf(uipanelBase) >= 0)
		{
			return;
		}
		this.panels.Add(uipanelBase);
		this.panels.Sort(new Comparison<UIPanelBase>(UIPanelBase.CompareIndices));
		uipanelBase.Container = this;
	}

	public void RemoveChild(GameObject go)
	{
		UIPanelBase uipanelBase = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));
		if (uipanelBase == null)
		{
			return;
		}
		this.panels.Remove(uipanelBase);
		uipanelBase.Container = null;
	}

	public void AddSubject(GameObject go)
	{
	}

	public void RemoveSubject(GameObject go)
	{
	}

	public void MakeChild(GameObject go)
	{
		this.AddChild(go);
		go.transform.parent = base.transform;
	}

	private void Awake()
	{
		if (UIPanelManager.m_instance == null)
		{
			UIPanelManager.m_instance = this;
		}
	}

	private IEnumerator Start()
	{
		if (this.m_started)
		{
			yield break;
		}
		this.m_started = true;
		this.ScanChildren();
		if (this.initialPanel != null)
		{
			this.curPanel = this.initialPanel;
			this.breadcrumbs.Add(this.curPanel);
		}
		if (this.circular)
		{
			this.linearNavigation = true;
		}
		if (this.deactivateAllButInitialAtStart)
		{
			yield return null;
			for (int i = 0; i < this.panels.Count; i++)
			{
				if (this.panels[i] != this.initialPanel && this.panels[i] != this.curPanel)
				{
					this.panels[i].gameObject.SetActive(false);
				}
			}
		}
		yield break;
	}

	protected virtual void OnEnable()
	{
		if (this.m_started && this.deactivateAllButInitialAtStart)
		{
			for (int i = 0; i < this.panels.Count; i++)
			{
				if (this.panels[i] != this.curPanel)
				{
					this.panels[i].gameObject.SetActive(false);
				}
			}
		}
	}

	public void ScanChildren()
	{
		this.panels.Clear();
		Component[] componentsInChildren = base.transform.GetComponentsInChildren(typeof(UIPanelBase), true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UIPanelManager.SetLayerRecursively(componentsInChildren[i].gameObject, base.gameObject.layer);
			UIPanelBase uipanelBase = (UIPanelBase)componentsInChildren[i];
			if (uipanelBase.RequestContainership(this))
			{
				this.panels.Add(uipanelBase);
			}
		}
		this.panels.Sort(new Comparison<UIPanelBase>(UIPanelBase.CompareIndices));
	}

	public int TransitioningPanelCount
	{
		get
		{
			return this.transitioningPanelCount;
		}
	}

	protected void DecrementTransitioningPanels(UIPanelBase p, EZTransition t)
	{
		this.transitioningPanelCount--;
	}

	protected void StartAndTrack(UIPanelBase p, UIPanelManager.SHOW_MODE mode)
	{
		p.StartTransition(mode);
		if (p.IsTransitioning)
		{
			p.AddTempTransitionDelegate(new UIPanelBase.TransitionCompleteDelegate(this.DecrementTransitioningPanels));
			this.transitioningPanelCount++;
		}
	}

	public bool MoveForward()
	{
		base.StartCoroutine("Start");
		int num = this.panels.IndexOf(this.curPanel);
		if (num >= this.panels.Count - 1)
		{
			if (!this.circular)
			{
				if (this.advancePastEnd)
				{
					if (this.curPanel != null)
					{
						this.StartAndTrack(this.curPanel, UIPanelManager.SHOW_MODE.DismissForward);
					}
					this.curPanel = null;
					if (this.breadcrumbs.Count > 0)
					{
						if (this.breadcrumbs[this.breadcrumbs.Count - 1] != null)
						{
							this.breadcrumbs.Add(null);
						}
					}
					else
					{
						this.breadcrumbs.Add(null);
					}
				}
				return false;
			}
			num = -1;
		}
		if (this.curPanel != null)
		{
			this.StartAndTrack(this.curPanel, UIPanelManager.SHOW_MODE.DismissForward);
		}
		num++;
		this.curPanel = this.panels[num];
		this.breadcrumbs.Add(this.curPanel);
		if (this.deactivateAllButInitialAtStart && !this.curPanel.gameObject.active)
		{
			this.curPanel.Start();
			this.curPanel.gameObject.SetActive(true);
		}
		this.StartAndTrack(this.curPanel, UIPanelManager.SHOW_MODE.BringInForward);
		return num < this.panels.Count - 1 || this.circular;
	}

	public bool MoveBack()
	{
		if (this.linearNavigation)
		{
			int num = this.panels.IndexOf(this.curPanel);
			if (num <= 0)
			{
				if (!this.circular)
				{
					if (this.advancePastEnd)
					{
						if (this.curPanel != null)
						{
							this.StartAndTrack(this.curPanel, UIPanelManager.SHOW_MODE.DismissBack);
						}
						this.curPanel = null;
					}
					return false;
				}
				num = this.panels.Count;
			}
			if (this.curPanel != null)
			{
				this.StartAndTrack(this.curPanel, UIPanelManager.SHOW_MODE.DismissBack);
			}
			num--;
			this.curPanel = this.panels[num];
			if (this.deactivateAllButInitialAtStart && !this.curPanel.gameObject.active)
			{
				this.curPanel.Start();
				this.curPanel.gameObject.SetActive(true);
			}
			this.StartAndTrack(this.curPanel, UIPanelManager.SHOW_MODE.BringInBack);
			return num > 0 || this.circular;
		}
		if (this.breadcrumbs.Count <= 1)
		{
			if (this.advancePastEnd)
			{
				if (this.curPanel != null)
				{
					this.StartAndTrack(this.curPanel, UIPanelManager.SHOW_MODE.DismissBack);
				}
				this.curPanel = null;
				if (this.breadcrumbs.Count > 0)
				{
					if (this.breadcrumbs[this.breadcrumbs.Count - 1] != null)
					{
						this.breadcrumbs.Add(null);
					}
				}
				else
				{
					this.breadcrumbs.Add(null);
				}
			}
			return false;
		}
		if (this.breadcrumbs.Count != 0)
		{
			this.breadcrumbs.RemoveAt(this.breadcrumbs.Count - 1);
		}
		if (this.curPanel != null)
		{
			this.StartAndTrack(this.curPanel, UIPanelManager.SHOW_MODE.DismissBack);
		}
		if (this.breadcrumbs.Count > 0)
		{
			this.curPanel = this.breadcrumbs[this.breadcrumbs.Count - 1];
		}
		if (this.curPanel != null)
		{
			if (this.deactivateAllButInitialAtStart && !this.curPanel.gameObject.active)
			{
				this.curPanel.Start();
				this.curPanel.gameObject.SetActive(true);
			}
			this.StartAndTrack(this.curPanel, UIPanelManager.SHOW_MODE.BringInBack);
		}
		return this.breadcrumbs.Count > 1;
	}

	public void BringIn(UIPanelBase panel, UIPanelManager.MENU_DIRECTION dir)
	{
		base.StartCoroutine("Start");
		if (this.curPanel == panel)
		{
			return;
		}
		if (dir == UIPanelManager.MENU_DIRECTION.Auto)
		{
			if (this.curPanel != null)
			{
				if (this.curPanel.index <= panel.index)
				{
					dir = UIPanelManager.MENU_DIRECTION.Forwards;
				}
				else
				{
					dir = UIPanelManager.MENU_DIRECTION.Backwards;
				}
			}
			else
			{
				dir = UIPanelManager.MENU_DIRECTION.Forwards;
			}
		}
		UIPanelManager.SHOW_MODE show_MODE = ((dir != UIPanelManager.MENU_DIRECTION.Forwards) ? UIPanelManager.SHOW_MODE.DismissBack : UIPanelManager.SHOW_MODE.DismissForward);
		UIPanelManager.SHOW_MODE show_MODE2 = ((dir != UIPanelManager.MENU_DIRECTION.Forwards) ? UIPanelManager.SHOW_MODE.BringInBack : UIPanelManager.SHOW_MODE.BringInForward);
		if (this.curPanel != null)
		{
			this.StartAndTrack(this.curPanel, show_MODE);
		}
		this.curPanel = panel;
		this.breadcrumbs.Add(this.curPanel);
		if (this.deactivateAllButInitialAtStart && !this.curPanel.gameObject.active)
		{
			this.curPanel.Start();
			this.curPanel.gameObject.SetActive(true);
		}
		this.StartAndTrack(this.curPanel, show_MODE2);
	}

	public void BringInImmediate(UIPanelBase panel, UIPanelManager.MENU_DIRECTION dir)
	{
		base.StartCoroutine("Start");
		UIPanelBase uipanelBase = this.curPanel;
		if (dir == UIPanelManager.MENU_DIRECTION.Auto)
		{
			if (this.curPanel != null)
			{
				if (this.curPanel.index <= panel.index)
				{
					dir = UIPanelManager.MENU_DIRECTION.Forwards;
				}
				else
				{
					dir = UIPanelManager.MENU_DIRECTION.Backwards;
				}
			}
			else
			{
				dir = UIPanelManager.MENU_DIRECTION.Forwards;
			}
		}
		UIPanelManager.SHOW_MODE show_MODE = ((dir != UIPanelManager.MENU_DIRECTION.Forwards) ? UIPanelManager.SHOW_MODE.DismissBack : UIPanelManager.SHOW_MODE.DismissForward);
		UIPanelManager.SHOW_MODE show_MODE2 = ((dir != UIPanelManager.MENU_DIRECTION.Forwards) ? UIPanelManager.SHOW_MODE.BringInBack : UIPanelManager.SHOW_MODE.BringInForward);
		this.BringIn(panel, dir);
		if (uipanelBase != null)
		{
			EZTransition eztransition = uipanelBase.GetTransition(show_MODE);
			eztransition.End();
		}
		if (this.curPanel != null)
		{
			EZTransition eztransition = this.curPanel.GetTransition(show_MODE2);
			eztransition.End();
		}
	}

	public void BringIn(string panelName, UIPanelManager.MENU_DIRECTION dir)
	{
		base.StartCoroutine("Start");
		UIPanelBase uipanelBase = null;
		for (int i = 0; i < this.panels.Count; i++)
		{
			if (string.Equals(this.panels[i].name, panelName, StringComparison.CurrentCultureIgnoreCase))
			{
				uipanelBase = this.panels[i];
				break;
			}
		}
		if (uipanelBase != null)
		{
			this.BringIn(uipanelBase, dir);
		}
	}

	public void BringIn(UIPanelBase panel)
	{
		this.BringIn(panel, UIPanelManager.MENU_DIRECTION.Auto);
	}

	public void BringIn(string panelName)
	{
		this.BringIn(panelName, UIPanelManager.MENU_DIRECTION.Auto);
	}

	public void BringIn(int panelIndex)
	{
		base.StartCoroutine("Start");
		for (int i = 0; i < this.panels.Count; i++)
		{
			if (this.panels[i].index == panelIndex)
			{
				this.BringIn(this.panels[i]);
				return;
			}
		}
		Debug.LogWarning("No panel found with index value of " + panelIndex);
	}

	public void BringIn(int panelIndex, UIPanelManager.MENU_DIRECTION dir)
	{
		base.StartCoroutine("Start");
		for (int i = 0; i < this.panels.Count; i++)
		{
			if (this.panels[i].index == panelIndex)
			{
				this.BringIn(this.panels[i], dir);
				return;
			}
		}
		Debug.LogWarning("No panel found with index value of " + panelIndex);
	}

	public void BringInImmediate(string panelName, UIPanelManager.MENU_DIRECTION dir)
	{
		base.StartCoroutine("Start");
		UIPanelBase uipanelBase = null;
		for (int i = 0; i < this.panels.Count; i++)
		{
			if (string.Equals(this.panels[i].name, panelName, StringComparison.CurrentCultureIgnoreCase))
			{
				uipanelBase = this.panels[i];
				break;
			}
		}
		if (uipanelBase != null)
		{
			this.BringInImmediate(uipanelBase, dir);
		}
	}

	public void BringInImmediate(UIPanelBase panel)
	{
		this.BringInImmediate(panel, UIPanelManager.MENU_DIRECTION.Auto);
	}

	public void BringInImmediate(string panelName)
	{
		this.BringInImmediate(panelName, UIPanelManager.MENU_DIRECTION.Auto);
	}

	public void BringInImmediate(int panelIndex)
	{
		base.StartCoroutine("Start");
		for (int i = 0; i < this.panels.Count; i++)
		{
			if (this.panels[i].index == panelIndex)
			{
				this.BringInImmediate(this.panels[i]);
				return;
			}
		}
		Debug.LogWarning("No panel found with index value of " + panelIndex);
	}

	public void BringInImmediate(int panelIndex, UIPanelManager.MENU_DIRECTION dir)
	{
		base.StartCoroutine("Start");
		for (int i = 0; i < this.panels.Count; i++)
		{
			if (this.panels[i].index == panelIndex)
			{
				this.BringInImmediate(this.panels[i], dir);
				return;
			}
		}
		Debug.LogWarning("No panel found with index value of " + panelIndex);
	}

	public void Dismiss(UIPanelManager.MENU_DIRECTION dir)
	{
		base.StartCoroutine("Start");
		if (dir == UIPanelManager.MENU_DIRECTION.Auto)
		{
			dir = UIPanelManager.MENU_DIRECTION.Backwards;
		}
		UIPanelManager.SHOW_MODE show_MODE = ((dir != UIPanelManager.MENU_DIRECTION.Forwards) ? UIPanelManager.SHOW_MODE.DismissBack : UIPanelManager.SHOW_MODE.DismissForward);
		if (this.curPanel != null)
		{
			this.StartAndTrack(this.curPanel, show_MODE);
		}
		this.curPanel = null;
		if (this.breadcrumbs.Count > 0 && this.breadcrumbs[this.breadcrumbs.Count - 1] != null)
		{
			this.breadcrumbs.Add(null);
		}
	}

	public void Dismiss()
	{
		this.Dismiss(UIPanelManager.MENU_DIRECTION.Auto);
	}

	public void DismissImmediate(UIPanelManager.MENU_DIRECTION dir)
	{
		base.StartCoroutine("Start");
		if (dir == UIPanelManager.MENU_DIRECTION.Auto)
		{
			dir = UIPanelManager.MENU_DIRECTION.Backwards;
		}
		UIPanelManager.SHOW_MODE show_MODE = ((dir != UIPanelManager.MENU_DIRECTION.Forwards) ? UIPanelManager.SHOW_MODE.DismissBack : UIPanelManager.SHOW_MODE.DismissForward);
		UIPanelBase uipanelBase = this.curPanel;
		this.Dismiss(dir);
		if (uipanelBase != null)
		{
			uipanelBase.GetTransition(show_MODE).End();
		}
	}

	public void DismissImmediate()
	{
		this.DismissImmediate(UIPanelManager.MENU_DIRECTION.Auto);
	}

	public static void SetLayerRecursively(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (object obj in go.transform)
		{
			Transform transform = (Transform)obj;
			UIPanelManager.SetLayerRecursively(transform.gameObject, layer);
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
			return false;
		}
		set
		{
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
				this.container = cont;
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

	public bool GotFocus()
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
	}

	public object Data
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public bool IsDraggable
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public LayerMask DropMask
	{
		get
		{
			return -1;
		}
		set
		{
		}
	}

	public float DragOffset
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public EZAnimation.EASING_TYPE CancelDragEasing
	{
		get
		{
			return EZAnimation.EASING_TYPE.Default;
		}
		set
		{
		}
	}

	public float CancelDragDuration
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public bool IsDragging
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public GameObject DropTarget
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public bool DropHandled
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public void DragUpdatePosition(POINTER_INFO ptr)
	{
	}

	public void CancelDrag()
	{
	}

	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
		if (this.dragDropDelegate != null)
		{
			this.dragDropDelegate(parms);
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

	public static UIPanelManager Create(string name, Vector3 pos)
	{
		return (UIPanelManager)new GameObject(name)
		{
			transform = 
			{
				position = pos
			}
		}.AddComponent(typeof(UIPanelManager));
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

	protected static UIPanelManager m_instance;

	protected List<UIPanelBase> panels = new List<UIPanelBase>();

	public UIPanelBase initialPanel;

	public bool deactivateAllButInitialAtStart;

	public bool linearNavigation;

	public bool circular;

	public bool advancePastEnd;

	protected UIPanelBase curPanel;

	protected int transitioningPanelCount;

	protected bool m_started;

	protected List<UIPanelBase> breadcrumbs = new List<UIPanelBase>();

	protected bool m_controlIsEnabled = true;

	protected IUIContainer container;

	protected EZInputDelegate inputDelegate;

	protected EZValueChangedDelegate changeDelegate;

	protected EZDragDropDelegate dragDropDelegate;

	public enum SHOW_MODE
	{
		BringInForward,
		BringInBack,
		DismissForward,
		DismissBack
	}

	public enum MENU_DIRECTION
	{
		Forwards,
		Backwards,
		Auto
	}
}
