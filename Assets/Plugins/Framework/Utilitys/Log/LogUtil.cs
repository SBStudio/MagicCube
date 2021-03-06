﻿using UnityEngine;
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

#if UNITY_EDITOR
		private const string TEXT = "[{0}] [{1}] [{2} # {3}] {4}";
#else
		private const string TEXT = "[{0}] [{1}] {2}";
#endif
		private const string DATE = "yyMMdd hh:mm:ss ffff";

		public static string path
		{
			get
			{
				if (null == m_Path)
				{
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
					m_Path = Application.persistentDataPath + "/Log/Log.txt";
#else
					m_Path = Application.dataPath + "/Log/Log.txt";
#endif
				}

				return m_Path;
			}
			set { m_Path = value; }
		}
		private static string m_Path;

		public static PrintType printType = PrintType.Console;
		
		public static int maxLine = 100;
		public static int size = 24;
		public static Color color
		{
			get { return guiStyle.normal.textColor; }
			set { guiStyle.normal.textColor = value; }
		}
		public static GUIStyle guiStyle
		{
			get
			{
				if (null == instance.m_GUIStyle)
				{
					instance.m_GUIStyle = new GUIStyle();
					instance.m_GUIStyle.normal.textColor = Color.black;
				}
				
				return instance.m_GUIStyle;
			}
		}
		private GUIStyle m_GUIStyle;

		private Queue<string> m_LogQue = new Queue<string>();
		private bool m_Toggle = true;
		private Vector2 m_ScrollViewPosition = Vector2.zero;

		public static void LogDebug(object info)
		{
			Log(printType, LogType.Log, info);
		}

		public static void LogWarning(object info)
		{
			Log(printType, LogType.Warning, info);
		}

		public static void LogError(object info)
		{
			Log(printType, LogType.Error, info);
		}

		public static void Log(PrintType printType, LogType logType, object msg)
		{
			string formatMsg = instance.Format(logType, msg);

			switch (printType)
			{
				case PrintType.Screen:
					instance.PrintScreen(logType, formatMsg);
					break;
				case PrintType.Console:
					instance.PrintConsole(logType, formatMsg);
					break;
				case PrintType.File:
					instance.PrintFile(logType, formatMsg);
					break;
				case PrintType.Http:
					break;
			}
		}

		public static void Clear()
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
				File.WriteAllText(path, string.Empty);
				break;
			case PrintType.Http:
				break;
			}
		}

		private void Awake()
		{
			Application.logMessageReceived += OnConsoleMessage;
		}

		private void OnGUI()
		{
			m_Toggle = GUILayout.Toggle(m_Toggle, "Show Log", guiStyle);

			if (!m_Toggle
			    || 0 >= m_LogQue.Count)
			{
				return;
			}

			string text = string.Empty;
			foreach (string log in m_LogQue)
			{
				text += log + "\n";
			}

			m_ScrollViewPosition = GUILayout.BeginScrollView(m_ScrollViewPosition);
			GUILayout.TextField(text, guiStyle);
			GUILayout.EndScrollView();
		}

		private void Update()
		{
			guiStyle.fontSize = (int)(size * ((float)Screen.height / 720));
		}
		
		private void OnConsoleMessage(string condition, string stackTrace, LogType logType)
		{
			if (printType == PrintType.Console)
			{
				return;
			}

			Log(printType, logType, condition + "\n" + stackTrace);
		}

		private void PrintScreen(LogType logType, string msg)
		{
			instance.m_LogQue.Enqueue(msg);
			if (instance.m_LogQue.Count > maxLine)
			{
				instance.m_LogQue.Dequeue();
			}
		}

		private void PrintConsole(LogType logType, string msg)
		{
			switch (logType)
			{
				case LogType.Log:
					UnityEngine.Debug.Log(msg);
					break;
				case LogType.Warning:
					UnityEngine.Debug.LogWarning(msg);
					break;
				case LogType.Error:
					UnityEngine.Debug.LogError(msg);
					break;
			}
		}

		private void PrintFile(LogType logType, string msg)
		{
			string directory = path.Substring(0, path.LastIndexOf('/'));
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
			
			TextWriter writter = null;
			if (!File.Exists(path))
			{
				writter = File.CreateText(path);
			}
			else
			{
				writter = new StreamWriter(path, true);
			}

			writter.WriteLine(msg);
			writter.Close();
		}

		private string Format(LogType type, object msg)
		{
			string date = DateTime.Now.ToString(DATE);

#if UNITY_EDITOR
			StackFrame sf = new StackTrace(new StackFrame(3, true)).GetFrame(0);
			int start = sf.GetFileName().LastIndexOf("/");
			int end = sf.GetFileName().LastIndexOf(".");
			string className = sf.GetFileName().Substring(start + 1, end - start - 1);
			string functionName = sf.GetMethod().Name;
			string str = string.Format(TEXT, type, date, className, functionName, msg);
#else
			string str = string.Format(TEXT, type, date, msg);
#endif

			return str;
		}
	}
}