using UnityEngine;

namespace Framework
{
	public sealed class FPSInfo : IDebugInfo
	{
		public float updateTime = 0.5f;
		public Color color = Color.green;
		public int size = 24;
		private GUIStyle m_GUIStyle = new GUIStyle();
		public bool showDetail = false;
		public int fps { get; private set; }
		public int averageFPS { get { return (int)(Time.frameCount / Time.time); } }
		public int minFPS { get; private set; }
		public int maxFPS { get; private set; }
		public int frameSum { get; private set; }
		public float runTime { get; private set; }

		public FPSInfo()
		{
			runTime = Time.time;
		}

		public void OnGUI()
		{
			string str = fps.ToString();

			if (showDetail)
			{
				str += " (Ave:" + averageFPS.ToString() + ",Min:" + minFPS + ",Max:" + maxFPS + ")";
			}

			GUI.depth = int.MinValue;
			GUILayout.Label(str, m_GUIStyle);
		}

		public void OnUpdate()
		{
			m_GUIStyle.normal.textColor = color;
			m_GUIStyle.fontSize = (int)(size * ((float)Screen.height / 720));

			frameSum++;

			float deltaTime = Time.time - runTime;
			if (deltaTime >= updateTime)
			{
				fps = (int)(frameSum / deltaTime);

				if (minFPS == 0 || fps < minFPS)
				{
					minFPS = fps;
				}

				if (fps > maxFPS)
				{
					maxFPS = fps;
				}

				frameSum = 0;
				runTime = Time.time;
			}
		}
	}
}