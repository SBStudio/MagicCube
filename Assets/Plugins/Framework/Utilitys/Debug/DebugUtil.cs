using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	public sealed class DebugUtil : MonoSingleton<DebugUtil>
	{
		private Dictionary<Type, IDebugInfo> m_DebugInfoDict = new Dictionary<Type, IDebugInfo>();

		private void OnGUI()
		{
			foreach (IDebugInfo debugInfo in m_DebugInfoDict.Values)
			{
				debugInfo.OnGUI();
			}
		}

		private void Update()
		{
			foreach (IDebugInfo debugInfo in m_DebugInfoDict.Values)
			{
				debugInfo.OnUpdate();
			}
		}

		public static void Add<T>()
			where T : IDebugInfo, new()
		{
			if (Contains<T>())
			{
				return;
			}

			T debugInfo = new T();

			instance.m_DebugInfoDict.Add(typeof(T), debugInfo);
		}

		public static T Find<T>()
			where T : IDebugInfo
		{
			if (!Contains<T>())
			{
				return default(T);
			}

			return (T)instance.m_DebugInfoDict[typeof(T)];
		}

		public static void Remove<T>()
			where T : IDebugInfo
		{
			if (!Contains<T>())
			{
				return;
			}

			instance.m_DebugInfoDict.Remove(typeof(T));
		}

		public static bool Contains<T>() where T : IDebugInfo
		{
			return instance.m_DebugInfoDict.ContainsKey(typeof(T));
		}
	}
}