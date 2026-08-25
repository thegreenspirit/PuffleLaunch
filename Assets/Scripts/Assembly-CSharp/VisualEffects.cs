using System;
using UnityEngine;

public class VisualEffects : MonoBehaviour
{
	public Transform slowMoFX;
	public Transform tutorialOverlay;
	public Material tutorialMaterial;
	private bool mForceSlowMoFX;
	private float mBaseOrthographicSize;
	private Transform mTutorialObject;

	private Vector3 mScreenRatioAdjustment;
	private Vector3 mScreenSizeInverse;

	private Color mWhite = new Color(1f, 1f, 1f, 0.5f);
	private Color mClearWhite = new Color(1f, 1f, 1f, 0f);

	public Transform TutorialObject
	{
		get { return this.mTutorialObject; }
		set
		{
			this.mTutorialObject = value;
			if (this.mTutorialObject == null)
			{
				this.tutorialOverlay.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(1f, 1f);
			}
		}
	}

	public void Start()
	{
		this.mBaseOrthographicSize = Camera.main.GetComponent<CameraFollow>().OriginalOrthographicSize;
		SpriteMeshGenerator spriteMeshGenerator = new SpriteMeshGenerator(this.slowMoFX.GetComponent<MeshFilter>());
		spriteMeshGenerator.Generate(new Vector2((float)(-(float)Screen.width / 2), (float)(-(float)Screen.height / 2)), new Vector2((float)Screen.width, (float)Screen.height), true);
		this.tutorialOverlay.GetComponent<MeshFilter>().sharedMesh = this.slowMoFX.GetComponent<MeshFilter>().sharedMesh;
		this.tutorialOverlay.transform.localPosition += new Vector3(0f, 0f, 5f);
		this.tutorialMaterial = this.tutorialOverlay.GetComponent<Renderer>().material;
		this.mScreenRatioAdjustment = new Vector3((float)Screen.width / (float)Screen.height, 1f);
		this.mScreenSizeInverse = new Vector3(1f / (float)Screen.width, 1f / (float)Screen.height);
	}

	public void Update()
	{
		if (this.slowMoFX.gameObject.active && !this.mForceSlowMoFX)
		{
			float timeScaleRatio = TimeManager.Instance.TimeScaleRatio;
			if (TimeManager.Instance.TimeScaleRatio == 1f)
			{
				this.slowMoFX.gameObject.active = false;
			}
			else
			{
				this.slowMoFX.GetComponent<Renderer>().material.SetColor("_TintColor", Color.Lerp(this.mWhite, this.mClearWhite, timeScaleRatio));
			}
		}
	}

	public void LateUpdate()
	{
		float num = Camera.main.orthographicSize / this.mBaseOrthographicSize;
		this.slowMoFX.localScale = Vector3.one * num;
		this.tutorialOverlay.localScale = Vector3.one * num;
		if (this.mTutorialObject)
		{
			Vector3 vector = this.mScreenRatioAdjustment;
			vector *= 0.2f * Mathf.Sin(Time.time * 6f) + 2f;
			this.tutorialMaterial.mainTextureScale = vector;
			Vector3 vector2 = Camera.main.WorldToScreenPoint(this.mTutorialObject.position);
			vector.Scale(this.mScreenSizeInverse);
			vector2.Scale(vector);
			this.tutorialMaterial.mainTextureOffset = -vector2 + Vector3.one * 0.5f;
		}
	}

	public void ShowSlowMoFX(bool aShow)
	{
		if (aShow)
		{
			this.slowMoFX.GetComponent<Renderer>().material.SetColor("_TintColor", this.mWhite);
		}
		this.slowMoFX.gameObject.active = aShow;
		this.mForceSlowMoFX = false;
	}

	public void ForceSlowMoFX()
	{
		this.ShowSlowMoFX(true);
		this.mForceSlowMoFX = true;
	}

	public void ShowTutorialFX(bool aShow)
	{
		this.tutorialOverlay.gameObject.active = aShow;
	}
}
