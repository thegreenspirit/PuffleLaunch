using System;
using UnityEngine;

public class Dynamic3DBackground : MonoBehaviour
{
	private void Awake()
	{
		this.LoadDeviceDependentTexture(true);
	}

	private void LoadDeviceDependentTexture(bool aShouldScale)
	{
		if (this.mo_meshRenderer != null)
		{
			if (this.msz_name != string.Empty)
			{
				string empty = string.Empty;
				if (LocalizationManager.IsFrench)
				{
					empty = this.msz_localisationSuffix_french;
				}
				else if (LocalizationManager.IsPortuguese)
				{
					empty = this.msz_localisationSuffix_portuguese;
				}
				else if (LocalizationManager.IsSpanish)
				{
					empty = this.msz_localisationSuffix_spanish;
				}
				else if (LocalizationManager.IsEnglish)
				{
					empty = this.msz_localisationSuffix_english;
				}
				else if (LocalizationManager.IsGerman)
				{
					empty = this.msz_localisationSuffix_german;
				}
				else if (LocalizationManager.IsJapanese)
				{
					empty = this.msz_localisationSuffix_japanese;
				}
				if (this.mb_forceToEnglish)
				{
					empty = this.msz_localisationSuffix_english;
				}
				string text = ((ResolutionManager.Instance.AssetResolution != ResolutionManager.eAssetResolution.eLowres) ? string.Empty : "_lowres");
				string text2 = string.Format("{0}{1}{2}{3}{4}", new object[]
				{
					this.msz_path,
					(!this.mb_useIpadPath || ResolutionManager.Instance.AssetResolution != ResolutionManager.eAssetResolution.eIPad) ? string.Empty : "IPad/",
					this.msz_name,
					empty,
					text
				});
				this.mo_meshRenderer.material.mainTexture = GUIUtil.LoadTexture2D(text2);
				if (this.mo_meshRenderer.material.mainTexture == null)
				{
					Debug.Log(string.Format("Did not find {0}. Defaulting to {1}\n", text2, this.msz_path + this.msz_name + empty + text));
					this.mo_meshRenderer.material.mainTexture = GUIUtil.LoadTexture2D(this.msz_path + this.msz_name + empty + text);
				}
			}
			Utilities.AssertMsg(this.mo_meshRenderer.material.mainTexture != null, "Dynamic3DBackground not loaded!");
		}
		if (aShouldScale)
		{
			if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eIPad && this.mb_applyIpadScaling)
			{
				Vector3 localScale = base.gameObject.transform.localScale;
				localScale.x *= 0.8888889f;
				base.transform.localScale = localScale;
			}
			if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eIPad && this.mb_applyIpadPositionScaling)
			{
				Vector3 localPosition = base.gameObject.transform.localPosition;
				localPosition.x *= 0.8888889f;
				base.transform.localPosition = localPosition;
			}
			if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eIPad)
			{
				Vector3 vector = base.gameObject.transform.localPosition;
				vector += this.mv3_iPadPositionOffset;
				base.transform.localPosition = vector;
			}
			if (ResolutionManager.Instance.AssetResolution == ResolutionManager.eAssetResolution.eIPad)
			{
				Vector3 localScale2 = base.gameObject.transform.localScale;
				localScale2.x *= this.mv3_iPadScaleMultiplier.x;
				localScale2.y *= this.mv3_iPadScaleMultiplier.y;
				localScale2.z *= this.mv3_iPadScaleMultiplier.z;
				base.transform.localScale = localScale2;
			}
			if (LocalizationManager.IsFrench)
			{
				Vector3 localScale3 = base.transform.localScale;
				localScale3.Scale(this.scaleFactorFrench);
				base.transform.localScale = localScale3;
			}
			else if (LocalizationManager.IsPortuguese)
			{
				Vector3 localScale4 = base.transform.localScale;
				localScale4.Scale(this.scaleFactorPortuguese);
				base.transform.localScale = localScale4;
			}
			else if (LocalizationManager.IsSpanish)
			{
				Vector3 localScale5 = base.transform.localScale;
				localScale5.Scale(this.scaleFactorSpanish);
				base.transform.localScale = localScale5;
			}
			else if (LocalizationManager.IsEnglish)
			{
				Vector3 localScale6 = base.transform.localScale;
				localScale6.Scale(this.scaleFactorEnglish);
				base.transform.localScale = localScale6;
			}
			else if (LocalizationManager.IsGerman)
			{
				Vector3 localScale7 = base.transform.localScale;
				localScale7.Scale(this.scaleFactorGerman);
				base.transform.localScale = localScale7;
			}
			else if (LocalizationManager.IsJapanese)
			{
				Vector3 localScale8 = base.transform.localScale;
				localScale8.Scale(this.scaleFactorJapanese);
				base.transform.localScale = localScale8;
			}
		}
	}

	public void LoadNewTexture(string aNewPath, string aNewName)
	{
		this.msz_path = aNewPath;
		this.msz_name = aNewName;
		this.LoadDeviceDependentTexture(false);
	}

	public string msz_path;

	public string msz_name;

	public MeshRenderer mo_meshRenderer;

	public bool mb_applyIpadScaling;

	public bool mb_applyIpadPositionScaling;

	public bool mb_useIpadPath = true;

	public Vector3 mv3_iPadPositionOffset = Vector3.zero;

	public Vector3 mv3_iPadScaleMultiplier = new Vector3(1f, 1f, 1f);

	public bool mb_forceToEnglish;

	public string msz_localisationSuffix_english = string.Empty;

	public string msz_localisationSuffix_french = string.Empty;

	public string msz_localisationSuffix_portuguese = string.Empty;

	public string msz_localisationSuffix_spanish = string.Empty;

	public string msz_localisationSuffix_german = string.Empty;

	public string msz_localisationSuffix_japanese = string.Empty;

	public Vector3 scaleFactorEnglish = Vector3.one;

	public Vector3 scaleFactorFrench = Vector3.one;

	public Vector3 scaleFactorPortuguese = Vector3.one;

	public Vector3 scaleFactorSpanish = Vector3.one;

	public Vector3 scaleFactorGerman = Vector3.one;

	public Vector3 scaleFactorJapanese = Vector3.one;
}
