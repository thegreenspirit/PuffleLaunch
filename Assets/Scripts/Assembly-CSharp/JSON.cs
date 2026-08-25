using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Procurios.Public
{
	public class JSON
	{
		public static object JsonDecode(string json)
		{
			bool flag = true;
			return JSON.JsonDecode(json, ref flag);
		}

		public static object JsonDecode(string json, ref bool success)
		{
			success = true;
			if (json != null)
			{
				char[] array = json.ToCharArray();
				int num = 0;
				return JSON.ParseValue(array, ref num, ref success);
			}
			return null;
		}

		public static string JsonEncode(object json)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			bool flag = JSON.SerializeValue(json, stringBuilder);
			return (!flag) ? null : stringBuilder.ToString();
		}

		protected static Hashtable ParseObject(char[] json, ref int index, ref bool success)
		{
			Hashtable hashtable = new Hashtable();
			JSON.NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				int num = JSON.LookAhead(json, index);
				if (num == 0)
				{
					success = false;
					return null;
				}
				if (num == 6)
				{
					JSON.NextToken(json, ref index);
				}
				else
				{
					if (num == 2)
					{
						JSON.NextToken(json, ref index);
						return hashtable;
					}
					string text = JSON.ParseString(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}
					num = JSON.NextToken(json, ref index);
					if (num != 5)
					{
						success = false;
						return null;
					}
					object obj = JSON.ParseValue(json, ref index, ref success);
					if (!success)
					{
						success = false;
						return null;
					}
					hashtable[text] = obj;
				}
			}
			return hashtable;
		}

		protected static ArrayList ParseArray(char[] json, ref int index, ref bool success)
		{
			ArrayList arrayList = new ArrayList();
			JSON.NextToken(json, ref index);
			bool flag = false;
			while (!flag)
			{
				int num = JSON.LookAhead(json, index);
				if (num == 0)
				{
					success = false;
					return null;
				}
				if (num == 6)
				{
					JSON.NextToken(json, ref index);
				}
				else
				{
					if (num == 4)
					{
						JSON.NextToken(json, ref index);
						break;
					}
					object obj = JSON.ParseValue(json, ref index, ref success);
					if (!success)
					{
						return null;
					}
					arrayList.Add(obj);
				}
			}
			return arrayList;
		}

		protected static object ParseValue(char[] json, ref int index, ref bool success)
		{
			switch (JSON.LookAhead(json, index))
			{
			case 1:
				return JSON.ParseObject(json, ref index, ref success);
			case 3:
				return JSON.ParseArray(json, ref index, ref success);
			case 7:
				return JSON.ParseString(json, ref index, ref success);
			case 8:
				return JSON.ParseNumber(json, ref index, ref success);
			case 9:
				JSON.NextToken(json, ref index);
				return true;
			case 10:
				JSON.NextToken(json, ref index);
				return false;
			case 11:
				JSON.NextToken(json, ref index);
				return null;
			}
			success = false;
			return null;
		}

		protected static string ParseString(char[] json, ref int index, ref bool success)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			JSON.EatWhitespace(json, ref index);
			char c = json[index++];
			bool flag = false;
			while (!flag)
			{
				if (index == json.Length)
				{
					break;
				}
				c = json[index++];
				if (c == '"')
				{
					flag = true;
					break;
				}
				if (c == '\\')
				{
					if (index == json.Length)
					{
						break;
					}
					c = json[index++];
					if (c == '"')
					{
						stringBuilder.Append('"');
					}
					else if (c == '\\')
					{
						stringBuilder.Append('\\');
					}
					else if (c == '/')
					{
						stringBuilder.Append('/');
					}
					else if (c == 'b')
					{
						stringBuilder.Append('\b');
					}
					else if (c == 'f')
					{
						stringBuilder.Append('\f');
					}
					else if (c == 'n')
					{
						stringBuilder.Append('\n');
					}
					else if (c == 'r')
					{
						stringBuilder.Append('\r');
					}
					else if (c == 't')
					{
						stringBuilder.Append('\t');
					}
					else if (c == 'u')
					{
						int num = json.Length - index;
						if (num < 4)
						{
							break;
						}
						uint num2;
						if (!(success = uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2)))
						{
							return string.Empty;
						}
						stringBuilder.Append(char.ConvertFromUtf32((int)num2));
						index += 4;
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if (!flag)
			{
				success = false;
				return null;
			}
			return stringBuilder.ToString();
		}

		protected static double ParseNumber(char[] json, ref int index, ref bool success)
		{
			JSON.EatWhitespace(json, ref index);
			int lastIndexOfNumber = JSON.GetLastIndexOfNumber(json, index);
			int num = lastIndexOfNumber - index + 1;
			double num2;
			success = double.TryParse(new string(json, index, num), NumberStyles.Any, CultureInfo.InvariantCulture, out num2);
			index = lastIndexOfNumber + 1;
			return num2;
		}

		protected static int GetLastIndexOfNumber(char[] json, int index)
		{
			int i;
			for (i = index; i < json.Length; i++)
			{
				if ("0123456789+-.eE".IndexOf(json[i]) == -1)
				{
					break;
				}
			}
			return i - 1;
		}

		protected static void EatWhitespace(char[] json, ref int index)
		{
			while (index < json.Length)
			{
				if (" \t\n\r".IndexOf(json[index]) == -1)
				{
					break;
				}
				index++;
			}
		}

		protected static int LookAhead(char[] json, int index)
		{
			int num = index;
			return JSON.NextToken(json, ref num);
		}

		protected static int NextToken(char[] json, ref int index)
		{
			JSON.EatWhitespace(json, ref index);
			if (index == json.Length)
			{
				return 0;
			}
			char c = json[index];
			index++;
			char c2 = c;
			switch (c2)
			{
			case '"':
				return 7;
			default:
				switch (c2)
				{
				case '[':
					return 3;
				default:
				{
					switch (c2)
					{
					case '{':
						return 1;
					case '}':
						return 2;
					}
					index--;
					int num = json.Length - index;
					if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
					{
						index += 5;
						return 10;
					}
					if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
					{
						index += 4;
						return 9;
					}
					if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
					{
						index += 4;
						return 11;
					}
					return 0;
				}
				case ']':
					return 4;
				}
				break;
			case ',':
				return 6;
			case '-':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				return 8;
			case ':':
				return 5;
			}
		}

		protected static bool SerializeValue(object value, StringBuilder builder)
		{
			bool flag = true;
			if (value is string)
			{
				flag = JSON.SerializeString((string)value, builder);
			}
			else if (value is Hashtable)
			{
				flag = JSON.SerializeObject((Hashtable)value, builder);
			}
			else if (value is ArrayList)
			{
				flag = JSON.SerializeArray((ArrayList)value, builder);
			}
			else if (JSON.IsNumeric(value))
			{
				flag = JSON.SerializeNumber(Convert.ToDouble(value), builder);
			}
			else if (value is bool && (bool)value)
			{
				builder.Append("true");
			}
			else if (value is bool && !(bool)value)
			{
				builder.Append("false");
			}
			else if (value == null)
			{
				builder.Append("null");
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		protected static bool SerializeObject(Hashtable anObject, StringBuilder builder)
		{
			builder.Append("{");
			IDictionaryEnumerator enumerator = anObject.GetEnumerator();
			bool flag = true;
			while (enumerator.MoveNext())
			{
				string text = enumerator.Key.ToString();
				object value = enumerator.Value;
				if (!flag)
				{
					builder.Append(", ");
				}
				JSON.SerializeString(text, builder);
				builder.Append(":");
				if (!JSON.SerializeValue(value, builder))
				{
					return false;
				}
				flag = false;
			}
			builder.Append("}");
			return true;
		}

		protected static bool SerializeArray(ArrayList anArray, StringBuilder builder)
		{
			builder.Append("[");
			bool flag = true;
			for (int i = 0; i < anArray.Count; i++)
			{
				object obj = anArray[i];
				if (!flag)
				{
					builder.Append(", ");
				}
				if (!JSON.SerializeValue(obj, builder))
				{
					return false;
				}
				flag = false;
			}
			builder.Append("]");
			return true;
		}

		protected static bool SerializeString(string aString, StringBuilder builder)
		{
			builder.Append("\"");
			foreach (char c in aString.ToCharArray())
			{
				if (c == '"')
				{
					builder.Append("\\\"");
				}
				else if (c == '\\')
				{
					builder.Append("\\\\");
				}
				else if (c == '\b')
				{
					builder.Append("\\b");
				}
				else if (c == '\f')
				{
					builder.Append("\\f");
				}
				else if (c == '\n')
				{
					builder.Append("\\n");
				}
				else if (c == '\r')
				{
					builder.Append("\\r");
				}
				else if (c == '\t')
				{
					builder.Append("\\t");
				}
				else
				{
					int num = Convert.ToInt32(c);
					if (num >= 32 && num <= 126)
					{
						builder.Append(c);
					}
					else
					{
						builder.Append("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
					}
				}
			}
			builder.Append("\"");
			return true;
		}

		protected static bool SerializeNumber(double number, StringBuilder builder)
		{
			builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
			return true;
		}

		protected static bool IsNumeric(object o)
		{
			double num;
			return o != null && double.TryParse(o.ToString(), out num);
		}

		public const int TOKEN_NONE = 0;

		public const int TOKEN_CURLY_OPEN = 1;

		public const int TOKEN_CURLY_CLOSE = 2;

		public const int TOKEN_SQUARED_OPEN = 3;

		public const int TOKEN_SQUARED_CLOSE = 4;

		public const int TOKEN_COLON = 5;

		public const int TOKEN_COMMA = 6;

		public const int TOKEN_STRING = 7;

		public const int TOKEN_NUMBER = 8;

		public const int TOKEN_TRUE = 9;

		public const int TOKEN_FALSE = 10;

		public const int TOKEN_NULL = 11;

		private const int BUILDER_CAPACITY = 2000;
	}
}
