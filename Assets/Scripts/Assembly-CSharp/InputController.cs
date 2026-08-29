using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
	public Vector3 StartTouchPos
	{
		get
		{
			return this.mStartTouchPos;
		}
	}

	public Vector3 TouchPosition1
	{
		get
		{
			return this.mTouchPosition1;
		}
	}

	public bool HasFinger1Moved
	{
		get
		{
			return this.m_Finger1Moved;
		}
	}

	public bool HasFinger2Moved
	{
		get
		{
			return this.m_Finger2Moved;
		}
	}

	public Vector3 TouchPosition2
	{
		get
		{
			return this.mTouchPosition2;
		}
	}

	public int TouchCount
	{
		get
		{
			return this.mTouchCount;
		}
	}

	public int PreviousTouchCount
	{
		get
		{
			return this.mPreviousTouchCount;
		}
	}

	public bool TouchDown
	{
		get
		{
			return this.mTouchDown;
		}
	}

	public Vector3 TapPosition
	{
		get
		{
			return this.mTapPosition;
		}
	}

	public int FirstFingerId
	{
		get
		{
			return (Input.touchCount <= 0) ? (-1) : Input.touches[0].fingerId;
		}
	}

	public int SecondFingerId
	{
		get
		{
			return (Input.touchCount <= 1) ? (-1) : Input.touches[1].fingerId;
		}
	}

	public Vector3 ReleasePosition
	{
		get
		{
			return this.mReleasePosition;
		}
	}

	public bool SingleTap
	{
		get
		{
			return this.mSingleTap;
		}
	}

	public bool DoubleTap
	{
		get
		{
			return this.mDoubleTap;
		}
	}

	public bool DetectingFirstTap
	{
		get
		{
			return this.mDetectingFirstTap;
		}
	}

	public bool Held
	{
		get
		{
			return this.mHold;
		}
	}

	public bool LongHold
	{
		get
		{
			return this.mLongHold;
		}
	}

	public bool Release
	{
		get
		{
			return this.mRelease;
		}
	}

	public bool Swipe
	{
		get
		{
			return this.mSwipe;
		}
	}

	public Vector2 SlideDirection
	{
		get
		{
			return this.mSlideDirection;
		}
	}

	public bool ReturnSwipe
	{
		get
		{
			return this.mReturnSwipe;
		}
	}

	public InputController.SwipeAxis ReturnSwipeAxis
	{
		get
		{
			return this.mSwipeAxis;
		}
	}

	public Vector2 AccelerometerDirection
	{
		get
		{
			return this.mAccelerometerDirection;
		}
	}

	public float AccelerometerDeadZone
	{
		get
		{
			return this.mAccelerometerDeadZone;
		}
	}

	public bool Zoom
	{
		get
		{
			return this.mZoom;
		}
	}

	public InputController.ZoomAxis ZoomDirection
	{
		get
		{
			return this.mZoomDirection;
		}
	}

	public float ZoomDistance
	{
		get
		{
			return this.mZoomDistance;
		}
	}

	public bool Tilt
	{
		get
		{
			return this.mTilt;
		}
	}

	public InputController.TiltAxis TiltDirection
	{
		get
		{
			return this.mTiltDirection;
		}
	}

	public float TiltAngle
	{
		get
		{
			return this.mTiltAngle;
		}
	}

	public bool Shake
	{
		get
		{
			return this.mShake;
		}
	}

	public Vector3 ShakeDirection
	{
		get
		{
			return this.mShakeDirection;
		}
	}

	private void Start()
	{
		this.mAccelerometerDirection.x = (this.mAccelerometerDirection.y = 0f);
#if UNITY_IOS
		if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen || UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone3G)
		{
			this.mHoldDetectTime = 0.2f;
		}
#endif
	}

	public void Reset()
	{
		this.mTouchPosition1 = Vector3.zero;
		this.mTouch = false;
		this.mTouchDown = false;
		this.mTouchCount = 0;
		this.mHold = false;
		this.mSingleTap = false;
		this.mDoubleTap = false;
		this.mSwipe = false;
		this.mRelease = false;
		this.mTapTimer = 0f;
		this.m_Finger1Moved = false;
		this.mMoveStartPos1 = Vector3.zero;
		this.mTouchWasDown = false;
		this.mPreviousTouchCount = 0;
		this.mb_skipNextUpdate = true;
	}

	private void Update()
	{
		this.mTouchPosition1 = Vector3.zero;
		this.mTouch = false;
		this.mTouchDown = false;
		this.mTouchCount = 0;
		this.mHold = false;
		this.mLongHold = false;
		this.mSingleTap = false;
		this.mDoubleTap = false;
		this.mSwipe = false;
		this.mRelease = false;
		if (GameManager.Instance.IsPause())
		{
			this.Reset();
		}
		else if (this.mb_skipNextUpdate)
		{
			this.mb_skipNextUpdate = false;
		}
		else
		{
			if (Application.isEditor)
			{
				if (Input.GetMouseButton(0))
				{
					this.mTouchCount = 1;
					this.mTouch = true;
					this.mTouchPosition1 = Input.mousePosition;
					if (!this.mTouchWasDown)
					{
						this.mTouchDown = true;
					}
				}
			}
			else
			{
				this.mZoom = false;
				this.mTouchPosition2 = Vector3.zero;
				this.mTilt = false;
				this.mTiltDirection = InputController.TiltAxis.eTilt_COUNT;
				this.mShake = false;
				this.mTouchCount = Input.touchCount;
				if (this.mTouchCount > 0)
				{
					this.mTouch = true;
					Vector2 vector = Input.touches[0].position;
					this.mTouchPosition1.x = vector.x;
					this.mTouchPosition1.y = vector.y;
					this.mTouchPosition1.z = 0f;
					if (this.mTouchCount > 1)
					{
						vector = Input.touches[1].position;
						this.mTouchPosition2.x = vector.x;
						this.mTouchPosition2.y = vector.y;
						this.mTouchPosition2.z = 0f;
					}
					if (!this.mTouchWasDown)
					{
						this.mTouchDown = true;
					}
				}
				this.mAccelerometerDirection.x = Input.acceleration.x;
				this.mAccelerometerDirection.y = Input.acceleration.y;
				this.mAccelerometerDirection = this.mAccelerometerDirection.normalized;
				switch (Input.deviceOrientation)
				{
				case DeviceOrientation.LandscapeLeft:
					this.mDeviceOrientation = InputController.Orientation.eLandscapeLeft;
					break;
				case DeviceOrientation.LandscapeRight:
					this.mDeviceOrientation = InputController.Orientation.eLandscapeRight;
					break;
				}
			}
			if (!this.mTouch && this.mTouchWasDown)
			{
				this.mRelease = true;
				this.mReleasePosition = this.mPreviousTouchPosition1;
			}
			this.MoveGesture();
			this.ZoomGesture();
			this.mTouchWasDown = this.mTouch;
			this.mPreviousTouchPosition1 = this.mTouchPosition1;
			if (!Application.isEditor)
			{
				this.mPreviousTouchPosition2 = this.mTouchPosition2;
			}
			this.mPreviousSwipeDirection = this.mSwipeDirection;
			this.mPreviousTouchCount = this.mTouchCount;
		}
	}

	private void TapGesture()
	{
		float deltaTime = Time.deltaTime;
		this.mTapDetectTime = 0.03f;
		if (deltaTime > this.mTapDetectTime)
		{
			this.mTapDetectTime = deltaTime;
		}
		if (this.mTapTimer >= this.mHoldDetectTime)
		{
			this.mHold = true;
		}
		if (this.mTapTimer >= this.mLongHoldDetectTime)
		{
			this.mLongHold = true;
		}
		if (this.mDetectingFirstTap && this.mTapTimer >= this.mTapDetectTime)
		{
			this.mDetectingFirstTap = false;
		}
		if (this.mTouch)
		{
			if (this.mTouchDown)
			{
				this.mTapPosition = this.mTouchPosition1;
				this.mTapTimer = 0f;
				if (this.mTapCount == 0)
				{
					this.mDoubleTapTimer = 0f;
					this.mDetectingFirstTap = true;
				}
			}
			else
			{
				this.mTapTimer += Time.deltaTime;
				this.mDoubleTapTimer += Time.deltaTime;
			}
		}
		else
		{
			this.mDoubleTapTimer += Time.deltaTime;
			if (this.mTapTimer >= this.mTapDetectTime && this.mTouchWasDown && !this.mHold)
			{
				this.mTapCount++;
			}
			if (this.mDoubleTapTimer >= this.mDoubleTapDetectTime && !this.mHold)
			{
				if (this.mTapCount == 1)
				{
					this.mSingleTap = true;
					this.mTapCount = 0;
					this.mDoubleTapTimer = 0f;
				}
				else if (this.mTapCount == 2)
				{
					this.mDoubleTap = true;
					this.mTapCount = 0;
					this.mDoubleTapTimer = 0f;
				}
				else
				{
					this.mTapCount = 0;
					this.mDoubleTapTimer = 0f;
				}
			}
		}
	}

	private void MoveGesture()
	{
		if (this.mTouch)
		{
			if (this.mTouchDown)
			{
				this.mMoveStartPos1 = this.mTouchPosition1;
				if (this.mTouchCount > 1)
				{
					this.mMoveStartPos2 = this.mTouchPosition2;
				}
			}
			else
			{
				this.m_Finger1Moved = (this.mTouchPosition1 - this.mMoveStartPos1).magnitude >= 30f;
				if (this.mTouchCount > 1)
				{
					this.m_Finger2Moved = (this.mTouchPosition2 - this.mMoveStartPos2).magnitude >= 30f;
				}
				else
				{
					this.m_Finger2Moved = false;
				}
			}
		}
		else
		{
			this.m_Finger1Moved = false;
			this.mMoveStartPos1 = Vector3.zero;
			this.m_Finger2Moved = false;
			this.mMoveStartPos2 = Vector3.zero;
		}
	}

	private void SwipeGesture()
	{
		if (this.mTouch)
		{
			if (this.mTouchDown)
			{
				this.mSwipeTimer = 0f;
				this.mStartTouchPos = this.mTouchPosition1;
				this.mNoReturnSwipe = true;
			}
			else
			{
				this.mSwipeTimer += Time.deltaTime;
			}
		}
		else if (this.mSwipeTimer >= 0.03f && this.mTouchWasDown)
		{
			Vector2 vector = this.mPreviousTouchPosition1 - this.mStartTouchPos;
			float magnitude = vector.magnitude;
			if (magnitude >= 60f && this.mNoReturnSwipe)
			{
				this.mSwipe = true;
				this.mSlideDirection = vector.normalized;
			}
		}
	}

	private void ReturnSwipeGesture()
	{
		if (this.mTouch)
		{
			if (!this.mTouchDown)
			{
				this.mReturnSwipeTimer += Time.deltaTime;
				Vector2 vector = this.mTouchPosition1 - this.mPreviousTouchPosition1;
				float magnitude = vector.magnitude;
				vector /= magnitude;
				float num = 360f;
				if (magnitude == 0f)
				{
					this.mNoMoveHoldTimer += Time.deltaTime;
				}
				else
				{
					this.mNoMoveHoldTimer = 0f;
					if (this.mNoMoveHoldTimer <= 0.3f)
					{
						for (int i = 0; i < 4; i++)
						{
							float num2 = Vector2.Angle(vector, this.mVectTable[i]);
							if (num2 < num)
							{
								num = num2;
								this.mSwipeDirection = (InputController.Slide)i;
							}
						}
						if (this.mSwipeDirection == InputController.Slide.eRight)
						{
							if (this.mPreviousSwipeDirection == InputController.Slide.eLeft && this.mReturnSwipeTimer >= 0.45f)
							{
								this.mSwipeAxis = InputController.SwipeAxis.eLeft_Right;
								this.mReturnSwipe = true;
								this.mNoReturnSwipe = false;
							}
						}
						else if (this.mSwipeDirection == InputController.Slide.eLeft)
						{
							if (this.mPreviousSwipeDirection == InputController.Slide.eRight && this.mReturnSwipeTimer >= 0.45f)
							{
								this.mSwipeAxis = InputController.SwipeAxis.eLeft_Right;
								this.mReturnSwipe = true;
								this.mNoReturnSwipe = false;
							}
						}
						else if (this.mSwipeDirection == InputController.Slide.eUp)
						{
							if (this.mPreviousSwipeDirection == InputController.Slide.eDown && this.mReturnSwipeTimer >= 0.45f)
							{
								this.mSwipeAxis = InputController.SwipeAxis.eUp_Down;
								this.mReturnSwipe = true;
								this.mNoReturnSwipe = false;
							}
						}
						else if (this.mSwipeDirection == InputController.Slide.eDown && this.mPreviousSwipeDirection == InputController.Slide.eUp && this.mReturnSwipeTimer >= 0.45f)
						{
							this.mSwipeAxis = InputController.SwipeAxis.eUp_Down;
							this.mReturnSwipe = true;
							this.mNoReturnSwipe = false;
						}
					}
					else
					{
						this.mReturnSwipe = false;
					}
				}
			}
		}
		else
		{
			this.mReturnSwipeTimer = 0f;
			this.mReturnSwipe = false;
		}
	}

	private void ZoomGesture()
	{
		if (Input.touchCount > 1)
		{
			Vector2 vector = this.mPreviousTouchPosition1 - this.mPreviousTouchPosition2;
			float magnitude = (this.mTouchPosition1 - this.mTouchPosition2).magnitude;
			float magnitude2 = vector.magnitude;
			if (this.mIsFirstZoom)
			{
				this.mIsFirstZoom = false;
				this.mZoomDistance = 0f;
			}
			else
			{
				this.mZoomDistance = magnitude - magnitude2;
			}
			if (magnitude > magnitude2)
			{
				this.mZoom = true;
				this.mZoomDirection = InputController.ZoomAxis.eZoomIn;
			}
			else if (magnitude < magnitude2)
			{
				this.mZoom = true;
				this.mZoomDirection = InputController.ZoomAxis.eZoomOut;
			}
			else
			{
				this.mIsFirstZoom = true;
				this.mZoom = false;
				this.mZoomDirection = InputController.ZoomAxis.eNone;
			}
		}
		else
		{
			this.mIsFirstZoom = true;
		}
	}

	private void TiltGesture()
	{
		if (Mathf.Abs(this.mAccelerometerDirection.y) >= this.mTiltDeadzone)
		{
			this.mTilt = true;
		}
		if (this.mTilt)
		{
			if (this.mAccelerometerDirection.y > 0f)
			{
				if (this.mDeviceOrientation == InputController.Orientation.eLandscapeLeft)
				{
					this.mTiltDirection = InputController.TiltAxis.eTiltLeft;
				}
				else
				{
					this.mTiltDirection = InputController.TiltAxis.eTiltRight;
				}
			}
			else if (this.mDeviceOrientation == InputController.Orientation.eLandscapeLeft)
			{
				this.mTiltDirection = InputController.TiltAxis.eTiltRight;
			}
			else
			{
				this.mTiltDirection = InputController.TiltAxis.eTiltLeft;
			}
			this.mTiltAngle = Mathf.Abs(this.mAccelerometerDirection.y);
		}
	}

	private void ShakeGesture()
	{
		if (Input.acceleration.magnitude >= 1.75f)
		{
			this.mShake = true;
			this.mShakeDirection = Input.acceleration;
		}
	}

	private const float mSwipeDetectTime = 0.03f;

	private const float mReturnSwipeDetectTime = 0.45f;

	private const float mNoMoveHoldDetectionTime = 0.3f;

	private const float mSlideDetectionDistance = 60f;

	private const float mMoveDetectionDistance = 30f;

	public float mTiltDeadzone = 0.05f;

	private float mHoldDetectTime = 0.1f;

	private float mLongHoldDetectTime = 0.3f;

	private float mTapDetectTime = 0.03f;

	private float mDoubleTapDetectTime = 0.3f;

	private float mTapTimer;

	private float mDoubleTapTimer;

	private float mSwipeTimer;

	private float mReturnSwipeTimer;

	private float mNoMoveHoldTimer;

	private Vector3 mTouchPosition1 = Vector3.zero;

	private Vector3 mPreviousTouchPosition1 = default(Vector3);

	private Vector3 mMoveStartPos1 = Vector3.zero;

	private bool m_Finger1Moved;

	private bool mb_skipNextUpdate;

	private Vector3 mTouchPosition2 = Vector3.zero;

	private Vector3 mPreviousTouchPosition2 = default(Vector3);

	private Vector3 mMoveStartPos2 = Vector3.zero;

	private bool m_Finger2Moved;

	private Vector3 mTapPosition = Vector3.zero;

	private Vector3 mReleasePosition = Vector3.zero;

	private Vector3 mStartTouchPos = default(Vector3);

	private Vector2 mSlideDirection = default(Vector2);

	private Vector2[] mVectTable = new Vector2[4];

	private bool mTouch;

	private bool mTouchDown;

	private bool mTouchWasDown;

	private int mTouchCount;

	private int mPreviousTouchCount;

	private int mTapCount;

	private bool mHold;

	private bool mLongHold;

	private bool mSwipe;

	private bool mNoReturnSwipe;

	private bool mReturnSwipe;

	private bool mRelease;

	private bool mSingleTap;

	private bool mDoubleTap;

	private bool mDetectingFirstTap;

	private InputController.Slide mSwipeDirection = InputController.Slide.eSlide_Count;

	private InputController.Slide mPreviousSwipeDirection = InputController.Slide.eSlide_Count;

	private InputController.SwipeAxis mSwipeAxis = InputController.SwipeAxis.eNone;

	private bool mIsFirstZoom = true;

	private bool mZoom;

	private InputController.ZoomAxis mZoomDirection = InputController.ZoomAxis.eNone;

	private float mZoomDistance;

	private Vector2 mAccelerometerDirection = default(Vector2);

	private float mAccelerometerDeadZone = 0.075f;

	private bool mShake;

	private Vector3 mShakeDirection = default(Vector3);

	private bool mTilt;

	private InputController.TiltAxis mTiltDirection = InputController.TiltAxis.eTilt_COUNT;

	private float mTiltAngle;

	private InputController.Orientation mDeviceOrientation = InputController.Orientation.eOrientation_COUNT;

	public enum SwipeAxis
	{
		eUp_Down,
		eLeft_Right,
		eNone,
		eSlide_COUNT
	}

	public enum TiltAxis
	{
		eTiltUp,
		eTiltLeft,
		eTiltDown,
		eTiltRight,
		eTilt_COUNT
	}

	public enum ZoomAxis
	{
		eZoomIn,
		eZoomOut,
		eNone,
		eZoom_COUNT
	}

	private enum Slide
	{
		eUp,
		eLeft,
		eDown,
		eRight,
		eSlide_Count
	}

	private enum Orientation
	{
		ePortrait,
		eLandscapeLeft,
		ePortraitUpsideDown,
		eLandscapeRight,
		eOrientation_COUNT
	}
}
