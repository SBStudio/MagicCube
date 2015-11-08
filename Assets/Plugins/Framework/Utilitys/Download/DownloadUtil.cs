using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	public sealed class DownloadUtil : MonoSingleton<DownloadUtil>
	{
		public delegate void DownloadCallback(WWW www);

		private struct DownloadRequest
		{
			public WWW www;
			public int version;
			public DownloadCallback onDownloadFinish;
			
			public DownloadRequest(string url,
			                       int version,
			                       DownloadCallback onDownloadFinish)
			{
				this.www = version > 0 ? WWW.LoadFromCacheOrDownload(url, version) : new WWW(url);
				this.version = version;
				this.onDownloadFinish = onDownloadFinish;
			}
		}
		
		private readonly Dictionary<string, DownloadRequest> m_DownloadDict = new Dictionary<string, DownloadRequest>();

		public static WWW Download(string url,
		                           DownloadCallback onDownloadFinish)
		{
			return Download(url, 0, onDownloadFinish);
		}

		public static WWW Download(string url,
		                           int version,
		                           DownloadCallback onDownloadFinish)
		{
			DownloadRequest request = default(DownloadRequest);

			if (instance.m_DownloadDict.ContainsKey(url))
			{
				request = instance.m_DownloadDict[url];

				if (null != onDownloadFinish)
				{
					if (request.www.isDone)
					{
						onDownloadFinish(request.www);
					}
					else
					{
						request.onDownloadFinish += onDownloadFinish;
					}
				}
			}
			else
			{
				request = new DownloadRequest(url, version, onDownloadFinish);
				
				instance.StartCoroutine(instance.OnDownloadCoroutine(request));
				instance.m_DownloadDict[url] = request;
			}

			return request.www;
		}

		private IEnumerator OnDownloadCoroutine(DownloadRequest request)
		{
			yield return request.www;

			request.onDownloadFinish(request.www);
		}
	}
}