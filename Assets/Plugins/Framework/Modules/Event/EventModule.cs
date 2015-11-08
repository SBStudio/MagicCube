﻿using System;
using System.Collections.Generic;

namespace Framework
{
	public sealed class EventModule<T> : Singleton<EventModule<T>>
	{
		public delegate void EventCallback(T arg);

		public EventCallback onEventCallback { get; private set; }

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