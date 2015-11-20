using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class CubeItem : MonoBehaviour
{
	public int layer;
	public float size { get { return transform.localScale.x; } }
	public Dictionary<AxisType, ItemType> axisDict { get; private set; }
	public BoxCollider collider { get; private set; }
	public Renderer renderer { get; private set; }
	public Material material { get; private set; }
	public Color color
	{
		get { return material.color; }
		set { material.color = value; }
	}

	private void Awake()
	{
		collider = GetComponent<BoxCollider>();
		renderer = GetComponent<Renderer>();
		material = renderer.material;
		
		color = GetColor();
	}

	public void Init()
	{
		axisDict = new Dictionary<AxisType, ItemType>();

		AxisType[] axisTypes = Enum.GetValues(typeof(AxisType)) as AxisType[];
		ItemType[] itemTypes = Enum.GetValues(typeof(ItemType)) as ItemType[];
		for (int i = axisTypes.Length; --i >= 0;)
		{
			AxisType axis = axisTypes[i];
			Vector3 direction = AxisUtil.Axis2Direction(transform, axis);
			if (Physics.Linecast(transform.position,
			                     transform.position + direction * (collider.size.x * 1.5f),
			                     1 << LayerDefine.CUBE))
			{
				continue;
			}
			
			axisDict[axis] = itemTypes[UnityEngine.Random.Range(0, itemTypes.Length)];
		}
	}

	private Color GetColor()
	{
		return new Color(UnityEngine.Random.Range(0.0f, 1.0f),
		                 UnityEngine.Random.Range(0.0f, 1.0f),
		                 UnityEngine.Random.Range(0.0f, 1.0f),
		                 0);
	}
}