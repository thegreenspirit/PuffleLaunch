using System;
using UnityEngine;

public class ScrollListManager : MonoBehaviour
{
	public static ScrollListManager Instance
	{
		get
		{
			return ScrollListManager.m_instance;
		}
	}

	public int ListID
	{
		get
		{
			return this.mCurrentListID;
		}
	}

	public int ListLevelCount
	{
		get
		{
			return this.mCurrentListLevelCount;
		}
	}

	private void Awake()
	{
		ScrollListManager.m_instance = this;
		this.mUIScrollList = base.GetComponent<UIScrollList>();
		int num = Mathf.CeilToInt(2.9166667f);
		int num2 = 36;
		for (int i = 0; i < num; i++)
		{
			this.mCurrentListID = i;
			if (num2 >= 12)
			{
				this.mCurrentListLevelCount = 12;
				num2 -= 12;
			}
			else
			{
				this.mCurrentListLevelCount = num2;
			}
			GameObject gameObject = global::UnityEngine.Object.Instantiate(this.ListItem) as GameObject;
			gameObject.transform.parent = base.transform;
			this.mUIScrollList.sceneItems[i] = gameObject;
		}
	}

	public GameObject ListItem;

	private static ScrollListManager m_instance;

	private UIScrollList mUIScrollList;

	private int mCurrentListLevelCount;

	private int mCurrentListID;
}
