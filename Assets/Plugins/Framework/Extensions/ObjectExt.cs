using UnityEngine;

namespace Framework
{
	public static class ObjectExt
	{
		public static string Replace(string value, params object[] args)
		{
			for (int i = args.Length; --i >= 0;)
			{
				string arg = args[i].ToString();

				value = value.Replace("%" + i, arg);
			}

			return value;
		}
	}
}