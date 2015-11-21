using UnityEngine;
using Framework;

public sealed class GlobalState : IState
{
	public CubeController controller;
	
	private CubeItem m_SelectCube;
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
		for (int i = 0; i <= controller.magicCube.maxLayer; ++i)
		{
			if (GUILayout.Button("Layer: " + i, LogUtil.guiStyle))
			{
				controller.magicCube.layer = i;
			}
		}
	}

	private void OnUpdateCamera(float deltaTime)
	{
		Vector3 position = Vector3.back * (controller.magicCube.layer + 1) * controller.distance * controller.viewDistance;
		position = Vector3.Lerp(controller.camera.transform.position, position, controller.viewLerp * deltaTime);
		
		controller.camera.transform.position = position;
	}

	private void OnInputStart(InputStartEvent evt)
	{
		controller.magicCube.enableCollision = true;
		CubeItem select = null;
		
		Ray ray = controller.camera.ScreenPointToRay(evt.gesture.position);
		RaycastHit[] raycastHits = Physics.RaycastAll(ray,
		                                              Mathf.Abs(controller.camera.transform.position.z),
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
			Vector3 deltaPosition = evt.gesture.deltaPosition;
			deltaPosition /= Screen.dpi;
			
			controller.magicCube.transform.Rotate(controller.camera.transform.up,
			                                      -deltaPosition.x * controller.viewSensitivity,
			                                      Space.World);
			controller.magicCube.transform.Rotate(controller.camera.transform.right,
			                                      deltaPosition.y * controller.viewSensitivity,
			                                      Space.World);
		}
	}
	
	private void OnInputEnd(InputEndEvent evt)
	{
		if (m_RollInputId == evt.gesture.inputId)
		{
			m_RollInputId = int.MinValue;

			CubeRollEvent cubeRollEvent = new CubeRollEvent();
			cubeRollEvent.cube = m_SelectCube;
			cubeRollEvent.deltaPosition = evt.gesture.deltaPosition;
			EventSystem<CubeRollEvent>.Broadcast(cubeRollEvent);
		}
		else if (m_ViewInputId == evt.gesture.inputId)
		{
			m_ViewInputId = int.MinValue;
		}
	}
}