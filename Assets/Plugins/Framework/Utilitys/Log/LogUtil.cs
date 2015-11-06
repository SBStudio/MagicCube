using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Framework
{
	public sealed class LogUtil : MonoSingleton<LogUtil>
	{
		public enum PrintType
		{
			Screen,
			Console,
			File,
			Http,
		}

		public static int maxLine = 20;

		public static string path
		{
			get { return m_Path; }
			
			set
			{
				string directory = value.Substring(0, value.LastIndexOf('/'));
				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				m_Path = value;
			}
		}
		private static string m_Path;

		private Queue<string> m_LogQue = new Queue<string>();
		private Vector2 m_ScrollViewPosition = Vector2.zero;

		private void InitPath()
		{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            path = Application.persistentDataPath + "/Log/Log.txt";
#else
			path = Application.dataPath + "/Log/Log.txt";
#endif
		}

		private void OnGUI()
		{
			m_ScrollViewPosition = GUILayout.BeginScrollView(m_ScrollViewPosition);

			foreach (string log in m_LogQue)
			{
				GUILayout.Label(log);
			}

			GUILayout.EndScrollView();
		}

		public static void Debug(object info, PrintType printType = PrintType.File)
		{
			Print(LogType.Log, info.ToString(), printType);
		}

		public static void Warning(object info, PrintType printType = PrintType.File)
		{
			Print(LogType.Warning, info.ToString(), printType);
		}

		public static void Error(object info, PrintType printType = PrintType.File)
		{
			Print(LogType.Error, info.ToString(), printType);
		}

		public static void Clear(PrintType printType = PrintType.File)
		{
			switch (printType)
			{
				case PrintType.Screen:
					instance.m_LogQue.Clear();
					break;
				case PrintType.Console:
					UnityEngine.Debug.ClearDeveloperConsole();
					break;
				case PrintType.File:
					if (null == path)
					{
						instance.InitPath();
					}
					File.WriteAllText(path, string.Empty);
					break;
				case PrintType.Http:
					break;
			}
		}

		private static void Print(LogType logType, string info, PrintType printType)
		{
			info = Format(logType, info);

			switch (printType)
			{
				case PrintType.Screen:
					PrintScreen(logType, info);
					break;
				case PrintType.Console:
					PrintConsole(logType, info);
					break;
				case PrintType.File:
					PrintFile(logType, info);
					break;
				case PrintType.Http:
					break;
			}
		}

		private static void PrintScreen(LogType logType, string info)
		{
			instance.m_LogQue.Enqueue(info);
			if (instance.m_LogQue.Count > maxLine)
			{
				instance.m_LogQue.Dequeue();
			}
		}

		private static void PrintConsole(LogType logType, string info)
		{
			switch (logType)
			{
				case LogType.Log:
					UnityEngine.Debug.Log(info);
					break;
				case LogType.Warning:
					UnityEngine.Debug.LogWarning(info);
					break;
				case LogType.Error:
					UnityEngine.Debug.LogError(info);
					break;
			}
		}

		private static void PrintFile(LogType logType, string info)
		{
			if (null == path)
			{
				instance.InitPath();
			}
			
			StreamWriter writer = new StreamWriter(path, true);

			try
			{
				writer.WriteLine(info);
			}
			catch (Exception e)
			{
				UnityEngine.Debug.Log(e);
			}
			finally
			{
				writer.Close();
			}
		}

		private static string Format(LogType type, string info)
		{
			StackFrame sf = new StackTrace(new StackFrame(3, true)).GetFrame(0);
			int start = sf.GetFileName().LastIndexOf("\\");
			int end = sf.GetFileName().LastIndexOf(".");
			string className = sf.GetFileName().Substring(start + 1, end - start - 1);
			string functionName = sf.GetMethod().Name;
			string logType = type.ToString();

			return "[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss ffff") + "] ["
				+ logType + "]" + " [" + className.ToString() + "#" + functionName + "] " + info.ToString();
		}
	}
}