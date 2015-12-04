﻿using UnityEngine;
using UnityEditor;
using Framework;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(MagicCube))]
public class MagicCubeInspector : Editor
{
	private MagicCube m_MagicCube;
	private int m_Step = 5;
	private float m_Size = 1;
	private float m_Space = 0;

	private void OnEnable()
	{
		m_MagicCube = target as MagicCube;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		OnDrawEditor();
	}

	private void OnDrawEditor()
	{
		GUILayout.BeginVertical(EditorStyles.helpBox);

		m_Step = EditorGUILayout.IntField("Step", m_Step);
		m_Size = EditorGUILayout.FloatField("Size", m_Size);
		m_Space = EditorGUILayout.FloatField("Space", m_Space);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Create"))
		{
			Create(m_Step);
		}
		if (GUILayout.Button("Save"))
		{
			Save();
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	private void Create(int step)
	{
		if (null != m_MagicCube)
		{
			GameObject.DestroyImmediate(m_MagicCube.gameObject);
		}

		GameObject gameObject = new GameObject(typeof(MagicCube).Name);
		m_MagicCube = gameObject.AddComponent<MagicCube>();
		m_MagicCube.Generate(m_Step, m_Size, m_Space);
	}

	private void Save()
	{
		string[] fields = new string[] { "id", "cube" };
		Type[] types = new Type[] { typeof(int), typeof(string) };
		DataSystem<MapData>.Init("Database.db", "Map", fields, types);

		for (int i = m_MagicCube.maxLayer + 1; --i >= 0;)
		{
			List<CubeItem> cubeList = m_MagicCube[i];
			for (int j = cubeList.Count; --j >= 0;)
			{
				CubeItem cube = cubeList[j];

				Dictionary<string, object> dataDict = new Dictionary<string, object>();
				dataDict["id"] = int.Parse(cube.name);

				string str = string.Empty;
				foreach (AxisType axisType in cube.itemDict.Keys)
				{
					str += (int)axisType + ":" + (int)cube.itemDict[axisType] + "|";
				}
				str = str.Remove(str.Length - 1);

				dataDict["cube"] = str;

				MapData data = new MapData();
				data.Parse(dataDict);
				DataSystem<MapData>.Set(data);
			}
		}
	}
}