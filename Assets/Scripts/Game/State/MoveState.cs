using UnityEngine;
using Framework;

public sealed class MoveState : IState
{
	public CubeController controller;

	private Vector3 m_StartPosition;
	private float m_StartTime;

	private void OnEnable()
	{
		EventSystem<CubeMoveEvent>.Add(OnCubeMove);
	}
	
	public override void OnEnter()
	{
		m_StartPosition = controller.player.transform.position;
		m_StartTime = Time.time;
	}
	
	public override void OnUpdate()
	{
		OnUpdateMove(Time.deltaTime);
	}
	
	private void OnUpdateMove(float deltaTime)
	{
		float progress = Mathf.Clamp01((Time.time - m_StartTime) / controller.moveTime);

		if (1 <= progress)
		{
			controller.stateMachine.Enter<IdleState>();
		}
	}
	
	private void OnCubeMove(CubeMoveEvent evt)
	{
		controller.player.SetCube(evt.cube, evt.rightAxis, evt.upAxis, evt.forwardAxis);
	}
}