using UnityEngine;

public sealed class Player : MonoBehaviour
{
	public CubeItem cube { get; private set; }
	public CubeAxis.Axis axis { get; private set; }

	public void SetCube(CubeItem cube, CubeAxis.Axis axis)
	{
		this.cube = cube;
		this.axis = axis;

		transform.parent = cube.transform;
	}

	private void LateUpdate()
	{
		transform.up = CubeAxis.Axis2Direction(cube.transform, axis);
		transform.position = cube.transform.position + transform.up * cube.size * 0.5f;
	}
}