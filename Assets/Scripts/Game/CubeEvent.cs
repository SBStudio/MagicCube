using UnityEngine;

public sealed class CubeRollEvent
{
	public CubeItem cube;
	public Vector3 deltaPosition;
}

public sealed class CubeTestEvent
{
	public CubeItem cube;
	public AxisType rightAxis;
	public AxisType upAxis;
	public AxisType forwardAxis;
}

public sealed class CubeMoveEvent
{

}