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

	public GameObject prefab;
	public int step;
	public float size;
	public float space;
	public GameObject[] cubes;
	public float rollTime = 1;

	private float m_Distance;
	private float m_RaycastRadius;
	private GameObject m_Selection;
	private GameObject[] m_Selections;
	private Vector3[] m_RollPositions;
	private Vector3[] m_RollEulerAngles;
	private Vector3 m_RollCenter;
	private RollAxis m_RollAxis;
	private float m_RollAngle;
	private float m_RollStartTime = 0;
	private int m_RollInputId = -1;

	private void Awake()
	{
		Debug.Log(InputModule.instance);
		EventSystem<InputEvent>.Add(OnInputEvet);

		m_RaycastRadius = size * 0.25f;
		m_Distance = size + space;
		float offset = (step - 1) * 0.5f;

		cubes = new GameObject[step * step * step];
		for (int i = cubes.Length; --i >= 0;)
		{
			Vector3 position = new Vector3(i % step - offset,
			                               (i / step) % step - offset,
			                               (i / (step * step)) % step - offset) * m_Distance;
			
			GameObject gameObject = Instantiate(prefab);
			gameObject.name = i.ToString();
			gameObject.transform.parent = transform;
			gameObject.transform.localPosition = position;
			gameObject.transform.localScale = Vector3.one * size;
			gameObject.GetComponent<Renderer>().material.color = GetColor();
			cubes[i] = gameObject;
		}
	}
	
	private void Update()
	{
		if (0 < m_RollStartTime)
		{
			float deltaTime = Mathf.Clamp01((Time.time - m_RollStartTime) / rollTime);
			float angle = m_RollAngle * deltaTime;
			
			if (1 <= deltaTime)
			{
				angle = m_RollAngle;
				m_Selection = null;
				m_RollStartTime = 0;
			}
			
			m_RollCenter = Vector3.zero;
			for (int i = m_Selections.Length; --i >= 0;)
			{
				GameObject cube = m_Selections[i];
				
				m_RollCenter += cube.transform.position;
			}
			m_RollCenter /= m_Selections.Length;
			
			for (int i = m_Selections.Length; --i >= 0;)
			{
				GameObject cube = m_Selections[i];
				
				cube.transform.localPosition = m_RollPositions[i];
				cube.transform.localEulerAngles = m_RollEulerAngles[i];
				cube.transform.RotateAround(m_RollCenter, ParseAxis(m_RollAxis), angle);
			}
		}
	}

	private void OnInputEvet(InputEvent evt)
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
		if (null == m_Selection)
		{
			Ray ray = Camera.main.ScreenPointToRay(evt.position);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit))
			{
				m_Selection = raycastHit.collider.gameObject;
				m_RollInputId = evt.inputId;
			}
		}
	}

	private void OnInputMove(InputEvent evt)
	{
		if (m_RollInputId == evt.inputId)
		{
		}
		else
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
			Vector3 direction = evt.deltaPosition.normalized;
			Vector3 right = Vector3.zero, up = Vector3.zero, forward = Vector3.zero;
			List<GameObject> cubeList = new List<GameObject>();

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
			
			SelectCubes(m_Selection, cubeList, forward, right);
			m_Selections = cubeList.ToArray();
			
			bool isHorizontal = Mathf.Abs(evt.deltaPosition.x) > Mathf.Abs(evt.deltaPosition.y);
			if (isHorizontal)
			{
				m_RollAngle = (direction.x * up.y) > 0 ? -90 : 90;
			}
			else
			{
				m_RollAngle = (direction.y * up.x) > 0 ? 90 : -90;
			}
			
			m_RollPositions = new Vector3[m_Selections.Length];
			m_RollEulerAngles = new Vector3[m_Selections.Length];
			for (int i = m_Selections.Length; --i >= 0;)
			{
				GameObject cube = m_Selections[i];
				
				m_RollPositions[i] = cube.transform.localPosition;
				m_RollEulerAngles[i] = cube.transform.localEulerAngles;
			}
			
			m_RollAxis = axisDict[up];
			m_RollStartTime = Time.time;
			m_RollInputId = -1;
		}
	}

	private void SelectCubes(GameObject cube, List<GameObject> cubeList, Vector3 forward, Vector3 right)
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
			Collider[] colliders = Physics.OverlapSphere(cube.transform.position + direction * m_Distance, m_RaycastRadius);
			if (0 >= colliders.Length)
			{
				continue;
			}

			GameObject collider = colliders[0].gameObject;
			if (cubeList.Contains(collider))
			{
				continue;
			}
			
			SelectCubes(collider, cubeList, forward, right);
			Debug.DrawLine(cube.transform.position, cube.transform.position + direction * m_Distance, Color.red, 3, false);
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