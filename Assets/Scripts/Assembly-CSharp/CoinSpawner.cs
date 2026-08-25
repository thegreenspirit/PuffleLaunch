using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : MonoBehaviour
{
	private void Start()
	{
		this.mLastFrameTimestamp = Time.realtimeSinceStartup;
	}

	private void FixedUpdate()
	{
		this.mDeltaTime = Time.realtimeSinceStartup - this.mLastFrameTimestamp;
		this.mLastFrameTimestamp = Time.realtimeSinceStartup;
		this.mSpawnTimer += this.mDeltaTime;
		if (this.mSpawnTimer > this.mMinTimeToSpawn && Random.Range(0f, 1f) < this.mChanceToSpawn)
		{
			this.SpawnRing();
		}
		if (this.mSpawnTimer > this.mMaxTimeToSpawn)
		{
			this.SpawnRing();
		}
	}

	public void SpawnRing()
	{
		GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/SpinningCoin", typeof(GameObject))) as GameObject;
		gameObject.transform.parent = this.spawnPoint;
		if (ResolutionManager.Instance.LayoutSize == ResolutionManager.eLayoutSize.eLowres)
		{
			gameObject.transform.localScale *= 0.5f;
		}
		this.mSpawnTimer = 0f;
	}

	public Transform spawnPoint;

	private float mSpawnTimer;

	private float mMinTimeToSpawn = 0.25f;

	private float mMaxTimeToSpawn = 1f;

	private float mChanceToSpawn = 0.25f;

	private float mDeltaTime;

	private float mLastFrameTimestamp;
}
