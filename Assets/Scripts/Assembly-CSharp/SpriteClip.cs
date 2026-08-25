using System;
using UnityEngine;

[Serializable]
public class SpriteClip
{
	public string[] stringTiles;
	public string[] urls;
	public Vector2 stride;
	public Vector2 offset;
	public int cols;
	public int rows;
	public int total;
	public bool useHighRes;
	private int m_CurrentFrame;
	private bool[] m_Ignore;
	private Texture2D[] tiles;
	public static bool FORCE_SCALE;

	public bool loaded
	{
		get
		{
			if ((this.tiles == null || this.tiles.Length == 0) && this.stringTiles.Length > 0)
			{
				this.tiles = new Texture2D[this.stringTiles.Length];
				for (int i = 0; i < this.stringTiles.Length; i++)
				{
					if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres && !this.useHighRes)
					{
						this.tiles[i] = Resources.Load(string.Format("{0}_lowres", this.stringTiles[i]), typeof(Texture2D)) as Texture2D;
						Utilities.AssertMsg(this.tiles[i] != null, string.Format("Low-res sprite sheet not found: {0}", this.stringTiles[i]));
					}
					else
					{
						this.tiles[i] = Resources.Load(this.stringTiles[i], typeof(Texture2D)) as Texture2D;
						Utilities.AssertMsg(this.tiles[i] != null, string.Format("Sprite sheet not found: {0}", this.stringTiles[i]));
					}
				}
			}
			for (int j = 0; j < this.stringTiles.Length; j++)
			{
				Texture2D texture2D = this.tiles[j];
				if (this.m_Ignore == null || j >= this.m_Ignore.Length || !this.m_Ignore[j])
				{
					if (texture2D == null)
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	public void Reset()
	{
		this.m_CurrentFrame = 0;
	}

	public void SetIgnore(bool[] aIgnore)
	{
		this.m_Ignore = aIgnore;
	}

	public void Apply(SpriteManager aManager, Material[] aMaterials)
	{
		int i = 0;
		if (this.tiles == null)
		{
			this.tiles = new Texture2D[this.stringTiles.Length];
		}
		for (int j = 0; j < this.stringTiles.Length; j++)
		{
			if (this.tiles[j] == null)
			{
				if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres && !this.useHighRes)
				{
					this.tiles[j] = Resources.Load(string.Format("{0}_lowres", this.stringTiles[j]), typeof(Texture2D)) as Texture2D;
					Utilities.AssertMsg(this.tiles[j] != null, string.Format("Low-res sprite sheet not found: {0}", this.stringTiles[j]));
				}
				else
				{
					this.tiles[j] = Resources.Load(this.stringTiles[j], typeof(Texture2D)) as Texture2D;
					Utilities.AssertMsg(this.tiles[j] != null, string.Format("Sprite sheet not found: {0}", this.stringTiles[j]));
				}
			}
			if (!(this.tiles[j] == null) && (this.m_Ignore == null || j >= this.m_Ignore.Length || !this.m_Ignore[j]))
			{
				if (aMaterials[i] != null)
				{
					Material material = aMaterials[i];
					if (SpriteClip.FORCE_SCALE || material.mainTexture != this.tiles[j])
					{
						material.mainTexture = this.tiles[j];
						material.mainTextureScale = new Vector2((this.stride.x - 1f) / (float)this.tiles[j].width, (this.stride.y - 1f) / (float)this.tiles[j].height);
						if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres && !this.useHighRes)
						{
							material.mainTextureScale *= 0.5f;
						}
					}
					material.mainTextureOffset = new Vector2(this.stride.x / (float)this.tiles[j].width * (float)(this.m_CurrentFrame % this.cols), this.stride.y / (float)this.tiles[j].height * (float)(this.m_CurrentFrame / this.cols));
					if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eLowres && !this.useHighRes)
					{
						material.mainTextureOffset *= 0.5f;
					}
					i++;
				}
			}
		}
		while (i < aMaterials.Length)
		{
			Material material2 = aMaterials[i];
			material2.mainTexture = null;
			i++;
		}
		aManager.ClipApplied(this);
	}

	public int Update(int aDeltaFrame)
	{
		this.m_CurrentFrame += aDeltaFrame;
		if (this.m_CurrentFrame >= this.total)
		{
			return this.m_CurrentFrame - (this.total - 1);
		}
		return 0;
	}

	public void Unload()
	{
		this.stringTiles = null;
		this.tiles = null;
	}
}
