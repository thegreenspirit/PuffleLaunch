using System;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
	private static CinematicManager m_cInstance;

	private Dictionary<CinematicManager.MovieId, CinematicManager.CinematicData> m_CinematicList = new Dictionary<CinematicManager.MovieId, CinematicManager.CinematicData>();
	private CinematicManager.CinematicData m_CurrentMovieData;
	private CinematicManager.MovieState m_MovieState;

	private bool m_ShowFullscreenBgWhenPlaying = true;

	private GameObject m_FullscreenBgObj;
	private MeshRenderer m_FullscreenBgMesh;

	private GameObject m_AboutCPPlayFailedObj;
	private AboutCPMoviePlayFailed m_AboutCPPlayFailed;

	private bool mErrorPopInitialized;

	public enum MovieId { eIntro, eAboutCP, eMovieId_COUNT }
	public enum MovieType { eUnknown = -1, eNetwork, eLocal, eMovieType_COUNT }
	public enum MovieState { eIdle, eReadyToPlay, ePlaySucceeded, ePlayFailed, eMovieState_COUNT }

	private class CinematicData
	{
		public string movieURL = string.Empty;
		public CinematicManager.MovieType movieType = CinematicManager.MovieType.eUnknown;
		//public FullScreenMovieControlMode movieControlMode;

		public CinematicData(string aMovieURL)
		{
			if (aMovieURL != null)
			{
				this.movieURL = aMovieURL;
				this.movieType = ((!this.IsNetworkBaseURL(this.movieURL)) ? CinematicManager.MovieType.eLocal : CinematicManager.MovieType.eNetwork);
			}
		}

		public event CinematicManager.PlayFailedHandler playFailed;

		public bool IsValid()
		{
			return this.movieURL != null && this.movieURL.Length > 0 && this.movieType != CinematicManager.MovieType.eUnknown;
		}

		public bool IsNetworkBaseURL(string aMovieURL)
		{
			return aMovieURL.ToLower().Contains("http://") || aMovieURL.ToLower().Contains("file://");
		}

		public void OnPlayFailed()
		{
			if (this.playFailed != null) this.playFailed();
		}

		public bool HasPlayFailedHandler()
		{
			return this.playFailed != null;
		}
	}

	public delegate void PlayFailedHandler();
	public delegate void PlayCompletedHandler(bool aSuccess);
	public event CinematicManager.PlayCompletedHandler playCompleted;

	public static CinematicManager Instance
	{
		get
		{
			if (CinematicManager.m_cInstance == null)
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Managers/CinematicManager", typeof(GameObject))) as GameObject;
				Utilities.AssertMsg(gameObject != null, "Fail to instantiate CinematicManager from prefab!");
			}
			return CinematicManager.m_cInstance;
		}
	}

	public bool ShowFullscreenBgWhenPlaying
	{
		get { return this.m_ShowFullscreenBgWhenPlaying; }
		set { this.m_ShowFullscreenBgWhenPlaying = value; }
	}

	private void Awake()
	{
		CinematicManager.m_cInstance = this;
		this.CreateCinematicList();
		this.m_FullscreenBgObj = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/FullscreenBG", typeof(GameObject))) as GameObject;
		Utilities.AssertMsg(this.m_FullscreenBgObj != null, "Fail to instantiate FullscreenBG from prefab!");
		this.m_FullscreenBgMesh = this.m_FullscreenBgObj.GetComponent<MeshRenderer>();
		Utilities.AssertMsg(this.m_FullscreenBgObj != null, "Fail to get MeshRenderer component from FullscreenBG object!");
		if (this.m_FullscreenBgMesh != null)
		{
			this.m_FullscreenBgMesh.enabled = false;
		}
	}

	private void Update()
	{
		switch (this.m_MovieState)
		{
		case CinematicManager.MovieState.eReadyToPlay:
			this.PlayMovie(this.m_CurrentMovieData);
			break;
		case CinematicManager.MovieState.ePlaySucceeded:
			this.ChangeMovieState(CinematicManager.MovieState.eIdle);
			break;
		case CinematicManager.MovieState.ePlayFailed:
			if (this.m_CurrentMovieData != null && this.m_CurrentMovieData.HasPlayFailedHandler())
			{
				this.m_CurrentMovieData.OnPlayFailed();
			}
			else
			{
				this.ChangeMovieState(CinematicManager.MovieState.eIdle);
			}
			break;
		}
	}

	private void OnDestroy()
	{
		CinematicManager.m_cInstance = null;
	}

	public static void Destory()
	{
		if (CinematicManager.m_cInstance != null)
		{
			global::UnityEngine.Object.Destroy(CinematicManager.m_cInstance);
		}
	}

	private const string kIntroMovieURL = "Trailer_480x320.m4v";
	private const string kAboutCPMovieEN = "http://wpc.176f.edgecastcdn.net/80176F/external01.tapulous.com/content/CPVideos/CPVideo_en_Android.mp4";
	private const string kAboutCPMovieFR = "http://wpc.176f.edgecastcdn.net/80176F/external01.tapulous.com/content/CPVideos/CPVideo_fr_Android.mp4";
	private const string kAboutCPMovieGER = "http://wpc.176f.edgecastcdn.net/80176F/external01.tapulous.com/content/CPVideos/CPVideo_ger_Android.mp4";
	private const string kAboutCPMoviePT = "http://wpc.176f.edgecastcdn.net/80176F/external01.tapulous.com/content/CPVideos/CPVideo_pt_Android.mp4";
	private const string kAboutCPMovieARG = "http://wpc.176f.edgecastcdn.net/80176F/external01.tapulous.com/content/CPVideos/CPVideo_arg_Android.mp4";
	private const string kAboutCPMovieES = "http://wpc.176f.edgecastcdn.net/80176F/external01.tapulous.com/content/CPVideos/CPVideo_es_Android.mp4";

	private void CreateCinematicList()
	{
		this.m_CinematicList[CinematicManager.MovieId.eIntro] = new CinematicManager.CinematicData(kIntroMovieURL)
		{
			// Green Spirit: changed CancelOnTouch
			//movieControlMode = FullScreenMovieControlMode.CancelOnInput
		};

		string text = string.Empty;
		string languageCode = LocalizationManager.GetLanguageCode();

		switch (languageCode)
		{
			case "fr":
				text = kAboutCPMovieFR;
				goto Callback;
			case "pt":
				text = kAboutCPMoviePT;
				goto Callback;
			case "es":
				if (LocalizationManager.GetRegionCode() == "es_AR")
				{
					text = kAboutCPMovieARG;
				}
				else
				{
					text = kAboutCPMovieES;
				}
				goto Callback;
			case "de":
				text = kAboutCPMovieGER;
				goto Callback;
		}

		// fallback to english
		text = kAboutCPMovieEN;

		Callback:
		this.m_CinematicList[CinematicManager.MovieId.eAboutCP] = new CinematicManager.CinematicData(text);
		this.m_CinematicList[CinematicManager.MovieId.eAboutCP].playFailed += this.OnAboutCPMoviePlayFailed;
	}

	private CinematicManager.CinematicData GetCinematicData(CinematicManager.MovieId aMovieId)
	{
		CinematicManager.CinematicData cinematicData = null;
		if (!this.m_CinematicList.TryGetValue(aMovieId, out cinematicData))
		{
			Utilities.AssertMsg(false, "Movie: '" + aMovieId + "' not found in cinematic list!");
			return null;
		}
		return cinematicData;
	}

	public void Play(CinematicManager.MovieId aMovieId)
	{
		this.m_CurrentMovieData = this.GetCinematicData(aMovieId);
		this.ChangeMovieState(CinematicManager.MovieState.eReadyToPlay);
	}

	public void Play(string aMovieURL)
	{
		this.m_CurrentMovieData = new CinematicManager.CinematicData(aMovieURL);
		this.ChangeMovieState(CinematicManager.MovieState.eReadyToPlay);
	}

	private void PlayMovie(CinematicManager.CinematicData aCineData)
	{
		if (aCineData == null || !aCineData.IsValid())
		{
			return;
		}
		AudioManager.Instance.Mute();
		CinematicManager.MovieType movieType = this.m_CurrentMovieData.movieType;
		if (movieType != CinematicManager.MovieType.eNetwork)
		{
			if (movieType != CinematicManager.MovieType.eLocal)
			{
				this.ChangeMovieState(CinematicManager.MovieState.ePlayFailed);
			}
			else
			{
				//Handheld.PlayFullScreenMovie(this.m_CurrentMovieData.movieURL, Color.black, this.m_CurrentMovieData.movieControlMode, FullScreenMovieScalingMode.AspectFit);
				this.ChangeMovieState(CinematicManager.MovieState.ePlaySucceeded);
			}
		}
		else if (this.IsNetworkReachable())
		{
			//Handheld.PlayFullScreenMovie(this.m_CurrentMovieData.movieURL, Color.black, this.m_CurrentMovieData.movieControlMode, FullScreenMovieScalingMode.AspectFit);
			this.ChangeMovieState(CinematicManager.MovieState.ePlaySucceeded);
		}
		else
		{
			this.ChangeMovieState(CinematicManager.MovieState.ePlayFailed);
		}
	}

	private void ChangeMovieState(CinematicManager.MovieState aNewState)
	{
		if (aNewState >= CinematicManager.MovieState.eMovieState_COUNT)
		{
			Utilities.AssertMsg(false, "Unknown movie state: " + aNewState);
			return;
		}
		if (aNewState != CinematicManager.MovieState.eIdle)
		{
			if (aNewState == CinematicManager.MovieState.eReadyToPlay)
			{
				if (this.m_ShowFullscreenBgWhenPlaying)
				{
					this.ShowFullscreenBg(true);
				}
			}
		}
		else
		{
			this.m_CurrentMovieData = null;
			if (this.playCompleted != null)
			{
				bool flag = this.m_MovieState == CinematicManager.MovieState.ePlaySucceeded;
				this.playCompleted(flag);
				this.playCompleted = null;
			}
			if (this.m_ShowFullscreenBgWhenPlaying)
			{
				this.ShowFullscreenBg(false);
			}
			AudioManager.Instance.Unmute();
		}
		this.m_MovieState = aNewState;
	}

	private void ShowFullscreenBg(bool aShow)
	{
		if (this.m_FullscreenBgMesh != null)
		{
			this.m_FullscreenBgMesh.enabled = aShow;
		}
	}

	private bool IsNetworkReachable()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	public void OnAboutCPMoviePlayFailed()
	{
		if (!this.mErrorPopInitialized)
		{
			NetManager.Instance.ShowErrorTextId("TXT_FailToConnect", false);
			this.mErrorPopInitialized = true;
		}
		else if (!NetManager.Instance.IsNetPopupShowing)
		{
			this.mErrorPopInitialized = false;
			this.ChangeMovieState(CinematicManager.MovieState.eIdle);
		}
	}
}
