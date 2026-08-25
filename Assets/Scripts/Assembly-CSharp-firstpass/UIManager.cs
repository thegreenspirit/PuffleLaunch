using System;
using System.Text;
using UnityEngine;

[AddComponentMenu("EZ GUI/Management/UI Manager")]
public class UIManager : MonoBehaviour
{
	public static UIManager instance
	{
		get
		{
			if (UIManager.s_Instance == null)
			{
				UIManager uimanager = global::UnityEngine.Object.FindObjectOfType(typeof(UIManager)) as UIManager;
				if (uimanager != null)
				{
					uimanager.Awake();
				}
				UIManager.s_Instance = uimanager;
				if (UIManager.s_Instance == null && Application.isEditor)
				{
					Debug.LogError("Could not locate a UIManager object. You have to have exactly one UIManager in the scene.");
				}
			}
			return UIManager.s_Instance;
		}
	}

	public static bool Exists()
	{
		if (UIManager.s_Instance == null)
		{
			UIManager uimanager = global::UnityEngine.Object.FindObjectOfType(typeof(UIManager)) as UIManager;
			if (uimanager != null)
			{
				uimanager.Awake();
			}
			UIManager.s_Instance = uimanager;
		}
		return UIManager.s_Instance != null;
	}

	public void OnDestroy()
	{
		UIManager.s_Instance = null;
	}

	public virtual void Awake()
	{
		if (this.m_awake)
		{
			return;
		}
		this.m_awake = true;
		if (UIManager.s_Instance != null)
		{
			Debug.LogError("You can only have one instance of this singleton object in existence.");
		}
		else
		{
			UIManager.s_Instance = this;
		}
		if (this.pointerType == UIManager.POINTER_TYPE.AUTO_TOUCHPAD && !Application.isEditor)
		{
			this.pointerType = UIManager.POINTER_TYPE.TOUCHPAD;
		}
		if (this.pointerType == UIManager.POINTER_TYPE.TOUCHPAD || this.pointerType == UIManager.POINTER_TYPE.TOUCHPAD_AND_RAY)
		{
			// Green Spirit: change TouchScreenKeyboard to Screen
			Screen.autorotateToPortrait = this.autoRotateKeyboardPortrait;
			Screen.autorotateToPortraitUpsideDown = this.autoRotateKeyboardPortraitUpsideDown;
			Screen.autorotateToLandscapeLeft = this.autoRotateKeyboardLandscapeLeft;
			Screen.autorotateToLandscapeRight = this.autoRotateKeyboardLandscapeRight;

			if (SystemInfo.deviceModel == "iPad")
			{
				this.numTouches = 11;
			}
			else
			{
				this.numTouches = 5;
			}
		}
		else if (this.pointerType == UIManager.POINTER_TYPE.AUTO_TOUCHPAD)
		{
			this.numTouches = 12;
		}
		else if (this.pointerType == UIManager.POINTER_TYPE.MOUSE_AND_RAY)
		{
			this.numTouches = 1;
		}
		else
		{
			this.numTouches = 1;
		}
		if (this.pointerType == UIManager.POINTER_TYPE.AUTO_TOUCHPAD || this.pointerType == UIManager.POINTER_TYPE.MOUSE || this.pointerType == UIManager.POINTER_TYPE.MOUSE_AND_RAY)
		{
			this.numTouchPointers = this.numTouches - 1;
		}
		else
		{
			this.numTouchPointers = this.numTouches;
		}
		if (this.uiCameras.Length < 1)
		{
			this.uiCameras = new EZCameraSettings[1];
			this.uiCameras[0].camera = Camera.main;
		}
		else
		{
			for (int i = 0; i < this.uiCameras.Length; i++)
			{
				if (this.uiCameras[i].camera == null)
				{
					this.uiCameras[i].camera = Camera.main;
				}
			}
		}
		if (this.rayCamera == null)
		{
			this.rayCamera = this.uiCameras[0].camera;
		}
	}

	private void Start()
	{
		if (this.m_started)
		{
			return;
		}
		this.m_started = true;
		this.numPointers = this.numTouches;
		this.activePointers = new int[this.numTouches];
		this.usedPointers = new bool[this.numPointers];
		this.nonUIHits = new UIManager.NonUIHitInfo[this.numTouches];
		this.usedNonUIHits = new bool[this.numPointers];
		this.numNonUIHits = 0;
		this.SetupPointers();
	}

	protected void SetupPointers()
	{
		this.Start();
		this.pointers = new POINTER_INFO[this.uiCameras.Length, this.numTouches];
		try
		{
			if (this.raycastingTransform == null)
			{
				this.raycastingTransform = Camera.main.transform;
			}
			this.raycastingTransform = this.rayCamera.gameObject.transform;
		}
		catch
		{
			Debug.LogError("There appears to be no \"Main\" camera. Please tag one of your cameras with the \"MainCamera\" tag.");
		}
		switch (this.pointerType)
		{
		case UIManager.POINTER_TYPE.MOUSE:
		{
			this.pointerPoller = new UIManager.PointerPollerDelegate(this.PollMouse);
			this.activePointers[0] = 0;
			this.numActivePointers = 1;
			for (int i = 0; i < this.uiCameras.Length; i++)
			{
				this.pointers[i, 0].id = 0;
				this.pointers[i, 0].rayDepth = this.uiCameras[i].rayDepth;
				this.pointers[i, 0].layerMask = this.uiCameras[i].mask;
				this.pointers[i, 0].camera = this.uiCameras[i].camera;
				this.pointers[i, 0].type = POINTER_INFO.POINTER_TYPE.MOUSE;
			}
			break;
		}
		case UIManager.POINTER_TYPE.TOUCHPAD:
		{
			this.pointerPoller = new UIManager.PointerPollerDelegate(this.PollTouchpad);
			for (int j = 0; j < this.uiCameras.Length; j++)
			{
				for (int k = 0; k < this.numPointers; k++)
				{
					this.pointers[j, k].id = k;
					this.pointers[j, k].rayDepth = this.uiCameras[j].rayDepth;
					this.pointers[j, k].layerMask = this.uiCameras[j].mask;
					this.pointers[j, k].camera = this.uiCameras[j].camera;
					this.pointers[j, k].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
				}
			}
			break;
		}
		case UIManager.POINTER_TYPE.AUTO_TOUCHPAD:
		{
			this.pointerPoller = new UIManager.PointerPollerDelegate(this.PollMouseAndTouchpad);
			for (int l = 0; l < this.uiCameras.Length; l++)
			{
				for (int m = 0; m < this.numPointers; m++)
				{
					this.pointers[l, m].id = m;
					this.pointers[l, m].rayDepth = this.uiCameras[l].rayDepth;
					this.pointers[l, m].layerMask = this.uiCameras[l].mask;
					this.pointers[l, m].camera = this.uiCameras[l].camera;
					this.pointers[l, m].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
				}
				this.pointers[l, this.numPointers - 1].type = POINTER_INFO.POINTER_TYPE.MOUSE;
			}
			break;
		}
		case UIManager.POINTER_TYPE.RAY:
			this.pointerPoller = new UIManager.PointerPollerDelegate(this.PollRay);
			this.numActivePointers = 0;
			this.rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
			this.rayPtr.id = -1;
			this.rayPtr.rayDepth = this.rayDepth;
			this.rayPtr.layerMask = this.rayMask;
			this.rayPtr.camera = this.rayCamera;
			break;
		case UIManager.POINTER_TYPE.MOUSE_AND_RAY:
		{
			this.pointerPoller = new UIManager.PointerPollerDelegate(this.PollMouseRay);
			this.activePointers[0] = 0;
			this.numActivePointers = 1;
			for (int n = 0; n < this.uiCameras.Length; n++)
			{
				this.pointers[n, 0].id = 0;
				this.pointers[n, 0].rayDepth = this.uiCameras[n].rayDepth;
				this.pointers[n, 0].layerMask = this.uiCameras[n].mask;
				this.pointers[n, 0].camera = this.uiCameras[n].camera;
				this.pointers[n, 0].type = POINTER_INFO.POINTER_TYPE.MOUSE;
			}
			this.rayPtr.id = -1;
			this.rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
			this.rayPtr.rayDepth = this.rayDepth;
			this.rayPtr.layerMask = this.rayMask;
			this.rayPtr.camera = this.rayCamera;
			break;
		}
		case UIManager.POINTER_TYPE.TOUCHPAD_AND_RAY:
		{
			this.pointerPoller = new UIManager.PointerPollerDelegate(this.PollTouchpadRay);
			for (int num = 0; num < this.uiCameras.Length; num++)
			{
				for (int num2 = 0; num2 < this.numPointers; num2++)
				{
					this.pointers[num, num2].id = num2;
					this.pointers[num, num2].rayDepth = this.uiCameras[num].rayDepth;
					this.pointers[num, num2].layerMask = this.uiCameras[num].mask;
					this.pointers[num, num2].camera = this.uiCameras[num].camera;
					this.pointers[num, num2].type = POINTER_INFO.POINTER_TYPE.TOUCHPAD;
				}
			}
			this.rayPtr.id = -1;
			this.rayPtr.type = POINTER_INFO.POINTER_TYPE.RAY;
			this.rayPtr.rayDepth = this.rayDepth;
			this.rayPtr.layerMask = this.rayMask;
			this.rayPtr.camera = this.rayCamera;
			break;
		}
		default:
			Debug.LogError("ERROR: Invalid pointer type selected!");
			break;
		}
	}

	public void SetNonUIHitDelegate(UIManager.PointerInfoDelegate del)
	{
		this.informNonUIHit = del;
	}

	public void AddNonUIHitDelegate(UIManager.PointerInfoDelegate del)
	{
		this.informNonUIHit = (UIManager.PointerInfoDelegate)Delegate.Combine(this.informNonUIHit, del);
	}

	public void RemoveNonUIHitDelegate(UIManager.PointerInfoDelegate del)
	{
		this.informNonUIHit = (UIManager.PointerInfoDelegate)Delegate.Remove(this.informNonUIHit, del);
	}

	public void AddMouseTouchPtrListener(UIManager.PointerInfoDelegate del)
	{
		this.mouseTouchListeners = (UIManager.PointerInfoDelegate)Delegate.Combine(this.mouseTouchListeners, del);
	}

	public void AddRayPtrListener(UIManager.PointerInfoDelegate del)
	{
		this.rayListeners = (UIManager.PointerInfoDelegate)Delegate.Combine(this.rayListeners, del);
	}

	public void RemoveMouseTouchPtrListener(UIManager.PointerInfoDelegate del)
	{
		this.mouseTouchListeners = (UIManager.PointerInfoDelegate)Delegate.Remove(this.mouseTouchListeners, del);
	}

	public void RemoveRayPtrListener(UIManager.PointerInfoDelegate del)
	{
		this.rayListeners = (UIManager.PointerInfoDelegate)Delegate.Remove(this.rayListeners, del);
	}

	protected void AddNonUIHit(int ptrIndex, int camIndex)
	{
		if (this.informNonUIHit == null)
		{
			return;
		}
		if (camIndex == -1)
		{
			this.rayIsNonUIHit = true;
			return;
		}
		if (this.usedPointers[ptrIndex])
		{
			return;
		}
		if (this.usedNonUIHits[ptrIndex])
		{
			return;
		}
		this.usedNonUIHits[ptrIndex] = true;
		this.nonUIHits[this.numNonUIHits] = new UIManager.NonUIHitInfo(ptrIndex, camIndex);
		this.numNonUIHits++;
	}

	protected void CallNonUIHitDelegate()
	{
		if (this.informNonUIHit == null)
		{
			return;
		}
		for (int i = 0; i < this.numNonUIHits; i++)
		{
			UIManager.NonUIHitInfo nonUIHitInfo = this.nonUIHits[i];
			this.usedNonUIHits[nonUIHitInfo.ptrIndex] = false;
			if (!this.usedPointers[nonUIHitInfo.ptrIndex])
			{
				this.informNonUIHit(this.pointers[nonUIHitInfo.camIndex, nonUIHitInfo.ptrIndex]);
			}
		}
		if (this.rayIsNonUIHit)
		{
			this.informNonUIHit(this.rayPtr);
		}
	}

	public bool DidPointerHitUI(int id)
	{
		if (this.lastUpdateFrame != Time.frameCount)
		{
			this.Update();
		}
		if (id == -1)
		{
			return this.rayPtr.targetObj != null;
		}
		Mathf.Clamp(id, 0, this.usedPointers.Length - 1);
		return this.usedPointers[id];
	}

	public bool DidAnyPointerHitUI()
	{
		if (this.lastUpdateFrame != Time.frameCount)
		{
			this.Update();
		}
		if (this.rayPtr.targetObj != null)
		{
			return true;
		}
		for (int i = 0; i < this.usedPointers.Length; i++)
		{
			if (this.usedPointers[i])
			{
				return true;
			}
		}
		return false;
	}

	public void AddCamera(Camera cam, LayerMask mask, float depth, int index)
	{
		EZCameraSettings[] array = new EZCameraSettings[this.uiCameras.Length + 1];
		index = Mathf.Clamp(index, 0, this.uiCameras.Length + 1);
		int i = 0;
		int num = 0;
		while (i < array.Length)
		{
			if (i == index)
			{
				array[i] = new EZCameraSettings();
				array[i].camera = cam;
				array[i].mask = mask;
				array[i].rayDepth = depth;
			}
			else
			{
				array[i] = this.uiCameras[num++];
			}
			i++;
		}
		this.uiCameras = array;
		this.SetupPointers();
	}

	public void RemoveCamera(int index)
	{
		EZCameraSettings[] array = new EZCameraSettings[this.uiCameras.Length - 1];
		index = Mathf.Clamp(index, 0, this.uiCameras.Length);
		int num = 0;
		for (int i = 0; i < this.uiCameras.Length; i++)
		{
			if (i != index)
			{
				array[num] = this.uiCameras[i];
				num++;
			}
		}
		this.uiCameras = array;
		this.SetupPointers();
	}

	public void ReplaceCamera(int index, Camera cam)
	{
		index = Mathf.Clamp(index, 0, this.uiCameras.Length);
		this.uiCameras[index].camera = cam;
		this.SetupPointers();
	}

	public void OnLevelWasLoaded(int level)
	{
		for (int i = 0; i < this.uiCameras.Length; i++)
		{
			if (this.uiCameras[i].camera == null)
			{
				this.uiCameras[i].camera = Camera.main;
			}
		}
		if (this.rayCamera == null)
		{
			this.rayCamera = Camera.main;
		}
		if (this.focusObj == null)
		{
			this.FocusObject = null;
		}
		this.blockInput = false;
		this.inputLockCount = 0;
	}

	protected void BeginDrag(ref POINTER_INFO curPtr)
	{
		curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Begin, curPtr.targetObj, curPtr));
		curPtr.targetObj.DragUpdatePosition(curPtr);
	}

	protected void DoDragUpdate(POINTER_INFO curPtr)
	{
		IUIObject targetObj = curPtr.targetObj;
		targetObj.DragUpdatePosition(curPtr);
		RaycastHit[] array = Physics.RaycastAll(curPtr.ray, curPtr.rayDepth, curPtr.layerMask & targetObj.DropMask);
		if (array.Length == 0 || (array.Length == 1 && array[0].transform == targetObj.transform))
		{
			for (int i = 0; i < this.uiCameras.Length; i++)
			{
				if (!(this.uiCameras[i].camera == curPtr.camera))
				{
					POINTER_INFO pointer_INFO = this.pointers[i, curPtr.id];
					array = Physics.RaycastAll(pointer_INFO.ray, pointer_INFO.rayDepth, pointer_INFO.layerMask & targetObj.DropMask);
					if (array.Length != 0 && (array.Length != 1 || !(array[0].transform == targetObj.transform)))
					{
						break;
					}
				}
			}
		}
		RaycastHit raycastHit = default(RaycastHit);
		raycastHit.distance = float.PositiveInfinity;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].transform != targetObj.transform && array[j].distance < raycastHit.distance)
			{
				raycastHit = array[j];
			}
		}
		targetObj.DropTarget = ((!raycastHit.transform) ? null : raycastHit.transform.gameObject);
		POINTER_INFO.INPUT_EVENT evt = curPtr.evt;
		switch (evt)
		{
		case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			break;
		default:
			if (evt != POINTER_INFO.INPUT_EVENT.DRAG)
			{
				return;
			}
			break;
		case POINTER_INFO.INPUT_EVENT.RELEASE:
			curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Dropped, targetObj, curPtr));
			return;
		}
		curPtr.targetObj.OnEZDragDrop_Internal(new EZDragDropParams(EZDragDropEvent.Update, targetObj, curPtr));
	}

	public virtual void Update()
	{
		this.time = Time.realtimeSinceStartup;
		this.realtimeDelta = this.time - this.startTime;
		this.startTime = this.time;
		if (this.lastUpdateFrame != Time.frameCount)
		{
			this.lastUpdateFrame = Time.frameCount;
			this.pointerPoller();
			if (this.focusObj != null)
			{
				this.PollKeyboard();
			}
			this.DispatchInput();
			return;
		}
	}

	protected void DispatchInput()
	{
		this.numNonUIHits = 0;
		this.rayIsNonUIHit = false;
		for (int i = 0; i < this.usedPointers.Length; i++)
		{
			this.usedPointers[i] = false;
		}
		if (this.mouseTouchListeners != null)
		{
			for (int j = 0; j < this.numActivePointers; j++)
			{
				for (int k = 0; k < this.uiCameras.Length; k++)
				{
					if (this.uiCameras[k].camera.gameObject.active)
					{
						this.DispatchHelper(ref this.pointers[k, this.activePointers[j]], k);
						if (this.mouseTouchListeners != null)
						{
							this.mouseTouchListeners(this.pointers[k, this.activePointers[j]]);
						}
						if (this.usedPointers[this.activePointers[j]])
						{
							break;
						}
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < this.numActivePointers; l++)
			{
				for (int m = 0; m < this.uiCameras.Length; m++)
				{
					if (this.uiCameras[m].camera.gameObject.active)
					{
						this.DispatchHelper(ref this.pointers[m, this.activePointers[l]], m);
						if (this.usedPointers[this.activePointers[l]])
						{
							break;
						}
					}
				}
			}
		}
		if (this.pointerType == UIManager.POINTER_TYPE.RAY || this.pointerType == UIManager.POINTER_TYPE.MOUSE_AND_RAY || this.pointerType == UIManager.POINTER_TYPE.TOUCHPAD_AND_RAY)
		{
			this.DispatchHelper(ref this.rayPtr, -1);
			if (this.rayListeners != null)
			{
				this.rayListeners(this.rayPtr);
			}
		}
		this.CallNonUIHitDelegate();
	}

	protected void DispatchHelper(ref POINTER_INFO curPtr, int camIndex)
	{
		if (curPtr.targetObj != null && curPtr.targetObj.IsDragging)
		{
			this.DoDragUpdate(curPtr);
		}
		else
		{
			switch (curPtr.evt)
			{
			case POINTER_INFO.INPUT_EVENT.NO_CHANGE:
			case POINTER_INFO.INPUT_EVENT.MOVE:
				this.tempObj = null;
				if (Physics.Raycast(curPtr.ray, out this.hit, curPtr.rayDepth, curPtr.layerMask))
				{
					this.tempObj = (IUIObject)this.hit.collider.gameObject.GetComponent("IUIObject");
					curPtr.hitInfo = this.hit;
					if (this.tempObj != null)
					{
						this.tempObj = this.tempObj.GetControl(ref curPtr);
					}
					if (this.tempObj == null)
					{
						this.AddNonUIHit(curPtr.id, camIndex);
						if (this.warnOnNonUiHits)
						{
							this.LogNonUIObjErr(this.hit.collider.gameObject);
						}
					}
					if (!curPtr.active)
					{
						if (curPtr.targetObj != this.tempObj && curPtr.targetObj != null)
						{
							this.tempPtr.Copy(curPtr);
							this.tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
							if (!this.blockInput)
							{
								curPtr.targetObj.OnInput(this.tempPtr);
							}
						}
						if (!this.blockInput)
						{
							curPtr.targetObj = this.tempObj;
							if (this.tempObj != null)
							{
								curPtr.targetObj.OnInput(curPtr);
							}
						}
					}
					else if (curPtr.targetObj != null && !this.blockInput)
					{
						curPtr.targetObj.OnInput(curPtr);
					}
				}
				else
				{
					curPtr.hitInfo = default(RaycastHit);
					if (curPtr.targetObj != null && !curPtr.active)
					{
						curPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
						curPtr.targetObj.OnInput(curPtr);
					}
					if (!curPtr.active)
					{
						curPtr.targetObj = null;
					}
				}
				break;
			case POINTER_INFO.INPUT_EVENT.PRESS:
				if (Physics.Raycast(curPtr.ray, out this.hit, curPtr.rayDepth, curPtr.layerMask))
				{
					this.tempObj = (IUIObject)this.hit.collider.gameObject.GetComponent("IUIObject");
					curPtr.hitInfo = this.hit;
					if (this.tempObj != null)
					{
						this.tempObj = this.tempObj.GetControl(ref curPtr);
					}
					if (this.tempObj == null)
					{
						this.AddNonUIHit(curPtr.id, camIndex);
						if (this.warnOnNonUiHits)
						{
							this.LogNonUIObjErr(this.hit.collider.gameObject);
						}
					}
					if (this.tempObj != curPtr.targetObj && curPtr.targetObj != null)
					{
						this.tempPtr.Copy(curPtr);
						this.tempPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE_OFF;
						if (!this.blockInput)
						{
							curPtr.targetObj.OnInput(this.tempPtr);
						}
					}
					if (!this.blockInput)
					{
						curPtr.targetObj = this.tempObj;
					}
					else
					{
						if (curPtr.targetObj != null)
						{
							this.tempPtr.Copy(curPtr);
							this.tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
							curPtr.targetObj.OnInput(this.tempPtr);
						}
						curPtr.targetObj = null;
					}
					if (curPtr.targetObj != null)
					{
						if (!this.blockInput)
						{
							curPtr.targetObj.OnInput(curPtr);
						}
						if (curPtr.targetObj != this.focusObj && curPtr.type == POINTER_INFO.POINTER_TYPE.RAY == this.focusWithRay)
						{
							this.FocusObject = curPtr.targetObj;
						}
					}
					else if (curPtr.type == POINTER_INFO.POINTER_TYPE.RAY == this.focusWithRay)
					{
						this.FocusObject = null;
					}
				}
				else
				{
					curPtr.hitInfo = default(RaycastHit);
					if (this.blockInput && curPtr.targetObj != null)
					{
						this.tempPtr.Copy(curPtr);
						this.tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
						curPtr.targetObj.OnInput(this.tempPtr);
					}
					curPtr.targetObj = null;
					if (curPtr.type == POINTER_INFO.POINTER_TYPE.RAY == this.focusWithRay)
					{
						this.FocusObject = null;
					}
				}
				break;
			case POINTER_INFO.INPUT_EVENT.RELEASE:
			case POINTER_INFO.INPUT_EVENT.TAP:
			case POINTER_INFO.INPUT_EVENT.DRAG:
				if (curPtr.evt == POINTER_INFO.INPUT_EVENT.RELEASE || curPtr.evt == POINTER_INFO.INPUT_EVENT.TAP)
				{
					this.tempObj = null;
					if (Physics.Raycast(curPtr.ray, out this.hit, curPtr.rayDepth, curPtr.layerMask))
					{
						this.tempObj = (IUIObject)this.hit.collider.gameObject.GetComponent("IUIObject");
						curPtr.hitInfo = this.hit;
						if (this.tempObj != null)
						{
							this.tempObj = this.tempObj.GetControl(ref curPtr);
						}
						if (this.tempObj == null)
						{
							this.AddNonUIHit(curPtr.id, camIndex);
						}
					}
					else
					{
						curPtr.hitInfo = default(RaycastHit);
					}
					if (this.tempObj != curPtr.targetObj)
					{
						if (curPtr.targetObj != null)
						{
							this.tempPtr.Copy(curPtr);
							if (curPtr.evt == POINTER_INFO.INPUT_EVENT.RELEASE)
							{
								this.tempPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
							}
							else
							{
								this.tempPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
							}
							curPtr.targetObj.OnInput(this.tempPtr);
						}
						if (curPtr.id >= 0)
						{
							this.usedPointers[curPtr.id] = true;
						}
						if (!this.blockInput)
						{
							curPtr.targetObj = this.tempObj;
						}
						if (this.tempObj != null && curPtr.evt != POINTER_INFO.INPUT_EVENT.TAP && !this.blockInput)
						{
							this.tempObj.OnInput(curPtr);
						}
					}
					else if (curPtr.targetObj != null)
					{
						curPtr.targetObj.OnInput(curPtr);
						if (curPtr.id >= 0)
						{
							this.usedPointers[curPtr.id] = true;
						}
					}
					if (curPtr.type == POINTER_INFO.POINTER_TYPE.TOUCHPAD)
					{
						curPtr.targetObj = null;
					}
				}
				else
				{
					if (Physics.Raycast(curPtr.ray, out this.hit, curPtr.rayDepth, curPtr.layerMask))
					{
						curPtr.hitInfo = this.hit;
						if (curPtr.targetObj == null)
						{
							this.AddNonUIHit(curPtr.id, camIndex);
						}
					}
					else
					{
						curPtr.hitInfo = default(RaycastHit);
					}
					if (curPtr.targetObj != null && !this.blockInput)
					{
						curPtr.targetObj.OnInput(curPtr);
						if (curPtr.targetObj.IsDraggable && !curPtr.isTap && curPtr.targetObj.controlIsEnabled)
						{
							this.BeginDrag(ref curPtr);
						}
					}
				}
				break;
			}
		}
		if (curPtr.targetObj != null && curPtr.id >= 0)
		{
			this.usedPointers[curPtr.id] = true;
		}
	}

	protected void PollMouse()
	{
		this.PollMouse(ref this.pointers[0, 0]);
		for (int i = 1; i < this.uiCameras.Length; i++)
		{
			if (this.uiCameras[i].camera.gameObject.active)
			{
				this.pointers[i, 0].Reuse(this.pointers[0, 0]);
				this.pointers[i, 0].prevRay = this.pointers[i, 0].ray;
				this.pointers[i, 0].ray = this.uiCameras[i].camera.ScreenPointToRay(this.pointers[i, 0].devicePos);
			}
		}
	}

	protected void PollMouseAndTouchpad()
	{
		this.PollTouchpad();
	}

	protected void PollMouse(ref POINTER_INFO curPtr)
	{
		this.down = Input.GetMouseButton(0);
		float num = Input.GetAxis("Mouse ScrollWheel");
		num *= this.realtimeDelta;
		bool flag = num != 0f;
		if (this.down && curPtr.active)
		{
			if (Input.mousePosition != curPtr.devicePos)
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.DRAG;
				curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
				curPtr.devicePos = Input.mousePosition;
				if (curPtr.isTap)
				{
					this.tempVec = curPtr.origPos - curPtr.devicePos;
					if (Mathf.Abs(this.tempVec.x) > this.dragThreshold || Mathf.Abs(this.tempVec.y) > this.dragThreshold)
					{
						curPtr.isTap = false;
					}
				}
			}
			else
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				curPtr.inputDelta = Vector3.zero;
			}
		}
		else if (this.down && !curPtr.active)
		{
			curPtr.Reset(this.curActionID++);
			curPtr.evt = POINTER_INFO.INPUT_EVENT.PRESS;
			curPtr.active = true;
			curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
			curPtr.origPos = Input.mousePosition;
			curPtr.isTap = true;
			curPtr.activeTime = Time.time;
		}
		else if (!this.down && curPtr.active)
		{
			curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
			curPtr.devicePos = Input.mousePosition;
			if (curPtr.isTap)
			{
				this.tempVec = curPtr.origPos - curPtr.devicePos;
				if (Mathf.Abs(this.tempVec.x) > this.dragThreshold || Mathf.Abs(this.tempVec.y) > this.dragThreshold)
				{
					curPtr.isTap = false;
				}
			}
			if (curPtr.isTap)
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
			}
			else
			{
				curPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
			}
			curPtr.active = false;
			curPtr.activeTime = 0f;
		}
		else if (!this.down && Input.mousePosition != curPtr.devicePos)
		{
			curPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
			curPtr.inputDelta = Input.mousePosition - curPtr.devicePos;
			curPtr.devicePos = Input.mousePosition;
		}
		else
		{
			curPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
			curPtr.inputDelta = Vector3.zero;
		}
		if (flag)
		{
			curPtr.inputDelta.z = num;
		}
		curPtr.devicePos = Input.mousePosition;
		curPtr.prevRay = curPtr.ray;
		curPtr.ray = this.uiCameras[0].camera.ScreenPointToRay(curPtr.devicePos);
	}

	protected void PollTouchpad()
	{
		this.numActivePointers = Mathf.Min(this.numTouches, Input.touchCount);
		for (int i = 0; i < this.numActivePointers; i++)
		{
			Touch touch = Input.GetTouch(i);
			int num = touch.fingerId;
			if (num >= this.numTouchPointers)
			{
				num = this.numTouchPointers - 1;
			}
			this.activePointers[i] = num;
			switch (touch.phase)
			{
			case TouchPhase.Began:
				this.pointers[0, num].Reset(this.curActionID++);
				this.pointers[0, num].evt = POINTER_INFO.INPUT_EVENT.PRESS;
				this.pointers[0, num].active = true;
				this.pointers[0, num].inputDelta = Vector3.zero;
				this.pointers[0, num].origPos = touch.position;
				this.pointers[0, num].isTap = true;
				this.pointers[0, num].activeTime = Time.time;
				break;
			case TouchPhase.Moved:
				this.pointers[0, num].evt = POINTER_INFO.INPUT_EVENT.DRAG;
				this.pointers[0, num].inputDelta = touch.deltaPosition;
				this.pointers[0, num].devicePos = touch.position;
				if (this.pointers[0, num].isTap)
				{
					this.tempVec = this.pointers[0, num].origPos - this.pointers[0, num].devicePos;
					if (Mathf.Abs(this.tempVec.x) > this.dragThreshold || Mathf.Abs(this.tempVec.y) > this.dragThreshold)
					{
						this.pointers[0, num].isTap = false;
					}
				}
				break;
			case TouchPhase.Stationary:
				this.pointers[0, num].evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				this.pointers[0, num].inputDelta = Vector3.zero;
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				if (this.pointers[0, num].isTap)
				{
					this.pointers[0, num].evt = POINTER_INFO.INPUT_EVENT.TAP;
				}
				else
				{
					this.pointers[0, num].evt = POINTER_INFO.INPUT_EVENT.RELEASE;
				}
				this.pointers[0, num].inputDelta = touch.deltaPosition;
				this.pointers[0, num].active = false;
				this.pointers[0, num].activeTime = 0f;
				break;
			}
			this.pointers[0, num].devicePos = touch.position;
			this.pointers[0, num].prevRay = this.pointers[0, num].ray;
			this.pointers[0, num].ray = this.uiCameras[0].camera.ScreenPointToRay(this.pointers[0, num].devicePos);
		}
		for (int j = 1; j < this.uiCameras.Length; j++)
		{
			for (int k = 0; k < this.numActivePointers; k++)
			{
				int num2 = this.activePointers[k];
				this.pointers[j, num2].Reuse(this.pointers[0, num2]);
				this.pointers[j, num2].prevRay = this.pointers[j, num2].ray;
				this.pointers[j, num2].ray = this.uiCameras[j].camera.ScreenPointToRay(this.pointers[j, num2].devicePos);
			}
		}
	}

	protected void PollRay()
	{
		if (this.actionAxis.Length != 0)
		{
			this.rayActive = Input.GetButton(this.actionAxis);
		}
		else
		{
			this.rayActive = this.rayState != UIManager.RAY_ACTIVE_STATE.Inactive;
			if (this.rayState == UIManager.RAY_ACTIVE_STATE.Momentary)
			{
				this.rayState = UIManager.RAY_ACTIVE_STATE.Inactive;
			}
		}
		if (this.rayActive && this.rayPtr.active)
		{
			if (this.raycastingTransform.forward != this.rayPtr.ray.direction || this.raycastingTransform.position != this.rayPtr.ray.origin)
			{
				this.rayPtr.evt = POINTER_INFO.INPUT_EVENT.DRAG;
				this.tempVec = this.raycastingTransform.position + this.raycastingTransform.forward * this.rayDepth;
				this.rayPtr.inputDelta = this.tempVec - this.rayPtr.devicePos;
				this.rayPtr.devicePos = this.tempVec;
				if (this.rayPtr.isTap)
				{
					this.tempVec = this.rayPtr.origPos - this.rayPtr.devicePos;
					if (this.tempVec.sqrMagnitude > this.rayDragThreshold * this.rayDragThreshold)
					{
						this.rayPtr.isTap = false;
					}
				}
			}
			else
			{
				this.rayPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
				this.rayPtr.inputDelta = Vector3.zero;
			}
		}
		else if (this.rayActive && !this.rayPtr.active)
		{
			this.rayPtr.Reset(this.curActionID++);
			this.rayPtr.evt = POINTER_INFO.INPUT_EVENT.PRESS;
			this.rayPtr.active = true;
			this.rayPtr.origPos = this.raycastingTransform.position + this.raycastingTransform.forward * this.rayDepth;
			this.rayPtr.inputDelta = this.rayPtr.origPos - this.rayPtr.devicePos;
			this.rayPtr.devicePos = this.rayPtr.origPos;
			this.rayPtr.isTap = true;
			this.rayPtr.activeTime = Time.time;
		}
		else if (!this.rayActive && this.rayPtr.active)
		{
			if (this.rayPtr.isTap)
			{
				this.rayPtr.evt = POINTER_INFO.INPUT_EVENT.TAP;
			}
			else
			{
				this.rayPtr.evt = POINTER_INFO.INPUT_EVENT.RELEASE;
			}
			this.tempVec = this.raycastingTransform.position + this.raycastingTransform.forward * this.rayDepth;
			this.rayPtr.inputDelta = this.tempVec - this.rayPtr.devicePos;
			this.rayPtr.devicePos = this.tempVec;
			this.rayPtr.active = false;
			this.rayPtr.activeTime = 0f;
		}
		else if (!this.rayActive && Input.mousePosition != this.rayPtr.devicePos)
		{
			this.rayPtr.evt = POINTER_INFO.INPUT_EVENT.MOVE;
			this.tempVec = this.raycastingTransform.position + this.raycastingTransform.forward * this.rayDepth;
			this.rayPtr.inputDelta = this.tempVec - this.rayPtr.devicePos;
			this.rayPtr.devicePos = this.tempVec;
		}
		else
		{
			this.rayPtr.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
			this.rayPtr.inputDelta = Vector3.zero;
		}
		this.rayPtr.prevRay = this.rayPtr.ray;
		this.rayPtr.ray = new Ray(this.raycastingTransform.position, this.raycastingTransform.forward);
	}

	protected void PollMouseRay()
	{
		this.PollMouse();
		this.PollRay();
	}

	protected void PollTouchpadRay()
	{
		this.PollTouchpad();
		this.PollRay();
	}

	protected void PollKeyboard()
	{
		if (!Application.isEditor)
		{
			if (this.iKeyboard == null)
			{
				return;
			}
			if (this.iKeyboard.done)
			{
				this.controlText = this.iKeyboard.text;
				this.controlText = ((IKeyFocusable)this.focusObj).SetInputText(this.controlText, ref this.insert);
				((IKeyFocusable)this.focusObj).Commit();
				this.FocusObject = null;
				return;
			}
			if (this.controlText == this.iKeyboard.text)
			{
				return;
			}
			string text = this.controlText;
			this.controlText = this.iKeyboard.text;
			this.insert = UIManager.FindInsertionPoint(text, this.controlText);
			((IKeyFocusable)this.focusObj).SetInputText(this.controlText, ref this.insert);
		}
		else
		{
			this.ProcessKeyboard();
		}
	}

	protected void ProcessKeyboard()
	{
		if (Input.inputString.Length == 0 && !Input.GetKeyDown(KeyCode.Delete))
		{
			return;
		}
		this.controlText = ((IKeyFocusable)this.focusObj).Content;
		this.insert = Mathf.Clamp(this.insert, 0, this.controlText.Length);
		this.sb.Length = 0;
		this.sb.Append(this.controlText);
		foreach (char c in Input.inputString)
		{
			if (c == '\b')
			{
				this.insert = Mathf.Max(0, this.insert - 1);
				if (this.insert < this.sb.Length)
				{
					this.sb.Remove(this.insert, 1);
				}
			}
			else
			{
				this.sb.Insert(this.insert, c);
				this.insert++;
			}
		}
		if (Input.GetKeyDown(KeyCode.Delete) && this.insert < this.sb.Length)
		{
			this.sb.Remove(this.insert, 1);
		}
		this.controlText = this.sb.ToString();
		this.controlText = ((IKeyFocusable)this.focusObj).SetInputText(this.controlText, ref this.insert);
	}

	public UIManager.RAY_ACTIVE_STATE RayActive
	{
		get
		{
			return this.rayState;
		}
		set
		{
			this.rayState = value;
		}
	}

	public void Detarget(IUIObject obj)
	{
		this.Retarget(obj, null);
	}

	public void Detarget(int pointerID)
	{
		if (this.uiCameras == null)
		{
			return;
		}
		for (int i = 0; i < this.uiCameras.Length; i++)
		{
			if (this.uiCameras[i].camera != null && this.uiCameras[i].camera.gameObject.active && this.pointers[i, pointerID].targetObj != null)
			{
				POINTER_INFO pointer_INFO = default(POINTER_INFO);
				pointer_INFO.Copy(this.pointers[i, pointerID]);
				pointer_INFO.isTap = false;
				pointer_INFO.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
				this.pointers[i, pointerID].targetObj.OnInput(pointer_INFO);
				this.pointers[i, pointerID].targetObj = null;
			}
		}
	}

	public void DetargetAllExcept(int pointerID)
	{
		for (int i = 0; i < this.numActivePointers; i++)
		{
			if (this.activePointers[i] != pointerID)
			{
				for (int j = 0; j < this.uiCameras.Length; j++)
				{
					if (this.uiCameras[j].camera != null && this.uiCameras[j].camera.gameObject.active && this.pointers[j, this.activePointers[i]].targetObj != null)
					{
						POINTER_INFO pointer_INFO = default(POINTER_INFO);
						pointer_INFO.Copy(this.pointers[j, pointerID]);
						pointer_INFO.isTap = false;
						pointer_INFO.evt = POINTER_INFO.INPUT_EVENT.RELEASE_OFF;
						this.pointers[j, pointerID].targetObj.OnInput(pointer_INFO);
						this.pointers[j, this.activePointers[i]].targetObj = null;
					}
				}
			}
		}
	}

	public void Retarget(IUIObject oldObj, IUIObject newObj)
	{
		if (this.uiCameras == null)
		{
			return;
		}
		for (int i = 0; i < this.numActivePointers; i++)
		{
			for (int j = 0; j < this.uiCameras.Length; j++)
			{
				if (this.uiCameras[j].camera != null && this.uiCameras[j].camera.gameObject.active && this.pointers[j, this.activePointers[i]].targetObj != null && this.pointers[j, this.activePointers[i]].targetObj == oldObj)
				{
					this.pointers[j, this.activePointers[i]].targetObj = newObj;
				}
			}
		}
		if (this.rayPtr.targetObj == oldObj)
		{
			this.rayPtr.targetObj = newObj;
		}
	}

	public bool GetPointer(IUIObject obj, out POINTER_INFO ptr)
	{
		if (this.uiCameras == null)
		{
			ptr = default(POINTER_INFO);
			return false;
		}
		for (int i = 0; i < this.numActivePointers; i++)
		{
			int j = 0;
			while (j < this.uiCameras.Length)
			{
				if (this.uiCameras[j].camera != null && this.uiCameras[j].camera.gameObject.active && this.pointers[j, this.activePointers[i]].targetObj != null)
				{
					if (this.pointers[j, this.activePointers[i]].targetObj == obj)
					{
						ptr = this.pointers[j, this.activePointers[i]];
						return true;
					}
					break;
				}
				else
				{
					j++;
				}
			}
		}
		if (this.rayPtr.targetObj == obj)
		{
			ptr = this.rayPtr;
			return true;
		}
		ptr = default(POINTER_INFO);
		return false;
	}

	public void LockInput()
	{
		this.blockInput = true;
		this.inputLockCount++;
	}

	public void UnlockInput()
	{
		this.inputLockCount--;
		if (this.inputLockCount < 1)
		{
			this.inputLockCount = 0;
			this.blockInput = false;
		}
	}

	public IUIObject FocusObject
	{
		get
		{
			return this.focusObj;
		}
		set
		{
			IUIObject iuiobject;
			if (value != null && value.GotFocus())
			{
				iuiobject = value;
			}
			else
			{
				iuiobject = null;
			}
			if (this.focusObj != null && this.focusObj is IKeyFocusable)
			{
				((IKeyFocusable)this.focusObj).LostFocus();
			}
			this.focusObj = iuiobject;
			if (this.focusObj != null)
			{
				this.controlText = ((IKeyFocusable)this.focusObj).GetInputText(ref this.kbInfo);
				if (this.controlText == null)
				{
					this.controlText = string.Empty;
				}
				if (!Application.isEditor)
				{
					this.iKeyboard = TouchScreenKeyboard.Open(this.controlText, this.kbInfo.type, this.kbInfo.autoCorrect, this.kbInfo.multiline, this.kbInfo.secure, this.kbInfo.alert, this.controlText);
					TouchScreenKeyboard.hideInput = this.kbInfo.hideInput;
					this.iKeyboard.text = this.controlText;
				}
				this.insert = this.kbInfo.insert;
				if (this.sb.Length > 0)
				{
					this.sb.Replace(this.sb.ToString(), this.controlText);
				}
				else
				{
					this.sb.Append(this.controlText);
				}
			}
			else if (this.iKeyboard != null)
			{
				this.iKeyboard.active = false;
				this.iKeyboard = null;
			}
		}
	}

	public int InsertionPoint
	{
		get
		{
			return this.insert;
		}
		set
		{
			this.insert = value;
		}
	}

	public int PointerCount
	{
		get
		{
			return this.numPointers;
		}
	}

	protected static int FindInsertionPoint(string before, string after)
	{
		if (before == null || after == null)
		{
			return 0;
		}
		int num = 0;
		while (num < before.Length && num < after.Length)
		{
			if (before[num] != after[num])
			{
				return num + 1;
			}
			num++;
		}
		return after.Length;
	}

	protected void LogNonUIObjErr(GameObject obj)
	{
		Debug.LogWarning("The UIManager encountered a collider on object \"" + obj.name + "\" that does not not contain an IUIObject or derivative component.  Please double-check that this object has the correct layer and components assigned.");
	}

	private static UIManager s_Instance;

	public UIManager.POINTER_TYPE pointerType = UIManager.POINTER_TYPE.AUTO_TOUCHPAD;

	public float dragThreshold = 8f;

	public float rayDragThreshold = 2f;

	public float rayDepth = float.PositiveInfinity;

	public LayerMask rayMask = -1;

	public bool focusWithRay;

	public string actionAxis = "Fire1";

	public UIManager.OUTSIDE_VIEWPORT inputOutsideViewport = UIManager.OUTSIDE_VIEWPORT.Move_Off;

	public bool warnOnNonUiHits = true;

	protected Transform raycastingTransform;

	public EZCameraSettings[] uiCameras = new EZCameraSettings[1];

	public Camera rayCamera;

	public bool blockInput;

	public float defaultDragOffset = 1f;

	public EZAnimation.EASING_TYPE cancelDragEasing = EZAnimation.EASING_TYPE.ExponentialOut;

	public float cancelDragDuration = 1f;

	public TextAsset defaultFont;

	public Material defaultFontMaterial;

	public bool autoRotateKeyboardPortrait = true;

	public bool autoRotateKeyboardPortraitUpsideDown = true;

	public bool autoRotateKeyboardLandscapeLeft = true;

	public bool autoRotateKeyboardLandscapeRight = true;

	protected bool rayActive;

	protected UIManager.RAY_ACTIVE_STATE rayState;

	protected POINTER_INFO[,] pointers;

	protected UIManager.NonUIHitInfo[] nonUIHits;

	protected bool[] usedPointers;

	protected bool[] usedNonUIHits;

	protected bool rayIsNonUIHit;

	protected int numPointers;

	protected int numTouchPointers;

	protected int[] activePointers;

	protected int numActivePointers;

	protected int numNonUIHits;

	protected POINTER_INFO rayPtr;

	protected UIManager.PointerPollerDelegate pointerPoller;

	protected UIManager.PointerInfoDelegate informNonUIHit;

	protected UIManager.PointerInfoDelegate mouseTouchListeners;

	protected UIManager.PointerInfoDelegate rayListeners;

	protected IUIObject focusObj;

	protected string controlText;

	protected int insert;

	private KEYBOARD_INFO kbInfo = default(KEYBOARD_INFO);

	protected int inputLockCount;

	protected bool m_started;

	protected bool m_awake;

	private float time;

	private float startTime;

	private float realtimeDelta;

	private int lastUpdateFrame;

	private int curActionID;

	private int numTouches;

	protected RaycastHit hit;

	protected Vector3 tempVec;

	private bool down;

	private IUIObject tempObj;

	private POINTER_INFO tempPtr;

	private StringBuilder sb = new StringBuilder();

	private TouchScreenKeyboard iKeyboard;

	public enum POINTER_TYPE
	{
		MOUSE,
		TOUCHPAD,
		AUTO_TOUCHPAD,
		RAY,
		MOUSE_AND_RAY,
		TOUCHPAD_AND_RAY
	}

	public enum RAY_ACTIVE_STATE
	{
		Inactive,
		Momentary,
		Constant
	}

	public enum OUTSIDE_VIEWPORT
	{
		Process_All,
		Ignore,
		Move_Off
	}

	public struct NonUIHitInfo
	{
		public NonUIHitInfo(int pIndex, int cIndex)
		{
			this.ptrIndex = pIndex;
			this.camIndex = cIndex;
		}

		public int ptrIndex;

		public int camIndex;
	}

	public delegate void PointerPollerDelegate();

	public delegate void PointerInfoDelegate(POINTER_INFO ptr);
}
