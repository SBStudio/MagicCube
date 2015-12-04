using UnityEngine;
using UnityEditor;
using Framework;
using System.Collections.Generic;

[CustomEditor(typeof(CubeItem))]
public class CubeItemInspector : Editor
{
	private CubeItem m_Cube;

	private void OnEnable()
	{
		m_Cube = target as CubeItem;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		OnDrawEditor();
	}

	private void OnDrawEditor()
	{
		GUILayout.BeginVertical(EditorStyles.helpBox);

		foreach (KeyValuePair<AxisType, ItemType> itemInfo in m_Cube.itemDict)
		{
			GUILayout.BeginHorizontal();

			ItemType itemType = (ItemType)EditorGUILayout.EnumPopup(itemInfo.Key.ToString(), itemInfo.Value);
			if (itemType != itemInfo.Value)
			{
				m_Cube[itemInfo.Key] = itemType;

				return;
			}

			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();
	}
}