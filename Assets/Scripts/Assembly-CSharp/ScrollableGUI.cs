using System;
using UnityEngine;

public abstract class ScrollableGUI : BaseGUI
{
	public ScrollableGUI(GameObject aRefObj)
		: base(aRefObj)
	{
		this.mf_scrollPercentage = 0f;
	}

	public ScrollableGUI()
	{
	}

	public float ScrollPercentage
	{
		get
		{
			return this.mf_scrollPercentage;
		}
		set
		{
			this.mf_scrollPercentage = value;
		}
	}

	protected void InitScrollArea(GUIDefines.RectInfo ao_scrollableArea, ScrollableGUI.ScrollDirection ae_ScrollDirection, float af_scrollableDistance)
	{
		ao_scrollableArea.Init();
		this.mo_scrollAreaRect = ao_scrollableArea.inPixel;
		this.me_scrollDirection = ae_ScrollDirection;
		if (this.me_scrollDirection == ScrollableGUI.ScrollDirection.eVertical)
		{
			this.mo_scrollAreaInnerRect = new Rect(0f, 0f, this.mo_scrollAreaRect.width, af_scrollableDistance * this.mo_scrollAreaRect.height);
		}
		else if (this.me_scrollDirection == ScrollableGUI.ScrollDirection.eHorizontal)
		{
			this.mo_scrollAreaInnerRect = new Rect(0f, 0f, af_scrollableDistance * this.mo_scrollAreaRect.width, this.mo_scrollAreaRect.height);
		}
		else
		{
			Utilities.AssertMsg(false, "Unsupported Scrolling type!\n");
		}
		this.mo_scrollAreaDetectZone = new Rect(this.mo_scrollAreaRect.xMin, (float)Screen.height - (this.mo_scrollAreaRect.yMin + this.mo_scrollAreaRect.height), this.mo_scrollAreaRect.width, this.mo_scrollAreaRect.height);
	}

	public virtual void Update()
	{
		bool flag = false;
		if (Input.GetMouseButton(0))
		{
			this.mb_isTouchDown = true;
			this.mv2_touchPosition = Input.mousePosition;
			if (Input.GetMouseButtonDown(0))
			{
				flag = true;
			}
		}
		else
		{
			this.mb_isTouchDown = false;
		}
		if (this.me_scrollDirection == ScrollableGUI.ScrollDirection.eVertical)
		{
			this.mi_touchPosition = (int)this.mv2_touchPosition.y;
		}
		else if (this.me_scrollDirection == ScrollableGUI.ScrollDirection.eHorizontal)
		{
			this.mi_touchPosition = (int)this.mv2_touchPosition.x;
		}
		if (!this.mb_isTouchDown && !this.mb_Scrolling)
		{
			this.mb_renableButtons = true;
		}
		float num = 0f;
		if (this.mb_isTouchDown)
		{
			if (flag)
			{
				this.mi_numSmoothingValues = 0;
				this.mf_scrollSpeed = 0f;
				this.mi_startTouchPosition = (this.mi_previousTouchPosition = this.mi_touchPosition);
				if (this.mo_scrollAreaDetectZone.Contains(this.mv2_touchPosition))
				{
					this.mb_ScrollAreaSelected = true;
				}
				else
				{
					this.mb_ScrollAreaSelected = false;
				}
			}
			if (this.mb_ScrollAreaSelected)
			{
				if ((float)Mathf.Abs(this.mi_touchPosition - this.mi_startTouchPosition) > 10f)
				{
					this.mb_Scrolling = true;
					this.mb_disableButtons = true;
					this.mf_scrollStopTimer = 1f;
				}
				if (this.mb_Scrolling)
				{
					num = (float)(this.mi_touchPosition - this.mi_previousTouchPosition);
					float num2 = num / Time.deltaTime;
					this.mf_scrollSpeed = num2;
					this.mi_numSmoothingValues++;
					this.mi_numSmoothingValues = ((this.mi_numSmoothingValues <= 5) ? this.mi_numSmoothingValues : 5);
					for (int i = this.mi_numSmoothingValues - 1; i > 0; i--)
					{
						this.m_scrollSpeeds[i] = this.m_scrollSpeeds[i - 1];
						this.mf_scrollSpeed += this.m_scrollSpeeds[i];
					}
					this.m_scrollSpeeds[0] = num2;
					this.mf_scrollSpeed /= (float)this.mi_numSmoothingValues;
				}
				this.mi_previousTouchPosition = this.mi_touchPosition;
			}
		}
		else
		{
			if (this.mb_Scrolling)
			{
				this.mf_scrollStopTimer -= Time.deltaTime;
				if (this.mf_scrollStopTimer <= 0f)
				{
					this.mf_scrollStopTimer = 0f;
					this.mb_Scrolling = false;
				}
				num += this.mf_scrollStopTimer * this.mf_scrollSpeed * Time.deltaTime;
			}
			this.mb_ScrollAreaSelected = false;
		}
		if (this.mf_scrollSpeed == 0f)
		{
			this.mb_Scrolling = false;
		}
		this.mf_scrollPosition += num;
		if (this.mf_scrollPosition < 0f)
		{
			this.mf_scrollPosition = 0f;
		}
		else if (this.mf_scrollPosition > this.mo_scrollAreaInnerRect.height - this.mo_scrollAreaRect.height)
		{
			this.mf_scrollPosition = ((this.mo_scrollAreaInnerRect.height - this.mo_scrollAreaRect.height <= 0f) ? 0f : (this.mo_scrollAreaInnerRect.height - this.mo_scrollAreaRect.height));
		}
		if (this.me_scrollDirection == ScrollableGUI.ScrollDirection.eVertical)
		{
			this.mo_scrollAreaInnerRect.y = -this.mf_scrollPosition;
		}
		else if (this.me_scrollDirection == ScrollableGUI.ScrollDirection.eHorizontal)
		{
			this.mo_scrollAreaInnerRect.y = -this.mf_scrollPosition;
		}
		this.mf_scrollPercentage = this.mf_scrollPosition / (this.mo_scrollAreaInnerRect.height - this.mo_scrollAreaRect.height);
		switch (this.me_scrollBarState)
		{
		case ScrollableGUI.ScrollBarState.eInactive:
			if (this.mb_Scrolling)
			{
				this.me_scrollBarState = ScrollableGUI.ScrollBarState.eActive;
			}
			break;
		case ScrollableGUI.ScrollBarState.eActive:
			this.mf_scrollBarAlpha = 1f;
			if (!this.mb_Scrolling && !this.mb_ScrollAreaSelected)
			{
				this.me_scrollBarState = ScrollableGUI.ScrollBarState.eFadeDelay;
				this.mf_scrollBarTimer = this.mf_scrollBarFadeDelayTime;
			}
			break;
		case ScrollableGUI.ScrollBarState.eFadeDelay:
			this.mf_scrollBarAlpha = 1f;
			this.mf_scrollBarTimer -= Time.deltaTime;
			if (this.mf_scrollBarTimer <= 0f)
			{
				this.me_scrollBarState = ScrollableGUI.ScrollBarState.eFading;
			}
			break;
		case ScrollableGUI.ScrollBarState.eFading:
			this.mf_scrollBarAlpha -= this.mf_scrollBarFadeSpeed * Time.deltaTime;
			if (this.mf_scrollBarAlpha <= 0f)
			{
				this.mf_scrollBarAlpha = 0f;
				this.me_scrollBarState = ScrollableGUI.ScrollBarState.eInactive;
			}
			break;
		}
	}

	public void RegisterCallback(ScrollableGUI.ScrollableGUICallback aCallback)
	{
		this.m_Callback = aCallback;
	}

	protected override void OnButtonSelect()
	{
	}

	protected override void OnButtonSelect(int aSelectedButton)
	{
		if (!this.mb_disableButtons && this.m_Callback != null)
		{
			this.m_Callback(aSelectedButton);
		}
	}

	public override void Draw()
	{
		if (this.CanDraw())
		{
			this.DrawScrollListContent();
			if (this.me_scrollBarState != ScrollableGUI.ScrollBarState.eInactive)
			{
				Color color = GUI.color;
				if (this.mf_scrollBarAlpha < 1f)
				{
					Color color2 = GUI.color;
					color2 = Color.white;
					color2.a = this.mf_scrollBarAlpha;
					GUI.color = color2;
				}
				this.DrawScrollBar();
				if (this.mf_scrollBarAlpha < 1f)
				{
					GUI.color = color;
				}
			}
			this.DrawBorders();
		}
		if (this.mb_renableButtons)
		{
			this.mb_renableButtons = false;
			this.mb_disableButtons = false;
		}
	}

	public virtual void DrawScrollListContent()
	{
		GUILayout.BeginArea(this.mo_scrollAreaRect);
		GUILayout.BeginArea(this.mo_scrollAreaInnerRect);
		base.Draw();
		GUILayout.EndArea();
		GUILayout.EndArea();
	}

	public virtual void DrawBorders()
	{
	}

	public virtual void DrawScrollBar()
	{
	}

	public void ResetScrollPosition()
	{
		this.mf_scrollPosition = 0f;
	}

	private const int ci_maxNumSmoothingValues = 5;

	private const float cf_scrollTolerance = 10f;

	private const float cf_scrollStopTime = 1f;

	protected Rect mo_scrollAreaRect;

	protected Rect mo_scrollAreaInnerRect;

	protected ScrollableGUI.ScrollableGUICallback m_Callback;

	public ScrollableGUI.ScrollDirection me_scrollDirection;

	private ScrollableGUI.ScrollBarState me_scrollBarState;

	private float mf_scrollBarTimer;

	public float mf_scrollBarFadeDelayTime = 0.5f;

	public float mf_scrollBarDissapearDelay = 0.5f;

	public float mf_scrollBarFadeSpeed = 3f;

	private float mf_scrollBarAlpha = 1f;

	private bool mb_isTouchDown;

	public float mf_scrollPosition;

	private Rect mo_scrollAreaDetectZone;

	private int mi_numSmoothingValues;

	private float[] m_scrollSpeeds = new float[5];

	private float mf_scrollSpeed;

	private int mi_touchPosition;

	private int mi_startTouchPosition;

	private int mi_previousTouchPosition;

	public bool mb_Scrolling;

	public bool mb_disableButtons;

	public bool mb_renableButtons;

	public bool mb_ScrollAreaSelected;

	private float mf_scrollStopTimer;

	private Vector2 mv2_touchPosition = new Vector2(0f, 0f);

	protected float mf_scrollPercentage;

	public enum ScrollDirection
	{
		eVertical,
		eHorizontal,
		eNone
	}

	private enum ScrollBarState
	{
		eInactive,
		eActive,
		eFadeDelay,
		eFading
	}

	public delegate void ScrollableGUICallback(int aSelectedButton);
}
