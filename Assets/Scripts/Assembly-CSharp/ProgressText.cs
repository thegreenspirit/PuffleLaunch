using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProgressText : MonoBehaviour
{
	private void Start()
	{
		this.mOriginalScale = base.transform.localScale;
		this.mState = ProgressText.TextState.eHidden;
		this.mTextRenderer = base.GetComponent<MeshRenderer>();
		this.mTextRenderer.material.color = new Color(0.85f, 0.85f, 0.85f);
		this.mTextRenderer.enabled = false;
		this.mBounds = this.mTextRenderer.bounds;
		if (this.textShadow != null)
		{
			this.textShadow.enabled = false;
		}
		base.transform.localScale = this.mOriginalScale * this.minScale;
		this.mLastFrameTimeStamp = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (this.mState == ProgressText.TextState.eFadingIn)
		{
			float num = Time.realtimeSinceStartup - this.mStateStart;
			Color color = this.mTextRenderer.material.color;
			float num2 = num / this.fadeTime;
			color.a = Mathf.Lerp(0f, 1f, num2);
			this.mTextRenderer.material.color = color;
			if (this.textShadow != null)
			{
				color = this.textShadow.material.color;
				color.a = Mathf.Lerp(0f, 1f, num2);
				this.textShadow.material.color = color;
			}
			if (num > this.fadeTime)
			{
				this.mStateStart = Time.realtimeSinceStartup;
				this.mState = ProgressText.TextState.eDisplayed;
			}
		}
		else if (this.mState == ProgressText.TextState.eFadingOut)
		{
			float num3 = Time.realtimeSinceStartup - this.mStateStart;
			Color color2 = this.mTextRenderer.material.color;
			float num4 = num3 / this.fadeTime;
			color2.a = Mathf.Lerp(1f, 0f, num4);
			this.mTextRenderer.material.color = color2;
			if (this.textShadow != null)
			{
				color2 = this.textShadow.material.color;
				color2.a = Mathf.Lerp(1f, 0f, num4);
				this.textShadow.material.color = color2;
			}
			if (num3 > this.fadeTime)
			{
				this.mStateStart = Time.realtimeSinceStartup;
				this.mState = ProgressText.TextState.eHidden;
			}
		}
		else if (this.mState == ProgressText.TextState.eDisplayed)
		{
			float num5 = Time.realtimeSinceStartup - this.mStateStart;
			if (num5 > this.displayTime)
			{
				this.mStateStart = Time.realtimeSinceStartup;
				this.mState = ProgressText.TextState.eFadingOut;
			}
		}
		else if (this.mState == ProgressText.TextState.eHidden)
		{
			this.mTextRenderer.enabled = false;
			if (this.textShadow != null)
			{
				this.textShadow.enabled = false;
			}
		}
		if (this.mState != ProgressText.TextState.eHidden)
		{
			float num6 = Time.realtimeSinceStartup - this.mScaleStart;
			float num7 = num6 / (2f * this.fadeTime + this.displayTime);
			base.transform.localScale = this.mOriginalScale * Mathf.Lerp(this.minScale, this.maxScale, num7);
			if (this.enableFireworks)
			{
				if (this.mFireworkTimer < 0f)
				{
					float num8 = this.mBounds.center.x + Random.Range(-(this.mBounds.extents.x / 2f), this.mBounds.extents.x / 2f);
					float num9 = this.mBounds.center.y + Random.Range(-(this.mBounds.extents.y / 2f), this.mBounds.extents.y / 2f);
					float num10 = -9f;
					GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FX/Sparkle", typeof(GameObject))) as GameObject;
					Vector3 position = Camera.main.transform.position;
					position.x += num8;
					position.y += num9;
					position.z = num10;
					gameObject.transform.position = position;
					this.mFireworkTimer = Random.Range(0.1f, 0.25f);
				}
				else
				{
					this.mFireworkTimer -= Time.realtimeSinceStartup - this.mLastFrameTimeStamp;
					this.mLastFrameTimeStamp = Time.realtimeSinceStartup;
				}
			}
		}
	}

	public bool Show
	{
		set
		{
			if (value && this.mState == ProgressText.TextState.eHidden)
			{
				this.mState = ProgressText.TextState.eFadingIn;
				this.mTextRenderer.enabled = true;
				if (this.textShadow != null)
				{
					this.textShadow.enabled = true;
				}
				this.mStateStart = (this.mScaleStart = Time.realtimeSinceStartup);
			}
			else
			{
				this.mState = ProgressText.TextState.eHidden;
				this.mTextRenderer.enabled = false;
				if (this.textShadow != null)
				{
					this.textShadow.enabled = false;
				}
			}
		}
	}

	public float fadeTime;

	public float displayTime;

	public float minScale;

	public float maxScale;

	public MeshRenderer textShadow;

	public bool enableFireworks;

	private float mStateStart;

	private float mScaleStart;

	private float mFireworkTimer;

	private MeshRenderer mTextRenderer;

	private ProgressText.TextState mState = ProgressText.TextState.eHidden;

	private Vector3 mOriginalScale;

	private Bounds mBounds;

	private float mLastFrameTimeStamp;

	private enum TextState
	{
		eFadingIn,
		eFadingOut,
		eDisplayed,
		eHidden,
		TextState_COUNT
	}
}
