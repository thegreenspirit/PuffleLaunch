using System;
using UnityEngine;

public class ResourceLoader
{
	private ResourceLoader()
	{
	}

	public static ResourceLoader Instance
	{
		get
		{
			if (ResourceLoader.m_cInstance == null)
			{
				ResourceLoader.m_cInstance = new ResourceLoader();
			}
			return ResourceLoader.m_cInstance;
		}
		set
		{
			if (Utilities.AssertMsg(value == null, "Cannot assign anything else but NULL to singleton instance!"))
			{
				ResourceLoader.m_cInstance = null;
			}
		}
	}

	public void SetMaterialTexture(GameObject aGameObj, string aTexturePath, bool aLocalized, out SizeCategory.CategoryId loadedAssetSizeCategoryId, out LocalizationManager.Language loadedAssetLanguage)
	{
		loadedAssetSizeCategoryId = SizeCategory.CategoryId.eUnknown;
		loadedAssetLanguage = LocalizationManager.Language.eEnglish;
		if (SizeCategory.Instance == null)
		{
			return;
		}
		if (aTexturePath == null || aTexturePath.Length <= 0)
		{
			Utilities.AssertMsg(false, "Invalid texturePath: " + aTexturePath);
			return;
		}
		MeshRenderer renderer = this.GetRenderer(aGameObj);
		if (renderer == null || renderer.sharedMaterial == null)
		{
			return;
		}
		SizeCategory.CategoryId categoryId = SizeCategory.Instance.CurCategoryId;
		Texture2D texture2D = null;
		while (texture2D == null)
		{
			string text = aTexturePath + SizeCategory.Instance.GetCategory(categoryId) + "/" + renderer.sharedMaterial.name;
			texture2D = Resources.Load(text, typeof(Texture2D)) as Texture2D;
			if (texture2D == null)
			{
				SizeCategory.CategoryId alternateCategoryId = SizeCategory.Instance.GetAlternateCategoryId(categoryId);
				if (alternateCategoryId == categoryId)
				{
					Utilities.AssertMsg(false, "No valid texture is found! Fail to set material texture of game object: " + aGameObj);
					return;
				}
				categoryId = alternateCategoryId;
			}
			else
			{
				if (aLocalized)
				{
					string languageCode = LocalizationManager.GetLanguageCode();
					string text2 = text + languageCode;
					Texture2D texture2D2 = Resources.Load(text2, typeof(Texture2D)) as Texture2D;
					if (texture2D2 != null)
					{
						texture2D = texture2D2;
					}
				}
				loadedAssetSizeCategoryId = categoryId;
				renderer.sharedMaterial.mainTexture = texture2D;
			}
		}
	}

	public void ResetMaterialTexture(GameObject aGameObj)
	{
		MeshRenderer renderer = this.GetRenderer(aGameObj);
		if (renderer == null || renderer.sharedMaterial == null)
		{
			return;
		}
		renderer.sharedMaterial.mainTexture = null;
	}

	public MeshRenderer GetRenderer(GameObject aGameObj)
	{
		if (aGameObj == null)
		{
			Utilities.AssertMsg(false, "Fail to get renderer due to invalid given game object!");
			return null;
		}
		MeshRenderer component = aGameObj.GetComponent<MeshRenderer>();
		Utilities.AssertMsg(component != null, "No MeshRenderer found in " + aGameObj);
		Utilities.AssertMsg(component.sharedMaterial != null, "No valid shared material in renderer of " + aGameObj);
		return component;
	}

	public static string GetLocalizedSuffixByLanguage(LocalizationManager.Language lang)
	{
		if (lang < LocalizationManager.Language.eEnglish || lang >= (LocalizationManager.Language)ResourceLoader.kLocalizedSuffixs.Length)
		{
			Utilities.AssertMsg(false, "Invalid language: " + lang);
			return string.Empty;
		}
		return ResourceLoader.kLocalizedSuffixs[(int)lang];
	}

	public static string[] kLocalizedSuffixs = new string[]
	{
		string.Empty,
		"_fr",
		"_es",
		"_pt",
		"_de",
		"_ja"
	};

	private static ResourceLoader m_cInstance = null;
}
