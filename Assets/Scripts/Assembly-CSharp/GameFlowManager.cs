using System;
using System.Collections;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
	public static bool mLoadingDone;
	public bool m_DoWindowBack;
	private static GameFlowManager m_cInstance;

	private GUIManager m_GUIManager;
	private InputController m_InputController;

	private AudioClip m_MenuClick24;
	private AudioManager mAudioManager;

	private string m_UnlockScreen = "Unlock phone";
	private AndroidJavaObject m_LockScreen;

	public static GameFlowManager Instance
	{
		get { return GameFlowManager.m_cInstance; }
	}

	public GUIManager GUIManager
	{
		get { return this.m_GUIManager; }
	}

	public AudioManager AudioManager
	{
		get { return this.mAudioManager; }
	}

	public InputController InputController
	{
		get { return this.m_InputController; }
	}

	public AudioClip MenuClick24
	{
		get { return this.m_MenuClick24; }
	}

	private void Awake()
	{
		BizIntel.StartBizIntel();

		if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres)
		{
			QualitySettings.currentLevel = QualityLevel.Fastest;
		}

		GameFlowManager.m_cInstance = this;

		this.m_InputController = base.GetComponent<InputController>();

		global::UnityEngine.Object.DontDestroyOnLoad(GameFlowManager.m_cInstance);

		this.mAudioManager = base.GetComponent<AudioManager>();
	}

	private void Start()
	{
		this.m_MenuClick24 = Resources.Load("Sounds/UI/Menu_Click24", typeof(AudioClip)) as AudioClip;
		global::UnityEngine.Object.DontDestroyOnLoad(this.m_MenuClick24);

#if UNITY_ANDROID
		base.gameObject.name = base.GetType().ToString();
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		this.m_LockScreen = new AndroidJavaObject("com.bhvr.LockScreen.LockScreen", new object[]
		{
			@static,
			base.gameObject.name,
			"LockScreenMsg",
			this.m_UnlockScreen
		});
#endif
	}

	private void Update()
	{
		if (Input.GetKeyUp("escape"))
		{
			this.AudioManager.PlayUISFx(this.MenuClick24);
			if (this.GUIManager.m_Popups.Count > 0)
			{
				int num = this.GUIManager.m_Popups.Count - 1;
				this.GUIManager.m_Popups[num].ClosePopup();
			}
			else
			{
				this.m_DoWindowBack = true;
			}
		}
		ResolutionManager.Instance.CheckDeviceOrientation();
		if (this.m_GUIManager == null)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/Managers/GUIManager", typeof(GameObject))) as GameObject;
			if (Utilities.Assert(gameObject != null))
			{
				this.m_GUIManager = gameObject.GetComponent(typeof(GUIManager)) as GUIManager;
			}
			return;
		}
	}

	private void OnDestroy()
	{
		if (this.m_GUIManager != null)
		{
			global::UnityEngine.Object.DestroyImmediate(this.m_GUIManager.gameObject);
			this.m_GUIManager = null;
		}

		BizIntel.StopBizIntel();
	}

	public void LoadSceneImmediate(string aSceneName, bool aLeaveLoadingScreen)
	{
		this.m_GUIManager.ShowLoadingScreen();
		Application.LoadLevel(aSceneName);
		this.m_GUIManager.ChangeCurrentScene(aSceneName);
	}

	public void LoadScene(string aSceneName, bool aLeaveLoadingScreen)
	{
		base.StartCoroutine(this.LoadNewSceneASync(aSceneName, aLeaveLoadingScreen));
	}

	private IEnumerator LoadNewSceneASync(string aSceneName, bool aLeaveLoadingScreen)
	{
		this.m_GUIManager.ShowLoadingScreen();
		AsyncOperation asyncInfo = Application.LoadLevelAsync(aSceneName);
		while (!asyncInfo.isDone)
		{
			yield return null;
		}
		if (!aLeaveLoadingScreen)
		{
			this.m_GUIManager.HideLoadingScreen();
		}
		this.m_GUIManager.ChangeCurrentScene(aSceneName);
		yield break;
	}

	public IEnumerator UnloadUnusedResources()
	{
		AsyncOperation unload = Resources.UnloadUnusedAssets();
		while (!unload.isDone)
		{
			yield return null;
		}
		yield break;
	}

	private void OnApplicationPause(bool aState)
	{
		if (aState && CinematicManager.Instance == null)
		{
			AudioManager.Instance.ForceMute();
		}
	}

	public void LockScreenMsg(string msg)
	{
		if (msg == this.m_UnlockScreen)
		{
			AudioManager.Instance.ResetMute();
		}
	}
}
