using UnityEngine;
using Framework;
using System.Collections.Generic;

public sealed class RollState : IState
{
	public CubeController controller;

	private float m_StartTime;

	public override void OnEnter ()
	{
		m_StartTime = Time.time;
	}

	public override void OnUpdate ()
	{
		OnUpdateRoll(Time.deltaTime);
	}
	
	public override void OnExit ()
	{
		controller.selectDict.Clear();
	}
	
	private void OnUpdateRoll(float deltaTime)
	{
		float progress = Mathf.Clamp01((Time.time - m_StartTime) / controller.rollTime);
		float angle = controller.rollAngle * progress;
		
		Vector3 center = Vector3.zero;
		foreach (CubeController.SelectCube select in controller.selectDict.Values)
		{
			center += select.cube.transform.position;
		}
		center /= controller.selectDict.Count;
		
		foreach (KeyValuePair<CubeItem, CubeController.SelectCube> select in controller.selectDict)
		{
			select.Key.transform.localPosition = select.Value.position;
			select.Key.transform.localRotation = select.Value.rotation;
			select.Key.transform.RotateAround(center, AxisUtil.Axis2Direction(controller.magicCube.transform, controller.rollAxis), angle);
		}
		
		if (1 <= progress)
		{
			controller.stateMachine.Enter<IdleState>();
		}
	}
}