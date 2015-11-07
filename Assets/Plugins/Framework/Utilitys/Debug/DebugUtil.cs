using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	public sealed class DebugUtil : MonoSingleton<DebugUtil>
	{
		public interface IDebugInfo
		{
			void OnGUI();
			void OnUpdate();
			void OnLateUpdate();
			void OnFixedUpdate();
		}

		private readonly Dictionary<Type, IDebugInfo> m_DebugInfoDict = new Dictionary<Type, IDebugInfo>();

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
		
		private void LateUpdate()
		{
			foreach (IDebugInfo debugInfo in m_DebugInfoDict.Values)
			{
				debugInfo.OnLateUpdate();
			}
		}
		
		private void FixedUpdate()
		{
			foreach (IDebugInfo debugInfo in m_DebugInfoDict.Values)
			{
				debugInfo.OnFixedUpdate();
			}
		}

		public static T Add<T>()
			where T : IDebugInfo, new()
		{
			if (Contains<T>())
			{
				return Get<T>();
			}

			T debugInfo = new T();

			instance.m_DebugInfoDict.Add(typeof(T), debugInfo);

			return debugInfo;
		}

		public static T Get<T>()
			where T : IDebugInfo, new()
		{
			if (!Contains<T>())
			{
				return default(T);
			}

			return (T)instance.m_DebugInfoDict[typeof(T)];
		}

		public static void Remove<T>()
			where T : IDebugInfo, new()
		{
			if (!Contains<T>())
			{
				return;
			}

			instance.m_DebugInfoDict.Remove(typeof(T));
		}

		public static bool Contains<T>()
			where T : IDebugInfo, new()
		{
			return instance.m_DebugInfoDict.ContainsKey(typeof(T));
		}
	}
}