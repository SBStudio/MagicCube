using UnityEngine;
using Framework;
using System;
using System.Collections.Generic;

public sealed class ItemData : IData
{
	public const string DATABASE = "Database.db";
	public const string TABLE = "Item";

	public static readonly Dictionary<string, Type> FIELDS = new Dictionary<string, Type>()
	{
		{ ItemData.FIELD_ID, typeof(int) },
		{ ItemData.FIELD_NAME, typeof(string) },
		{ ItemData.FIELD_COLOR, typeof(string) },
	};

	public const string FIELD_ID = "id";
	public const string FIELD_NAME = "name";
	public const string FIELD_COLOR = "color";

	public Dictionary<string, object> dataDict { get; private set; }

	public int id { get; private set; }
	public string name { get; private set; }
	public Color color { get; private set; }

	public void Parse(Dictionary<string, object> arg)
	{
		dataDict = arg;

		id = (int)arg[FIELD_ID];
		name = (string)arg[FIELD_NAME];
		color = color.Parse((string)arg[FIELD_COLOR]);
	}
}