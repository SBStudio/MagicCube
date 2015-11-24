using UnityEngine;
using Framework;

public sealed class MoveState : IState
{
	public CubeController controller;

	private CubeMoveEvent m_MoveEvent;
	private Vector3 m_StartPosition;
	private Quaternion m_StartRotation;
	private Vector3 m_EndPosition;
	private Quaternion m_EndRotation;
	private float m_StartTime;

	private void OnEnable()
	{
		EventSystem<CubeMoveEvent>.Add(OnCubeMove);
	}
	
	public override void OnEnter()
	{
		m_StartPosition = controller.player.transform.position;
		m_StartRotation = controller.player.transform.rotation;
		m_StartTime = Time.time;
	}
	
	public override void OnUpdate()
	{
		OnUpdateMove(Time.deltaTime);
	}
	
	private void OnUpdateMove(float deltaTime)
	{
		float progress = Mathf.Clamp01((Time.time - m_StartTime) / controller.moveTime);
		
		controller.player.transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, progress);
		controller.player.transform.rotation = Quaternion.Lerp(m_StartRotation, m_EndRotation, progress);

		if (1 <= progress)
		{
			controller.player.SetCube(m_MoveEvent.cube, m_MoveEvent.rightAxis, m_MoveEvent.upAxis, m_MoveEvent.forwardAxis);
			controller.stateMachine.Enter<ItemState>();
		}
	}
	
	private void OnCubeMove(CubeMoveEvent evt)
	{
		this.m_MoveEvent = evt;

		Vector3 right = AxisUtil.Axis2Direction(evt.cube.transform, evt.rightAxis);
		Vector3 up = AxisUtil.Axis2Direction(evt.cube.transform, evt.upAxis);
		Vector3 forward = AxisUtil.Axis2Direction(evt.cube.transform, evt.forwardAxis);

		m_EndRotation = Quaternion.LookRotation(forward, up);
		m_EndPosition = evt.cube.transform.position + m_EndRotation * Vector3.up * evt.cube.size * 0.5f;
	}
}