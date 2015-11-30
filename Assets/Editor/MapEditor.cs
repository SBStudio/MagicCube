using UnityEngine;
using UnityEditor;
using Framework;
using System.Collections.Generic;

public sealed class MapEditor : EditorWindow
{
	private MagicCube m_MagicCube;
	private int m_Step = 5;

	[MenuItem("Tools/MapEditor")]
	public static void Open()
	{
		GetWindow<MapEditor>();
	}

	private void OnEnable()
	{
	}

	private void OnGUI()
	{
		GUILayout.BeginHorizontal();
		m_Step = EditorGUILayout.IntField("Size", m_Step);
		if (GUILayout.Button("Create"))
		{
			Create(m_Step);
		}
		if (GUILayout.Button("Save"))
		{
			DataSystem<MapData>.Init("Database.db", "Map", "id", "cube");

			for (int i = m_MagicCube.maxLayer + 1; --i >= 0;)
			{
				List<CubeItem> cubeList = m_MagicCube[i];
				for (int j = cubeList.Count; --j >= 0;)
				{
					CubeItem cube = cubeList[j];

					Dictionary<string, object> dataDict = new Dictionary<string, object>();
					dataDict["id"] = int.Parse(cube.name);
					dataDict["cube"] = string.Empty;
					foreach (AxisType axisType in cube.itemDict.Keys)
					{
						dataDict["cube"] += (int)axisType + ":" + (int)cube.itemDict[axisType] + "|";
					}

					MapData data = new MapData();
					data.Parse(dataDict);
					DataSystem<MapData>.Set(data);
				}
			}
		}
		GUILayout.EndHorizontal();
	}

	private void Create(int step)
	{
		if (null != m_MagicCube)
		{
			GameObject.DestroyImmediate(m_MagicCube.gameObject);
		}

		GameObject gameObject = new GameObject(typeof(MagicCube).Name);
		m_MagicCube = gameObject.AddComponent<MagicCube>();
		m_MagicCube.Generate(step, 1, 0, 1);
	}
}