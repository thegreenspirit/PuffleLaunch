using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LocalizationManager
{
	public static LocalizationManager Instance
	{
		get
		{
			if (LocalizationManager.m_cInstance == null)
			{
				LocalizationManager.m_cInstance = new LocalizationManager();
				LocalizationManager.m_cInstance.LoadData();
			}
			return LocalizationManager.m_cInstance;
		}
	}

	public static bool IsFrench
	{
		get
		{
			return LocalizationManager.GetLanguageCode() == "fr";
		}
	}

	public static bool IsPortuguese
	{
		get
		{
			return LocalizationManager.GetLanguageCode() == "pt";
		}
	}

	public static bool IsSpanish
	{
		get
		{
			return LocalizationManager.GetLanguageCode() == "es";
		}
	}

	public static bool IsEnglish
	{
		get
		{
			return LocalizationManager.GetLanguageCode() == "en";
		}
	}

	public static bool IsGerman
	{
		get
		{
			return LocalizationManager.GetLanguageCode() == "de";
		}
	}

	public static bool IsJapanese
	{
		get
		{
			return LocalizationManager.GetLanguageCode() == "ja";
		}
	}

	public static string GetLanguageCode()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		if (systemLanguage == SystemLanguage.French)
		{
			return "fr";
		}
		if (systemLanguage == SystemLanguage.German)
		{
			return "de";
		}
		if (systemLanguage == SystemLanguage.Portuguese)
		{
			return "pt";
		}
		if (systemLanguage != SystemLanguage.Spanish)
		{
			return "en";
		}
		return "es";
	}

	public static string GetRegionCode()
	{
#if UNITY_ANDROID || UNITY_IOS
		// Green Spirit: Android shit, fixed this up and changed how this works
		try
		{
			using (AndroidJavaClass localeClass = new AndroidJavaClass("java.util.Locale"))
			using (AndroidJavaObject defaultLocale = localeClass.CallStatic<AndroidJavaObject>("getDefault"))
			{
				string country = defaultLocale.Call<string>("getCountry");

				if (country == "AR")
				{
					return "es_AR";
				}
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Failed to fetch Android region code: " + ex.Message);
		}
#endif
		return string.Empty;
	}

	public void LoadData()
	{
		this.m_TextDict = new Dictionary<string, string>();
		TextAsset textAsset = Resources.Load(this.m_FilePathPrefix + LocalizationManager.GetLanguageCode(), typeof(TextAsset)) as TextAsset;
		if (textAsset != null)
		{
			StringReader stringReader = new StringReader(textAsset.text);
			bool flag = this.HasUTF8BOM(textAsset.bytes);
			int num = 0;
			string text = stringReader.ReadLine();
			while (text != null)
			{
				int num2 = text.IndexOf('\t');
				if (num2 != -1)
				{
					string text2 = text.Substring(0, num2);
					string text3 = text.Substring(num2 + 1);
					if (flag && num == 0)
					{
						text2 = this.RemoveUTF8BOM(text2);
					}
					this.m_TextDict[text2] = text3.Replace("\\n", "\n");
				}
				text = stringReader.ReadLine();
				num++;
			}
		}
		this.m_TermsOfUseTextDict = new Dictionary<string, string>();
		textAsset = Resources.Load(this.m_TermsOfUseFilePathPrefix + LocalizationManager.GetLanguageCode(), typeof(TextAsset)) as TextAsset;
		if (textAsset != null)
		{
			StringReader stringReader2 = new StringReader(textAsset.text);
			for (string text4 = stringReader2.ReadLine(); text4 != null; text4 = stringReader2.ReadLine())
			{
				int num3 = text4.IndexOf('\t');
				if (num3 != -1)
				{
					string text5 = text4.Substring(0, num3);
					string text6 = text4.Substring(num3 + 1);
					this.m_TermsOfUseTextDict[text5] = text6.Replace("\\n", "\n");
				}
			}
		}
		LocalizationManager.m_cInstance = this;
	}

	public bool HasUTF8BOM(byte[] aBytes)
	{
		int num = this.kUTF8ByteOrederMarks.Length;
		if (aBytes.Length < num)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (aBytes[i] != this.kUTF8ByteOrederMarks[i])
			{
				return false;
			}
		}
		return true;
	}

	public string RemoveUTF8BOM(string aString)
	{
		char[] array = aString.ToCharArray();
		StringBuilder stringBuilder = new StringBuilder(string.Empty);
		foreach (char c in array)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(new char[] { c });
			if (bytes.Length > 2)
			{
				stringBuilder.Append("?");
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		string text = stringBuilder.ToString().Trim();
		return text.TrimStart(new char[] { '?' });
	}

	public string GetTOUString(string aTextId)
	{
		if (this.m_TermsOfUseTextDict.ContainsKey(aTextId))
		{
			return this.m_TermsOfUseTextDict[aTextId];
		}
		return aTextId;
	}

	public string GetString(string aTextId)
	{
		if (this.m_TextDict.ContainsKey(aTextId))
		{
			return this.m_TextDict[aTextId];
		}
		return aTextId;
	}

	public string GetString(string aTextId, object aObject)
	{
		string text = this.GetString(aTextId);
		if (text != aTextId && aObject != null)
		{
			text = string.Format(text, aObject);
		}
		return text;
	}

	public string GetString(string aTextId, object aObject1, object aObject2)
	{
		string text = this.GetString(aTextId);
		if (text != aTextId && aObject1 != null && aObject2 != null)
		{
			text = string.Format(text, aObject1, aObject2);
		}
		return text;
	}

	public const string kEnglishLocale = "en";

	public const string kFrenchLocale = "fr";

	public const string kSpanishLocale = "es";

	public const string kPortugueseLocale = "pt";

	public const string kGermanLocale = "de";

	public const string kJapaneseLocale = "ja";

	public const string kArgentinaRegion = "es_AR";

	public const string kUnknownRegion = "";

	public byte[] kUTF8ByteOrederMarks = new byte[] { 239, 187, 191 };

	private static LocalizationManager m_cInstance;

	private Dictionary<string, string> m_TextDict;

	private Dictionary<string, string> m_TermsOfUseTextDict;

	private string m_FilePathPrefix = "Text/LocalizedText_";

	private string m_TermsOfUseFilePathPrefix = "Text/tou_clubpios_";

	public enum Language
	{
		eEnglish,
		eFrench,
		eSpanish,
		ePortuguese,
		eGerman,
		eJanpanies,
		eLanguage_COUNT
	}
}
