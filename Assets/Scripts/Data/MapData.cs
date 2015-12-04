using UnityEngine;
using Framework;
using System.Collections.Generic;

public sealed class MapData : IData
{
	public Dictionary<string, object> dataDict { get; private set; }

	public int id { get; private set; }
	public string cube { get; private set; }

	public MapData()
	{
		dataDict = new Dictionary<string, object>();
	}

	public void Parse(Dictionary<string, object> arg)
	{
		dataDict = arg;

		id = (int)arg["id"];
		cube = (string)arg["cube"];
	}
}