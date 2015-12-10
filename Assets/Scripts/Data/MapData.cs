using UnityEngine;
using Framework;
using System;
using System.Collections.Generic;

public sealed class MapData : IData
{
	public const string DATABASE = "Database.db";
	public const string TABLE = "Map";

	public static readonly Dictionary<string, Type> FIELDS = new Dictionary<string, Type>()
	{
		{ MapData.FIELD_ID, typeof(int) },
		{ MapData.FIELD_STEP, typeof(int) },
		{ MapData.FIELD_SIZE, typeof(float) },
		{ MapData.FIELD_SPACE, typeof(float) },
		{ MapData.FIELD_DEST, typeof(int) },
		{ MapData.FIELD_SPAWN, typeof(string) },
		{ MapData.FIELD_CUBE, typeof(string) },
	};

	public const string FIELD_ID = "id";
	public const string FIELD_STEP = "step";
	public const string FIELD_SIZE = "size";
	public const string FIELD_SPACE = "space";
	public const string FIELD_DEST = "dest";
	public const string FIELD_SPAWN = "spawn";
	public const string FIELD_CUBE = "cube";
	private const char SPLIT_DATA = '|';
	private const char SPLIT_FIELD = ';';
	private const char SPLIT_CUBE = ',';
	private const char SPLIT_ITEM = ':';

	public Dictionary<string, object> dataDict { get; private set; }

	public int id { get; private set; }
	public int step { get; private set; }
	public float size { get; private set; }
	public float space { get; private set; }
	public SpawnInfo spawnInfo { get; private set; }
	public int destId { get; private set; }
	public Dictionary<AxisType, int>[] cubeItems { get; private set; }

	public MapData()
	{
		dataDict = new Dictionary<string, object>();
	}

	public void Parse(Dictionary<string, object> arg)
	{
		dataDict = arg;

		this.id = (int)arg[FIELD_ID];
		step = (int)arg[FIELD_STEP];
		size = (float)arg[FIELD_SIZE];
		space = (float)arg[FIELD_SPACE];
		destId = (int)arg[FIELD_DEST];
		spawnInfo = ParseSpawn((string)arg[FIELD_SPAWN]);
		cubeItems = ParseCube((string)arg[FIELD_CUBE]);
	}

	public static SpawnInfo ParseSpawn(string value)
	{
		SpawnInfo spawnInfo = new SpawnInfo();
		string[] datas = value.Split(SPLIT_DATA);

		spawnInfo.id = int.Parse(datas[0]);
		spawnInfo.right = (AxisType)int.Parse(datas[1]);
		spawnInfo.up = (AxisType)int.Parse(datas[2]);
		spawnInfo.forward = (AxisType)int.Parse(datas[3]);

		return spawnInfo;
	}

	public static string ParseSpawn(SpawnInfo spawnInfo)
	{
		return spawnInfo.id + SPLIT_DATA.ToString()
			+ (int)spawnInfo.right + SPLIT_DATA.ToString()
			+ (int)spawnInfo.up + SPLIT_DATA.ToString()
			+ (int)spawnInfo.forward;
	}

	public static Dictionary<AxisType, int>[] ParseCube(string value)
	{
		string[] datas = value.Split(SPLIT_DATA);
		Dictionary<AxisType, int>[] cubeItems = new Dictionary<AxisType, int>[datas.Length];
		for (int i = datas.Length; --i >= 0;)
		{
			string[] mapDatas = datas[i].Split(SPLIT_FIELD);
			int id = int.Parse(mapDatas[0]);
			string mapData = mapDatas[1];

			Dictionary<AxisType, int> itemDict = new Dictionary<AxisType, int>();
			cubeItems[id] = itemDict;

			string[] cubeDatas = mapData.Split(SPLIT_CUBE);
			for (int j = cubeDatas.Length; --j >= 0;)
			{
				string[] itemDatas = cubeDatas[j].Split(SPLIT_ITEM);
				AxisType axisType = (AxisType)int.Parse(itemDatas[0]);
				int itemId = int.Parse(itemDatas[1]);
				itemDict[axisType] = itemId;
			}
		}

		return cubeItems;
	}

	public static string ParseCube(Dictionary<AxisType, int>[] cubeItems)
	{
		string str = string.Empty;
		for (int i = cubeItems.Length; --i >= 0;)
		{
			Dictionary<AxisType, int> cubeItem = cubeItems[i];
			string id = i.ToString();
			string data = string.Empty;
			foreach (KeyValuePair<AxisType, int> dataInfo in cubeItem)
			{
				data += (int)dataInfo.Key + SPLIT_ITEM.ToString() + dataInfo.Value + SPLIT_CUBE.ToString();
			}
			data = data.Remove(data.Length - 1);

			str += id + SPLIT_FIELD + data + SPLIT_DATA;
		}
		str = str.Remove(str.Length - 1);

		return str;
	}
}