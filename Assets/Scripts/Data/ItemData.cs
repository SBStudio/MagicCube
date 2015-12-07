using UnityEngine;
using Framework;
using System.Collections.Generic;

public sealed class ItemData : IData
{
	public const string FIELD_ID = "id";
	public const string FIELD_TYPE = "type";
	public const string FIELD_COLOR = "color";

	public Dictionary<string, object> dataDict { get; private set; }
	public int id { get; private set; }
	public ItemType type { get; private set; }
	public Color color { get; private set; }

	public void Parse(Dictionary<string, object> arg)
	{
		dataDict = arg;

		id = (int)arg[FIELD_ID];
		type = (ItemType)arg[FIELD_TYPE];
		color.Parse((string)arg[FIELD_COLOR]);
	}
}