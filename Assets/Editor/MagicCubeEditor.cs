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
	private static int s_LoadId = 0;
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

		OnDrawSave();
		OnDrawCreate();
		OnDrawLoad();
	}

	private void OnDrawSave()
	{
		GUILayout.BeginVertical("Current Settings", EditorStyles.helpBox);
		GUILayout.Space(15);

		EditorGUILayout.LabelField("Id", s_MagicCube.id.ToString());
		EditorGUILayout.LabelField("Step", s_MagicCube.step.ToString());
		EditorGUILayout.LabelField("Size", s_MagicCube.size.ToString());
		EditorGUILayout.LabelField("Space", s_MagicCube.space.ToString());

		if (GUILayout.Button("Save"))
		{
			Save();
		}

		GUILayout.EndVertical();
	}

	private void OnDrawCreate()
	{
		GUILayout.BeginVertical("Create Settings", EditorStyles.helpBox);
		GUILayout.Space(15);

		s_Id = EditorGUILayout.IntField("Id", s_Id);
		s_Step = EditorGUILayout.IntField("Step", s_Step);
		s_Size = EditorGUILayout.FloatField("Size", s_Size);
		s_Space = EditorGUILayout.FloatField("Space", s_Space);

		if (GUILayout.Button("Create"))
		{
			Create();
		}

		GUILayout.EndVertical();
	}

	private void OnDrawLoad()
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
		MapData[] mapDatas = DataSystem<MapData>.GetAll();
		if (null == mapDatas)
		{
			return;
		}

		string[] names = new string[mapDatas.Length];
		int[] ids = new int[mapDatas.Length];
		for (int i = 0; i < mapDatas.Length; ++i)
		{
			MapData mapData = mapDatas[i];
			names[i] = mapData.id.ToString();
			ids[i] = mapData.id;
		}

		GUILayout.BeginVertical("Load Settings", EditorStyles.helpBox);
		GUILayout.Space(15);

		s_LoadId = EditorGUILayout.IntPopup("Id", s_LoadId, names, ids);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Load"))
		{
			Load();
		}
		else if (GUILayout.Button("Delete"))
		{
			Delete();
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}

	[MenuItem("GameObject/MagicCube")]
	private static void Create()
	{
		if (null == s_MagicCube)
		{
			GameObject gameObject = new GameObject(typeof(MagicCube).Name);
			s_MagicCube = gameObject.AddComponent<MagicCube>();
		}

		s_MagicCube.Generate(s_Id, s_Step, s_Size, s_Space);
		Selection.activeGameObject = s_MagicCube.gameObject;
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
		MapData mapData = DataSystem<MapData>.Get(s_LoadId);
		if (null == mapData)
		{
			return;
		}

		s_MagicCube.Load(mapData);
	}

	private static void Delete()
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
		DataSystem<MapData>.Delete(s_LoadId);
	}
}