using UnityEngine;
using System.Collections.Generic;

namespace Framework
{
	public sealed class AssetModule : Singleton<AssetModule>
	{
		private readonly Dictionary<string, AssetBundle> m_AssetDict = new Dictionary<string, AssetBundle>();

		public static void Add(string asset)
		{
			if (instance.m_AssetDict.ContainsKey(asset))
			{
				return;
			}

			DownloadUtil.Download(instance.OnDownloadAsset, asset);
		}

		public static void Remove(string asset)
		{
			if (!instance.m_AssetDict.ContainsKey(asset))
			{
				return;
			}

			UnloadAsset(asset, true);
			instance.m_AssetDict.Remove(asset);
		}

		public static void Clear()
		{
			foreach (string asset in instance.m_AssetDict.Keys)
			{
				UnloadAsset(asset, true);
			}

			instance.m_AssetDict.Clear();
		}

		public static T Load<T>(string asset, string name) where T : Object
		{
			if (!instance.m_AssetDict.ContainsKey(asset))
			{
				return default(T);
			}

			AssetBundle assetBundle = instance.m_AssetDict[asset];
			T resource = assetBundle.LoadAsset<T>(name);

			return resource;
		}

		public static T[] LoadAll<T>(string asset) where T : Object
		{
			if (!instance.m_AssetDict.ContainsKey(asset))
			{
				return default(T[]);
			}

			AssetBundle assetBundle = instance.m_AssetDict[asset];
			T[] resources = assetBundle.LoadAllAssets<T>();

			return resources;
		}

		public static void UnloadAsset(string asset, bool unloadAll)
		{
			if (!instance.m_AssetDict.ContainsKey(asset))
			{
				return;
			}

			AssetBundle assetBundle = instance.m_AssetDict[asset];
			assetBundle.Unload(unloadAll);
		}

		private void OnDownloadAsset(WWW www)
		{
			m_AssetDict[www.url] =  www.assetBundle;
		}
	}
}