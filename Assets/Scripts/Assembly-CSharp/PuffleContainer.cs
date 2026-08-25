using System;
using UnityEngine;

public class PuffleContainer : MonoBehaviour
{
	private Puffle mContainedPuffle;

	public void Start()
	{
		this.mContainedPuffle = null;
	}

	public void Update() {}

	public void OnPuffleEnter(Puffle aOther)
	{
		this.mContainedPuffle = aOther;
	}

	public void ReleasePuffle()
	{
		this.mContainedPuffle = null;
	}

	public Puffle GetContainedPuffle()
	{
		return this.mContainedPuffle;
	}

	public bool IsPuffleInside()
	{
		return this.mContainedPuffle != null;
	}
}
