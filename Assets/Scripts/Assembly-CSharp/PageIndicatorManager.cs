using System;
using UnityEngine;

public class PageIndicatorManager : MonoBehaviour
{
	private void Start()
	{
		if (GameObject.Find("ScrollList(Clone)") != null)
		{
			this.m_scrollList = GameObject.Find("ScrollList(Clone)").GetComponent<UIScrollList>();
		}
		int num = Mathf.CeilToInt(2.9166667f);
		float num2 = (float)(num - 1) * -0.5f;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/PageIndicator", typeof(GameObject))) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = new Vector3(num2 + (float)i * 1f, 0f, 0f);
		}
		this.m_pageIndicators = base.gameObject.GetComponentsInChildren<BHUITexture>();
	}

	private void Update()
	{
		if (this.m_scrollList == null || this.m_scrollList.Count == 0)
		{
			return;
		}
		if (!this.isInitialize)
		{
			this.isInitialize = true;
			int num = Mathf.Clamp(LevelSelect.Instance.mPrevItemSelected, 0, this.m_scrollList.Count - 1);
			this.m_scrollList.ScrollToItem(num, 0.001f);
			this.m_currentPage = Mathf.FloorToInt(this.m_scrollList.ScrollPosition * (float)this.m_scrollList.Count);
			this.UpdatePageIndicator();
		}
		this.m_currentPage = Mathf.Clamp(Mathf.FloorToInt(this.m_scrollList.ScrollPosition * (float)this.m_scrollList.Count), 0, this.m_scrollList.Count - 1);
		if (this.m_currentPage != this.m_prevPage)
		{
			this.m_prevPage = this.m_currentPage;
			this.UpdatePageIndicator();
		}
	}

	private void UpdatePageIndicator()
	{
		string text = string.Empty;
		for (int i = 0; i < this.m_pageIndicators.Length; i++)
		{
			if (i == this.m_currentPage)
			{
				text = "PageDotActive";
			}
			else
			{
				text = "PageDot";
			}
			this.ChangeMaterial(this.m_pageIndicators[i].gameObject, text);
		}
	}

	private void ChangeMaterial(GameObject aGameObject, string aMaterialName)
	{
		aGameObject.GetComponent<MeshRenderer>().material = Resources.Load("EZGUI/LevelSelect/" + aMaterialName, typeof(Material)) as Material;
		ResourceLoader.Instance.SetMaterialTexture(aGameObject, "EZGUI/LevelSelect/", false, out this.m_AssetSizeCategoryId, out this.m_AssetLanguage);
	}

	protected SizeCategory.CategoryId m_AssetSizeCategoryId = SizeCategory.CategoryId.eUnknown;

	protected LocalizationManager.Language m_AssetLanguage;

	private UIScrollList m_scrollList;

	private BHUITexture[] m_pageIndicators;

	private int m_currentPage;

	private int m_prevPage;

	private bool isInitialize;
}
