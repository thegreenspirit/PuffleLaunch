using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CinematicDebug
{
	[Conditional("ENABLE_CINEMETIC_DEBUG")]
	public static void Log(string message)
	{
		Debug.Log(message);
	}

	[Conditional("ENABLE_CINEMETIC_DEBUG")]
	public static void LogWarning(string message)
	{
		Debug.LogWarning(message);
	}
}
