using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	public sealed class DebugUtil : MonoSingleton<DebugUtil>
	{
		private readonly Dictionary<Type, ScriptableBehaviour> m_DebugInfoDict = new Dictionary<Type, ScriptableBehaviour>();
		
		private void OnGUI()
		{
			foreach (ScriptableBehaviour debugInfo in m_DebugInfoDict.Values)
			{
				debugInfo.OnGUI();
			}
		}

		private void Update()
		{
			foreach (ScriptableBehaviour debugInfo in m_DebugInfoDict.Values)
			{
				debugInfo.OnUpdate();
			}
		}
		
		private void LateUpdate()
		{
			foreach (ScriptableBehaviour debugInfo in m_DebugInfoDict.Values)
			{
				debugInfo.OnLateUpdate();
			}
		}
		
		private void FixedUpdate()
		{
			foreach (ScriptableBehaviour debugInfo in m_DebugInfoDict.Values)
			{
				debugInfo.OnFixedUpdate();
			}
		}

		private void OnDestroy()
		{
			foreach (ScriptableBehaviour debugInfo in m_DebugInfoDict.Values)
			{
				debugInfo.OnDestroy();
			}
		}

		public static T Add<T>()
			where T : ScriptableBehaviour
		{
			if (Contains<T>())
			{
				return Get<T>();
			}

			T debugInfo = ScriptableObject.CreateInstance<T>();

			instance.m_DebugInfoDict.Add(typeof(T), debugInfo);

			return debugInfo;
		}

		public static T Get<T>()
			where T : ScriptableBehaviour
		{
			if (!Contains<T>())
			{
				return default(T);
			}

			return (T)instance.m_DebugInfoDict[typeof(T)];
		}

		public static void Remove<T>()
			where T : ScriptableBehaviour
		{
			if (!Contains<T>())
			{
				return;
			}

			instance.m_DebugInfoDict.Remove(typeof(T));
		}

		public static bool Contains<T>()
			where T : ScriptableBehaviour
		{
			return instance.m_DebugInfoDict.ContainsKey(typeof(T));
		}
	}
}