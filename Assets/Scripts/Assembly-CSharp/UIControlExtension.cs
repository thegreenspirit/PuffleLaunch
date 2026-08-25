using System;
using UnityEngine;

public class UIControlExtension : MonoBehaviour
{
	public SizeCategory.CategoryId AssetSizeCategoryId
	{
		get
		{
			return this.m_AssetSizeCategoryId;
		}
	}

	public LocalizationManager.Language AssetLanguage
	{
		get
		{
			return this.m_AssetLanguage;
		}
	}

	public virtual void Awake()
	{
	}

	public virtual void OnDestroy()
	{
	}

	public virtual void SetMaterialTexture()
	{
		if (this.texturePath != null && this.texturePath.Length > 0)
		{
			ResourceLoader.Instance.SetMaterialTexture(base.gameObject, this.texturePath, false, out this.m_AssetSizeCategoryId, out this.m_AssetLanguage);
		}
	}

	public virtual void SetMaterialLocalizedTexture(bool aLocalized)
	{
		if (this.texturePath != null && this.texturePath.Length > 0)
		{
			ResourceLoader.Instance.SetMaterialTexture(base.gameObject, this.texturePath, aLocalized, out this.m_AssetSizeCategoryId, out this.m_AssetLanguage);
		}
	}

	public virtual string GetLocalizeText()
	{
		if (this.m_TextId != null && this.m_TextId.Length > 0)
		{
			if (GameFlowManager.Instance != null && LocalizationManager.Instance != null)
			{
				this.m_Text = LocalizationManager.Instance.GetString(this.m_TextId);
			}
			else
			{
				this.m_Text = this.m_TextId;
			}
		}
		return this.m_Text;
	}

	public string texturePath;

	public string m_TextId = string.Empty;

	public string m_Text = string.Empty;

	protected SizeCategory.CategoryId m_AssetSizeCategoryId = SizeCategory.CategoryId.eUnknown;

	protected LocalizationManager.Language m_AssetLanguage;
}
