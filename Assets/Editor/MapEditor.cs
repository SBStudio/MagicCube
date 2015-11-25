using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public sealed class MapEditor : EditorWindow
{
	private MagicCube m_MagicCube;
	private int m_Step;

	[MenuItem("Tools/MapEditor")]
	public static void Open()
	{
		GetWindow<MapEditor>();
	}

	private void OnGUI()
	{
		m_Step = EditorGUILayout.IntField("Size", m_Step);

		if (GUILayout.Button("Create"))
		{
			Create(m_Step);
		}
	}

	private void Create(int step)
	{
		if (null != m_MagicCube)
		{
			GameObject.DestroyImmediate(m_MagicCube.gameObject);
		}

		GameObject gameObject = new GameObject(typeof(MagicCube).Name);
		m_MagicCube = gameObject.AddComponent<MagicCube>();
		m_MagicCube.Init(step, 1, 0, 1);
	}
}