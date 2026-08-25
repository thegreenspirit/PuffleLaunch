using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimatedText : MonoBehaviour
{
	private void Start()
	{
		this.mOriginalScale = base.transform.localScale;
		this.mState = AnimatedText.TextState.eHidden;
		this.mTextRenderer = base.GetComponent<MeshRenderer>();
		this.mTextRenderer.material.color = this.textColor;
		this.mTextRenderer.enabled = false;
		if (this.textShadow != null)
		{
			this.textShadow.enabled = false;
		}
		base.transform.localScale = this.mOriginalScale * this.minScale;
		this.mLastFrameTimeStamp = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		switch (this.mState)
		{
		case AnimatedText.TextState.eFadingIn:
			this.FadeIn();
			break;
		}
		if (this.mState != AnimatedText.TextState.eHidden)
		{
			this.ScaleUpText();
			float num = Time.realtimeSinceStartup - this.mStateStart;
			if (this.enableFireworks && num <= this.fireworkDisplayTime)
			{
				this.FireworkEffect();
			}
		}
	}

	private void FadeIn()
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
			this.mState = AnimatedText.TextState.eDisplayed;
		}
	}

	private void ScaleUpText()
	{
		float num = Time.realtimeSinceStartup - this.mScaleStart;
		float num2 = num / this.scaleTime;
		base.transform.localScale = this.mOriginalScale * Mathf.Lerp(this.minScale, this.maxScale, num2);
	}

	private void FireworkEffect()
	{
		if (this.mFireworkTimer < 0f)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/FX/AnimatedSparkle", typeof(GameObject))) as GameObject;
			gameObject.layer = base.gameObject.layer;
			Vector3 localScale = gameObject.transform.localScale;
			localScale.x *= this.fireworkScale;
			localScale.y *= this.fireworkScale;
			gameObject.transform.localScale = localScale;
			Vector3 position = base.transform.position;
			gameObject.transform.position = position;
			this.mFireworkTimer = Random.Range(0.1f, 0.25f);
		}
		else
		{
			this.mFireworkTimer -= Time.realtimeSinceStartup - this.mLastFrameTimeStamp;
			this.mLastFrameTimeStamp = Time.realtimeSinceStartup;
		}
	}

	public bool Show
	{
		set
		{
			if (value && this.mState == AnimatedText.TextState.eHidden)
			{
				this.mState = AnimatedText.TextState.eFadingIn;
				this.mTextRenderer.enabled = true;
				if (this.textShadow != null)
				{
					this.textShadow.enabled = true;
				}
				this.mStateStart = (this.mScaleStart = Time.realtimeSinceStartup);
			}
			else
			{
				this.mState = AnimatedText.TextState.eHidden;
				this.mTextRenderer.enabled = false;
				if (this.textShadow != null)
				{
					this.textShadow.enabled = false;
				}
			}
		}
	}

	public Color textColor;

	public float fadeTime;

	public float scaleTime;

	public float minScale;

	public float maxScale;

	public MeshRenderer textShadow;

	public bool enableFireworks;

	public float fireworkDisplayTime;

	public float fireworkScale;

	private float mStateStart;

	private float mScaleStart;

	private float mFireworkTimer;

	private MeshRenderer mTextRenderer;

	private AnimatedText.TextState mState = AnimatedText.TextState.eHidden;

	private Vector3 mOriginalScale;

	private float mLastFrameTimeStamp;

	private enum TextState
	{
		eFadingIn,
		eDisplayed,
		eHidden,
		TextState_COUNT
	}
}
