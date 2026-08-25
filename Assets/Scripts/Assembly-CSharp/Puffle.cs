using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Puffle : MonoBehaviour
{
	public enum ControlType
	{
		eTouchScreen,
		eTilting,
		eControlType_COUNT
	}

	public enum PuffleState
	{
		eFlying,
		eInCannon,
		eInSlingshot,
		eLaunching,
		eRespawning,
		ePuffleState_COUNT
	}

	public const int kPlayerSpriteIndex = 1;

	public const int kPuffleCount = 10;

	private const float kBreakSpeed = 1f;

	private const float kOppositeHorizontalVelocityScalingRange = 1f;

	private const float kInitialBoostSpeed = 0.75f;

	private const float kInitialBoostHorizontalVelocityScalingRange = 0.25f;

	private const float kTurboModeSensitivityMultiplier = 0.85f;

	private const float mkShootControlTimeout = 0.1f;

	private const float mkTapMovementInterval = 0.2f;

	public float tiltTransitionSize = 1f;

	public Vector3 spawnPoint;

	public int respawnCount;

	public float groundPosition;

	public float ceilingPosition;

	public AudioClip waterFallSound;

	public AudioClip cloudFallSound;

	public ParticleEmitter trail;

	public static Puffle.ControlType smControlType;

	private static Puffle mInstance;

	private Transform mTransform;

	private Transform mTrailTransform;

	private SpriteManager mSpriteManager;

	private Vector3 mVelocity;

	private float mAngularVelocity;

	private bool mInvertGravity;

	private bool mStopMovement;

	private float mInitialTrailTime;

	private Collider mCurrentContainer;

	private Cannon mCurrentCannon;

	private InputController mInputController;

	private float mControlTimeout;

	private Puffle.PuffleState mState;

	private float mTrailDelay;

	private Splash mSplashObject;

	private float mMovementChangeTimer;

	private float mLastMovement;

	private Vector3 mPrevPosition;

	private bool mDisableInput;

	public event PuffleDeathEventHandler puffleDeath;

	public static Puffle Instance
	{
		get
		{
			return Puffle.mInstance;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			return this.mVelocity;
		}
		set
		{
			this.mVelocity = value;
		}
	}

	public Splash Splash
	{
		get
		{
			return this.mSplashObject;
		}
		set
		{
			this.mSplashObject = value;
		}
	}

	public float AngularVelocity
	{
		get
		{
			return this.mAngularVelocity;
		}
		set
		{
			this.mAngularVelocity = value;
		}
	}

	public bool InvertGravity
	{
		get
		{
			return this.mInvertGravity;
		}
		set
		{
			this.mInvertGravity = value;
		}
	}

	public bool StopMovement
	{
		get
		{
			return this.mStopMovement;
		}
		set
		{
			this.mStopMovement = value;
		}
	}

	public bool DisableInput
	{
		get
		{
			return this.mDisableInput;
		}
		set
		{
			this.mDisableInput = value;
		}
	}

	public Puffle.PuffleState State
	{
		get
		{
			return this.mState;
		}
	}

	public void Awake()
	{
		Puffle.mInstance = this;
	}

	public void Start()
	{
		this.mInvertGravity = false;
		this.mTransform = base.transform;
		this.mTrailTransform = this.trail.transform;
		this.mCurrentContainer = null;
		this.mInputController = GameFlowManager.Instance.InputController;
		Puffle.smControlType = Puffle.ControlType.eTouchScreen;
		this.mControlTimeout = 0f;
		this.mTrailDelay = 0f;
		this.mState = Puffle.PuffleState.eFlying;
		this.mSpriteManager = base.GetComponent<SpriteManager>();
		this.mSpriteManager.Seek(1);
	}

	public void Update()
	{
		if (GameManager.Instance.IsPause())
		{
			return;
		}
		if (!this.mDisableInput && this.mState == Puffle.PuffleState.eInCannon && this.GetLaunchPuffle())
		{
			this.mState = Puffle.PuffleState.eLaunching;
		}
		this.mControlTimeout = Mathf.Max(this.mControlTimeout - Time.deltaTime, 0f);
		float num = this.mTrailDelay;
		if (num > 0f)
		{
			this.mTrailDelay -= Time.deltaTime;
			if (this.mTrailDelay <= 0f)
			{
				this.mTrailDelay = 0f;
				this.trail.emit = true;
			}
		}
	}

	public void FixedUpdate()
	{
		if (GameManager.Instance.IsPause())
		{
			return;
		}
		float deltaTime = TimeManager.Instance.DeltaTime;
		if (LevelLoader.Instance == null)
		{
			return;
		}
		float levelScale = ScaleItem.Instance.LevelScale;
		bool flag = false;
		if (this.mState == Puffle.PuffleState.eFlying)
		{
			if (this.mTransform.position.y < this.groundPosition || this.mTransform.position.y > this.ceilingPosition)
			{
				if (GameManager.Instance.CurrentWorld == GameManager.World.eWorld_BlueSky)
				{
					AudioManager.Instance.PlayObstacleSound(this.waterFallSound);
				}
				else
				{
					AudioManager.Instance.PlayObstacleSound(this.cloudFallSound);
				}
				this.mSplashObject.transform.position = this.mTransform.position;
				this.mSplashObject.Reset();
				if (this.mTransform.position.y > this.ceilingPosition)
				{
					Vector3 localScale = this.mSplashObject.transform.localScale;
					localScale.y *= -1f;
					this.mSplashObject.transform.localScale = localScale;
				}
				this.mSplashObject.Puffle = this;
				this.mState = Puffle.PuffleState.eRespawning;
				base.GetComponent<MeshRenderer>().enabled = false;
				TimeManager.Instance.StopSlowmo();
				return;
			}
			if (Mathf.Abs(this.mVelocity.x) < 5f * levelScale && Mathf.Abs(this.mVelocity.y) < 5f * levelScale)
			{
				this.mTrailDelay = 0f;
				this.trail.emit = false;
			}
			this.mVelocity += new Vector3(0f, (float)((!this.mInvertGravity) ? (-1) : 1) * deltaTime * levelScale, 0f);
			this.mVelocity *= 1f - 0.02f * deltaTime;
			this.mAngularVelocity *= 1f - 0.02f * deltaTime;
			float num = 0f;
			if (!this.mDisableInput)
			{
				num = this.GetPuffleMovement();
			}
			if (num != 0f)
			{
				if (num > 0f)
				{
					if (this.mVelocity.x < 0f)
					{
						float num2 = 1f * Mathf.Abs(this.mVelocity.x / 1f);
						num *= 0.75f + Mathf.Clamp(num2, 0f, 1f);
					}
					else
					{
						float num3 = 0.75f * Mathf.Clamp(1f - Mathf.Abs(this.mVelocity.x / 0.25f), 0f, 1f);
						num *= 1f + Mathf.Clamp(num3, 0f, 0.75f);
					}
				}
				else if (this.mVelocity.x > 0f)
				{
					float num4 = 1f * Mathf.Abs(this.mVelocity.x / 1f);
					num *= 0.75f + Mathf.Clamp(num4, 0f, 1f);
				}
				else
				{
					float num5 = 0.75f * Mathf.Clamp(1f - Mathf.Abs(this.mVelocity.x / 0.25f), 0f, 1f);
					num *= 1f + Mathf.Clamp(num5, 0f, 0.75f);
				}
				if (GameManager.Instance.EnableTurboMode)
				{
					num *= 0.85f;
				}
			}
			if (Puffle.smControlType == Puffle.ControlType.eTouchScreen && num != 0f && num != this.mLastMovement)
			{
				if (this.mMovementChangeTimer > 0f)
				{
					this.mVelocity += new Vector3(num * 0.9f * deltaTime * levelScale / Mathf.Pow(TimeManager.Instance.TimeScale, 2f), 0f, 0f);
				}
				this.mMovementChangeTimer = 0.2f;
			}
			this.mLastMovement = num;
			this.mVelocity += new Vector3(num * 0.8f * deltaTime * levelScale / Mathf.Pow(TimeManager.Instance.TimeScale, 2f), 0f, 0f);
			if (num != 0f)
			{
				this.mAngularVelocity += 0.4f * deltaTime;
			}
			this.mMovementChangeTimer = Mathf.Max(this.mMovementChangeTimer - Time.deltaTime, 0f);
			flag = true;
		}
		else if (this.mState == Puffle.PuffleState.eLaunching)
		{
			this.mCurrentCannon.LaunchPuffle();
			flag = true;
		}
		if (this.mStopMovement)
		{
			flag = false;
			this.mVelocity = Vector3.zero;
		}
		if (flag)
		{
			this.mPrevPosition = this.mTransform.position;
			this.mTransform.position += this.mVelocity * deltaTime;
			this.mTransform.eulerAngles += new Vector3(0f, 0f, this.mAngularVelocity * deltaTime);
			this.trail.worldVelocity = -this.mVelocity * 2f * deltaTime;
            if (this.mTrailTransform != null)
            {
				this.mTrailTransform.position = this.mTransform.position - this.mVelocity * deltaTime * 3f + Vector3.forward * 0.01f;
			}
		}
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (this.mState == Puffle.PuffleState.eFlying && aOther != this.mCurrentContainer)
		{
			PuffleContainer component = aOther.GetComponent<PuffleContainer>();
			if (component)
			{
				this.mCurrentContainer = aOther;
				component.OnPuffleEnter(this);
				this.mVelocity = Vector3.zero;
				this.mAngularVelocity = 0f;
				this.mTransform.parent = aOther.transform;
				this.mTransform.localPosition = new Vector3(-0.08f, 0.13f, 0f);
				this.mTransform.localRotation = Quaternion.identity;
				this.mTrailDelay = 0f;
				this.trail.emit = false;
				TimeManager.Instance.StopSlowmo();
				this.mSpriteManager.Seek(11);
				this.mCurrentCannon = this.mCurrentContainer.GetComponent<Cannon>();
				if (this.mCurrentCannon)
				{
					this.mCurrentCannon.OnCannonEnter();
					this.mState = Puffle.PuffleState.eInCannon;
				}
				else
				{
					this.mState = Puffle.PuffleState.eInSlingshot;
				}
			}
		}
	}

	public void Launch(Vector3 aDirection, float aForce)
	{
		this.mTransform.localPosition = Vector3.zero;
		if (this.mCurrentCannon != null && !this.mCurrentCannon.autoLaunch)
		{
			this.mControlTimeout = 0.1f;
		}
		this.mState = Puffle.PuffleState.eFlying;
		this.mTransform.parent = null;
		this.mCurrentContainer = null;
		Vector3 vector = aDirection * aForce;
		this.mVelocity = vector * 0.8f;
		this.mTransform.position += vector;
		this.mAngularVelocity = (Mathf.Abs(vector.x) + Mathf.Abs(vector.y)) / ScaleItem.Instance.LevelScale;
		this.mSpriteManager.Seek(1);
		this.trail.worldVelocity = -this.mVelocity * 2f;
        if (mTrailTransform != null)
        {
			this.mTrailTransform.position = this.mTransform.position - this.mVelocity * 3f + Vector3.forward * 0.01f;
		}
		this.mTrailDelay = 0.1f;
	}

	public void Respawn()
	{
		if (this.puffleDeath != null)
		{
			this.puffleDeath(this, EventArgs.Empty);
		}
		this.mTransform.position = this.spawnPoint;
		this.mPrevPosition = this.mTransform.position;
		this.mVelocity = Vector3.zero;
		this.mAngularVelocity = 0f;
		this.mCurrentContainer = null;
		this.mCurrentCannon = null;
		this.mState = Puffle.PuffleState.eFlying;
		this.mMovementChangeTimer = 0f;
		this.mLastMovement = 0f;
		this.mTrailDelay = 0f;
		this.trail.emit = false;
		this.mInvertGravity = false;
		base.GetComponent<MeshRenderer>().enabled = true;
		this.respawnCount++;
	}

	private float GetPuffleMovement()
	{
		float num = 0f;
		if (this.mControlTimeout <= 0f)
		{
			if (Application.isEditor)
			{
				Puffle.ControlType controlType = Puffle.smControlType;
				if (controlType != Puffle.ControlType.eTouchScreen)
				{
					if (controlType == Puffle.ControlType.eTilting)
					{
						if (Input.GetMouseButton(0))
						{
							bool flag = false;
							if (GameManager.HasCompletedTurboMode(GameManager.Instance.CurrentWorld) && GameFlowManager.Instance.GUIManager.HudManager.InGameHud.mo_slowMoButton.ContainsTouch())
							{
								flag = true;
							}
							if (!flag)
							{
								if (Input.mousePosition.x > (float)Screen.width * 0.5f)
								{
									num = 1f;
								}
								else
								{
									num = -1f;
								}
							}
						}
					}
				}
				else
				{
					num = Input.GetAxisRaw("Horizontal");
				}
			}
			else
			{
				Puffle.ControlType controlType = Puffle.smControlType;
				if (controlType != Puffle.ControlType.eTouchScreen)
				{
					if (controlType == Puffle.ControlType.eTilting)
					{
						if (this.mInputController.Tilt)
						{
							float num2 = 0f;
							if (this.mInputController.TiltDirection == InputController.TiltAxis.eTiltLeft)
							{
								num2 = -this.mInputController.TiltAngle / this.tiltTransitionSize;
							}
							else if (this.mInputController.TiltDirection == InputController.TiltAxis.eTiltRight)
							{
								num2 = this.mInputController.TiltAngle / this.tiltTransitionSize;
							}
							num2 = Mathf.Clamp(num2, -1f, 1f);
							num = Mathf.Sign(num2) * (-Mathf.Cos(num2 * 3.1415927f * 0.5f) + 1f);
						}
					}
				}
				else if (this.mInputController.TouchCount > 0 && !this.mInputController.Zoom)
				{
					bool flag2 = false;
					Vector2 vector = new Vector2(this.mInputController.TouchPosition1.x, (float)Screen.height - this.mInputController.TouchPosition1.y);
					if (GameManager.HasCompletedTurboMode(GameManager.Instance.CurrentWorld) && (vector - GameFlowManager.Instance.GUIManager.HudManager.InGameHud.mv2_slowmotionButtonCenterPixelPosition).magnitude < GameFlowManager.Instance.GUIManager.HudManager.InGameHud.mv2_slowmotionButtonSizeRatio.x * (float)Screen.width)
					{
						flag2 = true;
					}
					if (!flag2)
					{
						if (this.mInputController.TouchPosition1.x > (float)Screen.width * 0.5f)
						{
							num = 1f;
						}
						else
						{
							num = -1f;
						}
					}
				}
			}
		}
		return Mathf.Clamp(num, -1f, 1f);
	}

	private bool GetLaunchPuffle()
	{
		if (!GameFlowManager.Instance.GUIManager.HudManager.InGameHud.mb_isInitialized)
		{
			return false;
		}
		if (GameFlowManager.Instance.GUIManager.HudManager.InGameHud.mo_pauseButton.ContainsTouch())
		{
			return false;
		}
		if (GameManager.HasCompletedTurboMode(GameManager.Instance.CurrentWorld) && GameFlowManager.Instance.GUIManager.HudManager.InGameHud.mo_slowMoButton.ContainsTouch())
		{
			return false;
		}
		if (GUIUtility.hotControl != 0 || (!Application.isEditor && this.mInputController.Zoom))
		{
			return false;
		}
		bool flag = false;
		if (Application.isEditor)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				flag = true;
			}
		}
		else
		{
			Puffle.ControlType controlType = Puffle.smControlType;
			if (controlType == Puffle.ControlType.eTouchScreen || controlType == Puffle.ControlType.eTilting)
			{
				if (this.mInputController.TouchDown && this.mCurrentContainer.tag != "ControllableCannon")
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	public Vector3 GetContactPoint(Collider aSender)
	{
		int num = 256;
		num = ~num;
		Vector3 vector = this.mTransform.position - this.mPrevPosition;
		bool isTrigger = aSender.isTrigger;
		aSender.isTrigger = false;
		float radius = base.GetComponent<SphereCollider>().radius;
		RaycastHit raycastHit;
		Physics.SphereCast(this.mPrevPosition - vector.normalized * radius, radius, vector, out raycastHit, vector.magnitude + radius, num);
		aSender.isTrigger = isTrigger;
		return raycastHit.point;
	}

	public static void SetControlType()
	{
		smControlType++;
		smControlType = (ControlType)((int)smControlType % 2);
	}
}
