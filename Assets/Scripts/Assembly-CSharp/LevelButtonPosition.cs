using System;
using UnityEngine;

public class LevelButtonPosition : MonoBehaviour
{
	public GameManager.World world;
	private GameObject m_LevelButton;

	private void Awake()
	{
		this.m_LevelButton = Resources.Load("Prefabs/LevelButton") as GameObject;
		for (int i = 0; i < ScrollListManager.Instance.ListLevelCount; i++)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate(this.m_LevelButton) as GameObject;
			gameObject.transform.parent = base.transform;
		}
		float num = 4.8f;
		float num2 = 1.5f;
		float num3 = (float)Screen.width / (float)Screen.height;
		float num4 = num * (num3 / num2);
		float num5 = num4 * 2.5f;
		int num6 = 0;
		Vector3 zero = Vector3.zero;
		float num7 = (float)Screen.width / 5f;
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
		Vector3 vector2 = Camera.main.ScreenToWorldPoint(new Vector3(num7, 0f, 0f));
		float magnitude = (vector - vector2).magnitude;
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			zero.x = -num5 + num4 * (float)(num6 % 6);
			if (num6 < 6)
			{
				zero.y = num4 * 0.5f;
			}
			else
			{
				zero.y = -num4 * 0.5f;
			}
			transform.localPosition = zero;
			transform.GetComponent<BHUIButton>().width = magnitude;
			transform.GetComponent<BHUIButton>().height = magnitude;
			num6++;
			transform.GetComponent<LevelButtonController>().buttonID = num6 + 12 * ScrollListManager.Instance.ListID;
		}
	}
}
