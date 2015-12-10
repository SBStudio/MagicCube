using UnityEngine;
using Framework;

public sealed class ItemState : IState
{
	public CubeController controller;

	private CubeItem m_Cube;
	private Vector3 m_StartPosition;
	private Quaternion m_StartRotation;
	private Vector3 m_EndPosition;
	private Quaternion m_EndRotation;
	private float m_StartTime;
	
	public override void OnEnter()
	{
		m_Cube = controller.player.cube;
		m_StartPosition = controller.player.transform.position;
		m_StartRotation = controller.player.transform.rotation;
		m_EndPosition = m_StartPosition;
		m_EndRotation = m_StartRotation;
		m_StartTime = Time.time;

		UseItem(m_Cube.itemDict[controller.player.upAxis]);
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
			AxisType rightAxis = AxisUtil.Direction2Axis(m_Cube.transform, controller.player.transform.right);
			AxisType upAxis = AxisUtil.Direction2Axis(m_Cube.transform, controller.player.transform.up);
			AxisType forwardAxis = AxisUtil.Direction2Axis(m_Cube.transform, controller.player.transform.forward);
			controller.player.SetCube(m_Cube, rightAxis, upAxis, forwardAxis);

			controller.stateMachine.Enter<IdleState>();
		}
	}

	private void UseItem(ItemData itemData)
	{
		if (ItemType.FORWARD == itemData.id)
		{
		}
		else if (ItemType.TURN_LEFT == itemData.id)
		{
			controller.player.transform.Rotate(Vector3.up, -90);
			m_EndRotation = controller.player.transform.rotation;
			controller.player.transform.rotation = m_StartRotation;
		}
		else if (ItemType.TURN_RIGHT == itemData.id)
		{
			controller.player.transform.Rotate(Vector3.up, 90);
			m_EndRotation = controller.player.transform.rotation;
			controller.player.transform.rotation = m_StartRotation;
		}
		else if (ItemType.TURN_BACK == itemData.id)
		{
			controller.player.transform.Rotate(Vector3.up, 180);
			m_EndRotation = controller.player.transform.rotation;
			controller.player.transform.rotation = m_StartRotation;
		}
		else if (ItemType.TURN_UP == itemData.id)
		{
			controller.magicCube.enableCollision = true;

			RaycastHit raycastHit;
			if (Physics.Raycast(controller.player.cube.transform.position,
				controller.player.transform.up,
				out raycastHit,
				controller.magicCube.distance,
				1 << LayerDefine.CUBE))
			{
				CubeItem cube = raycastHit.collider.GetComponent<CubeItem>();
				if (null != cube)
				{
					m_Cube = cube;
					m_EndPosition = cube.transform.position + controller.player.transform.up * cube.size * 0.5f;
					++controller.magicCube.layer;
				}
				else
				{
					controller.stateMachine.Enter<IdleState>();
				}
			}

			controller.magicCube.enableCollision = false;
		}
		else if (ItemType.TURN_DOWN == itemData.id)
		{
			controller.magicCube.enableCollision = true;
			
			RaycastHit raycastHit;
			if (Physics.Raycast(controller.player.cube.transform.position,
			                    -controller.player.transform.up,
			                    out raycastHit,
								controller.magicCube.distance,
			                    1 << LayerDefine.CUBE))
			{
				CubeItem cube = raycastHit.collider.GetComponent<CubeItem>();
				if (null != cube)
				{
					m_Cube = cube;
					m_EndPosition = cube.transform.position + controller.player.transform.up * cube.size * 0.5f;
					--controller.magicCube.layer;
				}
				else
				{
					controller.stateMachine.Enter<IdleState>();
				}
			}

			controller.magicCube.enableCollision = false;
		}
	}
}