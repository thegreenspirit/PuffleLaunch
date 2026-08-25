using System;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
	public static AssetLoader Instance
	{
		get
		{
			return AssetLoader.m_cInstance;
		}
	}

	public Transform PuffleTemplate
	{
		get
		{
			return this.m_PuffleTemplate;
		}
		set
		{
			this.m_PuffleTemplate = value;
		}
	}

	public Transform GiantPuffleOTemplate
	{
		get
		{
			return this.m_GiantPuffleOTemplate;
		}
		set
		{
			this.m_GiantPuffleOTemplate = value;
		}
	}

	public GameObject EmptyScrollList
	{
		get
		{
			return this.m_EmptyScrollList;
		}
		set
		{
			this.m_EmptyScrollList = value;
		}
	}

	public GameObject ScrollList
	{
		get
		{
			return this.m_ScrollList;
		}
		set
		{
			this.m_ScrollList = value;
		}
	}

	private void Start()
	{
		AssetLoader.m_cInstance = this;
		global::UnityEngine.Object.DontDestroyOnLoad(this);
		this.LoadAssets();
	}

	private void LoadAssets()
	{
		this.LoadScrollLists();
		this.GiantPuffleOTemplate = (Transform)global::UnityEngine.Object.Instantiate(this.gpo, new Vector3(-100f, 0f, 0f), default(Quaternion));
		this.GiantPuffleOTemplate.GetComponent<Renderer>().enabled = false;
		this.GiantPuffleOTemplate.gameObject.active = false;
		global::UnityEngine.Object.DontDestroyOnLoad(this.GiantPuffleOTemplate);
		this.PuffleTemplate = (Transform)global::UnityEngine.Object.Instantiate(this.puffle, new Vector3(-100f, 0f, 0f), default(Quaternion));
		this.PuffleTemplate.GetComponent<Renderer>().enabled = false;
		this.PuffleTemplate.gameObject.active = false;
		this.PuffleTemplate.GetComponent<Rigidbody>().Sleep();
		global::UnityEngine.Object.DontDestroyOnLoad(this.PuffleTemplate);
	}

	private void LoadScrollLists()
	{
		Vector3 vector = new Vector3(0f, -2.2f, 0f);
		this.EmptyScrollList = global::UnityEngine.Object.Instantiate(this.EmptyList, vector, default(Quaternion)) as GameObject;
		this.EmptyScrollList.SetActive(false);
		global::UnityEngine.Object.DontDestroyOnLoad(this.EmptyScrollList);
		this.ScrollList = global::UnityEngine.Object.Instantiate(this.bonusScrollList, vector, default(Quaternion)) as GameObject;
		this.ScrollList.SetActive(false);
		global::UnityEngine.Object.DontDestroyOnLoad(this.ScrollList);
		this.EmptyScrollList.SetActive(true);
	}

	public Transform gpo;

	public Transform puffle;

	public GameObject EmptyList;

	public GameObject bonusScrollList;

	private static AssetLoader m_cInstance;

	private Transform m_PuffleTemplate;

	private Transform m_GiantPuffleOTemplate;

	private GameObject m_ScrollList;

	private GameObject m_EmptyScrollList;
}
