using System;
using UnityEngine;

public class InGameHud : BaseGUI
{
	public InGameHud(GameObject aRefObj)
		: base(aRefObj)
	{
		this.mv2_slowmotionButtonCenterPixelPosition = new Vector2((float)(((double)this.mv2_slowmotionButtonPositionRatio.x + 0.5 * (double)this.mv2_slowmotionButtonSizeRatio.x) * (double)GUIConstants.kReferenceScreenWidth), (float)(((double)this.mv2_slowmotionButtonPositionRatio.y + 0.5 * (double)this.mv2_slowmotionButtonSizeRatio.y) * (double)Screen.height));
		this.mb_isInitialized = false;
		this.mo_pauseButton = null;
		this.mo_slowMoButton = null;
		this.mo_timer = null;
		this.mo_timerShadow = null;
		this.mi_loadFrames = 10;
	}

	public void OnPause(object sender, EventArgs e)
	{
		GameFlowManager.Instance.AudioManager.PlayUISFx(GameFlowManager.Instance.MenuClick24);
		GameFlowManager.Instance.GUIManager.ShowPauseMenu(true);
	}

	public void OnSlowMo(object sender, EventArgs e)
	{
		if (this.mSlowMoButtonEnable)
		{
			if (this.mo_slowMoButton.mb_toggleState)
			{
				GameManager.Instance.ActivatePlayerSlowMo();
			}
			else
			{
				GameManager.Instance.StopPlayerSlowMo();
			}
		}
	}

	protected override void CreateLayouts()
	{
	}

	private bool Init()
	{
		if (this.mi_loadFrames > 0)
		{
			this.mi_loadFrames--;
			return false;
		}
		if (!this.mb_isInitialized)
		{
			Camera mainCamera = Camera.main;
			float num = mainCamera.orthographicSize * mainCamera.aspect;
			if (this.mo_pauseButton == null && GameObject.Find("PauseButton"))
			{
				this.mo_pauseButton = GameObject.Find("PauseButton").GetComponent<CustomButton3D>();
				if (this.mo_pauseButton != null)
				{
					Bounds bounds = this.mo_pauseButton.GetComponent<MeshFilter>().GetComponent<Renderer>().bounds;
					this.m_WorkingVector = this.mo_pauseButton.transform.position;
					this.m_WorkingVector.x = 1.5f * bounds.extents.x - num;
					this.mo_pauseButton.transform.position = this.m_WorkingVector;
					this.mo_pauseButton.mf_detectionZoneScale = 5f;
					this.mo_pauseButton.InitButtonBounds();
					this.mo_pauseButton.customOnSelect += this.OnPause;
					this.mo_pauseButton.gameObject.SetActive(false);
				}
			}
			if (this.mo_slowMoButton == null && GameObject.Find("SlowMoButton"))
			{
				this.mo_slowMoButton = GameObject.Find("SlowMoButton").GetComponent<CustomButton3D>();
				if (this.mo_slowMoButton != null)
				{
					Bounds bounds2 = this.mo_slowMoButton.GetComponent<MeshFilter>().GetComponent<Renderer>().bounds;
					this.m_WorkingVector = this.mo_slowMoButton.transform.position;
					this.m_WorkingVector.x = 1.5f * bounds2.extents.x - num;
					this.mo_slowMoButton.transform.position = this.m_WorkingVector;
					this.mo_slowMoButton.mf_detectionZoneScale = 5f;
					this.mo_slowMoButton.InitButtonBounds();
					this.mo_slowMoButton.customOnSelect += this.OnSlowMo;
					this.mo_slowMoButton.gameObject.SetActive(false);
				}
			}
			if (this.mo_timer == null || this.mo_timerShadow == null)
			{
				if (this.mo_timer == null && GameObject.Find("Timer"))
				{
					this.mo_timer = GameObject.Find("Timer").GetComponent<TextMesh>();
				}
				if (this.mo_timerShadow == null && GameObject.Find("TimerShadow"))
				{
					this.mo_timerShadow = GameObject.Find("TimerShadow").GetComponent<TextMesh>();
				}
				if (this.mo_timer != null && this.mo_timerShadow != null)
				{
					this.UpdateTimeDisplay();
					this.mo_timer.gameObject.SetActive(false);
					this.mo_timerShadow.gameObject.SetActive(false);
				}
			}
			if (this.mo_pauseButton != null && this.mo_slowMoButton != null && this.mo_timer != null && this.mo_timerShadow != null)
			{
				this.mb_isInitialized = true;
			}
		}
		return this.mb_isInitialized;
	}

	public void Update()
	{
		if (this.Init())
		{
			bool flag = StartOfGameDelay.Instance == null && GameFlowManager.Instance.GUIManager.CurrentScene != GUIManager.Scene.eTallyMenu && this.mb_isVisible;
			this.mo_pauseButton.gameObject.SetActive(flag);
			this.mo_slowMoButton.gameObject.SetActive(GameManager.HasCompletedTurboMode(GameManager.Instance.CurrentWorld) && flag);
			this.mo_timer.gameObject.SetActive(GameManager.HasCollectedAllRings(GameManager.Instance.CurrentWorld) && flag);
			this.mo_timerShadow.gameObject.SetActive(GameManager.HasCollectedAllRings(GameManager.Instance.CurrentWorld) && flag);
			this.UpdateTimeDisplay();
		}
	}

	public void SetVisible(bool ab_isVisible)
	{
		this.mb_isVisible = ab_isVisible;
		bool flag = StartOfGameDelay.Instance == null && GameFlowManager.Instance.GUIManager.CurrentScene != GUIManager.Scene.eTallyMenu && this.mb_isVisible;
		this.mo_pauseButton.gameObject.SetActive(flag);
		this.mo_slowMoButton.gameObject.SetActive(flag);
		this.mo_timer.gameObject.SetActive(flag);
		this.mo_timerShadow.gameObject.SetActive(flag);
	}

	protected override void OnButtonSelect()
	{
		InGameHud.Button selectedButton = (InGameHud.Button)base.SelectedButton;
		if (selectedButton == InGameHud.Button.eCompleteLevel)
		{
			if (GameObject.FindGameObjectWithTag("Player") != null && GameObject.FindGameObjectWithTag("Finish") != null)
			{
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("Finish").transform.position;
			}
		}
		this.ResetButton();
	}

	public void SetSlowmoButtonState(bool aActive)
	{
		this.mo_slowMoButton.mb_toggleState = aActive;
	}

	public void SetSlowmoButtonVisible(bool aVisible)
	{
		this.mo_slowMoButton.gameObject.SetActive(aVisible);
	}

	public void SetSlowMoButtonEnable(bool aEnable)
	{
		this.mSlowMoButtonEnable = aEnable;
		this.mo_slowMoButton.DisableTouch(!aEnable);
	}

	private void UpdateTimeDisplay()
	{
		if (this.mo_timer != null && this.mo_timer.gameObject.activeSelf)
		{
			this.mo_timer.text = GameManager.GetTimeFormatedString(GameManager.smCurrentTimeCount);
		}
		if (this.mo_timerShadow != null && this.mo_timerShadow.gameObject.activeSelf)
		{
			this.mo_timerShadow.text = GameManager.GetTimeFormatedString(GameManager.smCurrentTimeCount);
		}
	}

	private int mi_loadFrames;

	public bool mb_isInitialized;

	public CustomButton3D mo_pauseButton;

	public CustomButton3D mo_slowMoButton;

	public TextMesh mo_timer;

	public TextMesh mo_timerShadow;

	private Vector3 m_WorkingVector = default(Vector3);

	private bool mSlowMoButtonEnable = true;

	public Vector2 mv2_slowmotionButtonPositionRatio = new Vector2(0.0175f, 0.875f);

	public Vector2 mv2_slowmotionButtonSizeRatio = new Vector2(0.084375f, 0.1046875f);

	public Vector2 mv2_slowmotionButtonCenterPixelPosition;

	private bool mb_isVisible = true;

	public enum Button
	{
		ePause,
		eSlowMo,
		eCompleteLevel,
		eButton_COUNT
	}
}
