using System;
using UnityEngine;

[RequireComponent(typeof(Cannon))]
public class CannonController : MonoBehaviour
{
	private const float m_MinDistanceFromCannonForRotation = 4f;
	public const float touchDectionBoundMultiplier = 1.8f;

	private int m_RotateFingerId = -1;
	public float rotationSpeed = 1f;

	private Cannon mCannon;
	private Transform mCannonTransform;
	private InputController mInputController;
	private Vector3 tempVector = default(Vector3);
	private Camera mCamera;
	private CannonController.CannonState mCannonState;

	public enum CannonState { eEmpty, eIdle, eRotate, eLaunch, eCannonState_COUNT }

	public void Start()
	{
		this.mCannon = base.GetComponent<Cannon>();
		this.mCannonTransform = this.mCannon.transform;
		this.mInputController = GameFlowManager.Instance.InputController;
		this.mCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		this.mCannonState = CannonController.CannonState.eEmpty;
	}

	public void Update()
	{
		if (GameManager.Instance.IsPause())
		{
			this.mCannonState = CannonController.CannonState.eIdle;
		}
		else if (this.mCannon.IsPuffleInside())
		{
			if (this.mCannonState == CannonController.CannonState.eEmpty)
			{
				if (this.mInputController.TouchCount > 0)
				{
					return;
				}
				this.mCannonState = CannonController.CannonState.eIdle;
			}
			else if (this.mInputController.PreviousTouchCount < 2 && this.mInputController.Release)
			{
				this.m_RotateFingerId = -1;
				bool flag = GameManager.HasCompletedTurboMode(GameManager.Instance.CurrentWorld) && GameFlowManager.Instance.GUIManager.HudManager.InGameHud.mo_slowMoButton.ContainsTouchRelease();
				this.mCannonState = ((this.mCannonState != CannonController.CannonState.eIdle || flag) ? CannonController.CannonState.eIdle : CannonController.CannonState.eLaunch);
			}
			else if (this.mInputController.TouchCount == 1 && (!GameManager.HasCompletedTurboMode(GameManager.Instance.CurrentWorld) || !GameFlowManager.Instance.GUIManager.HudManager.InGameHud.mo_slowMoButton.ContainsTouchRelease()) && !this.mInputController.DetectingFirstTap && (this.mInputController.HasFinger1Moved || this.mInputController.LongHold) && this.m_RotateFingerId == -1)
			{
				this.m_RotateFingerId = this.mInputController.FirstFingerId;
				this.mCannonState = CannonController.CannonState.eRotate;
			}
			CannonController.CannonState cannonState = this.mCannonState;
			if (cannonState != CannonController.CannonState.eRotate)
			{
				if (cannonState == CannonController.CannonState.eLaunch)
				{
					this.mCannon.LaunchPuffle();
					this.mCannonState = CannonController.CannonState.eEmpty;
				}
			}
			else
			{
				this.RotateCannon();
			}
		}
	}

	private void RotateCannon()
	{
		Puffle.ControlType smControlType = Puffle.smControlType;
		if (smControlType == Puffle.ControlType.eTouchScreen || smControlType == Puffle.ControlType.eTilting)
		{
			if (this.mInputController.TouchCount > 0)
			{
				if (this.m_RotateFingerId == this.mInputController.FirstFingerId)
				{
					this.tempVector = this.mCamera.ScreenToWorldPoint(this.mInputController.TouchPosition1);
					this.tempVector.z = this.mCannonTransform.position.z;
					Vector3 vector = this.mCannonTransform.position - this.tempVector;
					if (vector.magnitude >= 4f)
					{
						float num = Vector3.Angle(this.mCannonTransform.right, vector);
						Vector3 vector2 = Vector3.Cross(this.mCannonTransform.right, vector);
						this.tempVector = this.mCannonTransform.eulerAngles;
						if (vector2.z > 0f)
						{
							this.tempVector.z = this.tempVector.z + num;
						}
						else
						{
							this.tempVector.z = this.tempVector.z - num;
						}
						this.mCannonTransform.eulerAngles = this.tempVector;
					}
				}
			}
			else
			{
				this.m_RotateFingerId = -1;
				this.mCannonState = CannonController.CannonState.eIdle;
			}
		}
	}
}
