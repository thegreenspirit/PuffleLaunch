using System;
using UnityEngine;

public struct POINTER_INFO
{
	public void Copy(POINTER_INFO ptr)
	{
		this.type = ptr.type;
		this.camera = ptr.camera;
		this.id = ptr.id;
		this.actionID = ptr.actionID;
		this.evt = ptr.evt;
		this.active = ptr.active;
		this.devicePos = ptr.devicePos;
		this.origPos = ptr.origPos;
		this.inputDelta = ptr.inputDelta;
		this.ray = ptr.ray;
		this.prevRay = ptr.prevRay;
		this.rayDepth = ptr.rayDepth;
		this.isTap = ptr.isTap;
		this.targetObj = ptr.targetObj;
		this.layerMask = ptr.layerMask;
		this.hitInfo = ptr.hitInfo;
		this.activeTime = ptr.activeTime;
	}

	public void Reuse(POINTER_INFO ptr)
	{
		this.evt = ptr.evt;
		this.actionID = ptr.actionID;
		this.active = ptr.active;
		this.devicePos = ptr.devicePos;
		this.origPos = ptr.origPos;
		this.inputDelta = ptr.inputDelta;
		this.isTap = ptr.isTap;
		this.hitInfo = default(RaycastHit);
		this.activeTime = ptr.activeTime;
	}

	public void Reset(int actID)
	{
		this.actionID = actID;
		this.evt = POINTER_INFO.INPUT_EVENT.NO_CHANGE;
		this.active = false;
		this.devicePos = Vector3.zero;
		this.origPos = Vector3.zero;
		this.inputDelta = Vector3.zero;
		this.ray = default(Ray);
		this.prevRay = default(Ray);
		this.isTap = true;
		this.hitInfo = default(RaycastHit);
		this.activeTime = 0f;
	}

	public POINTER_INFO.POINTER_TYPE type;

	public Camera camera;

	public int id;

	public int actionID;

	public POINTER_INFO.INPUT_EVENT evt;

	public RaycastHit hitInfo;

	public bool active;

	public Vector3 devicePos;

	public Vector3 origPos;

	public Vector3 inputDelta;

	public bool isTap;

	public Ray ray;

	public Ray prevRay;

	public float rayDepth;

	public IUIObject targetObj;

	public int layerMask;

	public bool callerIsControl;

	public float activeTime;

	public enum INPUT_EVENT
	{
		NO_CHANGE,
		PRESS,
		RELEASE,
		TAP,
		MOVE,
		MOVE_OFF,
		RELEASE_OFF,
		DRAG
	}

	public enum POINTER_TYPE
	{
		MOUSE = 1,
		TOUCHPAD,
		MOUSE_TOUCHPAD,
		RAY
	}
}
