using UnityEngine;

public sealed class Player : MonoBehaviour
{
	public CubeItem cube { get; private set; }

	public void SetCube(CubeItem cube)
	{
		this.cube = cube;
	}

	private void LateUpdate()
	{
		transform.position = cube.transform.position;
		transform.rotation = cube.transform.rotation;
	}
}