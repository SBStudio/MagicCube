using UnityEngine;

namespace Framework
{
	public static class ObjectExt
	{
		public static void Log(this object obj, object info, LogUtil.PrintType printType = LogUtil.PrintType.File)
		{
			LogUtil.Debug(info, printType);
		}

		public static string Replace(this string str, string value, params object[] args)
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