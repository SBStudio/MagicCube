using UnityEngine;
using Framework;

public sealed class ItemState : IState
{
	public CubeController controller;

	private Vector3 m_StartPosition;
	private Quaternion m_StartRotation;
	private Vector3 m_EndPosition;
	private Quaternion m_EndRotation;
	private float m_StartTime;
	
	public override void OnEnter()
	{
		m_StartPosition = controller.player.transform.position;
		m_StartRotation = controller.player.transform.rotation;
		m_EndPosition = m_StartPosition;
		m_EndRotation = m_StartRotation;
		m_StartTime = Time.time;

		UseItem(controller.player.cube.itemDict[controller.player.upAxis]);
	}

	public override void OnUpdate()
	{
		OnUpdateItem(Time.deltaTime);
	}
	
	private void OnUpdateItem(float deltaTime)
	{
		float progress = Mathf.Clamp01((Time.time - m_StartTime) / controller.moveTime);
		
		controller.player.transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, progress);
		controller.player.transform.rotation = Quaternion.Lerp(m_StartRotation, m_EndRotation, progress);
		
		if (1 <= progress)
		{
			AxisType rightAxis = AxisUtil.Direction2Axis(controller.player.cube.transform, controller.player.transform.right);
			AxisType upAxis = AxisUtil.Direction2Axis(controller.player.cube.transform, controller.player.transform.up);
			AxisType forwardAxis = AxisUtil.Direction2Axis(controller.player.cube.transform, controller.player.transform.forward);
			controller.player.SetCube(controller.player.cube, rightAxis, upAxis, forwardAxis);

			controller.stateMachine.Enter<IdleState>();
		}
	}

	private void UseItem(ItemType itemType)
	{
		if (ItemType.NONE == itemType)
		{
		}
		else if (ItemType.TURN_LEFT == itemType)
		{
			controller.player.transform.Rotate(Vector3.up, -90);
			m_EndRotation = controller.player.transform.rotation;
			controller.player.transform.rotation = m_StartRotation;
		}
		else if (ItemType.TURN_RIGHT == itemType)
		{
			controller.player.transform.Rotate(Vector3.up, 90);
			m_EndRotation = controller.player.transform.rotation;
			controller.player.transform.rotation = m_StartRotation;
		}
		else if (ItemType.TURN_BACK == itemType)
		{
			controller.player.transform.Rotate(Vector3.up, 180);
			m_EndRotation = controller.player.transform.rotation;
			controller.player.transform.rotation = m_StartRotation;
		}
		else if (ItemType.TURN_DOWN == itemType)
		{
		}
	}
}