using System;
using UnityEngine;

public interface ISpriteAggregator
{
	void Aggregate(PathFromGUIDDelegate guid2Path, LoadAssetDelegate load, GUIDFromPathDelegate path2Guid);

	Texture2D[] SourceTextures { get; }

	CSpriteFrame[] SpriteFrames { get; }

	Material GetPackedMaterial(out string errString);

	CSpriteFrame DefaultFrame { get; }

	void SetUVs(Rect uvs);

	GameObject gameObject { get; }

	bool DoNotTrimImages { get; }
}
