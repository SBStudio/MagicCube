using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
	public sealed class ObjectPool<T>
	{
		public delegate T ActiveCallback(object content);
		public delegate void DeactiveCallback(T obj);
		public ActiveCallback onGenerateCallback;
		public ActiveCallback onActiveCallback;
		public DeactiveCallback onDeactiveCallback;
		public DeactiveCallback onClearCallback;
		public int step = 1;

		private List<T> m_ActiveList = new List<T>();
		private Queue<T> m_DeactivePool = new Queue<T>();

		public ObjectPool(ActiveCallback onGenerateCallback)
		{
			if (null == onGenerateCallback)
			{
				throw new System.NullReferenceException("The generate object callback is null!");
			}

			this.onGenerateCallback = onGenerateCallback;
		}
		
		public void Generate(object content)
		{
			for (int i = step; --i >= 0; )
			{
				T obj = onGenerateCallback(content);
				m_DeactivePool.Enqueue(obj);
			}
		}

		public T Active(object content)
		{
			if (m_DeactivePool.Count == 0)
			{
				Generate(content);
			}

			T obj = m_DeactivePool.Dequeue();
			m_ActiveList.Add(obj);

			if (null != onActiveCallback)
			{
				onActiveCallback(obj);
			}

			return obj;
		}

		public void Deactive(T obj)
		{
			if (!m_ActiveList.Contains(obj))
			{
				return;
			}

			m_ActiveList.Remove(obj);
			m_DeactivePool.Enqueue(obj);

			if (null != onDeactiveCallback)
			{
				onDeactiveCallback(obj);
			}
		}

		public void Clear()
		{
			if (null != onClearCallback)
			{
				for (int i = m_ActiveList.Count; --i >= 0;)
				{
					T obj = m_ActiveList[i];
					onClearCallback(obj);
				}
				
				foreach (T obj in m_DeactivePool)
				{
					onClearCallback(obj);
				}
			}

			m_ActiveList.Clear();
			m_DeactivePool.Clear();
		}
	}
}