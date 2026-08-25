using System;
using UnityEngine;

[RequireComponent(typeof(PuffleContainer))]
public class Cannon : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
		this.mTweening = base.GetComponentInChildren<TweeningController>();
	}

	public void Awake()
	{
		this.mThisContainer = base.GetComponent<PuffleContainer>();
	}

	public void FixedUpdate()
	{
		if (this.autoLaunch && this.mThisContainer.IsPuffleInside())
		{
			this.LaunchPuffle();
		}
	}

	public virtual void LaunchPuffle()
	{
		this.mThisContainer.GetContainedPuffle().Launch(this.mTransform.right, 50f * ScaleItem.Instance.LevelScale);
		this.mThisContainer.ReleasePuffle();
		this.mTweening.Play(true);
		AudioManager.Instance.PlayCannonSound(this.mLaunchCannonSound);
	}

	public bool IsPuffleInside()
	{
		return this.mThisContainer.IsPuffleInside();
	}

	public virtual void OnCannonEnter()
	{
		this.mTweening.Reset(true);
	}

	public bool autoLaunch;

	public AudioClip mLaunchCannonSound;

	private Transform mTransform;

	private PuffleContainer mThisContainer;

	private TweeningController mTweening;

	public enum ControlType
	{
		eButtonPointTouch,
		eButtonRotate,
		eTouchRelease,
		eControlType_COUNT
	}
}
