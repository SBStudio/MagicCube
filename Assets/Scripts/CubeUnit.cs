using UnityEngine;

public sealed class CubeUnit : MonoBehaviour
{
	public Vector3 grids;
	public int layer;
	public Material material;
	public Color color
	{
		get { return material.color; }
		set { material.color = value; }
	}

	private void Awake()
	{
		material = GetComponent<Renderer>().material;
	}
}