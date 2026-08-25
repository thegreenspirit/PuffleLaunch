using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
	public void Awake()
	{
		this.mTransform = base.transform;
		this.mCamera = base.GetComponent<Camera>();
		this.mOriginalOrthographicSize = this.mCamera.orthographicSize;
	}

	public void Start()
	{
		this.mTargetPosition = this.mTransform.position;
		this.mCamera.orthographicSize = (this.mTargetSize = this.defaultSize);
		this.mZoomOverride = false;
	}

	public void Update()
	{
		if (!this.mZoomOverride)
		{
			this.HandlePinchZoom();
		}
	}

	public void LateUpdate()
	{
	}

	public void FixedUpdate()
	{
		this.UpdateZoom(TimeManager.Instance.DeltaTime);
		if (this.mTarget != null)
		{
			Vector3 vector = this.mTarget.position - this.mTargetPosition;
			float num = Mathf.Abs(vector.x) - this.deadZone.width * 0.5f;
			float num2 = Mathf.Abs(vector.y) - this.deadZone.height * 0.5f;
			if (num > 0f)
			{
				this.mTargetPosition.x = this.mTargetPosition.x + ((vector.x <= 0f) ? (-num) : num);
			}
			if (num2 > 0f)
			{
				this.mTargetPosition.y = this.mTargetPosition.y + ((vector.y <= 0f) ? (-num2) : num2);
			}
			this.UpdateTransform(TimeManager.Instance.DeltaTime);
		}
	}

	public void UpdateTransform(float aDeltaTime)
	{
		Vector3 vector = this.mTargetPosition - this.mTransform.position;
		float magnitude = (vector * this.moveSpeed * aDeltaTime).magnitude;
		float magnitude2 = vector.magnitude;
		float num = Mathf.Min(magnitude2, magnitude);
		this.mTransform.position += vector.normalized * num;
	}

	private void UpdateZoom(float aDeltaTime)
	{
		float num = this.mTargetSize - this.mCamera.orthographicSize;
		this.mCamera.orthographicSize += num * this.zoomSpeed * aDeltaTime;
	}

	private void HandlePinchZoom()
	{
		if (this.ZoomEnabled && GameFlowManager.Instance.InputController.Zoom)
		{
			Debug.Log("zoom distance: " + GameFlowManager.Instance.InputController.ZoomDistance);
			this.mTargetSize = this.mCamera.orthographicSize - GameFlowManager.Instance.InputController.ZoomDistance * this.zoomScaleFactor;
			this.mTargetSize = Mathf.Clamp(this.mTargetSize, this.minimumSize, this.maximumSize);
			Debug.Log("ortho size: " + this.mCamera.orthographicSize);
		}
	}

	public float OriginalOrthographicSize
	{
		get
		{
			return this.mOriginalOrthographicSize;
		}
	}

	public Vector3 TargetPosition
	{
		get
		{
			return this.mTargetPosition;
		}
		set
		{
			this.mTargetPosition = value;
		}
	}

	public float TargetSize
	{
		get
		{
			return this.mTargetSize;
		}
		set
		{
			this.mTargetSize = value;
		}
	}

	public bool ZoomEnabled
	{
		get
		{
			return this.mEnabled;
		}
		set
		{
			this.mEnabled = value;
		}
	}

	public Transform Target
	{
		get
		{
			return this.mTarget;
		}
		set
		{
			this.mTarget = value;
		}
	}

	public bool ZoomOverride
	{
		get
		{
			return this.mZoomOverride;
		}
		set
		{
			this.mZoomOverride = value;
		}
	}

	public Rect deadZone;

	public float minimumSize;

	public float maximumSize;

	public float defaultSize;

	public float moveSpeed;

	public float zoomSpeed;

	public float zoomScaleFactor = 0.05f;

	private Transform mTransform;

	private Transform mTarget;

	private Camera mCamera;

	private float mOriginalOrthographicSize;

	private Vector3 mTargetPosition;

	private float mTargetSize;

	private bool mZoomOverride;

	private bool mEnabled = true;
}
