using UnityEngine;
using UnityEditor;
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

	private void OnGUI()
	{
		GUILayout.BeginHorizontal();
		m_Step = EditorGUILayout.IntField("Size", m_Step);
		if (GUILayout.Button("Create"))
		{
			Create(m_Step);
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