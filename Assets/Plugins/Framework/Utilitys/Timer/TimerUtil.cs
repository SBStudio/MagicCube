using UnityEngine;

namespace Framework
{
	public sealed class TimerUtil : MonoSingleton<TimerUtil>
	{
		public delegate void TimerCallback();
		public delegate void TimerWithArgsCallback(params object[] args);

		public static TimerBehaviour Begin(TimerCallback onTimerCallback,
		                                   float starTime)
		{
			return Begin(onTimerCallback, starTime, 0, 0);
		}

		public static TimerBehaviour Begin(TimerCallback onTimerCallback,
		                                   float startTime,
		                                   float repeatRate)
		{
			return Begin(onTimerCallback, startTime, repeatRate, 0);
		}

		public static TimerBehaviour Begin(TimerCallback onTimerCallback,
		                                   float startTime,
		                                   float repeatRate,
		                                   int repeatTimes)
		{
			if (null == onTimerCallback)
			{
				throw new System.NullReferenceException("The timer callback is null!");
			}

			TimerBehaviour timer = instance.gameObject.AddComponent<TimerBehaviour>();
			timer.Begin(onTimerCallback, startTime, repeatRate, repeatTimes);

			return timer;
		}
		
		public static TimerBehaviour Begin(TimerWithArgsCallback onTimerWithArgsCallback,
		                                   float starTime)
		{
			return Begin(onTimerWithArgsCallback, starTime, 0, 0);
		}
		
		public static TimerBehaviour Begin(TimerWithArgsCallback onTimerWithArgsCallback,
		                                   float startTime,
		                                   float repeatRate)
		{
			return Begin(onTimerWithArgsCallback, startTime, repeatRate, 0);
		}
		
		public static TimerBehaviour Begin(TimerWithArgsCallback onTimerWithArgsCallback,
		                                   float startTime,
		                                   float repeatRate,
		                                   int repeatTimes)
		{
			return Begin(onTimerWithArgsCallback, startTime, repeatRate, repeatTimes);
		}

		public static TimerBehaviour Begin(TimerWithArgsCallback onTimerWithArgsCallback,
		                                   float startTime,
		                                   float repeatRate,
		                                   int repeatTimes,
		                                   params object[] args)
		{
			if (null == onTimerWithArgsCallback)
			{
				throw new System.NullReferenceException("The timer callback is null!");
			}

			TimerBehaviour timer = instance.gameObject.AddComponent<TimerBehaviour>();
			timer.Begin(onTimerWithArgsCallback, startTime, repeatRate, repeatTimes, args);

			return timer;
		}
	}
}