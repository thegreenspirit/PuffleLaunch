using System;
using UnityEngine;

public class StartOfGameDelay : MonoBehaviour
{
	public static StartOfGameDelay Instance
	{
		get
		{
			return StartOfGameDelay.m_cInstance;
		}
	}

	private void Awake()
	{
		StartOfGameDelay.m_cInstance = this;
	}

	private void Start()
	{
		this.mState = StartOfGameDelay.State.eUnPaused;
		this.m_DoInit = true;
		GameManager.Instance.Pause(true);
		this.mCountdownStarted = false;
		this.mCountdownValue = 0;
		GameObject.Find("TouchIndicator").GetComponent<Renderer>().material.mainTexture = Resources.Load("Textures/TouchIndicator/tap-thing", typeof(Texture2D)) as Texture2D;
	}

	public void OnApplicationPause(bool aPause)
	{
		if (aPause)
		{
			this.mState = StartOfGameDelay.State.ePaused;
			this.mTimeWhenPause = Time.realtimeSinceStartup;
		}
	}

	public void RestartLevel()
	{
		this.mStartTime = Time.realtimeSinceStartup;
		this.mTimePaused = 0f;
	}

	private void Update()
	{
		GameManager.Instance.Pause(true);
		if (!LevelLoader.Instance.isLoadingFinished)
		{
			return;
		}
		if (this.mState == StartOfGameDelay.State.ePaused)
		{
			if (GameFlowManager.Instance.GUIManager.CurrentScene == GUIManager.Scene.ePauseMenu)
			{
				return;
			}
			this.mTimePaused += Time.realtimeSinceStartup - this.mTimeWhenPause;
			this.mState = StartOfGameDelay.State.eUnPaused;
		}
		if (this.m_DoInit)
		{
			this.m_DoInit = false;
			Resources.UnloadUnusedAssets();
			this.mStartTime = Time.realtimeSinceStartup;
			GameFlowManager.Instance.GUIManager.LoadingScreen.TextureData[0].bgInfo.bgColor = new Color(1f, 1f, 1f, 1f);
			if (GameManager.smCurrentLevel == GameManager.Level.eLevel_3 || GameManager.smCurrentLevel == GameManager.Level.eLevel_6)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Tutorial", typeof(GameObject))) as GameObject;
				gameObject.transform.parent = Camera.main.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0f, -100f);
				gameObject.transform.eulerAngles = new Vector3(90f, 180f, 0f);
			}
			return;
		}
		float num = Time.realtimeSinceStartup - this.mStartTime - this.mTimePaused;
		float num2 = 1f - num / this.m_InitialDelay;
		GameFlowManager.Instance.GUIManager.LoadingScreen.TextureData[0].bgInfo.useBgColor = true;
		GameFlowManager.Instance.GUIManager.LoadingScreen.TextureData[0].bgInfo.bgColor.a = num2;
		if (this.mCountdownStarted)
		{
			if (num > this.m_InitialDelay + this.m_CountdownDelay * (float)(this.mCountdownValue + 1))
			{
				this.mCountdownValue++;
				if (this.m_CountdownCount - this.mCountdownValue <= 0)
				{
					if (GameFlowManager.Instance.GUIManager.CurrentScene != GUIManager.Scene.ePauseMenu)
					{
						GameManager.Instance.Pause(false);
						Puffle.Instance.StopMovement = false;
					}
					GameManager.Instance.EnableTiming = true;
					GameFlowManager.Instance.GUIManager.LoadingScreen.TextureData[0].bgInfo.bgColor.a = 1f;
					global::UnityEngine.Object.Destroy(this.m_CountdownText.gameObject);
					global::UnityEngine.Object.Destroy(base.gameObject);
				}
				else
				{
					this.m_CountdownText.Show = false;
					string text = (this.m_CountdownCount - this.mCountdownValue).ToString();
					this.m_CountdownText.GetComponent<TextMesh>().text = text;
					if (this.m_CountdownText.textShadow != null)
					{
						this.m_CountdownText.textShadow.GetComponent<TextMesh>().text = text;
					}
					this.m_CountdownText.Show = true;
				}
			}
		}
		else if (num > this.m_InitialDelay)
		{
			if ((GameManager.smCurrentLevel == GameManager.Level.eLevel_3 || GameManager.smCurrentLevel == GameManager.Level.eLevel_6) && TutorialPopup.Instance != null)
			{
				GameManager.Instance.Pause(false);
				Puffle.Instance.StopMovement = true;
				return;
			}
			this.mStartTime = Time.realtimeSinceStartup - this.m_InitialDelay - this.mTimePaused;
			GameFlowManager.Instance.GUIManager.HideLoadingScreen(false);
			GameFlowManager.Instance.GUIManager.LoadingScreen.TextureData[0].bgInfo.useBgColor = false;
			this.mCountdownStarted = true;
			this.m_CountdownText.GetComponent<TextMesh>().text = this.m_CountdownCount.ToString();
			if (this.m_CountdownText.textShadow != null)
			{
				this.m_CountdownText.textShadow.GetComponent<TextMesh>().text = this.m_CountdownCount.ToString();
			}
			this.m_CountdownText.Show = true;
		}
	}

	public float m_InitialDelay;

	public float m_CountdownDelay;

	public int m_CountdownCount;

	public ProgressText m_CountdownText;

	private static StartOfGameDelay m_cInstance;

	private float mStartTime;

	private bool mCountdownStarted;

	private int mCountdownValue;

	private bool m_DoInit = true;

	private StartOfGameDelay.State mState;

	private float mTimeWhenPause;

	private float mTimePaused;

	private enum State
	{
		ePaused,
		eUnPaused,
		eStateCount
	}
}
