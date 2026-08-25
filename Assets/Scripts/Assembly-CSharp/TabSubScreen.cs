using System;
using UnityEngine;

public class TabSubScreen : BaseMonoScreen
{
	private void Awake()
	{
		this.UpdateTab((int)GameManager.Instance.CurrentWorld);
	}

	public void UpdateTab(int aSelectedTab)
	{
		this.iPadTextureEN = this.worldBackgroundsIPad[aSelectedTab];
		this.normalTextureEN = this.worldBackgroundsNormal[aSelectedTab];
		this.lowresTextureEN = this.worldBackgroundsLowres[aSelectedTab];
		this.iPadTextureES = this.worldBackgroundsIPad[aSelectedTab];
		this.normalTextureES = this.worldBackgroundsNormal[aSelectedTab];
		this.lowresTextureES = this.worldBackgroundsLowres[aSelectedTab];
		this.iPadTextureFR = this.worldBackgroundsIPad[aSelectedTab];
		this.normalTextureFR = this.worldBackgroundsNormal[aSelectedTab];
		this.lowresTextureFR = this.worldBackgroundsLowres[aSelectedTab];
		this.iPadTexturePT = this.worldBackgroundsIPad[aSelectedTab];
		this.normalTexturePT = this.worldBackgroundsNormal[aSelectedTab];
		this.lowresTexturePT = this.worldBackgroundsLowres[aSelectedTab];
		this.iPadTextureDE = this.worldBackgroundsIPad[aSelectedTab];
		this.normalTextureDE = this.worldBackgroundsNormal[aSelectedTab];
		this.lowresTextureDE = this.worldBackgroundsLowres[aSelectedTab];
		this.iPadTextureJA = this.worldBackgroundsIPad[aSelectedTab];
		this.normalTextureJA = this.worldBackgroundsNormal[aSelectedTab];
		this.lowresTextureJA = this.worldBackgroundsLowres[aSelectedTab];
		this.Init(base.gameObject);
	}

	protected override void CreateMainScreenLayouts()
	{
	}

	protected override void OnMainScreenButtonSelect()
	{
	}

	protected override void OnBack()
	{
		base.MainScreen.StopGUI();
		AssetLoader.Instance.ScrollList.SetActive(false);
		GameFlowManager.Instance.LoadScene("!Loader_MainMenu", false);
	}

	public MeshRenderer tabBackground;

	public string[] worldBackgroundsIPad;

	public string[] worldBackgroundsNormal;

	public string[] worldBackgroundsLowres;
}
