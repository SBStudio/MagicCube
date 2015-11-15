using UnityEngine;
using System.Collections.Generic;
using Framework;

public sealed class MagicCube : MonoBehaviour
{
	public GameObject prefab;
	public int step;
	public float size;
	public float space;
	public GameObject[] cubes;
	public float rollTime = 1;

	private float distance;
	private float raycastRadius;
	private GameObject selection;
	private GameObject[] selections;
	private Vector3[] rollPositions;
	private Vector3[] rollEulerAngles;
	private Vector3 rollCenter;
	private Vector3 rollAxis;
	private float rollAngle;
	private float rollStartTime;
	private Vector3 lastTouchPosition;

	private void Start()
	{
		raycastRadius = size * 0.25f;
		distance = size + space;
		float offset = (step - 1) * 0.5f;

		cubes = new GameObject[step * step * step];
		for (int i = cubes.Length; --i >= 0;)
		{
			Vector3 position = new Vector3(i % step - offset,
			                               (i / step) % step - offset,
			                               (i / (step * step)) % step - offset) * distance;
			
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
		if (Input.GetMouseButtonDown(0))
		{
			if (null == selection)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit))
				{
					selection = raycastHit.collider.gameObject;
				}
			}
			
			lastTouchPosition = Input.mousePosition;
		}
		else if (Input.GetMouseButton(0))
		{
			if (null == selection)
			{
				Vector3 deltaPosition = Input.mousePosition - lastTouchPosition;
				deltaPosition /= Screen.dpi * Time.deltaTime;
				lastTouchPosition = Input.mousePosition;

				transform.Rotate(Camera.main.transform.up, -deltaPosition.x, Space.World);
				transform.Rotate(Camera.main.transform.right, deltaPosition.y, Space.World);
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (null != selection
			    && 0 >= rollStartTime)
			{
				Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward);
				Vector3 lastPosition = Camera.main.ScreenToWorldPoint(lastTouchPosition + Vector3.forward);
				Vector3 direction = position - lastPosition;
				Vector3 right = Vector3.zero, up = Vector3.zero, forward = Vector3.zero;
				List<GameObject> cubeList = new List<GameObject>();
				
				List<Vector3> axisList = new List<Vector3>();
				axisList.Add(transform.right);
				axisList.Add(transform.up);
				axisList.Add(transform.forward);
				
				for (int i = axisList.Count; --i >= 0;)
				{
					float project1 = Vector3.Project(Vector3.forward, forward).magnitude;
					float project2 = Vector3.Project(Vector3.forward, axisList[i]).magnitude;
					forward = project1 > project2 ? forward : axisList[i];
					
					project1 = Vector3.Project(direction, right).magnitude;
					project2 = Vector3.Project(direction, axisList[i]).magnitude;
					right = project1 > project2 ? right : axisList[i];
				}
				
				axisList.Remove(forward);
				axisList.Remove(right);
				up = axisList[0];

				SelectCubes(selection, cubeList, forward, right);
				selections = cubeList.ToArray();
				
				float deltaX = Input.mousePosition.x - lastTouchPosition.x;
				float deltaY = Input.mousePosition.y - lastTouchPosition.y;
				bool isHorizontal = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);

				if (isHorizontal)
				{
					rollAngle = (direction.x * up.y) > 0 ? -90 : 90;
				}
				else
				{
					rollAngle = (direction.y * up.x) > 0 ? 90 : -90;
				}

				rollAxis = up;
				rollPositions = new Vector3[cubeList.Count];
				rollEulerAngles = new Vector3[cubeList.Count];
				rollCenter = Vector3.zero;
				for (int i = cubeList.Count; --i >= 0;)
				{
					GameObject cube = cubeList[i];

					rollPositions[i] = cube.transform.localPosition;
					rollEulerAngles[i] = cube.transform.localEulerAngles;
					
					rollCenter += cube.transform.position;
				}
				rollCenter /= cubeList.Count;

				rollStartTime = Time.time;
			}
		}
		
		if (null != selection
		    && 0 < rollStartTime)
		{
			float deltaTime = Mathf.Clamp01((Time.time - rollStartTime) / rollTime);
			float angle = rollAngle * deltaTime;
			
			if (1 <= deltaTime)
			{
				angle = rollAngle;
				selection = null;
				rollStartTime = 0;
			}

			for (int i = selections.Length; --i >= 0;)
			{
				GameObject cube = selections[i];
				
				cube.transform.localPosition = rollPositions[i];
				cube.transform.localEulerAngles = rollEulerAngles[i];
				cube.transform.RotateAround(rollCenter, rollAxis, angle);
			}
		}
	}

	private void SelectCubes(GameObject cube, List<GameObject> cubeList, Vector3 forward, Vector3 right)
	{
		if (cubeList.Contains(cube))
		{
			return;
		}

		cubeList.Add(cube);

		Collider[] colliders = Physics.OverlapSphere(cube.transform.position + forward * distance, raycastRadius);
		for (int i = colliders.Length; --i >= 0;)
		{
			SelectCubes(colliders[i].gameObject, cubeList, forward, right);
		}
		colliders = Physics.OverlapSphere(cube.transform.position - forward * distance, raycastRadius);
		for (int i = colliders.Length; --i >= 0;)
		{
			SelectCubes(colliders[i].gameObject, cubeList, forward, right);
		}
		colliders = Physics.OverlapSphere(cube.transform.position + right * distance, raycastRadius);
		for (int i = colliders.Length; --i >= 0;)
		{
			SelectCubes(colliders[i].gameObject, cubeList, forward, right);
		}
		colliders = Physics.OverlapSphere(cube.transform.position - right * distance, raycastRadius);
		for (int i = colliders.Length; --i >= 0;)
		{
			SelectCubes(colliders[i].gameObject, cubeList, forward, right);
		}
	}
	
	private Color GetColor()
	{
		return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
	}
}