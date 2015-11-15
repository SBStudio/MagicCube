using UnityEngine;

namespace Framework
{
	public abstract class Singleton<T> where T : new()
	{
		public static readonly T instance = new T();
	}
}