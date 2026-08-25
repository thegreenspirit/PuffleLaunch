using System;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
	private void Start()
	{
		this.m_ButtonList = global::UnityEngine.Object.FindObjectsOfType(typeof(BHUIButton)) as BHUIButton[];
	}

	private void Update()
	{
		if (this.m_ChangeState)
		{
			this.FrameCount++;
			if (this.FrameCount >= this.FrameDelay)
			{
				this.FrameCount = 0;
				this.m_ChangeState = false;
				this.SetButtonsEnable(this.m_NextState);
			}
		}
	}

	public void RequestChangeButtonsState(bool aEnable)
	{
		this.m_ChangeState = true;
		this.m_NextState = aEnable;
	}

	private void SetButtonsEnable(bool aEnable)
	{
		foreach (BHUIButton bhuibutton in this.m_ButtonList)
		{
			bhuibutton.Enable(aEnable);
		}
	}

	private BHUIButton[] m_ButtonList;

	private bool m_ChangeState;

	private bool m_NextState = true;

	private int FrameCount;

	private int FrameDelay = 2;
}
