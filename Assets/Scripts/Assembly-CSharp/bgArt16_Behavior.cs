using System;
using UnityEngine;

public class bgArt16_Behavior : MonoBehaviour
{
	private void Start()
	{
		this.mLastControlType = Puffle.smControlType;
		this.UpdateString();
		float num = 640f / (float)Screen.height;
		base.transform.Find("ForegroundImageTilt").transform.localPosition *= num;
		base.transform.Find("ForegroundImageTouch").transform.localPosition *= num;
		base.transform.Find("InstructionText").transform.localScale *= num;
	}

	private void Update()
	{
		if (Puffle.smControlType != this.mLastControlType)
		{
			this.mLastControlType = Puffle.smControlType;
			this.UpdateString();
		}
	}

	private void UpdateString()
	{
		if (Puffle.smControlType == Puffle.ControlType.eTilting)
		{
			base.transform.Find("InstructionText").GetComponent<TextMesh>().text = LocalizationManager.Instance.GetString("TXT_Instructions3");
			base.transform.Find("ForegroundImageTilt").GetComponent<MeshRenderer>().enabled = true;
			base.transform.Find("ForegroundImageTouch").GetComponent<MeshRenderer>().enabled = false;
		}
		else
		{
			base.transform.Find("InstructionText").GetComponent<TextMesh>().text = LocalizationManager.Instance.GetString("TXT_Instructions3");
			base.transform.Find("ForegroundImageTouch").GetComponent<MeshRenderer>().enabled = true;
			base.transform.Find("ForegroundImageTilt").GetComponent<MeshRenderer>().enabled = false;
		}
	}

	private const float kReferenceScreenHeight = 640f;

	private Puffle.ControlType mLastControlType;
}
