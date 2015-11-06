using UnityEngine;

namespace Framework
{
	public abstract class Singleton<T> : ScriptableObject where T : Singleton<T>
	{
		public static readonly T instance = ScriptableObject.CreateInstance<T>();
	}
}