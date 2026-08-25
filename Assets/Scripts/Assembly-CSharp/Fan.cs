using System;
using UnityEngine;

public class Fan : MonoBehaviour
{
	public void Start()
	{
		this.mTransform = base.transform;
	}

	public void OnTriggerEnter(Collider aOther)
	{
		if (aOther.tag == "Player")
		{
			this.PushPlayer(aOther.GetComponent<Puffle>());
		}
	}

	public void OnTriggerStay(Collider aOther)
	{
		if (aOther.tag == "Player")
		{
			this.PushPlayer(aOther.GetComponent<Puffle>());
		}
	}

	private void PushPlayer(Puffle aPlayer)
	{
		if (aPlayer.State == Puffle.PuffleState.eFlying)
		{
			float num = this.mTransform.eulerAngles.z + 90f;
			Vector3 vector = new Vector3(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f), 0f);
			vector *= 1.8f * ScaleItem.Instance.LevelScale;
			Vector3 velocity = aPlayer.Velocity;
			velocity.x += vector.x;
			if (velocity.y < 0f)
			{
				velocity.y += vector.y - velocity.y / 3f;
			}
			else
			{
				velocity.y += vector.y;
			}
			aPlayer.Velocity = velocity;
			aPlayer.AngularVelocity += vector.y / ScaleItem.Instance.LevelScale;
		}
	}

	private Transform mTransform;
}
