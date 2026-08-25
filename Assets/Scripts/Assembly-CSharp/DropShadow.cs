using System;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
	public void Awake()
	{
		this.m_SourceSpriteText = base.GetComponent<SpriteText>();
		Utilities.AssertMsg(this.m_SourceSpriteText != null, "Drop Shadow might not work because no source sprite text is found!");
		this.m_SourceSpriteMeshRenderer = this.m_SourceSpriteText.gameObject.GetComponent<MeshRenderer>();
		Utilities.AssertMsg(this.m_SourceSpriteMeshRenderer != null, "Source sprite text doesn't have a mesh renderer!");
	}

	public void Update()
	{
		if (!this.m_IsShadowCreated)
		{
			return;
		}
		if (this.m_SourceSpriteMeshRenderer.enabled != this.m_CopySpriteMeshRenderer.enabled)
		{
			this.HideDropShadowText(!this.m_SourceSpriteMeshRenderer.enabled);
			return;
		}
		if (this.m_SourceSpriteText.Text != this.m_CopySpriteText.Text)
		{
			this.UpdateDropShadowText();
		}
	}

	public void CreateShadow()
	{
		if (this.m_IsShadowCreated || this.m_SourceSpriteText == null)
		{
			return;
		}
		Vector3 dropShadowOffset = this.GetDropShadowOffset();
		GameObject gameObject = global::UnityEngine.Object.Instantiate(Resources.Load("Prefabs/EZGUI/DropShadow", typeof(GameObject)), dropShadowOffset, base.transform.rotation) as GameObject;
		gameObject.transform.parent = base.transform;
		this.m_CopySpriteText = gameObject.GetComponent<SpriteText>();
		this.m_CopySpriteText.Copy(this.m_SourceSpriteText);
		this.m_CopySpriteText.SetFont(this.m_SourceSpriteText.font, base.GetComponent<Renderer>().material);
		this.m_CopySpriteText.SetCharacterSize(this.m_SourceSpriteText.characterSize);
		this.m_CopySpriteText.CharacterSpacing = this.m_SourceSpriteText.CharacterSpacing;
		this.m_CopySpriteText.maxWidth = this.m_SourceSpriteText.maxWidth;
		this.m_CopySpriteText.Text = this.RemoveColorTags(this.m_SourceSpriteText.Text);
		Color dropShadowColor = this.GetDropShadowColor(this.m_SourceSpriteText.color, this.m_DropColor);
		this.m_CopySpriteText.SetColor(dropShadowColor);
		this.m_CopySpriteMeshRenderer = gameObject.GetComponent<MeshRenderer>();
		Utilities.AssertMsg(this.m_CopySpriteMeshRenderer != null, "Copy sprite text doesn't have a mesh renderer!");
		this.m_CopySpriteMeshRenderer.enabled = this.m_SourceSpriteMeshRenderer.enabled;
		this.m_IsShadowCreated = true;
	}

	private Vector3 GetDropShadowOffset()
	{
		Vector3 position = base.transform.position;
		DropShadow.DropShadowOffset dropOffset = this.m_DropOffset;
		if (dropOffset != DropShadow.DropShadowOffset.eAuto)
		{
			if (dropOffset == DropShadow.DropShadowOffset.eCustom)
			{
				position.x += this.m_CustomOffset.x;
				position.y += this.m_CustomOffset.y;
				position.z += this.m_CustomOffset.z;
			}
		}
		else
		{
			position.x += DropShadow.m_DefaultDropOffset.x;
			position.y += DropShadow.m_DefaultDropOffset.y;
			position.z += DropShadow.m_DefaultDropOffset.z;
		}
		return position;
	}

	private Color GetDropShadowColor(Color aSpriteTextColor, DropShadow.DropShadowColor aDropColor)
	{
		Color color = DropShadow.sm_DropShadowColorList[0];
		DropShadow.DropShadowColor dropColor = this.m_DropColor;
		if (dropColor != DropShadow.DropShadowColor.eAuto)
		{
			if (dropColor != DropShadow.DropShadowColor.eCustom)
			{
				if (this.m_DropColor >= DropShadow.DropShadowColor.eBlack && this.m_DropColor < (DropShadow.DropShadowColor)DropShadow.sm_DropShadowColorList.Length)
				{
					color = DropShadow.sm_DropShadowColorList[(int)this.m_DropColor];
				}
			}
			else
			{
				color = this.m_CustomColor;
			}
		}
		else if (aSpriteTextColor.Equals(GUIConstants.kDarkBrownColor))
		{
			color = DropShadow.sm_DropShadowColorList[1];
		}
		else if (aSpriteTextColor.Equals(GUIConstants.kLightBrownColor))
		{
			color = DropShadow.sm_DropShadowColorList[1];
		}
		else if (aSpriteTextColor.Equals(GUIConstants.kDarkerBrownColor))
		{
			color = DropShadow.sm_DropShadowColorList[2];
		}
		else if (aSpriteTextColor.Equals(GUIConstants.kBlackColor))
		{
			color = DropShadow.sm_DropShadowColorList[1];
		}
		else
		{
			color = DropShadow.sm_DropShadowColorList[0];
		}
		return color;
	}

	private string RemoveColorTags(string text)
	{
		string text2 = string.Empty;
		bool flag = false;
		char[] array = text.ToCharArray();
		for (int i = 0; i < text.Length; i++)
		{
			if (array[i] == '[')
			{
				flag = true;
			}
			if (array[i] == ']')
			{
				flag = false;
				i++;
			}
			if (!flag)
			{
				text2 += array[i];
			}
		}
		return text2;
	}

	public void UpdateDropShadowText()
	{
		if (this.m_CopySpriteText != null)
		{
			this.m_CopySpriteText.Text = this.m_SourceSpriteText.Text;
		}
	}

	public void UpdateDropShadowSize()
	{
		if (this.m_CopySpriteText != null)
		{
			this.m_CopySpriteText.SetCharacterSize(this.m_SourceSpriteText.characterSize);
		}
	}

	public void HideDropShadowText(bool aHide)
	{
		if (this.m_CopySpriteText != null)
		{
			this.m_CopySpriteText.Hide(aHide);
		}
	}

	public static Color[] sm_DropShadowColorList = new Color[]
	{
		Color.black,
		Color.white,
		new Color(0.62352943f, 0.5019608f, 0.44313726f, 1f)
	};

	public static Vector3 m_DefaultDropOffset = new Vector3(0.03f, -0.06f, 0.06f);

	public DropShadow.DropShadowOffset m_DropOffset;

	public Vector3 m_CustomOffset;

	public DropShadow.DropShadowColor m_DropColor = DropShadow.DropShadowColor.eAuto;

	public Color m_CustomColor;

	private SpriteText m_SourceSpriteText;

	private SpriteText m_CopySpriteText;

	private MeshRenderer m_SourceSpriteMeshRenderer;

	private MeshRenderer m_CopySpriteMeshRenderer;

	private bool m_IsShadowCreated;

	public enum DropShadowOffset
	{
		eAuto,
		eCustom
	}

	public enum DropShadowColor
	{
		eAuto = -2,
		eCustom,
		eBlack,
		eWhite,
		eBrown
	}
}
