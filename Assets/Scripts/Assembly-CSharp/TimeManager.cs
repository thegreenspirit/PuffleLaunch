using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	private const float mkOriginalFramerate = 24f;
	private const float mkSlowmoScale = 0.2f;
	private const float mkSlowmoDuration = 2.5f;
	public const float mkTurboScaleMin = 1f;
	public const float mkTurboScaleMax = 3f;
	public const float mkTurboScaleInc = 0.025f;

	private static TimeManager mInstance;
	private float mTimeScale = 1f;
	private float mSlowmoTimer;
	private bool mPaused;
	private float mOriginalTimeScale;
	private bool mSlowmoOverride;
	private float mkTurboScale = 1.45f;

	public float GetTurboScale()
	{
		return this.mkTurboScale;
	}

	public void AdjustTurboScale(float af_adjustmentAmount)
	{
		this.mkTurboScale += af_adjustmentAmount;
		if (this.mkTurboScale < mkTurboScaleMin) this.mkTurboScale = mkTurboScaleMin;
		if (this.mkTurboScale > mkTurboScaleMax) this.mkTurboScale = mkTurboScaleMax;
		Time.timeScale = this.mkTurboScale;
		this.mOriginalTimeScale = Time.timeScale;
	}

	public void Awake()
	{
		TimeManager.mInstance = this;
		Time.fixedDeltaTime = 0.041666668f;
	}

	public void Start()
	{
		this.mOriginalTimeScale = Time.timeScale;
	}

	public void FixedUpdate()
	{
		if (!this.mSlowmoOverride)
		{
			if (this.mSlowmoTimer > 0f)
			{
				this.mSlowmoTimer -= Time.deltaTime;
			}
			else if (this.mTimeScale < 1f)
			{
				this.mTimeScale = Mathf.Min(1f, this.mTimeScale + 0.015f * Time.deltaTime * mkOriginalFramerate);
			}
		}
	}

	public void ActivateSlowmo()
	{
		this.mSlowmoTimer = mkSlowmoDuration;
		this.mTimeScale = mkSlowmoScale;
		Camera.main.GetComponentInChildren<VisualEffects>().ShowSlowMoFX(true);
	}

	public void StopSlowmo()
	{
		if (!this.mSlowmoOverride)
		{
			this.mTimeScale = 1f;

			VisualEffects componentInChildren = Camera.main.GetComponentInChildren<VisualEffects>();

			if (componentInChildren != null)
				componentInChildren.ShowSlowMoFX(false);
		}
	}

	public bool IsSlowMo()
	{
		return this.mTimeScale == mkSlowmoScale;
	}

	public void ActivateTurbo()
	{
		Time.timeScale = this.mkTurboScale;
		this.mOriginalTimeScale = Time.timeScale;
	}

	public void StopTurbo()
	{
		Time.timeScale = 1f;
		this.mOriginalTimeScale = Time.timeScale;
	}

	public void Pause(bool aPaused)
	{
		this.mPaused = aPaused;
		if (this.mPaused) Time.timeScale = 0f;
		else Time.timeScale = this.mOriginalTimeScale;
	}

	public static TimeManager Instance
	{
		get { return TimeManager.mInstance; }
	}

	public float DeltaTime
	{
		get
		{
			if (this.mPaused) return 0f;
			return Time.deltaTime * mkOriginalFramerate * this.mTimeScale;
		}
	}

	public float TimeScale
	{
		get { return this.mTimeScale; }
	}

	public float TimeScaleRatio
	{
		get
		{
			if (this.mSlowmoTimer > 0f) return 0f;
			return (this.mTimeScale - mkSlowmoScale) / 0.8f;
		}
	}

	public bool SlowmoOverride
	{
		get { return this.mSlowmoOverride; }
		set { this.mSlowmoOverride = value; }
	}
}
