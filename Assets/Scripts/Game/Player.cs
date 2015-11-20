using UnityEngine;

public sealed class Player : MonoBehaviour
{
	public CubeItem cube { get; private set; }
	public AxisType rightAxis { get; private set; }
	public AxisType upAxis { get; private set; }
	public AxisType forwardAxis { get; private set; }

	public void SetCube(CubeItem cube, AxisType rightAxis, AxisType upAxis, AxisType forwardAxis)
	{
		this.cube = cube;
		this.rightAxis = rightAxis;
		this.upAxis = upAxis;
		this.forwardAxis = forwardAxis;
	}

	private void LateUpdate()
	{
		transform.forward = AxisUtil.Axis2Direction(cube.transform, forwardAxis);
		transform.right = AxisUtil.Axis2Direction(cube.transform, rightAxis);
		transform.up = AxisUtil.Axis2Direction(cube.transform, upAxis);

		transform.position = cube.transform.position + transform.up * cube.size * 0.5f;
	}
}