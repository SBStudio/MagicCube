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

	public GameObject cubePrefab;
	public int step;
	public float size;
	public float space;
	public float rollTime = 1;
	
	public bool isRolling { get { return m_RollStartTime > 0; } }

	private CubeUnit[] m_CubeUnits;
	private float m_Distance;
	private float m_RaycastRadius;
	private CubeUnit m_SelectCube;
	private CubeUnit[] m_RollCubes;
	private Vector3[] m_RollPositions;
	private Vector3[] m_RollEulerAngles;
	private RollAxis m_RollAxis;
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

		m_RaycastRadius = size * 0.1f;
		m_Distance = size + space;
		float offset = (step - 1) * 0.5f;

		m_CubeUnits = new CubeUnit[step * step * step];
		for (int i = m_CubeUnits.Length; --i >= 0;)
		{
			Vector3 position = new Vector3(i % step - offset,
			                               (i / step) % step - offset,
			                               (i / (step * step)) % step - offset) * m_Distance;
			
			CubeUnit cubeUnit = Instantiate(cubePrefab).AddComponent<CubeUnit>();
			cubeUnit.name = i.ToString();
			cubeUnit.transform.parent = transform;
			cubeUnit.transform.localPosition = position;
			cubeUnit.transform.localScale = Vector3.one * size;
			cubeUnit.GetComponent<Renderer>().material.color = GetColor();
			m_CubeUnits[i] = cubeUnit;
		}
	}
	
	private void Update()
	{
		UpdateRoll();
	}

	private void UpdateRoll()
	{
		if (!isRolling)
		{
			return;
		}

		float deltaTime = Mathf.Clamp01((Time.time - m_RollStartTime) / rollTime);
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
			Ray ray = Camera.main.ScreenPointToRay(evt.position);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit))
			{
				m_SelectCube = raycastHit.collider.GetComponent<CubeUnit>();
				if (null != m_SelectCube)
				{
					m_RollInputId = evt.inputId;
					
					return;
				}
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
			deltaPosition /= Screen.dpi * Time.deltaTime;
			
			transform.Rotate(Camera.main.transform.up, -deltaPosition.x, Space.World);
			transform.Rotate(Camera.main.transform.right, deltaPosition.y, Space.World);
		}
	}

	private void OnInputEnd(InputEvent evt)
	{
		if (m_RollInputId == evt.inputId)
		{
			m_RollInputId = int.MinValue;

			Vector3 direction = evt.deltaPosition.normalized;

			RollCubes(direction);
		}
		else if (m_ViewInputId == evt.inputId)
		{
			m_ViewInputId = int.MinValue;
		}
	}

	private void RollCubes(Vector3 direction)
	{
		if (isRolling)
		{
			return;
		}

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
			float project1 = Vector3.Project(Camera.main.transform.forward, forward).magnitude;
			float project2 = Vector3.Project(Camera.main.transform.forward, axisList[i]).magnitude;
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

		for (int i = directions.Length; --i >= 0;)
		{
			Vector3 direction = directions[i];
			Vector3 position = cube.transform.position + direction * m_Distance;

			Collider[] colliders = Physics.OverlapSphere(position, m_RaycastRadius);
			if (0 >= colliders.Length)
			{
				continue;
			}

			CubeUnit nextCube = colliders[0].GetComponent<CubeUnit>();
			if (null == nextCube
			    || cubeList.Contains(nextCube))
			{
				continue;
			}
			
			SelectCubes(nextCube, cubeList, forward, right);
			Debug.DrawLine(cube.transform.position, position, Color.red, 2, false);
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