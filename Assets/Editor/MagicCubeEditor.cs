using UnityEngine;
using UnityEditor;
using Framework;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(MagicCube))]
public class MagicCubeEditor : Editor
{
	private static int s_Id = 0;
	private static int s_LoadId = 0;
	private static int s_Step = 5;
	private static float s_Size = 1;
	private static float s_Space = 0;
	private static CubeItem s_DestCube = null;
	private static SpawnInfo s_SpawnInfo;
	private static MagicCube s_MagicCube;

	private DataSystem<MapData> mapDatabase
	{
		get
		{
			if (null == m_MapDatabase)
			{
				m_MapDatabase = DataSystem<MapData>.instance;
				m_MapDatabase.Init(MapData.DATABASE, MapData.TABLE, MapData.FIELDS);
			}

			return m_MapDatabase;
		}
	}
	private DataSystem<MapData> m_MapDatabase;

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

		s_DestCube = EditorGUILayout.ObjectField("DestCube", s_DestCube, typeof(CubeItem), true) as CubeItem;
		s_SpawnInfo.id = EditorGUILayout.IntField("SpawnId", s_SpawnInfo.id);
		s_SpawnInfo.right = (AxisType)EditorGUILayout.EnumPopup("SpawnRight", s_SpawnInfo.right);
		s_SpawnInfo.up = (AxisType)EditorGUILayout.EnumPopup("SpawnUp", s_SpawnInfo.up);
		s_SpawnInfo.forward = (AxisType)EditorGUILayout.EnumPopup("SpawnForward", s_SpawnInfo.forward);

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
		MapData[] mapDatas = mapDatabase.GetAll();
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

		s_MagicCube.Generate(s_Id, s_Step, s_Size, s_Space, 0);
		s_DestCube = s_MagicCube.destCube;

		List<CubeItem> cubeList = s_MagicCube[s_MagicCube.layer];
		CubeItem cube = cubeList[UnityEngine.Random.Range(0, cubeList.Count)];
		
		List<AxisType> axisTypes = new List<AxisType>(cube.itemDict.Keys);
		int index = UnityEngine.Random.Range(0, axisTypes.Count);
		AxisType upAxis = axisTypes[index];

		axisTypes = new List<AxisType>(Enum.GetValues(typeof(AxisType)) as AxisType[]);

		axisTypes.Remove(upAxis);
		axisTypes.Remove((AxisType)(-(int)upAxis));

		index = UnityEngine.Random.Range(0, axisTypes.Count);
		AxisType rightAxis = axisTypes[index];

		axisTypes.Remove(rightAxis);
		axisTypes.Remove((AxisType)(-(int)rightAxis));

		index = UnityEngine.Random.Range(0, axisTypes.Count);
		AxisType forwardAxis = axisTypes[index];

		s_SpawnInfo = new SpawnInfo();
		s_SpawnInfo.id = cube.id;
		s_SpawnInfo.right = rightAxis;
		s_SpawnInfo.up = upAxis;
		s_SpawnInfo.forward = forwardAxis;

		Selection.activeGameObject = s_MagicCube.gameObject;
	}

	private void Save()
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
		dataDict[MapData.FIELD_DEST] = s_DestCube.id;
		dataDict[MapData.FIELD_SPAWN] = MapData.ParseSpawn(s_SpawnInfo);
		dataDict[MapData.FIELD_CUBE] = MapData.ParseCube(cubeItems);

		MapData data = new MapData();
		data.Parse(dataDict);

		mapDatabase.Set(data);
	}

	private void Load()
	{
		MapData mapData = mapDatabase.Get(s_LoadId);
		if (null == mapData)
		{
			return;
		}

		s_MagicCube.Load(mapData);
		s_DestCube = s_MagicCube.destCube;
		s_SpawnInfo = mapData.spawnInfo;
	}

	private void Delete()
	{
		mapDatabase.Delete(s_LoadId);
	}
}