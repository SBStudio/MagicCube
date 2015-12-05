using UnityEngine;
using UnityEditor;
using Framework;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(MagicCube))]
public class MagicCubeEditor : Editor
{
	private static int s_Id = 0;
	private static int s_Step = 5;
	private static float s_Size = 1;
	private static float s_Space = 0;
	private static MagicCube s_MagicCube;

	private void OnEnable()
	{
		if (null != s_MagicCube
			&& target != s_MagicCube)
		{
			DestroyImmediate(s_MagicCube.gameObject);
		}

		s_MagicCube = target as MagicCube;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		OnDrawEditor();
	}

	private void OnDrawEditor()
	{
		GUILayout.BeginVertical(EditorStyles.helpBox);

		s_Id = EditorGUILayout.IntField("Id", s_Id);
		s_Step = EditorGUILayout.IntField("Step", s_Step);
		s_Size = EditorGUILayout.FloatField("Size", s_Size);
		s_Space = EditorGUILayout.FloatField("Space", s_Space);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Create"))
		{
			Create();
		}
		else if (GUILayout.Button("Save"))
		{
			Save();
		}
		else if (GUILayout.Button("Load"))
		{
			Load();
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	[MenuItem("GameObject/MagicCube")]
	private static void Create()
	{
		if (null != s_MagicCube)
		{
			GameObject.DestroyImmediate(s_MagicCube.gameObject);
		}

		GameObject gameObject = new GameObject(typeof(MagicCube).Name);
		s_MagicCube = gameObject.AddComponent<MagicCube>();
		s_MagicCube.Generate(s_Id, s_Step, s_Size, s_Space);

		Selection.activeGameObject = gameObject;
	}

	private static void Save()
	{
		Dictionary<AxisType, ItemType>[] cubeItems = new Dictionary<AxisType, ItemType>[s_MagicCube.num];
		for (int i = s_MagicCube.maxLayer + 1; --i >= 0;)
		{
			List<CubeItem> cubeList = s_MagicCube[i];
			for (int j = cubeList.Count; --j >= 0;)
			{
				CubeItem cube = cubeList[j];
				cubeItems[cube.id] = cube.itemDict;
			}
		}

		Dictionary<string, object> dataDict = new Dictionary<string, object>();
		dataDict[MapData.FIELD_ID] = s_MagicCube.id;
		dataDict[MapData.FIELD_STEP] = s_MagicCube.step;
		dataDict[MapData.FIELD_SIZE] = s_MagicCube.size;
		dataDict[MapData.FIELD_SPACE] = s_MagicCube.space;
		dataDict[MapData.FIELD_CUBE] = MapData.ParseCube(cubeItems);

		MapData data = new MapData();
		data.Parse(dataDict);

		string[] fields = new string[] { MapData.FIELD_ID,
			MapData.FIELD_STEP,
			MapData.FIELD_SIZE,
			MapData.FIELD_SPACE,
			MapData.FIELD_CUBE };
		Type[] types = new Type[] { typeof(int),
			typeof(int),
			typeof(float),
			typeof(float),
			typeof(string) };
		DataSystem<MapData>.Init("Database.db", "Map", fields, types);
		DataSystem<MapData>.Set(data);
	}

	private static void Load()
	{
		string[] fields = new string[] { MapData.FIELD_ID,
			MapData.FIELD_STEP,
			MapData.FIELD_SIZE,
			MapData.FIELD_SPACE,
			MapData.FIELD_CUBE };
		Type[] types = new Type[] { typeof(int),
			typeof(int),
			typeof(float),
			typeof(float),
			typeof(string) };
		DataSystem<MapData>.Init("Database.db", "Map", fields, types);
		MapData mapData = DataSystem<MapData>.Get(s_Id);
		if (null == mapData)
		{
			return;
		}

		if (null != s_MagicCube)
		{
			GameObject.DestroyImmediate(s_MagicCube.gameObject);
		}

		GameObject gameObject = new GameObject(typeof(MagicCube).Name);
		s_MagicCube = gameObject.AddComponent<MagicCube>();
		s_MagicCube.Generate(mapData.id, mapData.step, mapData.size, mapData.space);

		for (int i = s_MagicCube.maxLayer + 1; --i >= 0;)
		{
			List<CubeItem> cubeList = s_MagicCube[i];
			for (int j = cubeList.Count; --j >= 0;)
			{
				CubeItem cube = cubeList[j];
				cube.Generate(mapData.cubeItems[cube.id]);
			}
		}

		Selection.activeGameObject = gameObject;
	}
}