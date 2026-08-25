using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("EZ GUI/Controls/Virtual Screen")]
[RequireComponent(typeof(MeshCollider))]
[Serializable]
public class UIVirtualScreen : MonoBehaviour, IEZDragDrop, IUIObject
{
	public virtual void Awake()
	{
		MeshCollider meshCollider = (MeshCollider)base.GetComponent(typeof(MeshCollider));
		if (meshCollider != null)
		{
			meshCollider.isTrigger = true;
		}
		else
		{
			Debug.LogError("The object \"" + base.name + "\" does not have the required MeshCollider attached.  Please add one, or else the screen functionality will not work.");
		}
		if (this.screenCamera == null)
		{
			this.screenCamera = Camera.main;
		}
		if (this.processPointerInfo)
		{
			this.SetupControls();
		}
	}

	public virtual IEnumerator Start()
	{
		yield return new WaitForEndOfFrame();
		if (this.onlyRenderWhenNeeded && this.screenCamera != null)
		{
			this.screenCamera.gameObject.active = false;
		}
		yield break;
	}

	protected void SetupControls()
	{
		for (int i = 0; i < this.controls.Count; i++)
		{
			this.controls[i].RemoveInputDelegate(new EZInputDelegate(this.InputProcessor));
		}
		this.controls.Clear();
		if (this.controlParent == null)
		{
			return;
		}
		foreach (IUIObject iuiobject in this.controlParent.GetComponentsInChildren(typeof(IUIObject), true))
		{
			this.controls.Add(iuiobject);
			iuiobject.AddInputDelegate(new EZInputDelegate(this.InputProcessor));
		}
	}

	public void AddControl(IUIObject obj)
	{
		if (obj == null)
		{
			return;
		}
		this.controls.Add(obj);
		obj.AddInputDelegate(new EZInputDelegate(this.InputProcessor));
	}

	public void RemoveControl(IUIObject obj)
	{
		this.controls.Remove(obj);
		obj.RemoveInputDelegate(new EZInputDelegate(this.InputProcessor));
	}

	public void SetScreenCamera(Camera cam)
	{
		this.screenCamera = cam;
	}

	public void SetControlParent(GameObject go)
	{
		this.controlParent = go;
		if (this.processPointerInfo)
		{
			this.SetupControls();
		}
	}

	protected void InputProcessor(ref POINTER_INFO ptr)
	{
		this.shuttingDown = false;
		base.StopAllCoroutines();
		ptr.devicePos = new Vector3(ptr.hitInfo.textureCoord.x * this.screenCamera.pixelWidth, ptr.hitInfo.textureCoord.y * this.screenCamera.pixelHeight);
		Vector3 devicePos = ptr.devicePos;
		float z = ptr.inputDelta.z;
		RaycastHit raycastHit;
		if (ptr.prevRay.direction.sqrMagnitude > 0f && base.GetComponent<Collider>().Raycast(ptr.prevRay, out raycastHit, ptr.rayDepth))
		{
			devicePos = new Vector3(raycastHit.textureCoord.x * this.screenCamera.pixelWidth, raycastHit.textureCoord.y * this.screenCamera.pixelHeight);
			ptr.inputDelta = ptr.devicePos - devicePos;
		}
		else
		{
			ptr.inputDelta = Vector3.zero;
		}
		ptr.inputDelta.z = z;
		ptr.ray = this.screenCamera.ScreenPointToRay(ptr.devicePos);
		ptr.prevRay = this.screenCamera.ScreenPointToRay(devicePos);
		ptr.camera = this.screenCamera;
		ptr.rayDepth = this.rayDepth;
		ptr.layerMask = this.layerMask;
		Physics.Raycast(ptr.ray, out ptr.hitInfo, this.rayDepth, this.layerMask);
		if (this.onlyRenderWhenNeeded)
		{
			if (ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE_OFF || ptr.evt == POINTER_INFO.INPUT_EVENT.MOVE_OFF)
			{
				base.StartCoroutine(this.DeactivateScreenCam(this.renderTimeout));
			}
			else if (ptr.evt == POINTER_INFO.INPUT_EVENT.TAP)
			{
				Component component = (Component)ptr.targetObj;
				if (!component.GetComponent<Collider>().Raycast(ptr.ray, out raycastHit, this.rayDepth))
				{
					base.StartCoroutine(this.DeactivateScreenCam(this.renderTimeout));
				}
			}
		}
	}

	protected IEnumerator DeactivateScreenCam(float timeout)
	{
		this.shuttingDown = true;
		if (this.renderTimeout == 0f)
		{
			yield return null;
		}
		else
		{
			yield return new WaitForSeconds(this.renderTimeout);
		}
		yield return new WaitForEndOfFrame();
		if (this.shuttingDown && this.screenCamera != null)
		{
			this.screenCamera.gameObject.active = false;
		}
		yield break;
	}

	public void RenderFrame()
	{
		if (this.screenCamera == null)
		{
			return;
		}
		if (this.screenCamera.gameObject.active)
		{
			return;
		}
		this.screenCamera.gameObject.active = true;
		this.DeactivateScreenCam(0f);
	}

	public void ForceOn()
	{
		if (this.screenCamera == null)
		{
			return;
		}
		base.gameObject.active = true;
		this.screenCamera.gameObject.active = true;
		this.shuttingDown = false;
		base.StopAllCoroutines();
		this.onlyRenderWhenNeeded = false;
	}

	public bool controlIsEnabled
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool DetargetOnDisable
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
		Vector2 vector = new Vector2(ptr.hitInfo.textureCoord.x * this.screenCamera.pixelWidth, ptr.hitInfo.textureCoord.y * this.screenCamera.pixelHeight);
		IUIObject iuiobject = null;
		bool flag = !this.screenCamera.gameObject.active;
		if (flag)
		{
			this.screenCamera.gameObject.active = true;
		}
		Ray ray = this.screenCamera.ScreenPointToRay(vector);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, this.rayDepth, this.layerMask))
		{
			iuiobject = (IUIObject)raycastHit.collider.gameObject.GetComponent("IUIObject");
		}
		if (this.onlyRenderWhenNeeded && iuiobject != null && iuiobject.controlIsEnabled)
		{
			this.shuttingDown = false;
			base.StopAllCoroutines();
			if (flag)
			{
				flag = false;
			}
		}
		if (flag)
		{
			this.screenCamera.gameObject.active = false;
		}
		return iuiobject;
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

	public bool GotFocus()
	{
		return false;
	}

	public void OnInput(POINTER_INFO ptr)
	{
	}

	public void SetInputDelegate(EZInputDelegate del)
	{
	}

	public void AddInputDelegate(EZInputDelegate del)
	{
	}

	public void RemoveInputDelegate(EZInputDelegate del)
	{
	}

	public void SetValueChangedDelegate(EZValueChangedDelegate del)
	{
	}

	public void AddValueChangedDelegate(EZValueChangedDelegate del)
	{
	}

	public void RemoveValueChangedDelegate(EZValueChangedDelegate del)
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
			return EZAnimation.EASING_TYPE.Linear;
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

	public void DragUpdatePosition(POINTER_INFO ptr)
	{
	}

	public void CancelDrag()
	{
	}

	public void OnEZDragDrop_Internal(EZDragDropParams parms)
	{
	}

	public void AddDragDropDelegate(EZDragDropDelegate del)
	{
	}

	public void RemoveDragDropDelegate(EZDragDropDelegate del)
	{
	}

	public void SetDragDropDelegate(EZDragDropDelegate del)
	{
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

	public Camera screenCamera;

	public LayerMask layerMask = -1;

	public float rayDepth = float.PositiveInfinity;

	public bool processPointerInfo = true;

	public GameObject controlParent;

	public bool onlyRenderWhenNeeded;

	public float renderTimeout;

	protected List<IUIObject> controls = new List<IUIObject>();

	protected bool shuttingDown;

	protected IUIContainer container;
}
