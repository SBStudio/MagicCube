using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	public sealed class DownloadUtil : MonoSingleton<DownloadUtil>
	{
		public delegate void DownloadStartCallback(WWW www);
		public delegate void DownloadCompeleteCallback(WWW www);

		private Queue<DownloadRequest> m_DownloadQue = new Queue<DownloadRequest>();
		private Dictionary<string, DownloadRequest> m_DownloadDict = new Dictionary<string, DownloadRequest>();

		public static float progress
		{
			get
			{
				if (instance.m_DownloadDict.Count == 0)
				{
					return 0;
				}

				float progress = 0;
				foreach (DownloadRequest request in instance.m_DownloadDict.Values)
				{
					if (null == request.www)
					{
						continue;
					}

					if (request.www.progress == 1)
					{
						if (request.isComplete)
						{
							progress += 1;
						}
						else
						{
							progress += 0.999999f;
						}
					}
					else
					{
						progress += request.www.progress;
					}
				}

				progress /= instance.m_DownloadDict.Count;

				return progress;
			}
		}

		public static void Download(string url, DownloadCompeleteCallback onDownloadComplete)
		{
			Download(url, 0, null, onDownloadComplete);
		}

		public static void Download(string url,
			DownloadStartCallback onDownloadStart,
			DownloadCompeleteCallback onDownloadComplete)
		{
			Download(url, 0, onDownloadStart, onDownloadComplete);
		}

		public static void Download(string url, int version = 0,
			DownloadStartCallback onDownloadStart = null,
			DownloadCompeleteCallback onDownloadComplete = null)
		{
			if (instance.m_DownloadDict.ContainsKey(url))
			{
				if (null != onDownloadComplete)
				{
					if (instance.m_DownloadDict[url].isComplete)
					{
						onDownloadComplete(instance.m_DownloadDict[url].www);
					}
					else
					{
						instance.m_DownloadDict[url].onDownloadComplete += onDownloadComplete;
					}
				}
				return;
			}

			DownloadRequest request = new DownloadRequest(url, version, onDownloadStart, onDownloadComplete);

			instance.m_DownloadQue.Enqueue(request);
			instance.m_DownloadDict.Add(url, request);

			if (instance.m_DownloadQue.Count == 1)
			{
				instance.StartCoroutine(instance.DownloadCoroutine(instance.m_DownloadQue.Dequeue()));
			}
		}

		private IEnumerator DownloadCoroutine(DownloadRequest request)
		{
			if (request.version > 0)
			{
				request.www = WWW.LoadFromCacheOrDownload(request.url, request.version);
			}
			else
			{
				request.www = new WWW(request.url);
			}

			if (null != request.onDownloadStart)
			{
				request.onDownloadStart(request.www);
			}

			yield return request.www.isDone;

			request.isComplete = true;

			if (null != request.onDownloadComplete)
			{
				request.onDownloadComplete(request.www);
			}

			if (instance.m_DownloadQue.Count > 0)
			{
				DownloadRequest downloadRequest = instance.m_DownloadQue.Dequeue();
				instance.StartCoroutine(instance.DownloadCoroutine(downloadRequest));
			}
		}

		private sealed class DownloadRequest
		{
			public string url;
			public int version;
			public WWW www;
			public bool isComplete;
			public DownloadStartCallback onDownloadStart;
			public DownloadCompeleteCallback onDownloadComplete;

			public DownloadRequest(string url, int version,
				DownloadStartCallback onDownloadStart,
				DownloadCompeleteCallback onDownloadComplete)
			{
				this.url = url;
				this.version = version;
				this.www = null;
				this.isComplete = false;
				this.onDownloadStart = onDownloadStart;
				this.onDownloadComplete = onDownloadComplete;
			}
		}
	}
}