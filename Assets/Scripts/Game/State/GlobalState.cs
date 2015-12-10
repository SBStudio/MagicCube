using UnityEngine;
using Framework;

public sealed class GlobalState : IState
{
	public CubeController controller;
	
	private CubeItem m_SelectCube;
	private Vector2 m_StartPosition;
	private int m_RollInputId = int.MinValue;
	private int m_ViewInputId = int.MinValue;
	
	private void OnEnable()
	{
		EventSystem<InputStartEvent>.Add(OnInputStart);
		EventSystem<InputMoveEvent>.Add(OnInputMove);
		EventSystem<InputEndEvent>.Add(OnInputEnd);
	}

	public override void OnUpdate()
	{
		OnUpdateCamera(Time.deltaTime);
	}
	
	public override void OnGUI()
	{
		GUILayout.Space(50);

		if (GUILayout.Button("Go!", LogUtil.guiStyle))
		{
			if (controller.stateMachine.Get<IdleState>() != controller.stateMachine.state)
			{
				return;
			}
			
			controller.magicCube.enableCollision = true;

			CubeMoveEvent evt = null;
			RaycastHit raycastHit;
			if (Physics.Raycast(controller.player.cube.transform.position,
			                    controller.player.transform.forward,
			                    out raycastHit,
								controller.magicCube.distance,
			                    1 << LayerDefine.CUBE))
			{
				CubeItem cube = raycastHit.collider.GetComponent<CubeItem>();
				if (null != cube
				    && cube.layer == controller.magicCube.layer)
				{
					evt = new CubeMoveEvent();
					evt.cube = cube;
					evt.rightAxis = AxisUtil.Direction2Axis(cube.transform, controller.player.transform.right);
					evt.upAxis = AxisUtil.Direction2Axis(cube.transform, controller.player.transform.up);
					evt.forwardAxis = AxisUtil.Direction2Axis(cube.transform, controller.player.transform.forward);
				}
			}

			if (null == evt)
			{
				CubeItem cube = controller.player.cube;
				evt = new CubeMoveEvent();
				evt.cube = cube;
				evt.rightAxis = AxisUtil.Direction2Axis(cube.transform, controller.player.transform.right);
				evt.upAxis = AxisUtil.Direction2Axis(cube.transform, controller.player.transform.forward);
				evt.forwardAxis = AxisUtil.Direction2Axis(cube.transform, -controller.player.transform.up);
			}
			
			controller.magicCube.enableCollision = false;

			if (null != evt)
			{
				if (!evt.cube.itemDict.ContainsKey(evt.upAxis))
				{
					return;
				}

				if (ItemType.STOP == evt.cube.itemDict[evt.upAxis].id)
				{
					controller.stateMachine.Enter<IdleState>();
				}
				else
				{
					controller.stateMachine.Enter<MoveState>();
					EventSystem<CubeMoveEvent>.Broadcast(evt);
				}
			}
		}
	}

	private void OnUpdateCamera(float deltaTime)
	{
		float distance = (controller.magicCube.layer + 1) * controller.magicCube.distance * controller.viewDistance;
		Vector3 position = -controller.camera.transform.forward * distance;
		position = Vector3.Lerp(controller.camera.transform.position, position, controller.viewLerp * deltaTime);
		
		controller.camera.transform.position = position;
	}

	private void OnInputStart(InputStartEvent evt)
	{
		controller.magicCube.enableCollision = true;
		CubeItem select = null;
		
		float distance = (controller.magicCube.layer + 1) * controller.magicCube.distance * controller.viewDistance;
		Ray ray = controller.camera.ScreenPointToRay(evt.gesture.position);
		RaycastHit[] raycastHits = Physics.RaycastAll(ray,
		                                              distance,
		                                              1 << LayerDefine.CUBE,
		                                              QueryTriggerInteraction.Collide);

		controller.magicCube.enableCollision = false;
		
		for (int i = 0; i < raycastHits.Length; ++i)
		{
			RaycastHit raycastHit = raycastHits[i];
			CubeItem cube = raycastHit.collider.GetComponent<CubeItem>();
			if (null == cube
			    || cube.layer != controller.magicCube.layer)
			{
				continue;
			}
			
			if (null == select)
			{
				select = cube;
			}
			else if (Vector3.Distance(ray.origin, select.transform.position)
			         > Vector3.Distance(ray.origin, cube.transform.position))
			{
				select = cube;
			}
		}
		
		if (null != select)
		{
			if (int.MinValue == m_RollInputId)
			{
				m_RollInputId = evt.gesture.inputId;
				m_StartPosition = evt.gesture.position;
				m_SelectCube = select;
			}
		}
		else
		{
			if (int.MinValue == m_ViewInputId)
			{
				m_ViewInputId = evt.gesture.inputId;
			}
		}
	}
	
	private void OnInputMove(InputMoveEvent evt)
	{
		if (m_RollInputId == evt.gesture.inputId)
		{
		}
		else if (m_ViewInputId == evt.gesture.inputId)
		{
			if (controller.stateMachine.Get<TestState>() == controller.stateMachine.state)
			{
				return;
			}

			Vector3 deltaPosition = evt.gesture.deltaPosition;
			deltaPosition /= Screen.dpi;
			
			controller.camera.transform.RotateAround(controller.magicCube.transform.position,
			                                         controller.camera.transform.up,
			                                         deltaPosition.x * controller.viewSensitivity);
			controller.camera.transform.RotateAround(controller.magicCube.transform.position,
			                                         controller.camera.transform.right,
			                                         -deltaPosition.y * controller.viewSensitivity);
		}
	}
	
	private void OnInputEnd(InputEndEvent evt)
	{
		if (m_RollInputId == evt.gesture.inputId)
		{
			m_RollInputId = int.MinValue;

			CubeRollEvent cubeRollEvent = new CubeRollEvent();
			cubeRollEvent.cube = m_SelectCube;
			cubeRollEvent.deltaPosition = evt.gesture.position - m_StartPosition;
			EventSystem<CubeRollEvent>.Broadcast(cubeRollEvent);
		}
		else if (m_ViewInputId == evt.gesture.inputId)
		{
			m_ViewInputId = int.MinValue;
		}
	}
}