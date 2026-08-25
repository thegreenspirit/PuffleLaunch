using System;
using UnityEngine;

public class SizeCategory
{
	public SizeCategory()
	{
		if (Utilities.AssertMsg(this.m_Resolutions.Length > 0, "You did not provide any Resolution Category!"))
		{
			this.m_CurrentCategory = this.GetCategory();
			this.m_CurrentCategoryId = this.GetCategoryId(this.m_CurrentCategory);
		}
	}

	public static SizeCategory Instance
	{
		get
		{
			if (SizeCategory.m_cInstance == null)
			{
				SizeCategory.m_cInstance = new SizeCategory();
			}
			return SizeCategory.m_cInstance;
		}
	}

	public string Category
	{
		get
		{
			return this.m_CurrentCategory;
		}
	}

	public SizeCategory.CategoryId CurCategoryId
	{
		get
		{
			return this.m_CurrentCategoryId;
		}
	}

	private string GetCategory()
	{
		if (this.m_Resolutions.Length == 1)
		{
			return this.m_Resolutions[0].name;
		}
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		foreach (SizeCategory.ResolutionCategory resolutionCategory in this.m_Resolutions)
		{
			if (resolutionCategory != null && ((num == resolutionCategory.width && num2 == resolutionCategory.height) || (num2 == resolutionCategory.width && num == resolutionCategory.height)))
			{
				return resolutionCategory.name;
			}
		}
		string text = string.Empty;
		float num3 = float.PositiveInfinity;
		float num4 = float.PositiveInfinity;
		foreach (SizeCategory.ResolutionCategory resolutionCategory2 in this.m_Resolutions)
		{
			if (resolutionCategory2 != null)
			{
				float num5 = Mathf.Abs(resolutionCategory2.width - num);
				float num6 = Mathf.Abs(resolutionCategory2.height - num2);
				if (num5 < num3 || num6 < num4)
				{
					num3 = num5;
					num4 = num6;
					text = resolutionCategory2.name;
				}
			}
		}
		return text;
	}

	private SizeCategory.CategoryId GetCategoryId(string category)
	{
		switch (category)
		{
		case "small":
			return SizeCategory.CategoryId.eSmall;
		case "large":
			return SizeCategory.CategoryId.eLarge;
		case "xlarge":
			return SizeCategory.CategoryId.eXLarge;
		}
		return SizeCategory.CategoryId.eMedium;
	}

	public SizeCategory.CategoryId GetAlternateCategoryId(SizeCategory.CategoryId categoryId)
	{
		switch (categoryId)
		{
		case SizeCategory.CategoryId.eSmall:
			return SizeCategory.CategoryId.eMedium;
		case SizeCategory.CategoryId.eLarge:
			return SizeCategory.CategoryId.eMedium;
		case SizeCategory.CategoryId.eXLarge:
			return SizeCategory.CategoryId.eLarge;
		}
		return SizeCategory.CategoryId.eMedium;
	}

	public string GetCategory(SizeCategory.CategoryId categoryId)
	{
		switch (categoryId)
		{
		case SizeCategory.CategoryId.eSmall:
			return "small";
		case SizeCategory.CategoryId.eLarge:
			return "large";
		case SizeCategory.CategoryId.eXLarge:
			return "xlarge";
		}
		return "medium";
	}

	public const string kSmall = "small";

	public const string kMedium = "medium";

	public const string kLarge = "large";

	public const string kXlarge = "xlarge";

	private static SizeCategory m_cInstance;

	private SizeCategory.ResolutionCategory[] m_Resolutions = new SizeCategory.ResolutionCategory[]
	{
		new SizeCategory.ResolutionCategory(480f, 320f, "small"),
		new SizeCategory.ResolutionCategory(800f, 480f, "medium"),
		new SizeCategory.ResolutionCategory(854f, 480f, "medium"),
		new SizeCategory.ResolutionCategory(960f, 540f, "large"),
		new SizeCategory.ResolutionCategory(1280f, 800f, "xlarge")
	};

	private string m_CurrentCategory = string.Empty;

	private SizeCategory.CategoryId m_CurrentCategoryId = SizeCategory.CategoryId.eMedium;

	public enum CategoryId
	{
		eUnknown = -1,
		eSmall,
		eMedium,
		eLarge,
		eXLarge
	}

	private class ResolutionCategory
	{
		public ResolutionCategory(float resW, float resH, string fName)
		{
			this.width = resW;
			this.height = resH;
			this.name = fName;
		}

		public float AspectRatio
		{
			get
			{
				return this.width / this.height;
			}
		}

		public float width;

		public float height;

		public string name = string.Empty;
	}
}
