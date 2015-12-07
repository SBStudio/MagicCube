using UnityEngine;

namespace Framework
{
	public static class ObjectExt
	{
		private const char SPLIT = ',';

		public static string ToString(this Color color)
		{
			return color.r.ToString() + SPLIT
				+ color.g.ToString() + SPLIT
				+ color.b.ToString() + SPLIT
				+ color.a;
		}

		public static void Parse(this Color color, string value)
		{
			string[] colors = value.Split(SPLIT);
			color.r = float.Parse(colors[0]);
			color.g = float.Parse(colors[1]);
			color.b = float.Parse(colors[2]);
			color.r = float.Parse(colors[3]);
		}
	}
}