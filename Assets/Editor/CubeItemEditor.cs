using UnityEngine;
using UnityEditor;
using Framework;
using System.Collections.Generic;

[CustomEditor(typeof(CubeItem))]
public class CubeItemEditor : Editor
{
	private DataSystem<ItemData> itemDatabase
	{
		get
		{
			if (null == m_ItemDatabase)
			{
				m_ItemDatabase = DataSystem<ItemData>.instance;
				m_ItemDatabase.Init(ItemData.DATABASE, ItemData.TABLE, ItemData.FIELDS);
			}

			return m_ItemDatabase;
		}
	}
	private DataSystem<ItemData> m_ItemDatabase;

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

		ItemData[] itemDatas = itemDatabase.GetAll();
		int[] itemIds = new int[itemDatas.Length];
		string[] itemNames = new string[itemDatas.Length];
		for (int i = itemDatas.Length; --i >= 0;)
		{
			itemIds[i] = itemDatas[i].id;
			itemNames[i] = itemDatas[i].name;
		}

		foreach (KeyValuePair<AxisType, ItemData> itemInfo in m_Cube.itemDict)
		{
			GUILayout.BeginHorizontal();

			int itemId = EditorGUILayout.IntPopup(itemInfo.Key.ToString(), itemInfo.Value.id, itemNames, itemIds);
			if (itemId != itemInfo.Value.id)
			{
				m_Cube[itemInfo.Key] = itemDatabase.Get(itemId);

				return;
			}

			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();
	}
}