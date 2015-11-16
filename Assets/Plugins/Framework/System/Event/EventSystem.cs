namespace Framework
{
	public sealed class EventSystem<T> : Singleton<EventSystem<T>>
	{
		public delegate void EventCallback(T arg);

		public event EventCallback onEventCallback;

		public static void Broadcast(T arg)
		{
			if (null == instance.onEventCallback)
			{
				return;
			}

			instance.onEventCallback(arg);
		}

		public static void Add(EventCallback onEventCallback)
		{
			instance.onEventCallback += onEventCallback;
		}

		public static void Remove(EventCallback onEventCallback)
		{
			instance.onEventCallback -= onEventCallback;
		}

		public static void Clear()
		{
			instance.onEventCallback = null;
		}
	}
}