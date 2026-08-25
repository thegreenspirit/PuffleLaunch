using System;
using UnityEngine;

public abstract class PackableStub : MonoBehaviour
{
	public abstract void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid);

	public abstract Texture2D[] SourceTextures { get; }

	public abstract CSpriteFrame[] SpriteFrames { get; }

	public abstract Material GetPackedMaterial(out string errString);

	public abstract CSpriteFrame DefaultFrame { get; }

	public abstract void SetUVs(Rect uvs);

	public abstract bool DoNotTrimImages { get; set; }
}
