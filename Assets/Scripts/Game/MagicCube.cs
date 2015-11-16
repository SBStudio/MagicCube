using UnityEngine;
using System.Collections.Generic;
using Framework;

public sealed class MagicCube : MonoBehaviour
{
	public enum Axis
	{
		RIGHT,
		UP,
		FORWARD,
	}

	private struct SelectCube
	{
		public CubeItem cube;
		public Vector3 position;
		public Quaternion rotation;
	}

	public Camera camera;
	public GameObject cubePrefab;
	public int step = 5;
	public float size = 1;
	public float space = 0.05f;
	public float rollError = 0;
	public float rollTime = 0.5f;
	public float viewDistance = 4;
	public float viewLerp = 2;
	public float viewSensitivity = 50;
	public float colorTime = 0.5f;
	
	public int layer { get; private set; }
	public int lastLayer { get; private set; }
	public int maxLayer { get; private set; }
	public bool isRolling { get { return 0 < m_RollStartTime; } }
	
	private Dictionary<int, List<CubeItem>> m_CubeDict = new Dictionary<int, List<CubeItem>>();
	private Dictionary<CubeItem, SelectCube> m_SelectDict = new Dictionary<CubeItem, SelectCube>();
	private BoxCollider m_TriggerCollider;
	private CubeItem m_SelectCube;
	private float m_Distance;
	private Axis m_RollAxis;
	private float m_RollAngle;
	private float m_RollStartTime = int.MinValue;
	private int m_RollInputId = int.MinValue;
	private int m_ViewInputId = int.MinValue;

	private void Awake()
	{
		DebugUtil.Add<FPSInfo>();
		LogUtil.printType = LogUtil.PrintType.Screen;
		EventSystem<InputEvent>.Add(OnInputEvent);
		InputModule inputModule = InputModule.instance;
		Application.targetFrameRate = 60;
		
		GameObject gameObject = new GameObject("Trigger");
		gameObject.transform.parent = transform;
		gameObject.SetActive(false);
		Trigger trigger = gameObject.AddComponent<Trigger>();
		trigger.onTriggerEnter += OnCubeTrigger;
		m_TriggerCollider = gameObject.AddComponent<BoxCollider>();
		m_TriggerCollider.isTrigger = true;
		Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
		rigidBody.isKinematic = true;

		m_Distance = size + space;
		float offset = (1 - step) * 0.5f;
		
		if (null == camera)
		{
			camera = Camera.main;
		}

		maxLayer = (step - 1) / 2;

		int num = step * step * step;
		for (int i = num; --i >= 0;)
		{
			Vector3 grids = new Vector3(i % step + offset,
			                            (i / step) % step + offset,
			                            (i / (step * step)) % step + offset);

			Vector3 position = grids * m_Distance;
			int layer = (int)Mathf.Max(Mathf.Abs(grids.x), Mathf.Abs(grids.y), Mathf.Abs(grids.z));
			
			CubeItem cube = Instantiate(cubePrefab).AddComponent<CubeItem>();
			cube.name = i.ToString();
			cube.gameObject.layer = LayerDefine.CUBE;
			cube.transform.parent = transform;
			cube.transform.localPosition = position;
			cube.transform.localScale = Vector3.one * size;
			cube.grids = grids;
			cube.layer = layer;
			cube.collider.size = Vector3.one * (1 + space);
			cube.renderer.enabled = false;
			cube.color = GetColor();
			
			if (!m_CubeDict.ContainsKey(layer))
			{
				m_CubeDict[layer] = new List<CubeItem>();
			}
			m_CubeDict[layer].Add(cube);
		}

		SetLayer(maxLayer);
	}
	
	private void Update()
	{
		UpdateRoll(Time.deltaTime);
		UpdateCamera(Time.deltaTime);
	}

	private void OnGUI()
	{
		for (int i = 0; i <= maxLayer; ++i)
		{
			if (GUILayout.Button("Layer: " + i, LogUtil.guiStyle))
			{
				SetLayer(i);
			}
		}
	}

	private void UpdateRoll(float deltaTime)
	{
		if (!isRolling)
		{
			return;
		}

		deltaTime = Mathf.Clamp01((Time.time - m_RollStartTime) / rollTime);
		float angle = m_RollAngle * deltaTime;
		
		Vector3 center = Vector3.zero;
		foreach (SelectCube select in m_SelectDict.Values)
		{
			center += select.cube.transform.position;
		}
		center /= m_SelectDict.Count;
		
		foreach (KeyValuePair<CubeItem, SelectCube> select in m_SelectDict)
		{
			select.Key.transform.localPosition = select.Value.position;
			select.Key.transform.localRotation = select.Value.rotation;
			select.Key.transform.RotateAround(center, Axis2Direction(m_RollAxis), angle);
		}
		
		if (1 <= deltaTime)
		{
			StopRoll();
		}
	}

	private void UpdateCamera(float deltaTime)
	{
		Vector3 position = Vector3.back * (layer + 1) * m_Distance * viewDistance;
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
		CubeItem selectCube = null;
		Ray ray = camera.ScreenPointToRay(evt.position);
		RaycastHit[] raycastHits = Physics.RaycastAll(ray,
		                                              Mathf.Abs(camera.transform.position.z),
		                                              1 << LayerDefine.CUBE,
		                                              QueryTriggerInteraction.Collide);
		for (int i = 0; i < raycastHits.Length; ++i)
		{
			RaycastHit raycastHit = raycastHits[i];
			CubeItem cube = raycastHit.collider.GetComponent<CubeItem>();
			if (null != cube
			    && cube.layer == layer)
			{
				selectCube = cube;

				break;
			}
		}

		if (null != selectCube)
		{
			if (int.MinValue == m_RollInputId
			    && !isRolling)
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
			
			transform.Rotate(camera.transform.up, -deltaPosition.x * viewSensitivity, Space.World);
			transform.Rotate(camera.transform.right, deltaPosition.y * viewSensitivity, Space.World);
		}
	}

	private void OnInputEnd(InputEvent evt)
	{
		if (m_RollInputId == evt.inputId)
		{
			m_RollInputId = int.MinValue;

			RollCubes(evt.deltaPosition);
		}
		else if (m_ViewInputId == evt.inputId)
		{
			m_ViewInputId = int.MinValue;
		}
	}

	public void SetLayer(int value)
	{
		value = Mathf.Clamp(value, 0, maxLayer);
		if (value == layer)
		{
			return;
		}

		lastLayer = layer;
		layer = value;

		TimerUtil.Begin(OnColorTimeout, colorTime);

		List<CubeItem> cubeList = m_CubeDict[lastLayer];
		for (int i = cubeList.Count; --i >= 0;)
		{
			CubeItem cube = cubeList[i];
			
			Color color = cube.color;
			color.a = 0;
			iTween.ColorTo(cube.gameObject, color, colorTime);
		}
		
		cubeList = m_CubeDict[layer];
		for (int i = cubeList.Count; --i >= 0;)
		{
			CubeItem cube = cubeList[i];

			cube.renderer.enabled = true;
			Color color = cube.color;
			color.a = 1;
			iTween.ColorTo(cube.gameObject, color, colorTime);
		}
	}

	private void OnColorTimeout()
	{
		List<CubeItem> cubeList = m_CubeDict[lastLayer];
		for (int i = cubeList.Count; --i >= 0;)
		{
			CubeItem cube = cubeList[i];

			cube.renderer.enabled = false;
		}
	}

	public void RollCubes(Vector3 deltaPosition)
	{
		if (isRolling
		    || Vector3.zero == deltaPosition)
		{
			return;
		}
		
		Vector3 direction = deltaPosition.normalized;
		Axis right, up, forward;

		GetRollAxis(direction, out right, out up, out forward);
		SelectCubes(m_SelectCube, right, up, forward);

		m_RollAxis = up;
		
		bool isHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
		if (isHorizontal)
		{
			m_RollAngle = (direction.x * Axis2Direction(up).y) > 0 ? -90 : 90;
		}
		else
		{
			m_RollAngle = (direction.y * Axis2Direction(up).x) > 0 ? 90 : -90;
		}
	}

	public void StopRoll()
	{
		m_RollStartTime = int.MinValue;
		m_SelectDict.Clear();
	}

	public void GetRollAxis(Vector3 direction, out Axis right, out Axis up, out Axis forward)
	{
		Vector3 r = Vector3.zero, u = Vector3.zero, f = Vector3.zero;
		List<Vector3> axisList = new List<Vector3>()
		{
			transform.right,
			transform.up,
			transform.forward
		};
		
		for (int i = axisList.Count; --i >= 0;)
		{
			float project1 = Vector3.Project(direction, r).magnitude;
			float project2 = Vector3.Project(direction, axisList[i]).magnitude;
			r = project1 > project2 ? r : axisList[i];
		}
		axisList.Remove(r);
		
		for (int i = axisList.Count; --i >= 0;)
		{
			float project1 = Vector3.Project(camera.transform.forward, f).magnitude;
			float project2 = Vector3.Project(camera.transform.forward, axisList[i]).magnitude;
			f = project1 > project2 ? f : axisList[i];
		}
		axisList.Remove(f);
		
		u = axisList[0];

		right = Direction2Axis(r);
		up = Direction2Axis(u);
		forward = Direction2Axis(f);
	}

	public void SelectCubes(CubeItem cube, Axis right, Axis up, Axis forward)
	{
		m_TriggerCollider.gameObject.SetActive(false);
		m_TriggerCollider.transform.position = cube.transform.position;
		m_TriggerCollider.transform.forward = Axis2Direction(forward);
		m_TriggerCollider.transform.right = Axis2Direction(right);
		m_TriggerCollider.transform.up = Axis2Direction(up);

		float size = step * m_Distance * 4;
		m_TriggerCollider.size = new Vector3(size, this.size, size);
		m_TriggerCollider.gameObject.SetActive(true);
	}

	private void AddCube(CubeItem cube)
	{
		if (null == cube
		    || m_SelectDict.ContainsKey(cube))
		{
			return;
		}

		SelectCube select = new MagicCube.SelectCube();
		select.cube = cube;
		select.position = cube.transform.localPosition;
		select.rotation = cube.transform.localRotation;

		m_SelectDict[cube] = select;
	}

	private void OnCubeTrigger(Collider collider)
	{
		CubeItem cube = collider.GetComponent<CubeItem>();
		if (null == cube)
		{
			return;
		}

		AddCube(cube);

		int num = step * step;
		if (num <= m_SelectDict.Count)
		{
			m_RollStartTime = Time.time;
			m_TriggerCollider.gameObject.SetActive(false);
		}
	}

	public Vector3 Axis2Direction(Axis axis)
	{
		if (Axis.RIGHT == axis)
		{
			return transform.right;
		}
		else if (Axis.UP == axis)
		{
			return transform.up;
		}

		return transform.forward;
	}

	public Axis Direction2Axis(Vector3 direction)
	{
		if (transform.right == direction)
		{
			return Axis.RIGHT;
		}
		else if (transform.up == direction)
		{
			return Axis.UP;
		}
		
		return Axis.FORWARD;
	}
	
	private Color GetColor()
	{
		return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0);
	}
}