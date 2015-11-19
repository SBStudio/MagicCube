using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class CubeItem : MonoBehaviour
{
	public int layer;
	public float size { get { return transform.localScale.x; } }
	public List<CubeAxis.Axis> axisList { get; private set; }
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
		axisList = new List<CubeAxis.Axis>();

		CubeAxis.Axis[] axiss = Enum.GetValues(typeof(CubeAxis.Axis)) as CubeAxis.Axis[];
		for (int i = axiss.Length; --i >= 0;)
		{
			CubeAxis.Axis axis = axiss[i];
			Vector3 direction = CubeAxis.Axis2Direction(transform, axis);
			if (Physics.Linecast(transform.position,
			                     transform.position + direction * (collider.size.x * 1.5f),
			                     1 << LayerDefine.CUBE))
			{
				continue;
			}
			
			axisList.Add(axis);
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