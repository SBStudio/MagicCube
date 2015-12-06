using UnityEngine;
using Framework;

public sealed class TestState : IState
{
	public CubeController controller;

	private TimerBehaviour m_TestTimer;

	private void OnEnable()
	{
		EventSystem<CubeTestEvent>.Add(OnCubeTest);
	}

	public override void OnEnter()
	{
		controller.cubeTrigger.onTriggerEnter += OnCubeTrigger;

		m_TestTimer = TimerUtil.Begin(OnTestTiemr, controller.testTime);
	}

	public override void OnExit ()
	{
		if (null != m_TestTimer)
		{
			m_TestTimer.Stop();
		}
	}
	
	private void OnCubeTest(CubeTestEvent evt)
	{
		controller.cubeCollider.gameObject.SetActive(false);
		controller.magicCube.enableCollision = true;
		
		Vector3 forward = AxisUtil.Axis2Direction(controller.magicCube.transform, evt.forwardAxis);
		Vector3 right = AxisUtil.Axis2Direction(controller.magicCube.transform, evt.rightAxis);
		Vector3 up = AxisUtil.Axis2Direction(controller.magicCube.transform, evt.upAxis);
		
		controller.cubeCollider.transform.rotation = Quaternion.LookRotation(forward, up);
		controller.cubeCollider.transform.position = evt.cube.transform.position;
		
		float size = controller.magicCube.step * controller.magicCube.distance * 4;
		controller.cubeCollider.size = new Vector3(size, controller.magicCube.distance * 0.5f, size);
		controller.cubeCollider.gameObject.SetActive(true);
	}
	
	private void OnCubeTrigger(Collider collider)
	{
		if (this != controller.stateMachine.state)
		{
			return;
		}

		CubeItem cube = collider.GetComponent<CubeItem>();
		if (null == cube)
		{
			return;
		}
		
		AddCube(cube);
	}
	
	private void AddCube(CubeItem cube)
	{
		if (null == cube
		    || controller.selectDict.ContainsKey(cube))
		{
			return;
		}
		
		CubeController.SelectCube select = new CubeController.SelectCube();
		select.cube = cube;
		select.position = cube.transform.localPosition;
		select.rotation = cube.transform.localRotation;
		
		controller.selectDict[cube] = select;
		
		int num = controller.magicCube.step * controller.magicCube.step;
		if (num <= controller.selectDict.Count)
		{
			controller.cubeTrigger.gameObject.SetActive(false);
			controller.magicCube.enableCollision = false;
			controller.stateMachine.Enter<RollState>();
		}
	}

	private void OnTestTiemr()
	{
		controller.stateMachine.Enter<IdleState>();
	}
}