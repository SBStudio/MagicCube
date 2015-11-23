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

		Vector3 forward = AxisUtil.Axis2Direction(cube.transform, forwardAxis);
		Vector3 right = AxisUtil.Axis2Direction(cube.transform, rightAxis);
		Vector3 up = AxisUtil.Axis2Direction(cube.transform, upAxis);

		transform.SetParent(cube.transform);
		transform.rotation = Quaternion.LookRotation(forward, up);
		transform.position = cube.transform.position + transform.up * cube.size * 0.5f;
	}
}