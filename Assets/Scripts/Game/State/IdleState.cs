using UnityEngine;
using Framework;

public sealed class IdleState : IState
{
	public CubeController controller;

	private void OnEnable()
	{
		EventSystem<CubeRollEvent>.Add(OnCubeRoll);
	}
	
	private void OnCubeRoll(CubeRollEvent evt)
	{
		if (this != controller.stateMachine.state
			|| Vector3.zero == evt.deltaPosition)
		{
			return;
		}
		
		Vector3 direction = evt.deltaPosition.normalized;
		AxisType rightAxis, upAxis, forwardAxis;
		AxisUtil.GetRollAxis(controller.magicCube.transform, direction, out rightAxis, out upAxis, out forwardAxis);
		
		controller.rollAxis = upAxis;
		
		bool isHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
		if (isHorizontal)
		{
			controller.rollAngle = (direction.x * AxisUtil.Axis2Direction(controller.magicCube.transform, upAxis).y) > 0 ? -90 : 90;
		}
		else
		{
			controller.rollAngle = (direction.y * AxisUtil.Axis2Direction(controller.magicCube.transform, upAxis).x) > 0 ? 90 : -90;
		}
		
		controller.stateMachine.Enter<HitTestState>();

		CubeTestEvent cubeTestEvent = new CubeTestEvent();
		cubeTestEvent.cube = evt.cube;
		cubeTestEvent.rightAxis = rightAxis;
		cubeTestEvent.upAxis = upAxis;
		cubeTestEvent.forwardAxis = forwardAxis;
		EventSystem<CubeTestEvent>.Broadcast(cubeTestEvent);
	}
}