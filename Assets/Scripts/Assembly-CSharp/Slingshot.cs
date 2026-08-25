using System;
using UnityEngine;

[RequireComponent(typeof(ElasticMovement))]
[RequireComponent(typeof(PuffleContainer))]
public class Slingshot : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mThisContainer = base.GetComponent<PuffleContainer>();
		this.mInputController = GameFlowManager.Instance.InputController;
		this.mInputActive = false;
		this.mInitialPosition = this.mTransform.position;
		this.mElasticMovement = base.GetComponent<ElasticMovement>();
		float num = 960f / (float)Screen.width;
		Vector3 vector = this.mLeftBalloonTransform.localPosition;
		vector.x *= num;
		this.mLeftBalloonTransform.localPosition = vector;
		vector = this.mRightBalloonTransform.localPosition;
		vector.x *= num;
		this.mRightBalloonTransform.localPosition = vector;
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (aOther.tag == "Player")
		{
			Puffle component = aOther.GetComponent<Puffle>();
			if (component.State == Puffle.PuffleState.eFlying)
			{
				Vector3 vector = component.Velocity.normalized;
				vector *= 20f * ScaleItem.Instance.LevelScale;
				this.mElasticMovement.Velocity = vector * 1.5f;
			}
		}
	}

	public void Update()
	{
		this.HandleControls();
	}

	private void HandleControls()
	{
		if (this.mThisContainer.IsPuffleInside())
		{
			if (this.mInputActive)
			{
				if (this.mInputController.Release)
				{
					this.mInputActive = false;
					Vector3 vector = this.mInitialPosition - this.mElasticMovement.TargetPosition;
					if (vector.y > 0.5f)
					{
						Vector3 vector2 = new Vector3(vector.x / 2.6f, vector.y, 0f);
						this.mThisContainer.GetContainedPuffle().Launch(vector2, this.launchForce);
						this.mThisContainer.ReleasePuffle();
						AudioManager.Instance.PlayObstacleSound(this.ReleaseSound);
					}
					this.mElasticMovement.TargetPosition = this.mInitialPosition;
				}
				else
				{
					Vector3 vector3 = Camera.main.ScreenToWorldPoint(this.mInputController.TouchPosition1);
					vector3.z = this.mTransform.position.z;
					Vector3 vector4 = (vector3 - this.mTouchDownPosition) * this.dragRatio;
					if (vector4.x > 98f * ScaleItem.Instance.LevelScale)
					{
						vector4.x = 98f * ScaleItem.Instance.LevelScale;
					}
					else if (vector4.x < -98f * ScaleItem.Instance.LevelScale)
					{
						vector4.x = -98f * ScaleItem.Instance.LevelScale;
					}
					if (vector4.y < -166f * ScaleItem.Instance.LevelScale)
					{
						vector4.y = -166f * ScaleItem.Instance.LevelScale;
					}
					vector4.y = Mathf.Min(vector4.y, 0f);
					this.mElasticMovement.TargetPosition = this.mInitialPosition + vector4;
				}
			}
			else if (this.mInputController.TouchDown)
			{
				Vector3 vector5 = Camera.main.ScreenToWorldPoint(this.mInputController.TouchPosition1);
				vector5.z = this.mTransform.position.z;
				if ((vector5 - this.mTransform.position).sqrMagnitude <= Mathf.Pow(this.touchRadius, 2f))
				{
					this.mInputActive = true;
					this.mTouchDownPosition = vector5;
				}
			}
		}
	}

	private const float kMinMovementX = 0.4f;

	private const float kMinMovementY = 0.2f;

	private const float kMinTimeToReplayStretchSound = 1f;

	public float launchForce = 1f;

	public float touchRadius = 1f;

	public float dragRatio = 1f;

	public Transform mLeftBalloonTransform;

	public Transform mRightBalloonTransform;

	public AudioClip ReleaseSound;

	public AudioClip StretchSound;

	private Transform mTransform;

	private PuffleContainer mThisContainer;

	private InputController mInputController;

	private Vector3 mInitialPosition;

	private float mElasticMultiplierDefault;

	private Vector3 mTouchDownPosition;

	private bool mInputActive;

	private ElasticMovement mElasticMovement;
}
