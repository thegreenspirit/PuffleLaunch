using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class GUICompoundControls
{
	private static TouchScreenKeyboard m_cKeyboard = null;
	private static TextInfo m_TextInfo = CultureInfo.InvariantCulture.TextInfo;
	private static List<GUIDefines.AutoResizeData> m_AutoResizeData = null;

	private static void InitAutoResizeData()
	{
		if (GUICompoundControls.m_AutoResizeData == null)
		{
			GUICompoundControls.m_AutoResizeData = new List<GUIDefines.AutoResizeData>();
		}
		GUICompoundControls.m_AutoResizeData.Clear();
	}

	public static int Buttons(Vector3 aReferencePos, GUIDefines.ButtonData[] aButtonData)
	{
		int num = -1;
		if (GameFlowManager.Instance.GUIManager.EnableAutoResize)
		{
			GUICompoundControls.InitAutoResizeData();
			for (int i = 0; i < aButtonData.Length; i++)
			{
				if (aButtonData[i] == null || aButtonData[i].isAutoResizeOff)
				{
					Utilities.AssertMsg(aButtonData[i] != null, "Button data () is null!");
				}
				else
				{
					Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aButtonData[i].pos);
					GUIContent guicontent = GUIUtil.CreateGuiContent(aButtonData[i].content);
					GUIStyle guiStyle = GUIUtil.GetGuiStyle(aButtonData[i].style);
					if (GUIUtil.AutoResizeAccordingToContent(guiStyle, guicontent, aButtonData[i].autoResizeAllignment, ref rect))
					{
						GUIDefines.AutoResizeData autoResizeData = new GUIDefines.AutoResizeData();
						autoResizeData.groupId = aButtonData[i].autoResizeGroupId;
						autoResizeData.index = i;
						autoResizeData.pos = new Rect(rect);
						GUICompoundControls.m_AutoResizeData.Add(autoResizeData);
					}
				}
			}
		}
		for (int j = 0; j < aButtonData.Length; j++)
		{
			if (aButtonData[j] != null && !aButtonData[j].invisible)
			{
				bool isControlBlocked = aButtonData[j].isControlBlocked;
				Rect rect2 = GUIUtil.ConvertToRelativePos(aReferencePos, aButtonData[j].pos);
				GUIContent guicontent2 = GUIUtil.CreateGuiContent(aButtonData[j].content);
				GUIStyle guistyle = GUIUtil.GetGuiStyle(aButtonData[j].style);
				if (guistyle == null)
				{
					guistyle = GUI.skin.button;
				}
				GUIStyle guistyle2 = GUIUtil.CreateDropShadowTextStyle(guistyle);
				GUIStyle guistyle3 = GUIUtil.CreateFrontTextStyle(guistyle);
				if (GameFlowManager.Instance.GUIManager.EnableAutoResize)
				{
					foreach (GUIDefines.AutoResizeData autoResizeData2 in GUICompoundControls.m_AutoResizeData)
					{
						if (j == autoResizeData2.index || (aButtonData[j].useAutoResizeGroup && aButtonData[j].autoResizeGroupId == autoResizeData2.groupId))
						{
							rect2.x = autoResizeData2.pos.x;
							rect2.width = autoResizeData2.pos.width;
						}
					}
				}
				Rect rect3 = rect2;
				if (aButtonData[j].detectZoneScale > 0f)
				{
					rect3.width *= aButtonData[j].detectZoneScale;
					rect3.height *= aButtonData[j].detectZoneScale;
					rect3.xMin -= rect3.width - rect2.width;
					rect3.yMin -= rect3.height - rect2.height;
				}
				bool flag = false;
				bool flag2 = Input.touchCount <= 1;
				flag2 &= !isControlBlocked;
				if (aButtonData[j].isTogglable)
				{
					if (flag2)
					{
						bool flag3 = GUI.Toggle(rect2, aButtonData[j].toggleState, guicontent2, guistyle2);
						if (flag3 != aButtonData[j].toggleState)
						{
							aButtonData[j].toggleState = flag3;
							flag = true;
						}
					}
					else
					{
						GUI.Label(rect2, guicontent2, guistyle2);
					}
				}
				else if (flag2)
				{
					flag = GUI.Button(rect2, guicontent2, guistyle2);
				}
				else
				{
					GUI.Label(rect2, guicontent2, guistyle2);
				}
				guicontent2.image = null;
				GUI.Label(rect2, guicontent2, guistyle3);
				if (aButtonData[j].detectZoneScale > 0f && flag2)
				{
					flag = flag || GUI.Button(rect3, string.Empty);
				}
				if (flag)
				{
					num = aButtonData[j].buttonId;
				}
			}
		}
		return num;
	}

	public static int MultiPageGroupButtons(Vector3 aReferencePos, GUIDefines.GroupButtonData aGroupButtonData, int aStartAtElement)
	{
		if (aStartAtElement >= aGroupButtonData.elements.Length)
		{
			return -1;
		}
		int num = -1;
		int num2 = aGroupButtonData.multiPage.elementPerRow * aGroupButtonData.multiPage.elementPerCol;
		int num3 = 0;
		GUILayoutOption[] array = GUIUtil.CreateGuiLayoutOptions(aGroupButtonData.size);
		Vector2 space = GUIUtil.GetSpace(aGroupButtonData.space);
		Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aGroupButtonData.area);
		GUILayout.BeginArea(rect);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		bool flag = false;
		for (int i = 0; i < aGroupButtonData.multiPage.elementPerCol; i++)
		{
			if (flag)
			{
				break;
			}
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			for (int j = 0; j < aGroupButtonData.multiPage.elementPerRow; j++)
			{
				int num4 = i * aGroupButtonData.multiPage.elementPerRow + j + aStartAtElement;
				if (num3 >= num2 || num4 >= aGroupButtonData.elements.Length)
				{
					flag = true;
					break;
				}
				GUIStyle guiStyle = GUIUtil.GetGuiStyle(aGroupButtonData.elements[num4].style);
				GUIContent guicontent = GUIUtil.CreateGuiContent(aGroupButtonData.elements[num4].content);
				bool flag2;
				if (guiStyle == null)
				{
					flag2 = GUILayout.Button(guicontent, array);
				}
				else
				{
					flag2 = GUILayout.Button(guicontent, guiStyle, array);
				}
				if (flag2)
				{
					num = aGroupButtonData.elements[num4].buttonId;
				}
				GUILayout.Space(space.x);
				num3++;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(space.y);
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		return num;
	}

	public static int HorizontalGroupButtons(Vector3 aReferencePos, GUIDefines.GroupButtonData aGroupButtonData)
	{
		int num = -1;
		GUIStyle guiStyle = GUIUtil.GetGuiStyle(aGroupButtonData.style);
		Vector2 space = GUIUtil.GetSpace(aGroupButtonData.space);
		Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aGroupButtonData.area);
		GUILayout.BeginArea(rect);
		for (int i = 0; i < aGroupButtonData.elements.Length; i++)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUIContent guicontent = GUIUtil.CreateGuiContent(aGroupButtonData.elements[i].content);
			bool flag;
			if (guiStyle == null)
			{
				flag = GUILayout.Button(guicontent, new GUILayoutOption[0]);
			}
			else
			{
				flag = GUILayout.Button(guicontent, guiStyle, new GUILayoutOption[0]);
			}
			if (flag)
			{
				num = aGroupButtonData.elements[i].buttonId;
			}
			GUILayout.Space(space.x);
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		return num;
	}

	public static int VertialGroupButtons(Vector3 aReferencePos, GUIDefines.GroupButtonData aGroupButtonData)
	{
		int num = -1;
		GUIStyle guiStyle = GUIUtil.GetGuiStyle(aGroupButtonData.style);
		Vector2 space = GUIUtil.GetSpace(aGroupButtonData.space);
		Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aGroupButtonData.area);
		GUILayout.BeginArea(rect);
		for (int i = 0; i < aGroupButtonData.elements.Length; i++)
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUIContent guicontent = GUIUtil.CreateGuiContent(aGroupButtonData.elements[i].content);
			bool flag;
			if (guiStyle == null)
			{
				flag = GUILayout.Button(guicontent, new GUILayoutOption[0]);
			}
			else
			{
				flag = GUILayout.Button(guicontent, guiStyle, new GUILayoutOption[0]);
			}
			if (flag)
			{
				num = aGroupButtonData.elements[i].buttonId;
			}
			GUILayout.Space(space.y);
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
		return num;
	}

	public static void Textures(Vector3 aReferencePos, GUIDefines.TextureData[] aTextureData)
	{
		for (int i = 0; i < aTextureData.Length; i++)
		{
			if (!aTextureData[i].invisible && !(aTextureData[i].icon.image == null))
			{
				if (aTextureData[i].bgInfo != null)
				{
					GUIUtil.ApplyBgColor(aTextureData[i].bgInfo.useBgColor, aTextureData[i].bgInfo.bgColor, false);
				}
				Matrix4x4 matrix = GUI.matrix;
				Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aTextureData[i].pos);
				if (aTextureData[i].rotate != GUIDefines.RotateDirection.eNone)
				{
					Vector2 vector = new Vector2(rect.xMin + rect.width / 2f, rect.yMin + rect.height / 2f);
					vector.x *= (float)Screen.width / GUIConstants.kReferenceScreenWidth;
					vector.y *= (float)Screen.height / GUIConstants.kReferenceScreenHeight;
					if (aTextureData[i].pivotPointOffset != null)
					{
						Vector2 space = GUIUtil.GetSpace(aTextureData[i].pivotPointOffset);
						vector.x += space.x;
						vector.y += space.y;
					}
					GUIUtility.RotateAroundPivot(aTextureData[i].rotateAngle, vector);
				}
				if (aTextureData[i].tiled)
				{
					float width = aTextureData[i].tileSize.inPixel.width;
					float height = aTextureData[i].tileSize.inPixel.height;
					Rect rect2 = new Rect(0f, 0f, width, height);
					for (float num = 0f; num < rect.width; num += width)
					{
						for (float num2 = 0f; num2 < rect.height; num2 += height)
						{
							rect2.x = rect.xMin + num;
							rect2.y = rect.yMin + num2;
							GUI.DrawTexture(rect2, aTextureData[i].icon.image);
						}
					}
				}
				else
				{
					GUI.DrawTexture(rect, aTextureData[i].icon.image);
				}
				if (aTextureData[i].rotate != GUIDefines.RotateDirection.eNone)
				{
					GUIDefines.RotateDirection rotate = aTextureData[i].rotate;
					if (rotate != GUIDefines.RotateDirection.eClockwise)
					{
						if (rotate == GUIDefines.RotateDirection.eCounterClockwise)
						{
							aTextureData[i].rotateAngle -= 4f;
						}
					}
					else
					{
						aTextureData[i].rotateAngle += 4f;
					}
					GUI.matrix = matrix;
				}
				if (aTextureData[i].bgInfo != null)
				{
					GUIUtil.RestoreBgColor(aTextureData[i].bgInfo.useBgColor, false);
				}
			}
		}
	}

	public static void FullScreenTexture(GUIDefines.TextureInfo aTextureInfo)
	{
		Rect rect = GUIUtil.FullScreenRect();
		GUI.DrawTexture(rect, aTextureInfo.image);
	}

	public static void Labels(Vector3 aReferencePos, GUIDefines.LabelData[] aLabelData)
	{
		for (int i = 0; i < aLabelData.Length; i++)
		{
			if (!aLabelData[i].invisible)
			{
				if (aLabelData[i].bgInfo != null)
				{
					GUIUtil.ApplyBgColor(aLabelData[i].bgInfo.useBgColor, aLabelData[i].bgInfo.bgColor, true);
				}
				Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aLabelData[i].pos);
				Rect rect2 = default(Rect);
				GUIContent guicontent = GUIUtil.CreateGuiContent(aLabelData[i].content);
				GUIStyle guistyle = GUIUtil.GetGuiStyle(aLabelData[i].style);
				if (guistyle == null)
				{
					guistyle = GUI.skin.label;
				}
				GUIStyle guistyle2;
				if (aLabelData[i].style != null && aLabelData[i].style.useCustomDropShadowColor)
				{
					guistyle2 = GUIUtil.CreateCustomDropShadowTextStyleForLabel(guistyle, aLabelData[i].style.customDropShadowColor);
				}
				else
				{
					guistyle2 = GUIUtil.CreateDropShadowTextStyleForLabel(guistyle);
				}
				GUIStyle guistyle3;
				if (aLabelData[i].disableDropShadow)
				{
					guistyle3 = GUIUtil.CreateFrontTextStyleWithNoDropShadow(guistyle);
				}
				else
				{
					guistyle3 = GUIUtil.CreateFrontTextStyle(guistyle);
				}
				if (aLabelData[i].disableDropShadow)
				{
					GUI.Label(rect, guicontent, guistyle3);
				}
				else
				{
					float num;
					float num2;
					if (aLabelData[i].style != null && aLabelData[i].style.useCustomDropShadowOffset)
					{
						num = aLabelData[i].style.customDropShadowOffset.x;
						num2 = aLabelData[i].style.customDropShadowOffset.y;
						if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eLowres)
						{
							num *= 0.5f;
							num2 *= 0.5f;
						}
					}
					else
					{
						num = (float)GameFlowManager.Instance.GUIManager.DropShadowOffsetX;
						num2 = (float)GameFlowManager.Instance.GUIManager.DropShadowOffsetY;
					}
					rect2.xMin = rect.xMin + num;
					rect2.yMin = rect.yMin + num2;
					rect2.xMax = rect.xMax + num;
					rect2.yMax = rect.yMax + num2;
					GUI.Label(rect2, guicontent, guistyle2);
					guicontent.image = null;
					GUI.Label(rect, guicontent, guistyle3);
				}
				if (aLabelData[i].bgInfo != null)
				{
					GUIUtil.RestoreBgColor(aLabelData[i].bgInfo.useBgColor, true);
				}
			}
		}
	}

	public static string[] TextFields(Vector3 aReferencePos, GUIDefines.TextFieldData[] aTextFieldData, bool aIsControlBlocked)
	{
		string[] array = new string[aTextFieldData.Length];
		for (int i = 0; i < aTextFieldData.Length; i++)
		{
			array[i] = string.Empty;
			Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aTextFieldData[i].pos);
			GUIUtil.SetControlName(aTextFieldData[i].controlName);
			bool flag = aTextFieldData[i].isFocused;
			bool flag2 = false;
			bool flag3 = false;
			GUIStyle guiStyle = GUIUtil.GetGuiStyle(aTextFieldData[i].style);
			if (guiStyle != null)
			{
				GUI.skin.textField.normal.background = guiStyle.normal.background;
				GUI.skin.textField.hover.background = guiStyle.normal.background;
				GUI.skin.textField.active.background = guiStyle.active.background;
				GUI.skin.textField.normal.textColor = GUIConstants.kGreyColor;
				GUI.skin.textField.active.textColor = guiStyle.active.textColor;
				GUI.skin.textField.focused.textColor = guiStyle.focused.textColor;
				GUI.skin.settings.cursorColor = Color.clear;
				GUI.skin.settings.selectionColor = GUIConstants.kGreyColor;
				if (flag)
				{
					GUI.skin.textField.normal.background = GUI.skin.textField.focused.background;
					GUI.skin.textField.hover.background = GUI.skin.textField.focused.background;
				}
			}
			GUIStyle guistyle = new GUIStyle(GUI.skin.textField);
			guistyle.normal.background = null;
			GUIStyle guistyle2 = new GUIStyle(guistyle);
			guistyle2.normal.textColor = guiStyle.focused.textColor;
			string text = string.Empty;
			string text2 = string.Empty;

			if (aTextFieldData[i].editedText != null && aTextFieldData[i].editedText.Length > 0)
			{
				text2 = aTextFieldData[i].editedText;
			}
#if UNITY_ANDROID || UNITY_IOS
			else if (aTextFieldData[i].defaultTextId != null && aTextFieldData[i].defaultTextId.Length > 0 && (!flag || !TouchScreenKeyboard.visible))
			{
				text = LocalizationManager.Instance.GetString(aTextFieldData[i].defaultTextId);
			}
#else
			else if (aTextFieldData[i].defaultTextId != null && aTextFieldData[i].defaultTextId.Length > 0 && (!flag))
			{
				text = LocalizationManager.Instance.GetString(aTextFieldData[i].defaultTextId);
			}
#endif

			if (aTextFieldData[i].isReadOnly)
			{
				array[i] = text2;
				GUI.Label(rect, text2, GUI.skin.textField);
			}
			else
			{
				GUI.skin.textField.normal.textColor = Color.clear;
				GUI.skin.textField.hover.textColor = Color.clear;
				GUI.skin.textField.active.textColor = Color.clear;
				GUI.skin.textField.focused.textColor = Color.clear;

				// Green Spirit: The if exp below was just "true", so I assumed it was a platform thing
				if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
				{
					flag2 = GUI.Button(rect, string.Empty, GUI.skin.textField);
				}
				else
				{
					// Green Spirit: Fix compatability for desktop envs
					if (!aIsControlBlocked)
					{
						array[i] = GUI.TextField(rect, text2, GUI.skin.textField);

						if (array[i] != text2) 
						{
							if (array[i].Length > aTextFieldData[i].maxLength)
							{
								array[i] = array[i].Substring(0, aTextFieldData[i].maxLength);
							}
							if (aTextFieldData[i].titleCase)
							{
								array[i] = GUICompoundControls.m_TextInfo.ToTitleCase(array[i]);
							}

							flag3 = true; 
						}
					}
					else
					{
						GUI.Label(rect, string.Empty, GUI.skin.textField);
					}
				}

#if UNITY_ANDROID || UNITY_IOS
				if (flag2 && !aIsControlBlocked)
				{
					if (GUICompoundControls.m_cKeyboard != null)
					{
						GUICompoundControls.m_cKeyboard.active = false;
						GUICompoundControls.m_cKeyboard = null;
						float realtimeSinceStartup = Time.realtimeSinceStartup;
						while (Time.realtimeSinceStartup - realtimeSinceStartup < 0.2f) {}
					}

					GUICompoundControls.m_cKeyboard = TouchScreenKeyboard.Open(text2, aTextFieldData[i].keyboardType, false, false, true);
					Utilities.AssertMsg(GUICompoundControls.m_cKeyboard != null, "Fail to create keyboard!");

					if (!flag)
					{
						flag = true;
						aTextFieldData[i].isFocused = true;
						for (int j = 0; j < aTextFieldData.Length; j++)
						{
							if (i != j)
							{
								aTextFieldData[j].isFocused = false;
							}
						}
					}
				}

				if (TouchScreenKeyboard.visible && GUICompoundControls.m_cKeyboard != null && GUICompoundControls.m_cKeyboard.active && flag)
				{
					array[i] = GUICompoundControls.m_cKeyboard.text;
					if (array[i].Length > aTextFieldData[i].maxLength)
					{
						array[i] = array[i].Substring(0, aTextFieldData[i].maxLength);
						GUICompoundControls.m_cKeyboard.text = array[i];
					}
					if (aTextFieldData[i].titleCase)
					{
						string text3 = array[i];
						array[i] = GUICompoundControls.m_TextInfo.ToTitleCase(array[i]);
						if (text3 != array[i])
						{
							GUICompoundControls.m_cKeyboard.text = array[i];
						}
					}
					flag3 = true;
				}
#endif
			}
			if (flag3)
			{
				if (aTextFieldData[i].isPassword)
				{
					if (array[i].Length > 0)
					{
						string text4 = GUIUtil.MaskPassword(ref aTextFieldData[i], array[i]);
						if (text4.Length > 0)
						{
							string stringToDisplay = GUIUtil.GetStringToDisplay(text4, guistyle2, rect.width, true);
							GUI.Label(rect, stringToDisplay, guistyle2);
							aTextFieldData[i].maskedPassword = text4.Remove(text4.Length - 1, 1) + '*';
						}
					}
					else
					{
						aTextFieldData[i].maskedPassword = string.Empty;
						string stringToDisplay2 = GUIUtil.GetStringToDisplay(aTextFieldData[i].maskedPassword, guistyle2, rect.width, true);
						GUI.Label(rect, stringToDisplay2, guistyle2);
					}
				}
				else
				{
					string stringToDisplay3 = GUIUtil.GetStringToDisplay(array[i], guistyle2, rect.width, true);
					GUI.Label(rect, stringToDisplay3, guistyle2);
				}
				aTextFieldData[i].editedText = array[i];
			}
			else if (text2.Length == 0 && text.Length > 0)
			{
				GUI.Label(rect, text, guistyle);
			}
			else if (aTextFieldData[i].isPassword)
			{
				GUI.Label(rect, aTextFieldData[i].maskedPassword, guistyle2);
				aTextFieldData[i].timeOfNukedPassword = 0f;
			}
			else
			{
				string stringToDisplay4 = GUIUtil.GetStringToDisplay(text2, guistyle2, rect.width, false);
				GUI.Label(rect, stringToDisplay4, guistyle2);
			}
		}
#if UNITY_ANDROID || UNITY_IOS
		if (TouchScreenKeyboard.visible)
		{
			bool flag5 = GUI.Button(GUIUtil.FullScreenRect(), string.Empty);
			if (GUICompoundControls.m_cKeyboard != null && flag5)
			{
				GUICompoundControls.m_cKeyboard.active = false;
				GUICompoundControls.m_cKeyboard = null;
			}
		}
#endif
		return array;
	}

	public static int HorizontalRadioButtons(Vector3 aReferencePos, GUIDefines.RadioButtonData aRadioButtonData)
	{
		int num = -1;
		bool[] array = new bool[aRadioButtonData.isOn.Length];
		GUIStyle guiStyle = GUIUtil.GetGuiStyle(aRadioButtonData.style);
		Vector2 space = GUIUtil.GetSpace(aRadioButtonData.space);
		Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aRadioButtonData.area);
		GUILayout.BeginArea(rect);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		for (int i = 0; i < aRadioButtonData.isOn.Length; i++)
		{
			bool flag;
			if (guiStyle == null)
			{
				flag = GUILayout.Toggle(aRadioButtonData.isOn[i], string.Empty, new GUILayoutOption[0]);
			}
			else
			{
				flag = GUILayout.Toggle(aRadioButtonData.isOn[i], string.Empty, guiStyle, new GUILayoutOption[0]);
			}
			array[i] = flag;
			GUILayout.Space(space.x);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		for (int j = 0; j < aRadioButtonData.isOn.Length; j++)
		{
			if (aRadioButtonData.isOn[j] != array[j] && array[j])
			{
				num = j;
				aRadioButtonData.isOn[j] = true;
			}
		}
		if (num != -1)
		{
			for (int k = 0; k < aRadioButtonData.isOn.Length; k++)
			{
				if (k != num)
				{
					aRadioButtonData.isOn[k] = false;
				}
			}
		}
		return num;
	}

	public static void HorizontalUnClickableRadioButtons(Vector3 aReferencePos, GUIDefines.UnClickableRadioButtonData aUnClickableRadioButtonData, int aCurrentOn)
	{
		Vector2 space = GUIUtil.GetSpace(aUnClickableRadioButtonData.space);
		Vector2 space2 = GUIUtil.GetSpace(aUnClickableRadioButtonData.onPadding);
		Vector2 space3 = GUIUtil.GetSpace(aUnClickableRadioButtonData.offPadding);
		Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aUnClickableRadioButtonData.area);
		GUILayout.BeginArea(rect);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		for (int i = 0; i < aUnClickableRadioButtonData.count; i++)
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			if (i == aCurrentOn)
			{
				GUILayout.Space(space2.y);
				GUILayout.Label(aUnClickableRadioButtonData.on.image, new GUILayoutOption[0]);
			}
			else
			{
				GUILayout.Space(space3.y);
				GUILayout.Label(aUnClickableRadioButtonData.off.image, new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			GUILayout.Space(space.x);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	public static void Window(Vector3 aReferencePos, GUIDefines.WindowData aWindowData, GUI.WindowFunction aFunction)
	{
		Rect rect = GUIUtil.ConvertToRelativePos(aReferencePos, aWindowData.pos);
		GUIStyle guiStyle = GUIUtil.GetGuiStyle(aWindowData.style);
		if (guiStyle == null)
		{
			GUI.Window(aWindowData.id, rect, aFunction, string.Empty);
		}
		else
		{
			GUI.Window(aWindowData.id, rect, aFunction, string.Empty, guiStyle);
		}
	}

	public static void Windows(Vector3 aReferencePos, GUIDefines.WindowData[] aWindowData, GUI.WindowFunction aFunction)
	{
		for (int i = 0; i < aWindowData.Length; i++)
		{
			GUICompoundControls.Window(aReferencePos, aWindowData[i], aFunction);
		}
	}
}
