using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
	public sealed class TimerUtil : MonoSingleton<TimerUtil>
	{
		public delegate void OnTimerCallback(object[] args);

		private Dictionary<OnTimerCallback, TimerObject> m_TimerDict = new Dictionary<OnTimerCallback, TimerObject>();

		private ObjectPool<TimerObject> m_TimerPool = new ObjectPool<TimerObject>(OnGenerateTimer, OnAcitveTimer, OnDeactiveTimer);

		public static void Start(OnTimerCallback onTimerCallback, float time)
		{
			Start(onTimerCallback, time, 0, 0, null);
		}

		public static void Start(OnTimerCallback onTimerCallback, float time, object[] args)
		{
			Start(onTimerCallback, time, 0, 0, args);
		}

		public static void Start(OnTimerCallback onTimerCallback, float time, float repeatRate)
		{
			Start(onTimerCallback, time, repeatRate, 0, null);
		}

		public static void Start(OnTimerCallback onTimerCallback, float time, float repeatRate, object[] args)
		{
			Start(onTimerCallback, time, repeatRate, 0, args);
		}

		public static void Start(OnTimerCallback onTimerCallback, float time, float repeatRate, int repeatTimes, object[] args)
		{
			if (null == onTimerCallback)
			{
				throw new System.NullReferenceException("The timer callback is null!");
			}

			if (instance.m_TimerDict.ContainsKey(onTimerCallback))
			{
				Stop(onTimerCallback);
			}

			TimerObject timerObject = instance.m_TimerPool.Active(null);
			instance.m_TimerDict[onTimerCallback] = timerObject;

			timerObject.StartTimer(onTimerCallback, time, repeatRate, repeatTimes, args);
		}
		
		public static void Stop(OnTimerCallback onTimerCallback)
		{
			if (null == onTimerCallback)
			{
				throw new System.NullReferenceException("The timer callback is null!");
			}

			TimerObject timerObject = instance.m_TimerDict[onTimerCallback];
			timerObject.StopTimer();
			instance.m_TimerPool.Deactive(timerObject);
			instance.m_TimerDict.Remove(onTimerCallback);
		}

		public static void StopAll()
		{
			instance.m_TimerDict.Clear();
			instance.StopAllCoroutines();
		}

		private static TimerObject OnGenerateTimer(object content)
		{
			GameObject obj = new GameObject("TimerObject");
			obj.transform.parent = instance.transform;
			obj.SetActive(false);
			return obj.AddComponent<TimerObject>();
		}

		private static void OnAcitveTimer(TimerObject timerObject)
		{
			timerObject.gameObject.SetActive(true);
		}

		private static void OnDeactiveTimer(TimerObject timerObject)
		{
			timerObject.gameObject.SetActive(false);
		}

		private sealed class TimerObject : MonoBehaviour
		{
			private const string FUNCTION = "OnTimerInvoke";

			private OnTimerCallback m_OnTimerCallback;

			private int m_RepeatTimes;

			private object[] m_Args;
			
			public void StartTimer(OnTimerCallback onTimerCallback,
				float time, float repeatRate, int repeatTimes, object[] args)
			{
				m_OnTimerCallback = onTimerCallback;
				m_Args = args;
				m_RepeatTimes = repeatTimes;

				name = onTimerCallback.Target + "_" + onTimerCallback.Method + "_Timer";

				if (repeatRate > 0)
				{
					InvokeRepeating(FUNCTION, time == 0 ? 0.0000001f : time, repeatRate);
				}
				else
				{
					Invoke(FUNCTION, time);
				}
			}

			public void StopTimer()
			{
				CancelInvoke(FUNCTION);
			}

			private void OnTimerInvoke()
			{
				m_OnTimerCallback(m_Args);

				if (m_RepeatTimes > 0)
				{
					m_RepeatTimes--;
					if (m_RepeatTimes <= 0)
					{
						TimerUtil.Stop(m_OnTimerCallback);
					}
				}
			}
		}
	}
}