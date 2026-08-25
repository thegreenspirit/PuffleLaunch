using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class UIPanelBase : MonoBehaviour, IEZDragDrop, IUIContainer, IUIObject
{
	protected EZLinkedList<EZLinkedListNode<IUIObject>> uiObjs = new EZLinkedList<EZLinkedListNode<IUIObject>>();
	protected EZLinkedList<EZLinkedListNode<UIPanelBase>> childPanels = new EZLinkedList<EZLinkedListNode<UIPanelBase>>();

	[HideInInspector]
	public bool[] blockInput = new bool[] { true, true, true, true };

	protected EZTransition prevTransition;

	protected int prevTransIndex;
	protected bool m_started;
	public int index;
	public bool deactivateAllOnDismiss;
	public bool detargetOnDisable;

	protected Dictionary<int, GameObject> subjects = new Dictionary<int, GameObject>();
	protected UIPanelBase.TransitionCompleteDelegate tempTransCompleteDel;

	protected bool m_controlIsEnabled = true;

	protected IUIContainer container;

	protected EZInputDelegate inputDelegate;
	protected EZValueChangedDelegate changeDelegate;
	protected EZDragDropDelegate dragDropDelegate;

	public delegate void TransitionCompleteDelegate(UIPanelBase panel, EZTransition transition);

	public abstract EZTransitionList Transitions { get; }

	protected virtual void OnDisable()
	{
		if (Application.isPlaying)
		{
			if (EZAnimator.Exists())
			{
				EZAnimator.instance.Stop(base.gameObject);
			}
			if (this.detargetOnDisable && UIManager.Exists())
			{
				UIManager.instance.Detarget(this);
			}
		}
	}

	public virtual void Start()
	{
		if (this.m_started)
		{
			return;
		}
		this.m_started = true;
		this.ScanChildren();
		for (int i = 0; i < this.Transitions.list.Length; i++)
		{
			this.Transitions.list[i].MainSubject = base.gameObject;
		}
		this.SetupTransitionSubjects();
	}

	public void ScanChildren()
	{
		this.uiObjs.Clear();
		Component[] array = base.transform.GetComponentsInChildren(typeof(IUIObject), true);
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == this) && !(array[i].gameObject == base.gameObject))
			{
				if (base.gameObject.layer == UIManager.instance.gameObject.layer)
				{
					UIPanelManager.SetLayerRecursively(array[i].gameObject, base.gameObject.layer);
				}
				IUIObject iuiobject = (IUIObject)array[i];
				this.uiObjs.Add(new EZLinkedListNode<IUIObject>(iuiobject));
				iuiobject.RequestContainership(this);
			}
		}
		array = base.transform.GetComponentsInChildren(typeof(UIPanelBase), true);
		for (int j = 0; j < array.Length; j++)
		{
			if (!(array[j] == this) && !(array[j].gameObject == base.gameObject))
			{
				if (base.gameObject.layer == UIManager.instance.gameObject.layer)
				{
					UIPanelManager.SetLayerRecursively(array[j].gameObject, base.gameObject.layer);
				}
				UIPanelBase uipanelBase = (UIPanelBase)array[j];
				this.childPanels.Add(new EZLinkedListNode<UIPanelBase>(uipanelBase));
				uipanelBase.RequestContainership(this);
			}
		}
	}

	protected virtual void SetupTransitionSubjects()
	{
		for (int i = 0; i < 4; i++)
		{
			this.Transitions.list[i].AddTransitionEndDelegate(new EZTransition.OnTransitionEndDelegate(this.TransitionCompleted));
		}
		if (this.uiObjs.Rewind())
		{
			do
			{
				GameObject gameObject = ((Component)this.uiObjs.Current.val).gameObject;
				int num = gameObject.GetHashCode();
				for (int j = 0; j < this.Transitions.list.Length; j++)
				{
					this.Transitions.list[j].AddSubSubject(gameObject);
				}
				if (!this.subjects.ContainsKey(num))
				{
					this.subjects.Add(num, gameObject);
				}
			}
			while (this.uiObjs.MoveNext());
		}
		Component[] componentsInChildren = base.transform.GetComponentsInChildren(typeof(SpriteRoot), true);
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			if (!(componentsInChildren[k].gameObject == base.gameObject))
			{
				GameObject gameObject = componentsInChildren[k].gameObject;
				int num = gameObject.GetHashCode();
				if (!this.subjects.ContainsKey(num))
				{
					for (int l = 0; l < this.Transitions.list.Length; l++)
					{
						this.Transitions.list[l].AddSubSubject(gameObject);
					}
					this.subjects.Add(num, gameObject);
				}
			}
		}
		Component[] componentsInChildren2 = base.transform.GetComponentsInChildren(typeof(SpriteText), true);
		for (int m = 0; m < componentsInChildren2.Length; m++)
		{
			if (!(componentsInChildren2[m].gameObject == base.gameObject))
			{
				GameObject gameObject = componentsInChildren2[m].gameObject;
				int num = gameObject.GetHashCode();
				if (!this.subjects.ContainsKey(num))
				{
					for (int n = 0; n < this.Transitions.list.Length; n++)
					{
						this.Transitions.list[n].AddSubSubject(gameObject);
					}
					this.subjects.Add(num, gameObject);
				}
			}
		}
	}

	public void AddChild(GameObject go)
	{
		IUIObject iuiobject = (IUIObject)go.GetComponent("IUIObject");
		if (iuiobject != null)
		{
			if (iuiobject.Container != this)
			{
				iuiobject.Container = this;
			}
			this.uiObjs.Add(new EZLinkedListNode<IUIObject>(iuiobject));
		}
		else
		{
			UIPanelBase uipanelBase = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));
			if (uipanelBase != null)
			{
				if (uipanelBase.Container != this)
				{
					uipanelBase.Container = this;
				}
				this.childPanels.Add(new EZLinkedListNode<UIPanelBase>(uipanelBase));
			}
		}
		if (!base.gameObject.active)
		{
			go.SetActive(false);
		}
		this.AddSubject(go);
	}

	public void RemoveChild(GameObject go)
	{
		IUIObject iuiobject = (IUIObject)go.GetComponent("IUIObject");
		if (iuiobject != null)
		{
			if (this.uiObjs.Rewind())
			{
				while (this.uiObjs.Current.val != iuiobject)
				{
					if (!this.uiObjs.MoveNext())
					{
						goto IL_0068;
					}
				}
				this.uiObjs.Remove(this.uiObjs.Current);
			}
			IL_0068:
			if (iuiobject.Container == this)
			{
				iuiobject.Container = null;
			}
		}
		else
		{
			UIPanelBase uipanelBase = (UIPanelBase)go.GetComponent(typeof(UIPanelBase));
			if (uipanelBase != null)
			{
				if (this.childPanels.Rewind())
				{
					while (!(this.childPanels.Current.val == uipanelBase))
					{
						if (!this.childPanels.MoveNext())
						{
							goto IL_00F8;
						}
					}
					this.childPanels.Remove(this.childPanels.Current);
				}
				IL_00F8:
				if (uipanelBase.Container == this)
				{
					uipanelBase.Container = null;
				}
			}
		}
		this.RemoveSubject(go);
	}

	public void MakeChild(GameObject go)
	{
		this.AddChild(go);
		go.transform.parent = base.transform;
	}

	public void AddSubject(GameObject go)
	{
		int hashCode = go.GetHashCode();
		if (this.subjects.ContainsKey(hashCode))
		{
			return;
		}
		this.subjects.Add(hashCode, go);
		for (int i = 0; i < this.Transitions.list.Length; i++)
		{
			this.Transitions.list[i].AddSubSubject(go);
		}
		if (this.container != null)
		{
			this.container.AddSubject(go);
		}
	}

	public void RemoveSubject(GameObject go)
	{
		int hashCode = go.GetHashCode();
		if (!this.subjects.ContainsKey(hashCode))
		{
			return;
		}
		this.subjects.Remove(hashCode);
		for (int i = 0; i < this.Transitions.list.Length; i++)
		{
			this.Transitions.list[i].RemoveSubSubject(go);
		}
		if (this.container != null)
		{
			this.container.RemoveSubject(go);
		}
	}

	public string[] GetTransitionNames()
	{
		if (this.Transitions == null)
		{
			return null;
		}
		string[] array = new string[this.Transitions.list.Length];
		for (int i = 0; i < this.Transitions.list.Length; i++)
		{
			array[i] = this.Transitions.list[i].name;
		}
		return array;
	}

	public EZTransition GetTransition(int index)
	{
		if (this.Transitions == null)
		{
			return null;
		}
		if (this.Transitions.list == null)
		{
			return null;
		}
		if (this.Transitions.list.Length <= index || index < 0)
		{
			return null;
		}
		return this.Transitions.list[index];
	}

	public EZTransition GetTransition(UIPanelManager.SHOW_MODE transition)
	{
		return this.GetTransition((int)transition);
	}

	public EZTransition GetTransition(string transName)
	{
		if (this.Transitions == null)
		{
			return null;
		}
		if (this.Transitions.list == null)
		{
			return null;
		}
		EZTransition[] list = this.Transitions.list;
		for (int i = 0; i < list.Length; i++)
		{
			if (string.Equals(list[i].name, transName, StringComparison.CurrentCultureIgnoreCase))
			{
				return list[i];
			}
		}
		return null;
	}

	public virtual void StartTransition(UIPanelManager.SHOW_MODE mode)
	{
		if (!this.m_started)
		{
			this.Start();
		}
		if (this.prevTransition != null)
		{
			this.prevTransition.StopSafe();
		}
		this.prevTransIndex = (int)mode;
		if (this.blockInput[this.prevTransIndex])
		{
			UIManager.instance.LockInput();
		}
		this.prevTransition = this.Transitions.list[this.prevTransIndex];
		if (this.deactivateAllOnDismiss && (mode == UIPanelManager.SHOW_MODE.BringInBack || mode == UIPanelManager.SHOW_MODE.BringInForward))
		{
			base.gameObject.SetActive(true);
			this.Start();
		}
		this.prevTransition.Start();
	}

	public virtual void StartTransition(string transName)
	{
		if (!this.m_started)
		{
			this.Start();
		}
		EZTransition[] list = this.Transitions.list;
		for (int i = 0; i < list.Length; i++)
		{
			if (string.Equals(list[i].name, transName, StringComparison.CurrentCultureIgnoreCase))
			{
				if (this.prevTransition != null)
				{
					this.prevTransition.StopSafe();
				}
				this.prevTransIndex = i;
				if (this.blockInput[this.prevTransIndex])
				{
					UIManager.instance.LockInput();
				}
				this.prevTransition = list[this.prevTransIndex];
				if (this.deactivateAllOnDismiss && (this.prevTransition == list[1] || this.prevTransition == list[0]))
				{
					base.gameObject.SetActive(true);
					this.Start();
				}
				this.prevTransition.Start();
			}
		}
	}

	public void TransitionCompleted(EZTransition transition)
	{
		this.prevTransition = null;
		if (this.deactivateAllOnDismiss && (transition == this.Transitions.list[2] || transition == this.Transitions.list[3]))
		{
			base.gameObject.SetActive(false);
		}
		if (this.tempTransCompleteDel != null)
		{
			this.tempTransCompleteDel(this, transition);
		}
		this.tempTransCompleteDel = null;
		if (this.blockInput[this.prevTransIndex] && UIManager.Exists())
		{
			UIManager.instance.UnlockInput();
		}
	}

	public virtual void BringIn()
	{
		this.StartTransition(UIPanelManager.SHOW_MODE.BringInForward);
	}

	public virtual void Dismiss()
	{
		this.StartTransition(UIPanelManager.SHOW_MODE.DismissForward);
	}

	public static int CompareIndices(UIPanelBase a, UIPanelBase b)
	{
		return a.index - b.index;
	}

	public void AddTempTransitionDelegate(UIPanelBase.TransitionCompleteDelegate del)
	{
		this.tempTransCompleteDel = (UIPanelBase.TransitionCompleteDelegate)Delegate.Combine(this.tempTransCompleteDel, del);
	}

	public bool IsTransitioning
	{
		get
		{
			return this.prevTransition != null;
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
			return this.detargetOnDisable;
		}
		set
		{
			this.detargetOnDisable = value;
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
			IUIContainer iuicontainer = this.container;
			this.container = value;
			if (this.container != null)
			{
				foreach (KeyValuePair<int, GameObject> keyValuePair in this.subjects)
				{
					this.container.AddSubject(keyValuePair.Value);
				}
			}
			if (iuicontainer != null && iuicontainer != this.container)
			{
				foreach (KeyValuePair<int, GameObject> keyValuePair2 in this.subjects)
				{
					this.container.RemoveSubject(keyValuePair2.Value);
				}
			}
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
		if (this.Container != null)
		{
			ptr.callerIsControl = true;
			this.Container.OnInput(ptr);
		}
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
}
