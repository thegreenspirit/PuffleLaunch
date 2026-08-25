using System;
using UnityEngine;

public class LoadedBundleData
{
	public LoadedBundleData(string aName, AssetBundle aBundle)
	{
		this.name = aName;
		this.bundle = aBundle;
		this.usercount = 0;
	}

	public bool valid
	{
		get
		{
			return this.bundle != null;
		}
	}

	public void Acquire()
	{
		this.usercount++;
	}

	public void Release()
	{
		this.usercount--;
		if (this.usercount == 0)
		{
			this.bundle.Unload(true);
			this.bundle = null;
		}
	}

	public void Destroy()
	{
		if (this.valid)
		{
			this.usercount = 0;
			this.bundle.Unload(true);
		}
	}

	public string name;

	public AssetBundle bundle;

	private int usercount;
}
