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

	public Camera camera;
	public GameObject cubePrefab;
	public int step;
	public int layer;
	public float size;
	public float space;
	public float rollError = 0;
	public float rollTime = 1;
	public float viewDistance = 4;
	public float viewLerp = 10;
	public float viewSensitivity = 50;
	
	public int maxLayer { get; private set; }
	public bool isRolling { get { return 0 < m_RollStartTime; } }
	
	private Dictionary<int, List<CubeItem>> m_CubeDict = new Dictionary<int, List<CubeItem>>();
	private float m_Distance;
	private CubeItem m_SelectCube;
	private CubeItem[] m_RollCubes;
	private Vector3[] m_RollPositions;
	private Vector3[] m_RollEulerAngles;
	private Axis m_RollAxis;
	private float m_RollAngle;
	private float m_RollStartTime = int.MinValue;
	private float m_ColorTime = int.MinValue;
	private int m_RollInputId = int.MinValue;
	private int m_ViewInputId = int.MinValue;

	private void Awake()
	{
		DebugUtil.Add<FPSInfo>();
		LogUtil.printType = LogUtil.PrintType.Screen;
		EventSystem<InputEvent>.Add(OnInputEvent);
		InputModule inputModule = InputModule.instance;
		Application.targetFrameRate = 60;

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
		UpdateColor(Time.deltaTime);
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
		for (int i = m_RollCubes.Length; --i >= 0;)
		{
			CubeItem cube = m_RollCubes[i];
			
			center += cube.transform.position;
		}
		center /= m_RollCubes.Length;
		
		for (int i = m_RollCubes.Length; --i >= 0;)
		{
			CubeItem cube = m_RollCubes[i];
			
			cube.transform.localPosition = m_RollPositions[i];
			cube.transform.localEulerAngles = m_RollEulerAngles[i];
			cube.transform.RotateAround(center, Axis2Direction(m_RollAxis), angle);
		}
		
		if (1 <= deltaTime)
		{
			m_RollStartTime = int.MinValue;
			m_SelectCube = null;
			m_RollCubes = null;
		}
	}

	private void UpdateColor(float deltaTime)
	{
		if (0 >= m_ColorTime)
		{
			return;
		}

		if (Time.time >= m_ColorTime)
		{
			m_ColorTime = int.MinValue;
			
			for (int i = maxLayer + 1; --i > layer;)
			{
				foreach (CubeItem cube in m_CubeDict[i])
				{
					cube.renderer.enabled = false;
				}
			}
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
		if (value == this.layer)
		{
			return;
		}
		
		this.layer = value;

		m_ColorTime = 1 / viewLerp;
		foreach (int layer in m_CubeDict.Keys)
		{
			List<CubeItem> cubeList = m_CubeDict[layer];
			bool enable = layer >= value;

			for (int i = cubeList.Count; --i >= 0;)
			{
				CubeItem cube = cubeList[i];

				cube.renderer.enabled = enable;
				if (enable)
				{
					Color color = cube.color;
					color.a = layer == value ? 1 : 0;
					iTween.ColorTo(cube.gameObject, color, m_ColorTime);
				}
			}
		}
		m_ColorTime += Time.time;
	}

	public void RollCubes(Vector3 deltaPosition)
	{
		if (isRolling
		    || Vector3.zero == deltaPosition)
		{
			return;
		}
		
		List<CubeItem> cubeList = new List<CubeItem>();
		Vector3 direction = deltaPosition.normalized;
		Axis right, up, forward;

		GetAxis(direction, out right, out up, out forward);
		SelectCubes(m_SelectCube, cubeList, forward, right);

		m_RollAxis = up;
		m_RollCubes = cubeList.ToArray();
		m_RollStartTime = Time.time;
		
		bool isHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
		if (isHorizontal)
		{
			m_RollAngle = (direction.x * Axis2Direction(up).y) > 0 ? -90 : 90;
		}
		else
		{
			m_RollAngle = (direction.y * Axis2Direction(up).x) > 0 ? 90 : -90;
		}
		
		m_RollPositions = new Vector3[m_RollCubes.Length];
		m_RollEulerAngles = new Vector3[m_RollCubes.Length];
		for (int i = m_RollCubes.Length; --i >= 0;)
		{
			CubeItem cube = m_RollCubes[i];
			
			m_RollPositions[i] = cube.transform.localPosition;
			m_RollEulerAngles[i] = cube.transform.localEulerAngles;
		}
	}

	public void GetAxis(Vector3 direction, out Axis right, out Axis up, out Axis forward)
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

	public void SelectCubes(CubeItem cube, List<CubeItem> cubeList, Axis forward, Axis right)
	{
		if (cubeList.Contains(cube))
		{
			return;
		}

		cubeList.Add(cube);

		Vector3[] directions = new Vector3[]
		{
			Axis2Direction(forward),
			-Axis2Direction(forward),
			Axis2Direction(right),
			-Axis2Direction(right)
		};

		float distance = m_Distance * step;

		for (int i = directions.Length; --i >= 0;)
		{
			Vector3 direction = directions[i];
			Vector3 position = cube.transform.position + direction * m_Distance;

			RaycastHit[] raycastHits = Physics.RaycastAll(cube.transform.position,
			                                              direction,
			                                              distance,
			                                              1 << LayerDefine.CUBE,
			                                              QueryTriggerInteraction.Collide);
			for (int j = raycastHits.Length; --j >= 0;)
			{
				RaycastHit raycastHit = raycastHits[j];
				CubeItem nextCube = raycastHit.collider.GetComponent<CubeItem>();
				if (null == nextCube
				    || cubeList.Contains(nextCube))
				{
					continue;
				}
				
				SelectCubes(nextCube, cubeList, forward, right);
			}
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
		return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
}