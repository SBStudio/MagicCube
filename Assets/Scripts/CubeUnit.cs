using UnityEngine;

public sealed class CubeUnit : MonoBehaviour
{
	public Vector3 grids;
	public int layer;
	public Renderer renderer { get; private set; }
	public Material material { get; private set; }
	public Color color
	{
		get { return material.color; }
		set { material.color = value; }
	}

	private void Awake()
	{
		renderer = GetComponent<Renderer>();
		material = renderer.material;
	}
}