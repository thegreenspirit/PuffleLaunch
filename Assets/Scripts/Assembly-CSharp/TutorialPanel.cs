using System;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
	private void Start()
	{
		this.m_DoInit = true;
	}

	private void Update()
	{
		if (GameManager.Instance.CurrentWorld == GameManager.World.eWorld_BonusWorld)
		{
			return;
		}
		if (!LevelLoader.Instance.isLoadingFinished)
		{
			return;
		}
		if (this.m_DoInit)
		{
			this.m_DoInit = false;
			if (ProfileManager.Instance.CurrentProfile.m_LevelData[(int)GameManager.smCurrentLevel].LevelComplete)
			{
				base.enabled = false;
			}
			this.mTransform = base.transform;
			this.mPlayer = Puffle.Instance;
			PuffleContainer[] array = (PuffleContainer[])global::UnityEngine.Object.FindObjectsOfType(typeof(PuffleContainer));
			this.mPuffleContainer = array[this.triggerIndex];
			this.mTutorialShown = false;
			this.mPanelDepth = this.mTransform.position.z;
			Cannon component = this.mPuffleContainer.GetComponent<Cannon>();
			if (component)
			{
				this.mAutoLaunch = component.autoLaunch;
			}
			if (this.keepContainerInView)
			{
				Camera.main.GetComponentInChildren<VisualEffects>().TutorialObject = this.mPuffleContainer.transform;
			}
			else
			{
				Camera.main.GetComponentInChildren<VisualEffects>().TutorialObject = null;
			}
			return;
		}
		if (this.mTutorialShown)
		{
			bool flag = false;
			if (GameFlowManager.Instance.GUIManager.CurrentScene != GUIManager.Scene.ePauseMenu && GameFlowManager.Instance.InputController.TouchCount > 0)
			{
				flag = true;
			}
			if (flag)
			{
				Puffle.Instance.DisableInput = false;
				CameraFollow component2 = Camera.main.GetComponent<CameraFollow>();
				component2.moveSpeed = this.mCameraMoveSpeed;
				component2.zoomSpeed = this.mCameraZoomSpeed;
				component2.TargetPosition = this.mCameraTarget;
				component2.TargetSize = this.mCameraSize;
				component2.ZoomOverride = false;
				component2.Target = Puffle.Instance.transform;
				Camera.main.GetComponentInChildren<VisualEffects>().ShowTutorialFX(false);
				Vector3 position = this.mTransform.position;
				position.z = this.mPanelDepth;
				this.mTransform.position = position;
				if (this.mAutoLaunch)
				{
					this.mPuffleContainer.GetComponent<Cannon>().autoLaunch = true;
				}
				base.enabled = false;
			}
		}
		else if ((this.mPlayer.State == Puffle.PuffleState.eInCannon || this.mPlayer.State == Puffle.PuffleState.eInSlingshot) && this.mPuffleContainer.IsPuffleInside())
		{
			Puffle.Instance.DisableInput = true;
			this.mTutorialShown = true;
			Vector3 position2 = this.mTransform.position;
			position2.z = Camera.main.transform.position.z + 4f;
			this.mTransform.position = position2;
			Camera.main.GetComponentInChildren<VisualEffects>().ShowTutorialFX(true);
			CameraFollow component3 = Camera.main.GetComponent<CameraFollow>();
			this.mCameraMoveSpeed = component3.moveSpeed;
			this.mCameraZoomSpeed = component3.zoomSpeed;
			component3.moveSpeed = 0.1f;
			component3.zoomSpeed = 0.1f;
			this.mCameraTarget = component3.TargetPosition;
			this.mCameraSize = component3.TargetSize;
			if (this.keepContainerInView)
			{
				Vector3 vector = (this.mTransform.position + this.mPuffleContainer.transform.position) / 2f;
				component3.TargetPosition = new Vector3(vector.x, vector.y, this.mCameraTarget.z);
			}
			else
			{
				component3.TargetPosition = new Vector3(this.mTransform.position.x, this.mTransform.position.y, this.mCameraTarget.z);
			}
			if (GameManager.smCurrentLevel != GameManager.Level.eLevel_1 && GameManager.smCurrentLevel != GameManager.Level.eLevel_5 && GameManager.smCurrentLevel != GameManager.Level.eLevel_6 && GameManager.smCurrentLevel != GameManager.Level.eLevel_14)
			{
				component3.Target = null;
			}
			component3.TargetSize = 10f;
			component3.ZoomOverride = true;
			if (this.mAutoLaunch)
			{
				this.mPuffleContainer.GetComponent<Cannon>().autoLaunch = false;
			}
		}
	}

	private const float kFocusMoveSpeed = 0.1f;

	private const float kFocusZoomSpeed = 0.1f;

	private const float kFocusSize = 10f;

	public int triggerIndex;

	public bool keepContainerInView = true;

	private Transform mTransform;

	private Puffle mPlayer;

	private PuffleContainer mPuffleContainer;

	private bool mTutorialShown;

	private float mPanelDepth;

	private float mCameraMoveSpeed;

	private float mCameraZoomSpeed;

	private Vector3 mCameraTarget;

	private float mCameraSize;

	private bool mAutoLaunch;

	private bool m_DoInit = true;
}
