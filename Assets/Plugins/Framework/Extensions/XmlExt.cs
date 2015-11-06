using UnityEngine;
using System.Xml;

namespace Framework
{
	public static class XmlExt
	{
		public static bool GetBool(this XmlElement element, string attribute)
		{
			return bool.Parse(element.GetAttribute(attribute));
		}

		public static sbyte GetSbyte(this XmlElement element, string attribute)
		{
			return sbyte.Parse(element.GetAttribute(attribute));
		}

		public static short GetShort(this XmlElement element, string attribute)
		{
			return short.Parse(element.GetAttribute(attribute));
		}

		public static int GetInt(this XmlElement element, string attribute)
		{
			return int.Parse(element.GetAttribute(attribute));
		}

		public static long GetLong(this XmlElement element, string attribute)
		{
			return long.Parse(element.GetAttribute(attribute));
		}

		public static float GetFloat(this XmlElement element, string attribute)
		{
			return float.Parse(element.GetAttribute(attribute));
		}

		public static double GetDouble(this XmlElement element, string attribute)
		{
			return double.Parse(element.GetAttribute(attribute));
		}

		public static char GetChar(this XmlElement element, string attribute)
		{
			return char.Parse(element.GetAttribute(attribute));
		}

		public static string GetString(this XmlElement element, string attribute)
		{
			return element.GetAttribute(attribute);
		}
	}
}