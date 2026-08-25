using System;
using UnityEngine;

public class TouchIndicatorController : MonoBehaviour
{
	private void Awake()
	{
		this.mRenderer = base.GetComponent<MeshRenderer>();
		this.mCamera = Camera.main;
	}

	private void Start()
	{
		this.mTransform = base.transform;
		this.mBaseScale = this.mTransform.localScale;
		this.mBaseOrthographicSize = this.mCamera.orthographicSize;
	}

	private void Update()
	{
		if (GameManager.Instance.IsPause() || GameManager.Instance.DuringCutscene || GameFlowManager.Instance.GUIManager.CurrentScene != GUIManager.Scene.eInGameHud)
		{
			this.mRenderer.enabled = false;
			return;
		}
		bool flag = Input.touchCount > 0;
		this.mRenderer.enabled = flag;
		if (flag)
		{
			this.mTouchPosition = Input.touches[0].position;
			this.mFXPosition = this.mCamera.ScreenToWorldPoint(this.mTouchPosition);
			this.mFXPosition.z = -1f;
			base.transform.position = this.mFXPosition;
		}
	}

	public void LateUpdate()
	{
		float num = this.mCamera.orthographicSize / this.mBaseOrthographicSize;
		this.mTransform.localScale = this.mBaseScale * num;
	}

	private Camera mCamera;

	private MeshRenderer mRenderer;

	private Vector3 mTouchPosition;

	private Vector3 mFXPosition;

	private Transform mTransform;

	private Vector3 mBaseScale;

	private float mBaseOrthographicSize;
}
