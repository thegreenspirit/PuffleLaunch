using System;
using UnityEngine;

public class AboutCPGUI
{
	public AboutCPGUI(GameObject[] aRefObj)
	{
		this.mb_isInitialized = false;
		this.m_PageControl = new GUIDefines.PageControlData[aRefObj.Length];
		for (int i = 0; i < aRefObj.Length; i++)
		{
			this.m_PageControl[i] = new GUIDefines.PageControlData();
			this.m_PageControl[i].refObj = aRefObj[i];
			this.m_PageControl[i].refTransform = aRefObj[i].transform;
		}
		this.CurrentPage = GameFlowManager.Instance.GUIManager.AboutCPCurrentPage;
		this.DestinationPage = 1;
	}

	public bool IsInitialized
	{
		get
		{
			return this.mb_isInitialized;
		}
	}

	public void InitCPGUI(Texture2D[] aTextures)
	{
		this.m_Textures = aTextures;
		this.CurrentPageTransform.GetComponent<Renderer>().material.mainTexture = this.m_Textures[this.CurrentPage];
		this.DestinationPageTransform.GetComponent<Renderer>().material.mainTexture = this.m_Textures[this.DestinationPage];
		this.mb_isInitialized = true;
	}

	public void Start()
	{
		this.CurrentPageTransform.localPosition = this.m_CenterPoint;
		this.DestinationPageTransform.localPosition = this.m_OutOfRightScreenPoint;
	}

	private Transform CurrentPageTransform
	{
		get
		{
			return this.m_PageControl[this.m_CurrentPage].refTransform;
		}
	}

	private Transform DestinationPageTransform
	{
		get
		{
			return this.m_PageControl[this.m_DestinationPage].refTransform;
		}
	}

	private int FirstItemInCurrentPage
	{
		get
		{
			return this.m_PageControl[this.m_CurrentPage].firstInPage;
		}
		set
		{
			this.m_PageControl[this.m_CurrentPage].firstInPage = value;
		}
	}

	private int FirstItemInDestinationPage
	{
		get
		{
			return this.m_PageControl[this.m_DestinationPage].firstInPage;
		}
		set
		{
			this.m_PageControl[this.m_DestinationPage].firstInPage = value;
		}
	}

	public int CurrentPage
	{
		get
		{
			return this.m_PageControl[this.m_CurrentPage].PageNumber;
		}
		set
		{
			this.m_PageControl[this.m_CurrentPage].PageNumber = value;
		}
	}

	public int DestinationPage
	{
		get
		{
			return this.m_PageControl[this.m_DestinationPage].PageNumber;
		}
		set
		{
			this.m_PageControl[this.m_DestinationPage].PageNumber = value;
		}
	}

	public int TotalPage
	{
		get
		{
			return 5;
		}
	}

	public bool ScrollDone
	{
		get
		{
			return this.DestinationPage == this.CurrentPage;
		}
	}

	public void Draw()
	{
	}

	public void RegisterCallback(AboutCPGUI.PageChangeCallback aCallback)
	{
		this.m_Callback = aCallback;
	}

	public void UpdateScroll()
	{
		Vector3 vector = this.m_TargetPoint - this.CurrentPageTransform.localPosition;
		Vector3 vector2 = this.CurrentPageTransform.localPosition - this.m_StartPageAnchorPoint;
		float num = Mathf.Abs(vector.x);
		if (vector2.x == 0f && !this.ScrollDone)
		{
			this.DestinationPage = this.CurrentPage;
			this.CurrentPageTransform.localPosition = (this.m_StartPageAnchorPoint = this.m_CenterPoint);
		}
		else if (vector2.x <= -30f)
		{
			this.ChangeToNextPage();
		}
		else if (vector2.x >= 30f)
		{
			this.ChangeToPreviousPage();
		}
		if (vector.x < 0f)
		{
			if (this.CurrentPage < this.TotalPage - 1 && vector2.x <= 0f)
			{
				this.ShowNextPage();
			}
			else if (vector2.x <= 0f)
			{
				this.m_TargetPoint = this.CurrentPageTransform.localPosition;
				vector = Vector3.zero;
				num = 0f;
			}
		}
		else if (vector.x > 0f)
		{
			if (this.CurrentPage > 0 && vector2.x >= 0f)
			{
				this.ShowPreviousPage();
			}
			else if (vector2.x >= 0f)
			{
				this.m_TargetPoint = this.CurrentPageTransform.localPosition;
				vector = Vector3.zero;
				num = 0f;
			}
		}
		if (Mathf.Abs(num) > 0f)
		{
			float num2 = Time.deltaTime * (10.5f + 189.5f * (num / 30f));
			Vector3 vector3 = vector;
			vector3.Normalize();
			vector3 *= Mathf.Min(num2, num);
			this.CurrentPageTransform.localPosition += vector3;
			this.DestinationPageTransform.localPosition += vector3;
		}
	}

	public void StartManualScroll()
	{
		this.m_TargetPoint = this.CurrentPageTransform.localPosition;
	}

	public void ManualScroll(Vector2 av2_scrollMovement)
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(av2_scrollMovement) - Camera.main.ScreenToWorldPoint(Vector2.zero);
		vector.y = 0f;
		this.m_TargetPoint += vector;
	}

	public void RecenterScroll()
	{
		Vector3 vector = this.m_StartPageAnchorPoint - this.m_TargetPoint;
		if (vector.x != 0f)
		{
			Vector3 vector2 = Vector3.zero;
			if (vector.x > 0f)
			{
				vector2 = this.m_OutOfLeftScreenPoint;
			}
			else
			{
				vector2 = this.m_OutOfRightScreenPoint;
			}
			if (Mathf.Abs(vector.x) > 2.25f && this.DestinationPage != this.CurrentPage)
			{
				if ((vector.x >= 0f || this.CurrentPage != 0) && (vector.x <= 0f || this.CurrentPage != this.TotalPage - 1))
				{
					this.m_TargetPoint = vector2;
				}
				else
				{
					this.m_TargetPoint = this.m_StartPageAnchorPoint;
				}
			}
			else
			{
				this.m_TargetPoint = this.m_StartPageAnchorPoint;
			}
		}
	}

	private void ShowNextPage()
	{
		if (this.DestinationPage <= this.CurrentPage)
		{
			this.DestinationPage = this.CurrentPage + 1;
			this.CurrentPageTransform.localPosition = (this.m_StartPageAnchorPoint = this.m_CenterPoint);
			this.DestinationPageTransform.localPosition = (this.m_DestinationPageAnchorPoint = this.m_OutOfRightScreenPoint);
			this.DestinationPageTransform.GetComponent<Renderer>().material.mainTexture = this.m_Textures[this.DestinationPage];
		}
	}

	private void ChangeToNextPage()
	{
		this.CurrentPage = this.DestinationPage;
		this.CurrentPageTransform.localPosition = (this.m_StartPageAnchorPoint = this.m_CenterPoint);
		this.CurrentPageTransform.GetComponent<Renderer>().material.mainTexture = this.m_Textures[this.CurrentPage];
		this.DestinationPageTransform.localPosition = (this.m_DestinationPageAnchorPoint = this.m_OutOfRightScreenPoint);
		if (this.m_Callback != null)
		{
			this.m_Callback();
		}
	}

	private void ShowPreviousPage()
	{
		if (this.DestinationPage >= this.CurrentPage)
		{
			this.DestinationPage = this.CurrentPage - 1;
			this.CurrentPageTransform.localPosition = (this.m_StartPageAnchorPoint = this.m_CenterPoint);
			this.DestinationPageTransform.localPosition = (this.m_DestinationPageAnchorPoint = this.m_OutOfLeftScreenPoint);
			this.DestinationPageTransform.GetComponent<Renderer>().material.mainTexture = this.m_Textures[this.DestinationPage];
		}
	}

	private void ChangeToPreviousPage()
	{
		this.CurrentPage = this.DestinationPage;
		this.CurrentPageTransform.localPosition = (this.m_StartPageAnchorPoint = this.m_CenterPoint);
		this.CurrentPageTransform.GetComponent<Renderer>().material.mainTexture = this.m_Textures[this.CurrentPage];
		this.DestinationPageTransform.localPosition = (this.m_DestinationPageAnchorPoint = this.m_OutOfLeftScreenPoint);
		if (this.m_Callback != null)
		{
			this.m_Callback();
		}
	}

	private const float kScrollSpeedMinimum = 10.5f;

	private const float kScrollSpeedMaximum = 200f;

	private const float kMinimum3DDistanceForScroll = 2.25f;

	private const int kNumOfPageControls = 5;

	private const float kReferenceScreenHeight = 640f;

	private bool mb_isInitialized;

	private Vector3 m_CenterPoint = new Vector3(0f, 0f, 0f);

	private Vector3 m_OutOfLeftScreenPoint = new Vector3(-30f, 0f, 0f);

	private Vector3 m_OutOfRightScreenPoint = new Vector3(30f, 0f, 0f);

	private GUIDefines.PageControlData[] m_PageControl;

	private int m_CurrentPage;

	private int m_DestinationPage = 1;

	private Texture2D[] m_Textures;

	public Vector3 m_StartPageAnchorPoint = new Vector3(0f, 0f, 0f);

	public Vector3 m_DestinationPageAnchorPoint = new Vector3(0f, 0f, 0f);

	public Vector3 m_TargetPoint;

	private AboutCPGUI.PageChangeCallback m_Callback;

	public delegate void PageChangeCallback();
}
