using System;
using System.Collections.Generic;
using UnityEngine;

public static class BizIntel
{
	private static void LogSimpleEvent(string eventDescription)
	{
		if (Application.isEditor)
		{
			return;
		}
	}

	public static void StartBizIntel()
	{
		if (Application.isEditor)
		{
			return;
		}

#if UNITY_ANDROID || UNITY_IOS
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		if (@static != null)
		{
			AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getApplication", new object[0]);
			if (androidJavaObject != null)
			{
				BizIntel.m_appMeasurement = new AndroidJavaObject("com.omniture.AppMeasurement", new object[] { androidJavaObject });
				if (BizIntel.m_appMeasurement != null)
				{
					BizIntel.m_appMeasurement.Set<string>("account", "wdgwdolcppuffleandroid");
					BizIntel.m_appMeasurement.Set<string>("pageName", string.Empty);
					BizIntel.m_appMeasurement.Set<string>("pageURL", string.Empty);
					BizIntel.m_appMeasurement.Set<string>("currencyCode", "USD");
					BizIntel.m_appMeasurement.Set<string>("trackingServer", "mdisney.112.2o7.net");
				}
			}
		}
#endif
	}

	public static void StopBizIntel()
	{
		if (Application.isEditor)
		{
			return;
		}
		if (BizIntel.m_appMeasurement != null)
		{
			BizIntel.m_appMeasurement.Dispose();
			BizIntel.m_appMeasurement = null;
		}
	}

	private static AndroidJavaObject m_appMeasurement;

	public class ContextualEvent
	{
		public ContextualEvent(string aScope)
		{
			this.m_Scope = aScope;
			this.m_Context = new List<BizIntel.ContextualEvent.KeyValue>();
		}

		public void AddContextItem(string aKey, string aValue)
		{
			if (aValue == null)
			{
				aValue = " ";
			}
			this.m_Context.Add(new BizIntel.ContextualEvent.KeyValue(aKey, aValue));
		}

		public void AddContextItem(string aKey, int aValue)
		{
			this.m_Context.Add(new BizIntel.ContextualEvent.KeyValue(aKey, string.Empty + aValue));
		}

		public void AddContextItem(string aKey, bool aValue)
		{
			this.m_Context.Add(new BizIntel.ContextualEvent.KeyValue(aKey, string.Empty + aValue));
		}

		public void Log()
		{
			if (Application.isEditor)
			{
				return;
			}
			if (BizIntel.m_appMeasurement != null)
			{
				BizIntel.m_appMeasurement.Call("clearVars", new object[0]);
				int num = 1;
				foreach (BizIntel.ContextualEvent.KeyValue keyValue in this.m_Context)
				{
					string text = string.Format("prop{0}", num);
					string text2 = string.Format("{0}={1}", keyValue.m_Key, keyValue.m_Value);
					BizIntel.m_appMeasurement.Set<string>(text, text2);
					num++;
					if (num > 50)
					{
						break;
					}
				}
				string text3 = string.Format("Puffle Launch Android - {0}", this.m_Scope);
				BizIntel.m_appMeasurement.Set<string>("pageName", text3);
				BizIntel.m_appMeasurement.Call<string>("track", new object[0]);
			}
		}

		private string m_Scope;

		private List<BizIntel.ContextualEvent.KeyValue> m_Context;

		private class KeyValue
		{
			public KeyValue(string aKey, string aValue)
			{
				this.m_Key = aKey;
				this.m_Value = aValue;
			}

			public string m_Key;

			public string m_Value;
		}
	}
}
