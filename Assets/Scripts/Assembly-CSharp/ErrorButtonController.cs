using System;
using UnityEngine;

public class ErrorButtonController : MonoBehaviour
{
	public bool ErrorHappened
	{
		get
		{
			return this.mErrorHappened;
		}
		set
		{
			this.mErrorHappened = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.ErrorHappened)
		{
			this.buttonToDisable.GetComponent<Button3DPressStateController>().Enabled = false;
		}
		else
		{
			this.buttonToDisable.GetComponent<Button3DPressStateController>().Enabled = true;
		}
	}

	public Transform errorButton;

	public Transform buttonToDisable;

	private bool mErrorHappened;
}
