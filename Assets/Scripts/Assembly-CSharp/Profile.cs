using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class Profile : ISerializable
{
	private const string BUILD_VERSION = "BuildVersion";
	private const string PROFILE_NAME = "ProfileName";
	private const string AUTH_TOKEN = "AuthToken";
	private const string TOTAL_COINS = "TotalCoins";
	private const string LEVEL_DATA_BEST_RING_COUNT = "LevelDataBestRingCount";
	private const string LEVEL_DATA_BEST_TIME_COUNT = "LevelDataBestTimeCount";
	private const string LEVEL_DATA_TURBO_LEVEL_COMPLETE = "LevelDataTurboLevelComplete";
	private const string LEVEL_DATA_LEVEL_COMPLETE = "LevelDataLevelComplete";
	private const string LEVEL_DATA_LEVEL_UNLOCKED = "LevelDataLevelUnlocked";

	private static int VersionIndex = 0;
	private static string[] Versions = new string[] { "0", "0.0.1" };
	private static int DeserializedVersionIndex;

	public int m_ProfileID;
	public Profile.LevelData[] m_LevelData;
	private int m_BuildVersion;
	private string m_ProfileName;
	private string m_AuthToken = string.Empty;
	private int m_TotalCoins;
	private int m_LastLevelPlayed;

	public class LevelData
	{
		private int mBestRingCount;
		private float mBestTimeCount;
		private bool mTurboLevelComplete;
		private bool mLevelComplete;
		private bool mLevelUnlocked;

		public LevelData()
		{
			this.mBestRingCount = 0;
			this.mBestTimeCount = float.MaxValue;
			this.mTurboLevelComplete = false;
			this.mLevelComplete = false;
			this.mLevelUnlocked = false;
		}

		public int BestRingCount
		{
			get { return this.mBestRingCount; }
			set { this.mBestRingCount = value; }
		}
		public float BestTimeCount
		{
			get { return this.mBestTimeCount; }
			set { this.mBestTimeCount = value; }
		}
		public bool TurboLevelComplete
		{
			get	{ return this.mTurboLevelComplete; }
			set { this.mTurboLevelComplete = value; }
		}
		public bool LevelComplete
		{
			get { return this.mLevelComplete; }
			set { this.mLevelComplete = value; }
		}
		public bool LevelUnlocked
		{
			get { return this.mLevelUnlocked; }
			set { this.mLevelUnlocked = value; }
		}
	}

	public Profile() {}
	protected Profile(SerializationInfo aInfo, StreamingContext aTxt)
	{
		this.Init();
		if (Profile.DeserializedVersionIndex >= 0)
		{
			try { this.m_BuildVersion = (int)aInfo.GetValue(BUILD_VERSION, typeof(int)); }
			catch (SerializationException e) { this.m_BuildVersion = 0; }

			try { this.m_ProfileName = (string)aInfo.GetValue(PROFILE_NAME, typeof(string)); }
			catch (SerializationException e) { this.ProfileName = "New Profile"; }

			try { this.m_AuthToken = (string)aInfo.GetValue(AUTH_TOKEN, typeof(string)); }
			catch (SerializationException e) { this.m_AuthToken = string.Empty; }

			try { this.m_TotalCoins = (int)aInfo.GetValue(TOTAL_COINS, typeof(int)); }
			catch (SerializationException e) { this.m_TotalCoins = 0; }

			int[] bestRingCount;
			try { bestRingCount = (int[])aInfo.GetValue(LEVEL_DATA_BEST_RING_COUNT, typeof(int[])); }
			catch (SerializationException e) { bestRingCount = new int[60]; }

			float[] bestTimeCount;
			try { bestTimeCount = (float[])aInfo.GetValue(LEVEL_DATA_BEST_TIME_COUNT, typeof(float[])); }
			catch (SerializationException e) { bestTimeCount = new float[60]; }

			bool[] turboLevelComplete;
			try { turboLevelComplete = (bool[])aInfo.GetValue(LEVEL_DATA_TURBO_LEVEL_COMPLETE, typeof(bool[])); }
			catch (SerializationException e) { turboLevelComplete = new bool[60]; }

			bool[] levelComplete;
			try { levelComplete = (bool[])aInfo.GetValue(LEVEL_DATA_LEVEL_COMPLETE, typeof(bool[])); }
			catch (SerializationException e) { levelComplete = new bool[60]; }

			bool[] levelUnlocked;
			try { levelUnlocked = (bool[])aInfo.GetValue(LEVEL_DATA_LEVEL_UNLOCKED, typeof(bool[])); }
			catch (SerializationException e) { levelUnlocked = new bool[60]; }

			int num = Mathf.Min(bestRingCount.Length, this.m_LevelData.Length);
			for (int i = 0; i < num; i++)
			{
				this.m_LevelData[i].BestRingCount = bestRingCount[i];
				this.m_LevelData[i].BestTimeCount = bestTimeCount[i];
				this.m_LevelData[i].TurboLevelComplete = turboLevelComplete[i];
				this.m_LevelData[i].LevelComplete = levelComplete[i];
				this.m_LevelData[i].LevelUnlocked = levelUnlocked[i];
			}
		}
	}

	private static string HeaderLine
	{
		get { return "Profile data at version "; }
	}
	private static string FullTypeName
	{
		get { return "Profile"; }
	}

	private static bool IsValidHeaderLine(string aAssemblyName)
	{
		for (int i = 0; i < Profile.Versions.Length; i++)
		{
			if (aAssemblyName == Profile.HeaderLine + Profile.Versions[i])
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsValidForDeserialization(string aAssemblyName, string aTypeName)
	{
		return Profile.FullTypeName == aTypeName && Profile.IsValidHeaderLine(aAssemblyName);
	}

	public static void ExtractDeserializedVersionIndex(string aAssemblyName)
	{
		Profile.DeserializedVersionIndex = -1;
		string text = aAssemblyName.Remove(0, Profile.HeaderLine.Length);
		for (int i = 0; i < Profile.Versions.Length; i++)
		{
			if (text == Profile.Versions[i])
			{
				Profile.DeserializedVersionIndex = i;
				break;
			}
		}
		Utilities.AssertMsg(Profile.DeserializedVersionIndex != -1, "Invalid Version Number: " + text);
	}

	public int BuildVersion
	{
		get { return this.m_BuildVersion; }
		set { this.m_BuildVersion = value; }
	}
	public string AuthToken
	{
		get { return this.m_AuthToken; }
		set { this.m_AuthToken = value; }
	}
	public string ProfileName
	{
		get { return this.m_ProfileName; }
		set { this.m_ProfileName = value; }
	}
	public int TotalCoins
	{
		get { return this.m_TotalCoins; }
		set { this.m_TotalCoins = value; }
	}
	public int LastLevelPlayed
	{
		get { return this.m_LastLevelPlayed; }
		set { this.m_LastLevelPlayed = value; }
	}

	public static Profile CreateProfile()
	{
		Profile profile = new Profile();
		profile.Init();
		return profile;
	}

	public void GetObjectData(SerializationInfo aInfo, StreamingContext aTxt)
	{
		aInfo.AssemblyName = Profile.HeaderLine + Profile.Versions[Profile.VersionIndex];
		aInfo.FullTypeName = Profile.FullTypeName;

		if (Profile.VersionIndex >= 0)
		{
			aInfo.AddValue(BUILD_VERSION, this.m_BuildVersion);
			aInfo.AddValue(PROFILE_NAME, this.m_ProfileName);
			aInfo.AddValue(AUTH_TOKEN, this.m_AuthToken);
			aInfo.AddValue(TOTAL_COINS, this.m_TotalCoins);

			int[] bestRingCount = new int[60];
			float[] bestTimeCount = new float[60];
			bool[] turboLevelComplete = new bool[60];
			bool[] levelComplete = new bool[60];
			bool[] levelUnlocked = new bool[60];

			for (int i = 0; i < 60; i++)
			{
				bestRingCount[i] = this.m_LevelData[i].BestRingCount;
				bestTimeCount[i] = this.m_LevelData[i].BestTimeCount;
				turboLevelComplete[i] = this.m_LevelData[i].TurboLevelComplete;
				levelComplete[i] = this.m_LevelData[i].LevelComplete;
				levelUnlocked[i] = this.m_LevelData[i].LevelUnlocked;
			}

			aInfo.AddValue(LEVEL_DATA_BEST_RING_COUNT, bestRingCount);
			aInfo.AddValue(LEVEL_DATA_BEST_TIME_COUNT, bestTimeCount);
			aInfo.AddValue(LEVEL_DATA_TURBO_LEVEL_COMPLETE, turboLevelComplete);
			aInfo.AddValue(LEVEL_DATA_LEVEL_COMPLETE, levelComplete);
			aInfo.AddValue(LEVEL_DATA_LEVEL_UNLOCKED, levelUnlocked);
		}
	}

	public bool HasAuthToken()
	{
		return this.m_AuthToken != null && this.m_AuthToken.Length > 0;
	}

	private void Init()
	{
		this.m_ProfileID = 0;
		this.m_LevelData = new Profile.LevelData[60];
		for (int i = 0; i < 60; i++) { this.m_LevelData[i] = new Profile.LevelData(); }
		this.m_TotalCoins = 0;
		this.m_LastLevelPlayed = -1;
	}
}
