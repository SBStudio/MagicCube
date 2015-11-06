using UnityEngine;

namespace Framework
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		public static T instance
		{
			get
			{
				if (null == s_Instance)
				{
					s_Instance = CreateInstance<T>();
				}

				return s_Instance;
			}
		}
		private static T s_Instance;

		public static T CreateInstance<T>() where T : MonoSingleton<T>
		{
			T instance = GameObject.FindObjectOfType<T>();

			if (null == instance)
			{
				string name = typeof(T).ToString();
				instance = new GameObject(name).AddComponent<T>();
			}

			instance.hideFlags = HideFlags.HideAndDontSave;
			GameObject.DontDestroyOnLoad(instance);

			return instance;
		}
	}
}