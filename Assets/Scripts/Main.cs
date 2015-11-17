using UnityEngine;
using Framework;

public sealed class Main : MonoBehaviour
{
	public Camera camera;
	public float viewDistance = 4;
	public float viewLerp = 2;
	public float viewSensitivity = 50;

	private MagicCube m_MagicCube;
	private CubeItem m_SelectCube;
	private int m_RollInputId = int.MinValue;
	private int m_ViewInputId = int.MinValue;

	private void Awake()
	{
		DebugUtil.Add<FPSInfo>();
		LogUtil.printType = LogUtil.PrintType.Screen;
		EventSystem<InputEvent>.Add(OnInputEvent);
		InputModule inputModule = InputModule.instance;
		Application.targetFrameRate = GameDefine.FPS;
		
		if (null == camera)
		{
			camera = Camera.main;
		}

		GameObject gameObject = Resources.Load<GameObject>(ResourceDefine.MAGIC_CUBE);
		m_MagicCube = Instantiate(gameObject).GetComponent<MagicCube>();
	}

	private void Start()
	{
	}
	
	private void OnGUI()
	{
		for (int i = 0; i <= m_MagicCube.maxLayer; ++i)
		{
			if (GUILayout.Button("Layer: " + i, LogUtil.guiStyle))
			{
				m_MagicCube.SetLayer(i);
			}
		}
	}

	private void Update()
	{
		OnUpdateCamera(Time.deltaTime);
	}
	
	private void OnUpdateCamera(float deltaTime)
	{
		Vector3 position = Vector3.back * (m_MagicCube.layer + 1) * m_MagicCube.distance * viewDistance;
		position = Vector3.Lerp(camera.transform.position, position, viewLerp * deltaTime);
		
		camera.transform.position = position;
	}

	private void OnInputEvent(InputEvent evt)
	{
		if (evt.inputType == InputEvent.InputType.InputStart)
		{
			OnInputStart(evt);
		}
		else if (evt.inputType == InputEvent.InputType.InputMove)
		{
			OnInputMove(evt);
		}
		else
		{
			OnInputEnd(evt);
		}
	}
	
	private void OnInputStart(InputEvent evt)
	{
		m_MagicCube.enableCollision = true;
		CubeItem selectCube = null;
		Ray ray = camera.ScreenPointToRay(evt.position);
		RaycastHit[] raycastHits = Physics.RaycastAll(ray,
		                                              Mathf.Abs(camera.transform.position.z),
		                                              1 << LayerDefine.CUBE,
		                                              QueryTriggerInteraction.Collide);
		m_MagicCube.enableCollision = false;

		for (int i = 0; i < raycastHits.Length; ++i)
		{
			RaycastHit raycastHit = raycastHits[i];
			CubeItem cube = raycastHit.collider.GetComponent<CubeItem>();
			if (null != cube
			    && cube.layer == m_MagicCube.layer)
			{
				selectCube = cube;
				
				break;
			}
		}
		
		if (null != selectCube)
		{
			if (int.MinValue == m_RollInputId
			    && !m_MagicCube.isRolling)
			{
				m_RollInputId = evt.inputId;
				m_SelectCube = selectCube;
			}
		}
		else
		{
			if (int.MinValue == m_ViewInputId)
			{
				m_ViewInputId = evt.inputId;
			}
		}
	}
	
	private void OnInputMove(InputEvent evt)
	{
		if (m_RollInputId == evt.inputId)
		{
		}
		else if (m_ViewInputId == evt.inputId)
		{
			Vector3 deltaPosition = evt.deltaPosition;
			deltaPosition /= Screen.dpi;
			
			m_MagicCube.transform.Rotate(camera.transform.up, -deltaPosition.x * viewSensitivity, Space.World);
			m_MagicCube.transform.Rotate(camera.transform.right, deltaPosition.y * viewSensitivity, Space.World);
		}
	}
	
	private void OnInputEnd(InputEvent evt)
	{
		if (m_RollInputId == evt.inputId)
		{
			m_RollInputId = int.MinValue;
			
			m_MagicCube.RollCubes(m_SelectCube, evt.deltaPosition);
		}
		else if (m_ViewInputId == evt.inputId)
		{
			m_ViewInputId = int.MinValue;
		}
	}
}
