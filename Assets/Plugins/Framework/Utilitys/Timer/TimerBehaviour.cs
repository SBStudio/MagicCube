using UnityEngine;

namespace Framework
{
	public sealed class TimerBehaviour : MonoBehaviour
	{
		private const string FUNCTION = "OnTimerInvoke";
		
		public TimerUtil.TimerCallback onTimerCallback { get; private set; }
		public TimerUtil.TimerWithArgsCallback onTimerWithArgsCallback { get; private set; }
		public float startTime { get; private set; }
		public float repeatRate { get; private set; }
		public int repeatTimes { get; private set; }
		public int curRepeatTimes { get; private set; }
		public object[] args { get; private set; }

		public void Begin(TimerUtil.TimerCallback onTimerCallback,
		                       float startTime,
		                       float repeatRate,
		                       int repeatTimes)
		{
			this.onTimerCallback = onTimerCallback;
			this.startTime = startTime;
			this.repeatRate = repeatRate;
			this.repeatTimes = repeatTimes;
			
			if (repeatRate > 0)
			{
				InvokeRepeating(FUNCTION, startTime, repeatRate);
			}
			else
			{
				Invoke(FUNCTION, startTime);
			}
		}
		
		public void Begin(TimerUtil.TimerWithArgsCallback onTimerWithArgsCallback,
		                  float startTime,
		                  float repeatRate,
		                  int repeatTimes,
		                  params object[] args)
		{
			this.onTimerWithArgsCallback = onTimerWithArgsCallback;
			this.startTime = startTime;
			this.repeatRate = repeatRate;
			this.repeatTimes = repeatTimes;
			this.args = args;
			
			if (repeatRate > 0)
			{
				InvokeRepeating(FUNCTION, startTime, repeatRate);
			}
			else
			{
				Invoke(FUNCTION, startTime);
			}
		}
		
		public void Stop()
		{
			CancelInvoke(FUNCTION);
			Destroy(this);
		}
		
		private void OnTimerInvoke()
		{
			if (null != onTimerCallback)
			{
				onTimerCallback();
			}
			else if (null != onTimerWithArgsCallback)
			{
				onTimerWithArgsCallback(args);
			}
			
			if (repeatRate <= 0)
			{
				Stop();
			}
			else if (repeatTimes > 0
			         && --curRepeatTimes >= repeatTimes)
			{
				Stop();
			}
		}
	}
}