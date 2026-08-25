using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public static class Utilities
{
	public static int CurrentBuildNumber
	{
		get
		{
			return Utilities.m_cCurrentBuildVersion;
		}
	}

	public static string CurrentBuildString
	{
		get
		{
			int num = Utilities.m_cCurrentBuildVersion;
			int num2 = ((num <= 99) ? 2 : 3);
			string text = string.Empty;
			for (int i = num2; i > 0; i--)
			{
				int num3 = (int)Mathf.Pow(10f, (float)(i - 1));
				int num4 = num / num3;
				num -= num4 * num3;
				text = text + num4.ToString() + ".";
			}
			return text.Remove(text.LastIndexOf("."));
		}
	}

	public static float AspectRatio
	{
		get
		{
			return (float)Screen.width / (float)Screen.height;
		}
	}

	public static float ReferenceAspectRatio
	{
		get
		{
			return GUIConstants.kReferenceScreenWidth / GUIConstants.kReferenceScreenHeight;
		}
	}

	public static bool RectHitTest(Vector3 aHitPosition, Rect aRect)
	{
		return aHitPosition.x >= aRect.x && aHitPosition.x <= aRect.x + aRect.width && aHitPosition.y >= aRect.y && aHitPosition.y <= aRect.y + aRect.height;
	}

	public static bool Assert(bool aCondition)
	{
		return true;
	}

	public static bool AssertMsg(bool aCondition, string aMsg)
	{
		if (aCondition == false)
		{
			Debug.LogWarning(aMsg);
		}

		return true;
	}

	public static bool AssertMsgCritical(bool aCondition, string aMsg)
	{
		if (aCondition == false)
		{
			Debug.LogError(aMsg);
		}

		return true;
	}

	private static bool AssertMsgHelper(bool aCondition, string aMsg, StackFrame aStackFrame, bool aIsCritical)
	{
		if (aCondition == false)
		{
			if (aIsCritical)
			{
				Debug.LogError(aMsg);
			}
			else
			{
				Debug.LogWarning(aMsg);
			}
		}

		return true;
	}

	private static string FilepathTrimmed(string aFilename)
	{
		string text = "\\Unity\\";
		int num = aFilename.IndexOf(text);
		num += text.Length;
		return aFilename.Substring(num);
	}

	public static int RandomRange(int aLow, int aHigh)
	{
		return Random.Range(aLow, aHigh);
	}

	public static bool IsFloatEqual(float aLHS, float aRHS)
	{
		return Utilities.IsFloatEqual(aLHS, aRHS, 0.01f);
	}

	public static bool IsFloatEqual(float aLHS, float aRHS, float epsilon)
	{
		float num = Mathf.Abs(aRHS - aLHS);
		return num <= epsilon;
	}

	public static void CreateFolderPath(string aCurrentPath)
	{
		if (Directory.Exists(aCurrentPath))
		{
			return;
		}
		Utilities.CreateFolderPath(aCurrentPath.Remove(aCurrentPath.LastIndexOf("/")));
		Directory.CreateDirectory(aCurrentPath);
		Debug.Log("Utilities::CreateFolderPath - Created folder path: " + aCurrentPath);
	}

	public static bool ArrayContains(string[] aArray, string aElem)
	{
		foreach (string text in aArray)
		{
			if (text == aElem)
			{
				return true;
			}
		}
		return false;
	}

	[DllImport("Texture")]
	public static extern string getSupportedTextureFormat();

	public static void GetSupportedTextureFormats()
	{
		string supportedTextureFormat = Utilities.getSupportedTextureFormat();
		if (supportedTextureFormat.Contains("GL_AMD_compressed_ATC_texture"))
		{
			Debug.Log("GATES --- This is a Qualcom ATI Device");
		}
		if (supportedTextureFormat.Contains("EXT_texture_compression_dxt1"))
		{
			Debug.Log("GATES --- This is a NVIDIA Tegra Device");
		}
		if (supportedTextureFormat.Contains("GL_IMG_texture_compression_pvrtc"))
		{
			Debug.Log("GATES --- This is a TI PowerVR Device");
		}
	}

	private static int m_cCurrentBuildVersion = 13;

	public static float m_cTargetWidthIphone = 960f;

	public static float m_cTargetHeightIphone = 640f;

	public static float m_cTargetWidth = 1024f;

	public static float m_cTargetHeight = 768f;

	public static Color[] m_cPenguinColors = new Color[]
	{
		new Color(0f, 0.2f, 0.4f),
		new Color(0f, 0.2f, 0.4f),
		new Color(0f, 0.6f, 0f),
		new Color(1f, 0.2f, 0.6f),
		new Color(0.2f, 0.2f, 0.2f),
		new Color(0.8f, 0f, 0f),
		new Color(1f, 0.4f, 0f),
		new Color(1f, 0.8f, 0f),
		new Color(0.4f, 0f, 0.6f),
		new Color(0.6f, 0.4f, 0f),
		new Color(1f, 0.4f, 0.4f),
		new Color(0f, 0.4f, 0f),
		new Color(0f, 0.6f, 0.8f),
		new Color(0.5411765f, 0.8901961f, 0.007843138f),
		new Color(0.5764706f, 0.627451f, 0.6431373f),
		new Color(0.007843138f, 0.654902f, 0.5921569f)
	};

	public static Color[] m_cPenguinHightlightColors = new Color[]
	{
		new Color(0.12156863f, 0.4f, 0.6156863f),
		new Color(0.12156863f, 0.4f, 0.6156863f),
		new Color(0.15686275f, 0.75686276f, 0.15686275f),
		new Color(1f, 0.43137255f, 0.8352941f),
		new Color(0.33333334f, 0.33333334f, 0.33333334f),
		new Color(0.95686275f, 0.15686275f, 0.15686275f),
		new Color(1f, 0.5529412f, 0.15686275f),
		new Color(1f, 0.95686275f, 0.15686275f),
		new Color(0.5568628f, 0.16078432f, 0.7607843f),
		new Color(0.75686276f, 0.5529412f, 0.15686275f),
		new Color(1f, 0.5529412f, 0.5529412f),
		new Color(0.15686275f, 0.5529412f, 0.15686275f),
		new Color(0.15686275f, 0.75686276f, 0.95686275f),
		new Color(0.50980395f, 1f, 0.16470589f),
		new Color(0.49803922f, 0.54901963f, 0.5647059f),
		new Color(0.24313726f, 0.7921569f, 0.73333335f)
	};

	public static Color[] m_cPenguinShadowColors = new Color[]
	{
		new Color(0f, 0f, 0.2f),
		new Color(0f, 0f, 0.2f),
		new Color(0f, 0.40392157f, 0f),
		new Color(0.8039216f, 0f, 0.40392157f),
		new Color(0.15686275f, 0.15686275f, 0.15686275f),
		new Color(0.6039216f, 0f, 0f),
		new Color(0.8039216f, 0.2f, 0f),
		new Color(0.8039216f, 0.6039216f, 0f),
		new Color(0.2f, 0f, 0.4f),
		new Color(0.40392157f, 0.2f, 0f),
		new Color(0.8039216f, 0.2f, 0.2f),
		new Color(0f, 0.2f, 0f),
		new Color(0f, 0.43529412f, 0.64705884f),
		new Color(0.4f, 0.73333335f, 0f),
		new Color(0.654902f, 0.7058824f, 0.72156864f),
		new Color(0f, 0.5058824f, 0.4862745f)
	};

	public enum PenguinColors
	{
		eDefaultBlue,
		eBlue,
		eGreen,
		ePink,
		eBlack,
		eRed,
		eOrange,
		eYellowMustard,
		eDarkPurple,
		eBrown,
		ePeach,
		eDarkGreen,
		eLightBlue,
		eLimeGreen,
		eGray,
		eAqua,
		ePenguinColor_COUNT
	}
}
