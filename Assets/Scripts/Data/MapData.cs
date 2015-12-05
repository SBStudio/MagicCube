using UnityEngine;
using Framework;
using System.Collections.Generic;

public sealed class MapData : IData
{
	public const string FIELD_ID = "id";
	public const string FIELD_STEP = "step";
	public const string FIELD_SIZE = "size";
	public const string FIELD_SPACE = "space";
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
	public Dictionary<AxisType, ItemType>[] cubeItems { get; private set; }

	public MapData()
	{
		dataDict = new Dictionary<string, object>();
	}

	public void Parse(Dictionary<string, object> arg)
	{
		dataDict = arg;

		id = (int)arg[FIELD_ID];
		step = (int)arg[FIELD_STEP];
		size = (float)arg[FIELD_SIZE];
		space = (float)arg[FIELD_SPACE];
		cubeItems = ParseCube((string)arg[FIELD_CUBE]);
	}

	public static Dictionary<AxisType, ItemType>[] ParseCube(string value)
	{
		string[] datas = value.Split(SPLIT_DATA);
		Dictionary<AxisType, ItemType>[] cubeItems = new Dictionary<AxisType, ItemType>[datas.Length];
		for (int i = datas.Length; --i >= 0;)
		{
			string[] mapDatas = datas[i].Split(SPLIT_FIELD);
			int id = int.Parse(mapDatas[0]);
			string mapData = mapDatas[1];

			Dictionary<AxisType, ItemType> itemDict = new Dictionary<AxisType, ItemType>();
			cubeItems[id] = itemDict;

			string[] cubeDatas = mapData.Split(SPLIT_CUBE);
			for (int j = cubeDatas.Length; --j >= 0;)
			{
				string[] itemDatas = cubeDatas[j].Split(SPLIT_ITEM);
				AxisType axisType = (AxisType)int.Parse(itemDatas[0]);
				ItemType itemType = (ItemType)int.Parse(itemDatas[1]);
				itemDict[axisType] = itemType;
			}
		}

		return cubeItems;
	}

	public static string ParseCube(Dictionary<AxisType, ItemType>[] cubeItems)
	{
		string str = string.Empty;
		for (int i = cubeItems.Length; --i >= 0;)
		{
			Dictionary<AxisType, ItemType> cubeItem = cubeItems[i];
			string id = i.ToString();
			string data = string.Empty;
			foreach (KeyValuePair<AxisType, ItemType> dataInfo in cubeItem)
			{
				data += (int)dataInfo.Key + SPLIT_ITEM.ToString() + (int)dataInfo.Value + SPLIT_CUBE.ToString();
			}
			data = data.Remove(data.Length - 1);

			str += id + SPLIT_FIELD + data + SPLIT_DATA;
		}
		str = str.Remove(str.Length - 1);

		return str;
	}
}