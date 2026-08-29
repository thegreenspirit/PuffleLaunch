using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance
	{
		get { return GameManager.m_cInstance; }
	}

	public bool EnableTurboMode
	{
		get
		{
			string text;
			if (this.CurrentWorld == GameManager.World.eWorld_BlueSky)
			{
				text = "TurboMode_1";
			}
			else
			{
				text = "TurboMode_2";
			}
			return PlayerPrefs.HasKey(text) && PlayerPrefs.GetInt(text) == 1;
		}
		set
		{
			string text;
			if (this.CurrentWorld == GameManager.World.eWorld_BlueSky)
			{
				text = "TurboMode_1";
			}
			else
			{
				text = "TurboMode_2";
			}
			if (value)
			{
				PlayerPrefs.SetInt(text, 1);
			}
			else
			{
				PlayerPrefs.SetInt(text, 0);
			}
		}
	}

	public GameManager.World CurrentWorld
	{
		get { return this.m_CurrentWorld; }
		set { this.m_CurrentWorld = value; }
	}

	public bool EnableTiming
	{
		get { return this.m_EnableTiming; }
		set { this.m_EnableTiming = value; }
	}

	public int CoinsBeforeTransfer
	{
		get { return this.m_CoinsBeforeTransfer; }
		set { this.m_CoinsBeforeTransfer = value; }
	}

	public bool DuringCutscene
	{
		get { return this.m_DuringCutscene; }
		set { this.m_DuringCutscene = value; }
	}

	private void Awake()
	{
		GameManager.m_cInstance = this;
		this.mte_unlockFlags = new bool[5];
		this.ResetUnlockFlags();

#if UNITY_ANDROID && UNITY_EDITOR
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		base.gameObject.name = base.GetType().ToString();
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		this.m_Headphones = new AndroidJavaObject("com.bhvr.Headphones.HeadphoneUtils", new object[]
		{
			@static,
			base.gameObject.name,
			"HeadphoneMsg",
			this.m_HeadPhonesPlugged,
			this.m_HeadPhonesUnplugged
		});
#endif

		int i = 0;
		for (int j = 0; j < GameManager.kLevelsPerWorld.Length; j++)
		{
			for (i = i; i < this.levelSeparation[j]; i++)
			{
				GameManager.kTotalRingCount[j] += GameManager.smMaxRingInLevel[i];
			}
		}
	}

	private void Start()
	{
		// this.Pause(false); // Green Spirit: this was here in the decomped code don't blame me
		this.m_CoinsBeforeTransfer = ProfileManager.Instance.CurrentProfile.TotalCoins;
	}

	private void Update()
	{
#if UNITY_ANDROID && UNITY_EDITOR
		this.CheckAndroidBackButton();
#endif
		if (this.m_Paused)
		{
			return;
		}

		if (this.m_EnableTiming)
		{
			GameManager.smCurrentTimeCount += Time.deltaTime / Time.timeScale;
		}
	}

	private void OnDestroy() {}

#if UNITY_ANDROID && UNITY_EDITOR
	public void HeadphoneMsg(string msg)
	{
		if (msg == this.m_HeadPhonesUnplugged && !this.m_Paused && this.m_IsInLevel)
		{
			GameFlowManager.Instance.GUIManager.ShowPauseMenu(true);
		}
	}
#endif

	public void Pause(bool aPause)
	{
		this.m_Paused = aPause;
		if (TimeManager.Instance)
		{
			TimeManager.Instance.Pause(aPause);
		}
	}

	public bool IsPause()
	{
		return this.m_Paused;
	}

	public static bool HasCollectedAllRings(GameManager.World aWorld)
	{
		if (aWorld == GameManager.World.eWorld_BonusWorld)
		{
			return false;
		}
		int num = 0;
		int num2 = (int)aWorld * 12;
		int num3 = (int)(aWorld + 1) * 12 - (int)GameManager.World.eWorld_SodaSunset;
		int num4 = 0;
		foreach (Profile.LevelData levelData2 in ProfileManager.Instance.CurrentProfile.m_LevelData)
		{
			if (num4 >= num2 && num4 <= num3)
			{
				num += levelData2.BestRingCount;
			}
			num4++;
		}
		return num == GameManager.kTotalRingCount[(int)aWorld];
	}

	public static int GetLevelCompletion(GameManager.World aWorld)
	{
		int num = (int)aWorld * 12;
		int num2 = (int)(aWorld + 1) * 12 - (int)GameManager.World.eWorld_SodaSunset;
		if (aWorld == GameManager.World.eWorld_BonusWorld)
		{
			num2 = 59;
		}
		int num3 = 0;
		int num4 = 0;
		foreach (Profile.LevelData levelData2 in ProfileManager.Instance.CurrentProfile.m_LevelData)
		{
			if (num3 >= num && num3 <= num2 && levelData2.LevelComplete)
			{
				num4++;
			}
			num3++;
		}
		return num4;
	}

	public static float GetRingCompletion(GameManager.World aWorld)
	{
		int num = (int)aWorld * 12;
		int num2 = (int)(aWorld + 1) * 12 - (int)GameManager.World.eWorld_SodaSunset;
		int num3 = 0;
		float num4 = 0f;
		foreach (Profile.LevelData levelData2 in ProfileManager.Instance.CurrentProfile.m_LevelData)
		{
			if (num3 >= num && num3 <= num2)
			{
				num4 += (float)levelData2.BestRingCount;
			}
			num3++;
		}
		return num4 / (float)GameManager.kTotalRingCount[(int)aWorld];
	}

	public static float GetTurboModeCompletion(GameManager.World aWorld)
	{
		if (!GameManager.Instance.HasAchievedTimeTrialFire(aWorld))
		{
			return 0f;
		}
		int num = (int)aWorld * 12;
		int num2 = (int)(aWorld + 1) * 12 - (int)GameManager.World.eWorld_SodaSunset;
		int num3 = 0;
		float num4 = 0f;
		foreach (Profile.LevelData levelData2 in ProfileManager.Instance.CurrentProfile.m_LevelData)
		{
			if (num3 >= num && num3 <= num2)
			{
				num4 += ((!levelData2.TurboLevelComplete) ? 0f : 1f);
			}
			num3++;
		}
		return num4 / 30f;
	}

	public static float GetTimeTrialBestTime(GameManager.World aWorld)
	{
		int num = (int)aWorld * 12;
		int num2 = (int)(aWorld + 1) * 12 - (int)GameManager.World.eWorld_SodaSunset;
		int num3 = 0;
		int num4 = 0;
		foreach (Profile.LevelData levelData2 in ProfileManager.Instance.CurrentProfile.m_LevelData)
		{
			if (num3 >= num && num3 <= num2)
			{
				if (levelData2.BestTimeCount == float.MaxValue)
				{
					return 0f;
				}
				num4 += (int)levelData2.BestTimeCount;
			}
			num3++;
		}

		return (float)num4;
	}

	public static float GetTimeTrialNewGoal(GameManager.World aWorld, float aBestTimeTotal)
	{
		if (aBestTimeTotal > 0f && aBestTimeTotal <= (float)GameManager.kTimeTrialTimes[(int)aWorld, 3])
		{
			return (float)GameManager.kTimeTrialTimes[(int)aWorld, 3];
		}
		if (aBestTimeTotal > 0f && aBestTimeTotal <= (float)GameManager.kTimeTrialTimes[(int)aWorld, 2])
		{
			return (float)GameManager.kTimeTrialTimes[(int)aWorld, 3];
		}
		if (aBestTimeTotal > 0f && aBestTimeTotal <= (float)GameManager.kTimeTrialTimes[(int)aWorld, 1])
		{
			return (float)GameManager.kTimeTrialTimes[(int)aWorld, 2];
		}

		return (float)GameManager.kTimeTrialTimes[(int)aWorld, 1];
	}

	public static void RetrieveTimeTrialCompletion(GameManager.World aWorld, out GameManager.LevelTimes aCompletedLevelTime, out float aCompletedPercentage)
	{
		if (!GameManager.HasCollectedAllRings(aWorld))
		{
			aCompletedLevelTime = GameManager.LevelTimes.eTime_None;
			aCompletedPercentage = 0f;
			return;
		}
		float timeTrialBestTime = GameManager.GetTimeTrialBestTime(aWorld);
		if (timeTrialBestTime > (float)GameManager.kTimeTrialTimes[(int)aWorld, 0] || timeTrialBestTime == 0f)
		{
			aCompletedLevelTime = GameManager.LevelTimes.eTime_None;
			aCompletedPercentage = 0f;
		}
		else
		{
			if (timeTrialBestTime <= (float)GameManager.kTimeTrialTimes[(int)aWorld, 3])
			{
				aCompletedLevelTime = GameManager.LevelTimes.eTime_Fire;
			}
			else if (timeTrialBestTime <= (float)GameManager.kTimeTrialTimes[(int)aWorld, 2])
			{
				aCompletedLevelTime = GameManager.LevelTimes.eTime_Gold;
			}
			else if (timeTrialBestTime <= (float)GameManager.kTimeTrialTimes[(int)aWorld, 1])
			{
				aCompletedLevelTime = GameManager.LevelTimes.eTime_Silver;
			}
			else
			{
				aCompletedLevelTime = GameManager.LevelTimes.eTime_None;
			}
			aCompletedPercentage = GameManager.GetTimeTrialCompletedPercentage(aWorld, timeTrialBestTime, aCompletedLevelTime);
		}
	}

	public static float GetTimeTrialCompletedPercentage(GameManager.World aWorld, float aBestTimeTotal, GameManager.LevelTimes aCompletedLevelTime)
	{
		if (aCompletedLevelTime == GameManager.LevelTimes.eTime_Fire)
		{
			return 1f;
		}
		float num = (float)GameManager.kTimeTrialTimes[(int)aWorld, (int)aCompletedLevelTime];
		float num2 = (float)GameManager.kTimeTrialTimes[(int)aWorld, (int)(aCompletedLevelTime + 1)];
		return 1f - (aBestTimeTotal - num2) / (num - num2);
	}

	public static bool HasCompletedTurboMode(GameManager.World aWorld)
	{
		if (!GameManager.HasCollectedAllRings(aWorld) || !GameManager.Instance.HasAchievedTimeTrialFire(aWorld))
		{
			return false;
		}
		int num = (int)aWorld * 12;
		int num2 = (int)(aWorld + 1) * 12 - (int)GameManager.World.eWorld_SodaSunset;
		int num3 = 0;
		foreach (Profile.LevelData levelData2 in ProfileManager.Instance.CurrentProfile.m_LevelData)
		{
			if (num3 >= num && num3 <= num2 && !levelData2.TurboLevelComplete)
			{
				return false;
			}
			num3++;
		}
		return true;
	}

	public void ActivatePlayerSlowMo()
	{
		TimeManager.Instance.SlowmoOverride = true;
		TimeManager.Instance.ActivateSlowmo();
	}

	public void StopPlayerSlowMo()
	{
		TimeManager.Instance.SlowmoOverride = false;
		TimeManager.Instance.StopSlowmo();
	}

	public void StartCutscene(bool aSlowmo)
	{
		if (GameManager.HasCompletedTurboMode(this.CurrentWorld))
		{
			this.StopPlayerSlowMo();
			GameFlowManager.Instance.GUIManager.HudManager.InGameHud.SetSlowmoButtonState(false);
			GameFlowManager.Instance.GUIManager.HudManager.InGameHud.SetSlowmoButtonVisible(false);
		}
		if (aSlowmo)
		{
			TimeManager.Instance.SlowmoOverride = true;
			TimeManager.Instance.ActivateSlowmo();
		}
	}

	public void EndCutscene()
	{
		if (GameManager.HasCompletedTurboMode(this.CurrentWorld))
		{
			GameFlowManager.Instance.GUIManager.HudManager.InGameHud.SetSlowmoButtonVisible(true);
		}
		TimeManager.Instance.SlowmoOverride = false;
		TimeManager.Instance.StopSlowmo();
	}

	public void StartLevel(GameManager.Level aSelectedLevel)
	{
		GameFlowManager.Instance.AudioManager.PlayMusic(AudioManager.MusicTrack.eMusic_Gameplay);
		this.UpdateUnlockFlags();
		GameManager.smCurrentLevelRingCount = 0;
		GameManager.smCurrentTimeCount = 0f;
		GameManager.smCurrentLevel = aSelectedLevel;
		string text = string.Empty;
		switch (this.m_CurrentWorld)
		{
			case GameManager.World.eWorld_SodaSunset:
				text = "sodasunset";
				goto IL_0071;
			case GameManager.World.eWorld_BonusWorld:
				text = "bonus";
				goto IL_0071;
		}
		text = "bluesky";
		IL_0071:
		string text2 = string.Concat(new string[] { "GUI/LoadingScreen/", SizeCategory.Instance.Category, "/", text, "_loading" });
		GameFlowManager.Instance.GUIManager.LoadingScreen.TextureData[0].icon.image = GUIUtil.LoadTexture2D(text2);
		GameFlowManager.Instance.GUIManager.LoadingScreen.StartLoadingBar();
		if (this.EnableTurboMode && this.HasAchievedTimeTrialFire(this.m_CurrentWorld))
		{
			TimeManager.Instance.ActivateTurbo();
		}
		else
		{
			TimeManager.Instance.StopTurbo();
		}
		this.StopPlayerSlowMo();
		ProfileManager.Instance.CurrentProfile.LastLevelPlayed = (int)GameManager.smCurrentLevel;
		ProfileManager.Instance.SaveCurrentProfile();
		this.m_IsInLevel = true;
	}

	private void ResetUnlockFlags()
	{
		for (int i = 0; i < 5; i++)
		{
			this.mte_unlockFlags[i] = false;
		}
	}

	private void UpdateUnlockFlags()
	{
		for (int i = 0; i < 5; i++)
		{
			this.mte_unlockFlags[i] = this.CheckUnlock((GameManager.Unlock)i);
		}
	}

	private void CheckAndroidBackButton()
	{
		if (GameFlowManager.Instance.m_DoWindowBack)
		{
			if (this.m_IsInLevel)
			{
				if (GameFlowManager.Instance.GUIManager.IsPauseMenu)
				{
					GameFlowManager.Instance.GUIManager.ShowPauseMenu(false);
					GameFlowManager.Instance.m_DoWindowBack = false;
				}
				else if (!this.m_Paused && TutorialPopup.Instance == null)
				{
					GameFlowManager.Instance.GUIManager.ShowPauseMenu(true);
					GameFlowManager.Instance.m_DoWindowBack = false;
				}
			}
			else if (GameFlowManager.Instance.GUIManager.IsLoginPopupShowing || GameFlowManager.Instance.GUIManager.IsCreateAccountPopupShowing)
			{
				if (GameFlowManager.Instance.GUIManager.IsLoginPopupShowing)
				{
					GameFlowManager.Instance.GUIManager.LoginPopupToBackTraceScene();
				}
				else if (GameFlowManager.Instance.GUIManager.IsCreateAccountPopupShowing)
				{
					GameFlowManager.Instance.GUIManager.ShowCreateAccountPopup(false);
					GameFlowManager.Instance.GUIManager.ShowLoginPopup(true);
				}
				GameFlowManager.Instance.m_DoWindowBack = false;
			}
		}
	}

	public GameManager.Unlock FindNextUnlock(GameManager.Unlock ae_Unlock)
	{
		for (int i = (int)(ae_Unlock + 1); i < 5; i++)
		{
			if (i >= 0 && !this.mte_unlockFlags[i] && this.CheckUnlock((GameManager.Unlock)i))
			{
				return (GameManager.Unlock)i;
			}
		}
		return GameManager.Unlock.eUnlock_None;
	}

	private bool CheckUnlock(GameManager.Unlock ae_unlock)
	{
		switch (ae_unlock)
		{
		case GameManager.Unlock.eUnlock_TimeTrial:
			return GameManager.HasCollectedAllRings(this.m_CurrentWorld);
		case GameManager.Unlock.eUnlock_TimeTrialSilver:
			return this.HasAchievedTimeTrialSilver(this.m_CurrentWorld);
		case GameManager.Unlock.eUnlock_TimeTrialGold:
			return this.HasAchievedTimeTrialGold(this.m_CurrentWorld);
		case GameManager.Unlock.eUnlock_TurboMode:
			return this.HasAchievedTimeTrialFire(this.m_CurrentWorld);
		case GameManager.Unlock.eUnlock_SlowMotion:
			return GameManager.HasCompletedTurboMode(this.m_CurrentWorld);
		default:
			return false;
		}
	}

	public bool HasAchievedTimeTrialSilver(GameManager.World aWorld)
	{
		float timeTrialBestTime = GameManager.GetTimeTrialBestTime(aWorld);
		return timeTrialBestTime > 0f && (int)timeTrialBestTime <= GameManager.kTimeTrialTimes[(int)aWorld, 1] && GameManager.HasCollectedAllRings(this.m_CurrentWorld);
	}

	public bool HasAchievedTimeTrialGold(GameManager.World aWorld)
	{
		float timeTrialBestTime = GameManager.GetTimeTrialBestTime(aWorld);
		return timeTrialBestTime > 0f && (int)timeTrialBestTime <= GameManager.kTimeTrialTimes[(int)aWorld, 2] && GameManager.HasCollectedAllRings(this.m_CurrentWorld);
	}

	public bool HasAchievedTimeTrialFire(GameManager.World aWorld)
	{
		float timeTrialBestTime = GameManager.GetTimeTrialBestTime(aWorld);
		return timeTrialBestTime > 0f && (int)timeTrialBestTime <= GameManager.kTimeTrialTimes[(int)aWorld, 3] && GameManager.HasCollectedAllRings(this.m_CurrentWorld);
	}

	public void ShowEndLevelScreens()
	{
		GameManager.Instance.CompleteLevel();
		if (GameManager.Instance.FindNextUnlock(GameManager.Unlock.eUnlock_None) != GameManager.Unlock.eUnlock_None)
		{
			GameFlowManager.Instance.GUIManager.ShowUnlockPopups(true);
		}
		else
		{
			GameFlowManager.Instance.GUIManager.ShowTallyMenu(true);
		}
	}

	public void QuitLevel()
	{
		GameFlowManager.Instance.InputController.enabled = true;
		this.CommonEndLevel(false);
	}

	public void CompleteLevel()
	{
		GameManager.smIsCurrentNewRingRecord = false;
		GameManager.smIsCurrentNewTimeRecord = false;
		ProfileManager.Instance.CurrentProfile.m_LevelData[(int)GameManager.smCurrentLevel].LevelComplete = true;
		if (GameManager.smCurrentLevel != GameManager.Level.eLevel_60)
		{
			ProfileManager.Instance.CurrentProfile.m_LevelData[(int)(GameManager.smCurrentLevel + 1)].LevelUnlocked = true;
		}
		if (GameManager.IsNewRingRecord(GameManager.smCurrentLevel, GameManager.smCurrentLevelRingCount))
		{
			ProfileManager.Instance.CurrentProfile.m_LevelData[(int)GameManager.smCurrentLevel].BestRingCount = GameManager.smCurrentLevelRingCount;
			GameManager.smIsCurrentNewRingRecord = true;
		}
		if (GameManager.IsNewTimeRecord(GameManager.smCurrentLevel, (double)GameManager.smCurrentTimeCount))
		{
			ProfileManager.Instance.CurrentProfile.m_LevelData[(int)GameManager.smCurrentLevel].BestTimeCount = (float)((int)GameManager.smCurrentTimeCount);
			GameManager.smIsCurrentNewTimeRecord = true;
		}
		if (this.EnableTurboMode && this.HasAchievedTimeTrialFire(this.m_CurrentWorld))
		{
			ProfileManager.Instance.CurrentProfile.m_LevelData[(int)GameManager.smCurrentLevel].TurboLevelComplete = true;
		}
		ProfileManager.Instance.CurrentProfile.TotalCoins += GameManager.smCurrentLevelRingCount;
		ProfileManager.Instance.SaveCurrentProfile();
		if (NetManager.Instance.IsPlayerLoggedIn())
		{
			NetManager.Instance.TransferCoins(ProfileManager.Instance.CurrentProfile.TotalCoins, new BaseNetRequest.RequestCompleteCB(this.TransferCallback), true);
		}
		else
		{
			this.m_CoinsBeforeTransfer = ProfileManager.Instance.CurrentProfile.TotalCoins;
		}
		this.CommonEndLevel(true);
	}

	private void TransferCallback(bool aSuccess) {}

	private void CommonEndLevel(bool aLevelComplete)
	{
		this.m_IsInLevel = false;
		this.m_EnableTiming = false;
		TimeManager.Instance.SlowmoOverride = false;
		TimeManager.Instance.StopSlowmo();

		BizIntel.ContextualEvent contextualEvent = new BizIntel.ContextualEvent("play-level");
		contextualEvent.AddContextItem("level-id", (int)GameManager.smCurrentLevel);
		contextualEvent.AddContextItem("elapsed-time", (int)Time.timeSinceLevelLoad);
		contextualEvent.AddContextItem("coins-collected", GameManager.smCurrentLevelRingCount);
		contextualEvent.AddContextItem("max-coins", GameManager.smMaxRingInLevel[(int)GameManager.smCurrentLevel]);
		contextualEvent.AddContextItem("level-passed", aLevelComplete);
		contextualEvent.AddContextItem("number-deaths", Puffle.Instance.respawnCount);
		contextualEvent.Log();

		GameFlowManager.Instance.GUIManager.LoadingScreen.TextureData[0].icon.image = GUIUtil.LoadTexture2D("GUI/LoadingScreen/BlackScreen");
		Resources.UnloadUnusedAssets();
	}

	public static bool IsNewRingRecord(GameManager.Level aLevel, int aRingCount)
	{
		return aRingCount > ProfileManager.Instance.CurrentProfile.m_LevelData[(int)aLevel].BestRingCount;
	}

	public static bool IsNewTimeRecord(GameManager.Level aLevel, double aTimeCount)
	{
		return aTimeCount < (double)ProfileManager.Instance.CurrentProfile.m_LevelData[(int)aLevel].BestTimeCount;
	}

	public static void CollectAllRings(GameManager.World aWorld)
	{
		if (aWorld != GameManager.World.eWorld_BonusWorld)
		{
			int num = (int)aWorld * 12;
			int num2 = (int)(aWorld + 1) * 12 - (int)GameManager.World.eWorld_SodaSunset;
			int num3 = 0;
			foreach (Profile.LevelData levelData2 in ProfileManager.Instance.CurrentProfile.m_LevelData)
			{
				if (num3 >= num && num3 <= num2)
				{
					levelData2.LevelComplete = true;
					levelData2.LevelUnlocked = true;
					levelData2.BestRingCount = GameManager.smMaxRingInLevel[num3];
				}
				if (GameManager.Instance.CurrentWorld == GameManager.World.eWorld_BlueSky && num3 == num2 + 1)
				{
					levelData2.LevelUnlocked = true;
				}
				num3++;
			}
		}
		else
		{
			int num4 = 24;
			int num5 = 59;
			int num6 = 0;
			foreach (Profile.LevelData levelData4 in ProfileManager.Instance.CurrentProfile.m_LevelData)
			{
				if (num6 >= num4 && num6 <= num5)
				{
					levelData4.LevelComplete = true;
					levelData4.LevelUnlocked = true;
					levelData4.BestRingCount = GameManager.smMaxRingInLevel[num6];
				}
				num6++;
			}
		}
	}

	public static void CompleteTimeTrial(GameManager.World aWorld, float aLevelTime)
	{
		GameManager.CollectAllRings(aWorld);
		int num = (int)aWorld * 12;
		int num2 = (int)(aWorld + 1) * 12 - (int)GameManager.World.eWorld_SodaSunset;
		int num3 = 0;
		foreach (Profile.LevelData levelData2 in ProfileManager.Instance.CurrentProfile.m_LevelData)
		{
			if (num3 >= num && num3 <= num2)
			{
				levelData2.BestTimeCount = aLevelTime;
			}
			num3++;
		}
	}

	public static void CompleteTurboMode(GameManager.World aWorld)
	{
		GameManager.CompleteTimeTrial(aWorld, 20f);
		int num = (int)aWorld * 12;
		int num2 = (int)(aWorld + 1) * 12 - (int)GameManager.World.eWorld_SodaSunset;
		int num3 = 0;
		foreach (Profile.LevelData levelData2 in ProfileManager.Instance.CurrentProfile.m_LevelData)
		{
			if (num3 >= num && num3 <= num2)
			{
				levelData2.TurboLevelComplete = true;
			}
			num3++;
		}
	}

	public static string GetTimeFormatedString(float aSeconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)aSeconds);
		return string.Format("{0}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
	}

	public const double kPlayerControlledSlowMoDuration = 6.0;

	public static int[] kLevelsPerWorld = new int[] { 12, 12, 34 };

	public static int[,] kTimeTrialTimes = new int[,]
	{
		{ 540, 480, 420, 360 },
		{ 960, 900, 840, 780 },
		{ 450, 390, 330, 270 }
	};

	public static int[] kTotalRingCount = new int[3];

	public static GameManager.Level smCurrentLevel = GameManager.Level.eLevel_1;

	public static int smCurrentLevelRingCount = 0;

	public static float smCurrentTimeCount = 0f;

	public static bool smIsCurrentNewRingRecord = false;

	public static bool smIsCurrentNewTimeRecord = false;

	private int[] levelSeparation = new int[] { 12, 24, 58 };

	public static int[] smMaxRingInLevel = new int[]
	{
		34, 46, 99, 90, 115, 39, 84, 42, 120, 123,
		183, 54, 59, 75, 243, 88, 284, 135, 122, 172,
		153, 113, 203, 69, 231, 148, 115, 262, 112, 123,
		124, 123, 131, 87, 179, 131, 219, 232, 135, 148,
		325, 102, 135, 121, 153, 115, 148, 283, 284, 166,
		118, 175, 202, 161, 168, 137, 159, 227, 104, 170,
		153, 152, 172, 336, 147, 225, 63, 137, 157, 337,
		458, 116
	};

	private static GameManager m_cInstance;

	private bool m_Paused;

	private bool m_EnableTurboMode;

	private GameManager.World m_CurrentWorld;

	private bool m_EnableTiming;

	private bool[] mte_unlockFlags;

	private int m_CoinsBeforeTransfer = -1;

	private bool m_DuringCutscene;

	private bool m_IsInLevel;

#if UNITY_ANDROID && UNITY_EDITOR
	private string m_HeadPhonesPlugged = "Headphones are Plugged";
	private string m_HeadPhonesUnplugged = "Headphones are not Plugged";
	private AndroidJavaObject m_Headphones;
#endif

	public enum Level
	{
		// World 1
		eLevel_1, eLevel_2, eLevel_3, eLevel_4,  eLevel_5,  eLevel_6,  eLevel_EndLite   = 5 ,
		eLevel_7, eLevel_8, eLevel_9, eLevel_10, eLevel_11, eLevel_12, eLevel_EndWorld1 = 11,
		// World 2
		eLevel_13, eLevel_14, eLevel_15, eLevel_16, eLevel_17, eLevel_18,
		eLevel_19, eLevel_20, eLevel_21, eLevel_22, eLevel_23, eLevel_24, eLevel_EndWorld2 = 23,

		// Bonus World
		eLevel_FirstBonusLevel,

		// First page
		eLevel_25 = 24, eLevel_26, eLevel_27, eLevel_28, eLevel_29, eLevel_30,
		eLevel_31     , eLevel_32, eLevel_33, eLevel_34, eLevel_35, eLevel_36,
		// Second page
		eLevel_37, eLevel_38, eLevel_39, eLevel_40, eLevel_41, eLevel_42,
		eLevel_43, eLevel_44, eLevel_45, eLevel_46, eLevel_47, eLevel_48,
		// Third page
		eLevel_49, eLevel_50, eLevel_51, eLevel_52, eLevel_53, eLevel_54,
		eLevel_55, eLevel_56, eLevel_57, eLevel_58, eLevel_59, eLevel_60,
		// Fourth page
		eLevel_61, eLevel_62, eLevel_63, eLevel_64, eLevel_65, eLevel_66,
		eLevel_67, eLevel_68, eLevel_69, eLevel_70, eLevel_71, eLevel_72,

		eLevel_EndBonusWorld = 59,
		eLevel_COUNT
	}

	public enum World { eWorld_BlueSky, eWorld_SodaSunset, eWorld_BonusWorld, eWorld_COUNT }
	public enum Unlock { eUnlock_None = -1, eUnlock_TimeTrial, eUnlock_TimeTrialSilver, eUnlock_TimeTrialGold, eUnlock_TurboMode, eUnlock_SlowMotion, eUnlock_Num }
	public enum LevelTimes { eTime_None, eTime_Silver, eTime_Gold, eTime_Fire }
}
