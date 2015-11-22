using UnityEngine;
using Framework;

public sealed class IdleState : IState
{
	public CubeController controller;

	private void OnEnable()
	{
		EventSystem<CubeRollEvent>.Add(OnCubeRoll);
		EventSystem<CubeMoveEvent>.Add(OnCubeMove);
	}
	
	private void OnCubeRoll(CubeRollEvent evt)
	{
		if (this != controller.stateMachine.state
			|| Vector3.zero == evt.deltaPosition)
		{
			return;
		}
		
		Vector3 direction = controller.camera.transform.right * evt.deltaPosition.x + controller.camera.transform.up * evt.deltaPosition.y;
		Vector3 forward = controller.camera.transform.forward;
		AxisType rightAxis, upAxis, forwardAxis;
		AxisUtil.GetRollAxis(controller.magicCube.transform, direction, forward, out rightAxis, out upAxis, out forwardAxis);
		
		controller.rollAxis = upAxis;
		
		bool isHorizontal = Mathf.Abs(evt.deltaPosition.x) > Mathf.Abs(evt.deltaPosition.y);
		if (isHorizontal)
		{
			float dot = Vector3.Dot(controller.camera.transform.up, AxisUtil.Axis2Direction(controller.transform, upAxis));
			controller.rollAngle = evt.deltaPosition.x * dot > 0 ? -90 : 90;
		}
		else
		{
			float dot = Vector3.Dot(controller.camera.transform.right, AxisUtil.Axis2Direction(controller.transform, upAxis));
			controller.rollAngle = evt.deltaPosition.y * dot > 0 ? 90 : -90;
		}
		
		controller.stateMachine.Enter<TestState>();

		CubeTestEvent cubeTestEvent = new CubeTestEvent();
		cubeTestEvent.cube = evt.cube;
		cubeTestEvent.rightAxis = rightAxis;
		cubeTestEvent.upAxis = upAxis;
		cubeTestEvent.forwardAxis = forwardAxis;
		EventSystem<CubeTestEvent>.Broadcast(cubeTestEvent);
	}

	private void OnCubeMove(CubeMoveEvent evt)
	{
		if (this != controller.stateMachine.state)
		{
			return;
		}

		controller.stateMachine.Enter<MoveState>();
	}
}