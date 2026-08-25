using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SpriteFont
{
	public SpriteFont(TextAsset def)
	{
		this.Load(def);
	}

	public int LineHeight
	{
		get
		{
			return this.lineHeight;
		}
		set
		{
			this.lineHeight = value;
		}
	}

	public int BaseHeight
	{
		get
		{
			return this.baseHeight;
		}
	}

	public int PixelSize
	{
		get
		{
			return this.pxSize;
		}
	}

	public float CharacterSpacing
	{
		get
		{
			return this.charSpacing;
		}
		set
		{
			float num = this.charSpacing;
			this.charSpacing = value;
			if (num != this.charSpacing && this.chars != null)
			{
				for (int i = 0; i < this.chars.Length; i++)
				{
					if (this.chars[i] != null)
					{
						this.chars[i].xAdvance *= this.charSpacing;
						if (this.chars[i].kernings != null)
						{
							int[] array = new int[this.chars[i].kernings.Keys.Count];
							this.chars[i].kernings.Keys.CopyTo(array, 0);
							for (int j = 0; j < array.Length; j++)
							{
								this.chars[i].kernings[array[j]] = this.charSpacing * this.chars[i].origKernings[array[j]];
							}
						}
					}
				}
			}
		}
	}

	public void Load(TextAsset def)
	{
		if (def == null)
		{
			return;
		}
		int num = 0;
		this.fontDef = def;
		string[] array = this.fontDef.text.Split(new char[] { '\n' });
		int num2 = this.ParseSection("info", array, new SpriteFont.ParserDel(this.HeaderParser), 0);
		num2 = this.ParseSection("common", array, new SpriteFont.ParserDel(this.CommonParser), num2);
		num2 = this.ParseSection("chars count", array, new SpriteFont.ParserDel(this.CharCountParser), num2);
		this.bleedCompUV = new Vector2(0f / (float)this.texWidth, 0f / (float)this.texHeight);
		this.bleedCompUVMax = this.bleedCompUV * -2f;
		while (num2 < array.Length && num < this.chars.Length)
		{
			if (!this.CharParser(array[num2++], num))
			{
				break;
			}
			num++;
		}
		num2--;
		num2 = this.ParseSection("kernings count", array, new SpriteFont.ParserDel(this.KerningCountParser), num2);
		num = 0;
		while (num2 < array.Length && num < this.kerningsCount)
		{
			if (this.KerningParser(array[num2++]))
			{
				num++;
			}
		}
		float num3 = this.charSpacing;
		this.charSpacing = 0f;
		this.CharacterSpacing = num3;
	}

	private int ParseSection(string tag, string[] lines, SpriteFont.ParserDel parser, int pos)
	{
		while (pos < lines.Length)
		{
			string text = lines[pos].Trim();
			if (text.Length >= 1)
			{
				if (text.StartsWith(tag))
				{
					parser(text);
					return ++pos;
				}
			}
			pos++;
		}
		return pos;
	}

	private int FindField(string label, string[] fields, int pos, bool logError)
	{
		while (pos < fields.Length)
		{
			if (label == fields[pos])
			{
				return pos;
			}
			pos++;
		}
		if (logError)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Missing \"",
				label,
				"\" field in font definition file \"",
				this.fontDef.name,
				"\". Please check the file or re-create it."
			}));
			return pos;
		}
		return -1;
	}

	private int FindField(string label, string[] fields, int pos)
	{
		return this.FindField(label, fields, pos, true);
	}

	private int FindFieldOptional(string label, string[] fields, int pos)
	{
		return this.FindField(label, fields, pos, false);
	}

	private void HeaderParser(string line)
	{
		string[] array = line.Split(new char[] { ' ', '=' });
		int num = this.FindField("face", array, 1);
		this.face = array[num + 1].Trim(new char[] { '"' });
		num = this.FindField("size", array, num);
		this.pxSize = Mathf.Abs(int.Parse(array[num + 1]));
		num = this.FindFieldOptional("charSpacing", array, num);
		if (num != -1)
		{
			this.charSpacing = Mathf.Abs(float.Parse(array[num + 1]));
		}
	}

	private void CommonParser(string line)
	{
		string[] array = line.Split(new char[] { ' ', '=' });
		int num = this.FindField("lineHeight", array, 1);
		this.lineHeight = int.Parse(array[num + 1]);
		num = this.FindField("base", array, num);
		this.baseHeight = int.Parse(array[num + 1]);
		num = this.FindField("scaleW", array, num);
		this.texWidth = int.Parse(array[num + 1]);
		num = this.FindField("scaleH", array, num);
		this.texHeight = int.Parse(array[num + 1]);
		num = this.FindField("pages", array, num);
		if (int.Parse(array[num + 1]) > 1)
		{
			Debug.LogError("Multiple pages/textures detected for font \"" + this.face + "\". only one font atlas is supported.");
		}
	}

	private void CharCountParser(string line)
	{
		string[] array = line.Split(new char[] { '=' });
		if (array.Length < 2)
		{
			Debug.LogError("Malformed \"chars count\" line in font definition file \"" + this.fontDef.name + "\". Please check the file or re-create it.");
			return;
		}
		this.chars = new SpriteChar[int.Parse(array[1]) + 1];
	}

	private bool CharParser(string line, int charNum)
	{
		if (!line.StartsWith("char"))
		{
			return false;
		}
		string[] array = line.Split(new char[] { ' ', '=' });
		int num = this.FindField("id", array, 1);
		this.chars[charNum] = new SpriteChar();
		this.chars[charNum].id = int.Parse(array[num + 1]);
		num = this.FindField("x", array, num);
		float num2 = float.Parse(array[num + 1]) / (float)this.texWidth;
		num = this.FindField("y", array, num);
		float num3 = 1f - float.Parse(array[num + 1]) / (float)this.texHeight;
		num = this.FindField("width", array, num);
		float num4 = float.Parse(array[num + 1]) / (float)this.texWidth;
		num = this.FindField("height", array, num);
		float num5 = float.Parse(array[num + 1]) / (float)this.texHeight;
		num = this.FindField("xoffset", array, num);
		this.chars[charNum].xOffset = float.Parse(array[num + 1]);
		num = this.FindField("yoffset", array, num);
		this.chars[charNum].yOffset = -float.Parse(array[num + 1]);
		num = this.FindField("xadvance", array, num);
		this.chars[charNum].xAdvance = (float)int.Parse(array[num + 1]);
		this.chars[charNum].UVs.x = num2 + this.bleedCompUV.x;
		this.chars[charNum].UVs.y = num3 - num5 + this.bleedCompUV.y;
		this.chars[charNum].UVs.xMax = num2 + num4 + this.bleedCompUVMax.x;
		this.chars[charNum].UVs.yMax = num3 + this.bleedCompUVMax.y;
		this.charMap.Add((int)Convert.ToChar(this.chars[charNum].id), charNum);
		return true;
	}

	private void KerningCountParser(string line)
	{
		string[] array = line.Split(new char[] { '=' });
		this.kerningsCount = int.Parse(array[1]);
	}

	private bool KerningParser(string line)
	{
		if (!line.StartsWith("kerning"))
		{
			return false;
		}
		string[] array = line.Split(new char[] { ' ', '=' });
		int num = this.FindField("first", array, 1);
		int num2 = int.Parse(array[num + 1]);
		num = this.FindField("second", array, num);
		int num3 = int.Parse(array[num + 1]);
		num = this.FindField("amount", array, num);
		int num4 = int.Parse(array[num + 1]);
		SpriteChar spriteChar = this.GetSpriteChar(Convert.ToChar(num3));
		if (spriteChar == null)
		{
			return true;
		}
		if (spriteChar.kernings == null)
		{
			spriteChar.kernings = new Dictionary<int, float>();
			spriteChar.origKernings = new Dictionary<int, float>();
		}
		spriteChar.kernings.Add((int)Convert.ToChar(num2), (float)num4);
		spriteChar.origKernings.Add((int)Convert.ToChar(num2), (float)num4);
		return true;
	}

	public SpriteChar GetSpriteChar(char ch)
	{
		int num;
		if (!this.charMap.TryGetValue((int)ch, out num))
		{
			return null;
		}
		return this.chars[num];
	}

	public bool ContainsCharacter(char ch)
	{
		return this.charMap.ContainsKey((int)ch);
	}

	public float GetWidth(string str)
	{
		float num = 0f;
		if (str.Length < 1)
		{
			return 0f;
		}
		SpriteChar spriteChar = this.GetSpriteChar(str[0]);
		if (spriteChar != null)
		{
			num = spriteChar.xAdvance;
		}
		for (int i = 1; i < str.Length; i++)
		{
			spriteChar = this.GetSpriteChar(str[i]);
			if (spriteChar != null)
			{
				num += spriteChar.xAdvance + spriteChar.GetKerning((int)str[i - 1]);
			}
		}
		return num;
	}

	public float GetWidth(string str, int start, int end)
	{
		float num = 0f;
		if (start >= str.Length || end < start)
		{
			return 0f;
		}
		end = Mathf.Clamp(end, 0, str.Length - 1);
		SpriteChar spriteChar = this.GetSpriteChar(str[start]);
		if (spriteChar != null)
		{
			num = spriteChar.xAdvance;
		}
		for (int i = start + 1; i <= end; i++)
		{
			spriteChar = this.GetSpriteChar(str[i]);
			if (spriteChar != null)
			{
				num += spriteChar.xAdvance + spriteChar.GetKerning((int)str[i - 1]);
			}
		}
		return num;
	}

	public float GetWidth(StringBuilder sb, int start, int end)
	{
		float num = 0f;
		if (start >= sb.Length || end < start)
		{
			return 0f;
		}
		end = Mathf.Clamp(end, 0, sb.Length - 1);
		SpriteChar spriteChar = this.GetSpriteChar(sb[start]);
		if (spriteChar != null)
		{
			num = spriteChar.xAdvance;
		}
		for (int i = start + 1; i <= end; i++)
		{
			spriteChar = this.GetSpriteChar(sb[i]);
			if (spriteChar != null)
			{
				num += spriteChar.xAdvance + spriteChar.GetKerning((int)sb[i - 1]);
			}
		}
		return num;
	}

	public float GetWidth(char prevChar, char c)
	{
		SpriteChar spriteChar = this.GetSpriteChar(c);
		if (spriteChar == null)
		{
			return 0f;
		}
		return spriteChar.xAdvance + spriteChar.GetKerning((int)prevChar);
	}

	public float GetAdvance(char c)
	{
		SpriteChar spriteChar = this.GetSpriteChar(c);
		if (spriteChar == null)
		{
			return 0f;
		}
		return spriteChar.xAdvance;
	}

	public string RemoveUnsupportedCharacters(string str)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < str.Length; i++)
		{
			if (this.charMap.ContainsKey((int)str[i]) || str[i] == '\n' || str[i] == '\t' || str[i] == '#' || str[i] == '[' || str[i] == ']' || str[i] == '(' || str[i] == ')' || str[i] == ',')
			{
				stringBuilder.Append(str[i]);
			}
		}
		return stringBuilder.ToString();
	}

	protected const float bleedCompensation = 0f;

	public TextAsset fontDef;

	protected Dictionary<int, int> charMap = new Dictionary<int, int>();

	protected SpriteChar[] chars;

	protected Vector2 bleedCompUV;

	protected Vector2 bleedCompUVMax;

	protected int lineHeight;

	protected int baseHeight;

	protected int texWidth;

	protected int texHeight;

	protected string face;

	protected int pxSize;

	protected float charSpacing = 1f;

	private int kerningsCount;

	protected delegate void ParserDel(string line);
}
