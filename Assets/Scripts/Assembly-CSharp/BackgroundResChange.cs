using System;
using UnityEngine;

public class BackgroundResChange : MonoBehaviour
{
	public float WidthRatio
	{
		get
		{
			if (!this.m_GotWidthRatio)
			{
				this.m_GotWidthRatio = true;
				this.m_WidthRatio = this.m_MyCamera.orthographicSize * this.m_MyCamera.aspect / base.GetComponent<MeshFilter>().GetComponent<Renderer>().bounds.extents.x;
			}
			return this.m_WidthRatio;
		}
	}

	public float HeightRatio
	{
		get
		{
			if (!this.m_GotHeightRatio)
			{
				this.m_GotHeightRatio = true;
				this.m_HeightRatio = this.m_MyCamera.orthographicSize / base.GetComponent<MeshFilter>().GetComponent<Renderer>().bounds.extents.y;
			}
			return this.m_HeightRatio;
		}
	}

	private void Awake()
	{
		this.m_GotWidthRatio = false;
		this.m_GotHeightRatio = false;
		this.m_MyTransform = base.transform;
		this.m_MyMeshRenderer = base.GetComponent<MeshRenderer>();
		if (this.m_AlternateMeshRenderer != null)
		{
			this.m_MyBackgroundBounds = this.m_AlternateMeshRenderer.bounds;
		}
		else
		{
			this.m_MyBackgroundBounds = base.GetComponent<MeshFilter>().GetComponent<Renderer>().bounds;
		}
		if (this.m_MyCamera == null)
		{
			this.m_MyCamera = Camera.main;
		}
		this.m_BottomLeft = this.m_MyCamera.ScreenToWorldPoint(new Vector3(0f, 0f, this.m_MyTransform.position.z));
		this.m_TopRight = this.m_MyCamera.ScreenToWorldPoint(new Vector3((float)Screen.width, (float)Screen.height, this.m_MyTransform.position.z));
	}

	private void Start()
	{
		Vector3 localScale = this.m_MyTransform.localScale;
		if (!this.m_GotWidthRatio)
		{
			this.m_GotWidthRatio = true;
			this.m_WidthRatio = this.m_MyCamera.orthographicSize * this.m_MyCamera.aspect / this.m_MyBackgroundBounds.extents.x;
		}
		if (!this.m_GotHeightRatio)
		{
			this.m_GotHeightRatio = true;
			this.m_HeightRatio = this.m_MyCamera.orthographicSize / this.m_MyBackgroundBounds.extents.y;
		}
		if (this.m_AdjustAspectRatioOnly)
		{
			if (this.m_RotateAxis)
			{
				localScale.z = localScale.x * (this.m_MyBackgroundBounds.extents.x * this.m_MyCamera.aspect);
			}
			else
			{
				localScale.x = localScale.z * (this.m_MyBackgroundBounds.extents.z * this.m_MyCamera.aspect);
			}
		}
		else
		{
			if (this.m_AdjustWidthToScreen)
			{
				if (this.m_RotateAxis)
				{
					localScale.z *= this.m_WidthRatio;
				}
				else
				{
					localScale.x *= this.m_WidthRatio;
				}
			}
			if (this.m_AdjustHeightToScreen)
			{
				if (this.m_RotateAxis)
				{
					localScale.x *= this.m_HeightRatio;
				}
				else
				{
					localScale.z *= this.m_HeightRatio;
				}
			}
		}
		this.m_MyTransform.localScale = localScale;
		this.m_WorkingVector = this.m_MyTransform.position;
		if (this.m_MoveToTopOfScreen)
		{
			this.m_WorkingVector.y = this.m_WorkingVector.y + (this.m_TopRight.y - this.m_MyBackgroundBounds.min.y);
		}
		else if (this.m_MoveToBottomOfScreen)
		{
			this.m_WorkingVector.y = this.m_WorkingVector.y + (this.m_BottomLeft.y - this.m_MyBackgroundBounds.max.y);
		}
		if (this.m_MoveToLeftOfScreen)
		{
			this.m_WorkingVector.x = this.m_WorkingVector.x + (this.m_BottomLeft.x - this.m_MyBackgroundBounds.min.x);
		}
		else if (this.m_MoveToRightOfScreen)
		{
			this.m_WorkingVector.x = this.m_WorkingVector.x + (this.m_TopRight.x - this.m_MyBackgroundBounds.max.x);
		}
		this.m_MyTransform.position = this.m_WorkingVector;
		this.LoadResolutionDependentTexture();
	}

	private void LoadResolutionDependentTexture()
	{
		if (this.m_MyMeshRenderer != null && this.m_BasePath != string.Empty && this.m_TextureName != string.Empty)
		{
			string text = this.m_BasePath + this.m_TextureName;
			if (this.m_IsLocalized)
			{
				string languageCode = LocalizationManager.GetLanguageCode();
				switch (languageCode)
				{
				case "fr":
					text += this.m_LocalizationFRSuffix;
					goto IL_0155;
				case "pt":
					text += this.m_LocalizationPTSuffix;
					goto IL_0155;
				case "es":
					text += this.m_LocalizationESSuffix;
					goto IL_0155;
				case "de":
					text += this.m_LocalizationDESuffix;
					goto IL_0155;
				case "ja":
					text += this.m_LocalizationJASuffix;
					goto IL_0155;
				}
				text += this.m_LocalizationENSuffix;
			}
			IL_0155:
			string text2 = text;
			if ((float)Screen.width <= 480f && (float)Screen.height <= 320f)
			{
				text2 += "_lowres";
			}
			else if ((float)Screen.width == 1024f && (float)Screen.height == 768f)
			{
				text2 += "_iPad";
			}
			this.m_MyMeshRenderer.material.mainTexture = GUIUtil.LoadTexture2D(text2);
			if (this.m_MyMeshRenderer.material.mainTexture == null)
			{
				this.m_MyMeshRenderer.material.mainTexture = GUIUtil.LoadTexture2D(text);
			}
		}
	}

	public Camera m_MyCamera;

	public bool m_RotateAxis;

	public bool m_AdjustAspectRatioOnly;

	public bool m_AdjustWidthToScreen = true;

	public bool m_AdjustHeightToScreen = true;

	public bool m_MoveToTopOfScreen;

	public bool m_MoveToBottomOfScreen;

	public bool m_MoveToLeftOfScreen;

	public bool m_MoveToRightOfScreen;

	public MeshRenderer m_AlternateMeshRenderer;

	public string m_LocalizationENSuffix = "_EN";

	public string m_LocalizationFRSuffix = "_FR";

	public string m_LocalizationPTSuffix = "_PT";

	public string m_LocalizationESSuffix = "_ES";

	public string m_LocalizationDESuffix = "_DE";

	public string m_LocalizationJASuffix = "_JA";

	public string m_BasePath = string.Empty;

	public string m_TextureName = string.Empty;

	public bool m_IsLocalized;

	private bool m_GotWidthRatio;

	private bool m_GotHeightRatio;

	private Transform m_MyTransform;

	private MeshRenderer m_MyMeshRenderer;

	private Bounds m_MyBackgroundBounds;

	private Vector3 m_BottomLeft;

	private Vector3 m_TopRight;

	private float m_WidthRatio = 1f;

	private float m_HeightRatio = 1f;

	private Vector3 m_WorkingVector = default(Vector3);
}
