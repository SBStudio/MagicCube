using UnityEngine;
using Framework;

public sealed class TestState : IState
{
	public CubeController controller;

	private void OnEnable()
	{
		EventSystem<CubeTestEvent>.Add(OnCubeTest);
	}

	public override void OnEnter()
	{
		controller.trigger.onTriggerEnter += OnCubeTrigger;
	}
	
	private void OnCubeTest(CubeTestEvent evt)
	{
		controller.triggerCollider.gameObject.SetActive(false);
		controller.magicCube.enableCollision = true;
		
		Vector3 forward = AxisUtil.Axis2Direction(controller.magicCube.transform, evt.forwardAxis);
		Vector3 right = AxisUtil.Axis2Direction(controller.magicCube.transform, evt.rightAxis);
		Vector3 up = AxisUtil.Axis2Direction(controller.magicCube.transform, evt.upAxis);
		
		controller.triggerCollider.transform.rotation = Quaternion.LookRotation(forward, up);
		controller.triggerCollider.transform.position = evt.cube.transform.position;
		
		float size = controller.step * controller.distance * 4;
		controller.triggerCollider.size = new Vector3(size, controller.distance * 0.5f, size);
		controller.triggerCollider.gameObject.SetActive(true);
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
		
		int num = controller.step * controller.step;
		if (num <= controller.selectDict.Count)
		{
			controller.trigger.gameObject.SetActive(false);
			controller.magicCube.enableCollision = false;
			controller.stateMachine.Enter<RollState>();
		}
	}
}