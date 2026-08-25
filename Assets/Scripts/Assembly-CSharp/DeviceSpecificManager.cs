using System;
using UnityEngine;

public class DeviceSpecificManager
{
	public DeviceSpecificManager()
	{
		Debug.Log("DeviceSpecificManager\n");
		switch (Application.platform)
		{
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			this.m_DataPath = Application.dataPath + "/StreamingAssets_PC/";
			goto IL_00A5;
		case RuntimePlatform.IPhonePlayer:
			this.m_DataPath = Application.dataPath + "/Raw/";
			goto IL_00A5;
		}
		this.m_DataPath = Application.dataPath + "/";
		IL_00A5:
		this.m_AnimationDataPath = this.m_DataPath;
		this.m_TileSize = 1024U;
	}

	private static DeviceSpecificManager get()
	{
		if (DeviceSpecificManager.s_Instance == null)
		{
			DeviceSpecificManager.s_Instance = new DeviceSpecificManager();
		}
		return DeviceSpecificManager.s_Instance;
	}

	public static string GetBasePath()
	{
		return DeviceSpecificManager.get().m_DataPath;
	}

	public static string GetAnimationBasePath()
	{
		return DeviceSpecificManager.get().m_AnimationDataPath;
	}

	public static uint GetTileSize()
	{
		return DeviceSpecificManager.get().m_TileSize;
	}

	public static bool CanDownloadInGameplay()
	{
		return DeviceSpecificManager.get().m_DownloadInGameplay;
	}

	public static bool IsBluetoothSupported()
	{
		return DeviceSpecificManager.get().m_IsBluetoothSupported;
	}

	private uint m_TileSize = 1024U;

	private string m_AnimationDataPath;

	private string m_DataPath;

	private bool m_DownloadInGameplay = true;

	private static DeviceSpecificManager s_Instance;

	private bool m_IsBluetoothSupported;
}
