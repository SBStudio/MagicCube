using UnityEngine;

namespace Framework
{
	public abstract class Singleton<T> where T : Singleton<T>, new()
	{
		public static readonly T instance = new T();
	}
}