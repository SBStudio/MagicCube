using UnityEngine;

namespace Framework
{
	public sealed class FPSInfo : ScriptableBehaviour
	{
		private const string INFO = "FPS:{0} (AVE:{1},MIN:{2},MAX:{3})";

		public int fps { get; private set; }
		public int aveFPS { get { return (int)(Time.frameCount / Time.time); } }
		public int minFPS { get; private set; }
		public int maxFPS { get; private set; }
		public int frameSum { get; private set; }
		public float runTime { get; private set; }
		public float updateTime = 0.5f;
		public bool showDetail = false;
		public int size = 24;
		public Color color
		{
			get { return guiStyle.normal.textColor; }
			set { guiStyle.normal.textColor = value; }
		}
		public GUIStyle guiStyle
		{
			get
			{
				if (null == m_GUIStyle)
				{
					m_GUIStyle = new GUIStyle();
					m_GUIStyle.normal.textColor = Color.green;
				}

				return m_GUIStyle;
			}
		}
		private GUIStyle m_GUIStyle;

		public FPSInfo()
		{
			runTime = Time.time;
		}

		public override void OnGUI()
		{
			string str = fps.ToString();

			if (showDetail)
			{
				str = string.Format(INFO, fps, aveFPS, minFPS, maxFPS);
			}

			GUI.depth = int.MinValue;
			GUILayout.Label(str, guiStyle);
		}

		public override void OnUpdate()
		{
			guiStyle.fontSize = (int)(size * ((float)Screen.height / 720));

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