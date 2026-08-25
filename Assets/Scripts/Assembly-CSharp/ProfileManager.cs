using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
	private Profile m_CurrentProfile;
	public List<Profile> m_Profiles;
	private static ProfileManager mInstance;

	private class FormatterBinder : SerializationBinder
	{
		public override Type BindToType(string aAssemblyName, string aTypeName)
		{
			if (Profile.IsValidForDeserialization(aAssemblyName, aTypeName))
			{
				Profile.ExtractDeserializedVersionIndex(aAssemblyName);
				return typeof(Profile);
			}
			return null;
		}
	}

	public enum Result { eSucceeded, eUserNameEmpty, eUserNameConflict, eExceedMaxSlots, eCOUNT }

	public static ProfileManager Instance
	{
		get { return ProfileManager.mInstance; }
	}

	public Profile CurrentProfile
	{
		get { return this.m_CurrentProfile; }
	}

	public int CurrentProfileID
	{
		get { return this.m_CurrentProfile.m_ProfileID; }
		set { this.m_CurrentProfile.m_ProfileID = value; }
	}

	public void SerializeProfile(string aFilename, Profile aProfileToSerialize)
	{
		Stream stream = File.Open(aFilename, FileMode.Create);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Serialize(stream, aProfileToSerialize);
		stream.Close();
	}

	public Profile DeSerializeProfile(string aFilename, ref bool aSucceeded)
	{
		aSucceeded = true;
		Profile profile = Profile.CreateProfile();

		Stream stream = File.Open(aFilename, FileMode.Open);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		binaryFormatter.Binder = new ProfileManager.FormatterBinder();

		try
		{
			profile = (Profile)binaryFormatter.Deserialize(stream);
		}
		catch
		{
			aSucceeded = false;
		}
		stream.Close();

		return profile;
	}

	public void SaveCurrentProfile()
	{
		string profileFilePath = this.GetProfileFilePath(this.CurrentProfileID);
		this.SerializeProfile(profileFilePath, this.CurrentProfile);
	}

	public bool LoadProfile(int aProfileID, ref Profile aProfileToLoad)
	{
		string profileFilePath = this.GetProfileFilePath(aProfileID);
		if (File.Exists(profileFilePath))
		{
			bool flag = false;
			aProfileToLoad = this.DeSerializeProfile(profileFilePath, ref flag);
			aProfileToLoad.m_ProfileID = aProfileID;
			if (!flag)
			{
				this.RemoveProfile(aProfileID);
			}
			return flag;
		}
		return false;
	}

	public void UnLoadAllProfiles()
	{
		this.m_Profiles.Clear();
	}

	public void SetCurrentProfile(Profile aNewProfile)
	{
		this.m_CurrentProfile = aNewProfile;
		this.CurrentProfileID = this.m_CurrentProfile.m_ProfileID;
	}

	public ProfileManager.Result ValidateProfileName(string aName)
	{
		if (aName == null || aName.Length == 0)
		{
			return ProfileManager.Result.eUserNameEmpty;
		}
		return ProfileManager.Result.eSucceeded;
	}

	public ProfileManager.Result CreateNewCurrentProfile()
	{
		if (!File.Exists(this.GetProfileFilePath(0)))
		{
			this.m_CurrentProfile = Profile.CreateProfile();
			this.SaveCurrentProfile();
			return ProfileManager.Result.eSucceeded;
		}
		return ProfileManager.Result.eExceedMaxSlots;
	}

	public void RemoveProfile(int aProfileID)
	{
		string profileFilePath = this.GetProfileFilePath(aProfileID);
		if (File.Exists(profileFilePath))
		{
			File.Delete(profileFilePath);
		}
	}

	public string GetProfileFilePath(int aProfileID)
	{
		return Path.Combine(Application.persistentDataPath, "Profile" + aProfileID.ToString() + ".dat");
	}

	public bool DoesProfileExist(int aProfileID)
	{
		return File.Exists(this.GetProfileFilePath(aProfileID));
	}

	public bool DoesCurrentProfileExist()
	{
		return this.DoesProfileExist(this.CurrentProfileID);
	}

	private void Awake()
	{
		ProfileManager.mInstance = this;
		this.m_CurrentProfile = Profile.CreateProfile();
		this.LoadProfile(this.CurrentProfileID, ref this.m_CurrentProfile);
		if (PlayerPrefs.GetInt("ClearedData", 0) == 0)
		{
			if (this.CurrentProfile.BuildVersion >= Utilities.CurrentBuildNumber)
			{
				this.RemoveProfile(0);
				this.CreateNewCurrentProfile();
				PlayerPrefs.DeleteAll();
			}
			PlayerPrefs.SetInt("ClearedData", 1);
			PlayerPrefs.Save();
		}
		this.CurrentProfile.m_LevelData[0].LevelUnlocked = true;
		for (int i = 1; i <= 23; i++)
		{
			if (!this.CurrentProfile.m_LevelData[i].LevelUnlocked && this.CurrentProfile.m_LevelData[i - 1].LevelComplete)
			{
				this.CurrentProfile.m_LevelData[i].LevelUnlocked = true;
				break;
			}
		}
		this.CurrentProfile.m_LevelData[24].LevelUnlocked = true;
		for (int j = 25; j <= 59; j++)
		{
			if (!this.CurrentProfile.m_LevelData[j].LevelUnlocked && this.CurrentProfile.m_LevelData[j - 1].LevelComplete)
			{
				this.CurrentProfile.m_LevelData[j].LevelUnlocked = true;
				break;
			}
		}
		this.CurrentProfile.BuildVersion = Utilities.CurrentBuildNumber;
		this.SaveCurrentProfile();
	}
}
