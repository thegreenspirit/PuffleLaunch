using System;
using UnityEngine;

public class AboutCPMoviePlayFailed : MonoBehaviour
{
	private bool m_IsCompleted;
	public bool IsCompleted
	{
		get { return this.m_IsCompleted; }
	}

	private void Update()
	{
		this.m_IsCompleted = Input.touchCount > 0;
	}
}
