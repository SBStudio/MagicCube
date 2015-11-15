using UnityEngine;
using System.Collections.Generic;
using Framework;

public sealed class MagicCube : MonoBehaviour
{
	public enum RollAxis
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
	
	private Dictionary<int, List<CubeUnit>> m_CubeDict = new Dictionary<int, List<CubeUnit>>();
	private float m_Distance;
	private CubeUnit m_SelectCube;
	private CubeUnit[] m_RollCubes;
	private Vector3[] m_RollPositions;
	private Vector3[] m_RollEulerAngles;
	private RollAxis m_RollAxis;
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
			
			CubeUnit cubeUnit = Instantiate(cubePrefab).AddComponent<CubeUnit>();
			cubeUnit.name = i.ToString();
			cubeUnit.gameObject.layer = LayerDefine.CUBE;
			cubeUnit.transform.parent = transform;
			cubeUnit.transform.localPosition = position;
			cubeUnit.transform.localScale = Vector3.one * size;
			cubeUnit.grids = grids;
			cubeUnit.layer = layer;
			cubeUnit.collider.size = Vector3.one * (1 + space);
			cubeUnit.color = GetColor();
			
			if (!m_CubeDict.ContainsKey(layer))
			{
				m_CubeDict[layer] = new List<CubeUnit>();
			}
			m_CubeDict[layer].Add(cubeUnit);
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
			CubeUnit cube = m_RollCubes[i];
			
			center += cube.transform.position;
		}
		center /= m_RollCubes.Length;
		
		for (int i = m_RollCubes.Length; --i >= 0;)
		{
			CubeUnit cube = m_RollCubes[i];
			
			cube.transform.localPosition = m_RollPositions[i];
			cube.transform.localEulerAngles = m_RollEulerAngles[i];
			cube.transform.RotateAround(center, ParseAxis(m_RollAxis), angle);
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
				foreach (CubeUnit cube in m_CubeDict[i])
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
		if (int.MinValue == m_RollInputId
		    && !isRolling)
		{
			Ray ray = camera.ScreenPointToRay(evt.position);
			RaycastHit[] raycastHits = Physics.RaycastAll(ray,
			                                              Mathf.Abs(camera.transform.position.z),
			                                              1 << LayerDefine.CUBE,
			                                              QueryTriggerInteraction.Collide);
			for (int i = 0; i < raycastHits.Length; ++i)
			{
				RaycastHit raycastHit = raycastHits[i];
				CubeUnit cube = raycastHit.collider.GetComponent<CubeUnit>();
				if (null == cube
				    || cube.layer != layer)
				{
					continue;
				}

				m_RollInputId = evt.inputId;
				m_SelectCube = cube;
				
#if UNITY_EDITOR
				Debug.DrawLine(camera.transform.position, camera.transform.position + ray.direction * Mathf.Abs(camera.transform.position.z), Color.green, 3, false);
#endif

				return;
			}
		}

		if (int.MinValue == m_ViewInputId)
		{
			m_ViewInputId = evt.inputId;
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
			
#if UNITY_EDITOR
			Ray ray = camera.ScreenPointToRay(evt.position);
			Debug.DrawLine(camera.transform.position, camera.transform.position + ray.direction * Mathf.Abs(camera.transform.position.z), Color.blue, 3, false);
#endif

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
			List<CubeUnit> cubeList = m_CubeDict[layer];
			bool enable = layer >= value;

			for (int i = cubeList.Count; --i >= 0;)
			{
				CubeUnit cube = cubeList[i];

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

	private void RollCubes(Vector3 deltaPosition)
	{
		if (isRolling
		    || Vector3.zero == deltaPosition)
		{
			return;
		}

#if UNITY_EDITOR
		Debug.DrawLine(camera.transform.position, m_SelectCube.transform.position, Color.red, 3, false);
#endif

		Vector3 direction = deltaPosition.normalized;
		Vector3 right = Vector3.zero, up = Vector3.zero, forward = Vector3.zero;
		List<CubeUnit> cubeList = new List<CubeUnit>();
		
		Dictionary<Vector3, RollAxis> axisDict = new Dictionary<Vector3, RollAxis>()
		{
			{ transform.right, RollAxis.RIGHT },
			{ transform.up, RollAxis.UP },
			{ transform.forward, RollAxis.FORWARD },
		};
		List<Vector3> axisList = new List<Vector3>();
		axisList.Add(transform.right);
		axisList.Add(transform.up);
		axisList.Add(transform.forward);
		
		for (int i = axisList.Count; --i >= 0;)
		{
			float project1 = Vector3.Project(direction, right).magnitude;
			float project2 = Vector3.Project(direction, axisList[i]).magnitude;
			right = project1 > project2 ? right : axisList[i];
		}
		axisList.Remove(right);
		
		for (int i = axisList.Count; --i >= 0;)
		{
			float project1 = Vector3.Project(camera.transform.forward, forward).magnitude;
			float project2 = Vector3.Project(camera.transform.forward, axisList[i]).magnitude;
			forward = project1 > project2 ? forward : axisList[i];
		}
		axisList.Remove(forward);
		
		up = axisList[0];
		
		m_RollAxis = axisDict[up];
		m_RollStartTime = Time.time;

		SelectCubes(m_SelectCube, cubeList, forward, right);
		m_RollCubes = cubeList.ToArray();
		
		bool isHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
		if (isHorizontal)
		{
			m_RollAngle = (direction.x * up.y) > 0 ? -90 : 90;
		}
		else
		{
			m_RollAngle = (direction.y * up.x) > 0 ? 90 : -90;
		}
		
		m_RollPositions = new Vector3[m_RollCubes.Length];
		m_RollEulerAngles = new Vector3[m_RollCubes.Length];
		for (int i = m_RollCubes.Length; --i >= 0;)
		{
			CubeUnit cube = m_RollCubes[i];
			
			m_RollPositions[i] = cube.transform.localPosition;
			m_RollEulerAngles[i] = cube.transform.localEulerAngles;
		}
	}

	private void SelectCubes(CubeUnit cube, List<CubeUnit> cubeList, Vector3 forward, Vector3 right)
	{
		if (cubeList.Contains(cube))
		{
			return;
		}

		cubeList.Add(cube);

		Vector3[] directions = new Vector3[4];
		directions[0] = forward;
		directions[1] = -forward;
		directions[2] = right;
		directions[3] = -right;

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
				CubeUnit nextCube = raycastHit.collider.GetComponent<CubeUnit>();
				if (null == nextCube
				    || cubeList.Contains(nextCube))
				{
					continue;
				}
				
#if UNITY_EDITOR
				Debug.DrawLine(cube.transform.position, position, Color.red, 3, false);
#endif
				SelectCubes(nextCube, cubeList, forward, right);
			}
		}
	}

	private Vector3 ParseAxis(RollAxis axis)
	{
		if (RollAxis.RIGHT == axis)
		{
			return transform.right;
		}
		else if (RollAxis.UP == axis)
		{
			return transform.up;
		}

		return transform.forward;
	}
	
	private Color GetColor()
	{
		return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
}