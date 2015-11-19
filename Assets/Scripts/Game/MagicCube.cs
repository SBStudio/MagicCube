using UnityEngine;
using Framework;
using System.Collections;
using System.Collections.Generic;

public sealed class MagicCube : MonoBehaviour
{
	private struct SelectCube
	{
		public CubeItem cube;
		public Vector3 position;
		public Quaternion rotation;
	}

	public int step = 5;
	public float size = 1;
	public float space = 0.05f;
	public float rollError = 0;
	public float rollTime = 0.5f;
	public float colorTime = 0.5f;

	public CubeAxis.Axis rollAxis { get; private set; }
	public float rollAngle { get; private set; }
	public float distance { get; private set; }
	public int layer { get; private set; }
	public int lastLayer { get; private set; }
	public int maxLayer { get; private set; }
	public bool isHitTesting { get { return m_TriggerCollider.gameObject.activeSelf; } }
	public bool isRolling { get { return 0 < m_RollStartTime; } }
	
	private List<CubeItem>[] m_CubeLists = null;
	private Dictionary<CubeItem, SelectCube> m_SelectDict = new Dictionary<CubeItem, SelectCube>();
	private BoxCollider m_TriggerCollider = null;
	private float m_RollStartTime = int.MinValue;

	public List<CubeItem> this[int layer]
	{
		get { return m_CubeLists[layer]; }
	}

	public bool enableCollision
	{
		get { return m_EnableCollision; }
		set
		{
			if (m_EnableCollision == value
			    || isHitTesting)
			{
				return;
			}

			m_EnableCollision = value;

			for (int i = m_CubeLists.Length; --i >= 0;)
			{
				List<CubeItem> cubeList = m_CubeLists[i];
				for (int j = cubeList.Count; --j >= 0;)
				{
					CubeItem cube = cubeList[j];

					cube.collider.enabled = value;
				}
			}
		}
	}
	private bool m_EnableCollision = false;

	private void Awake()
	{
		GameObject gameObject = new GameObject("Trigger");
		gameObject.transform.parent = transform;
		gameObject.SetActive(false);

		Trigger trigger = gameObject.AddComponent<Trigger>();
		trigger.onTriggerEnter += OnCubeTrigger;

		m_TriggerCollider = gameObject.AddComponent<BoxCollider>();
		m_TriggerCollider.isTrigger = true;

		Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
		rigidBody.isKinematic = true;
	}

	public void Init()
	{
		distance = size + space;
		float offset = (1 - step) * 0.5f;
		
		maxLayer = (step - 1) / 2;

		m_CubeLists = new List<CubeItem>[maxLayer + 1];
		int num = step * step * step;
		for (int i = num; --i >= 0;)
		{
			Vector3 grids = new Vector3(i % step + offset,
			                            (i / step) % step + offset,
			                            (i / (step * step)) % step + offset);
			
			Vector3 position = grids * distance;
			int layer = (int)Mathf.Max(Mathf.Abs(grids.x), Mathf.Abs(grids.y), Mathf.Abs(grids.z));
			
			GameObject gameObject = Instantiate(Resources.Load<GameObject>(ResourceDefine.CUBE_ITEM));
			CubeItem cube = gameObject.AddComponent<CubeItem>();
			cube.name = i.ToString();
			cube.gameObject.layer = LayerDefine.CUBE;
			cube.transform.parent = transform;
			cube.transform.localPosition = position;
			cube.transform.localScale = Vector3.one * size;
			cube.layer = layer;
			cube.renderer.enabled = false;
			cube.collider.size = Vector3.one * (1 + space);
			
			if (null == m_CubeLists[layer])
			{
				m_CubeLists[layer] = new List<CubeItem>();
			}
			m_CubeLists[layer].Add(cube);
		}

		for (int i = m_CubeLists.Length; --i >= 0;)
		{
			List<CubeItem> cubeList = m_CubeLists[i];
			for (int j = cubeList.Count; --j >= 0;)
			{
				CubeItem cube = cubeList[j];
				
				cube.Init();
			}

			for (int j = cubeList.Count; --j >= 0;)
			{
				CubeItem cube = cubeList[j];
				
				cube.collider.enabled = false;
			}
		}
		
		SetLayer(maxLayer);
	}

	private void Update()
	{
		OnUpdateRoll(Time.deltaTime);
	}

	private void OnUpdateRoll(float deltaTime)
	{
		if (!isRolling)
		{
			return;
		}

		float progress = Mathf.Clamp01((Time.time - m_RollStartTime) / rollTime);
		float angle = rollAngle * progress;
		
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
			select.Key.transform.RotateAround(center, CubeAxis.Axis2Direction(transform, rollAxis), angle);
		}
		
		if (1 <= progress)
		{
			StopRoll();
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

		TimerUtil.Begin(OnColorFadeout, colorTime);

		List<CubeItem> cubeList = m_CubeLists[lastLayer];
		for (int i = cubeList.Count; --i >= 0;)
		{
			CubeItem cube = cubeList[i];
			
			Color color = cube.color;
			color.a = 0;
			iTween.ColorTo(cube.gameObject, color, colorTime);
		}
		
		cubeList = m_CubeLists[layer];
		for (int i = cubeList.Count; --i >= 0;)
		{
			CubeItem cube = cubeList[i];

			cube.renderer.enabled = true;
			Color color = cube.color;
			color.a = 1;
			iTween.ColorTo(cube.gameObject, color, colorTime);
		}
	}

	public void RollCubes(CubeItem cube, Vector3 deltaPosition)
	{
		if (isRolling
		    || Vector3.zero == deltaPosition)
		{
			return;
		}
		
		Vector3 direction = deltaPosition.normalized;
		CubeAxis.Axis right, up, forward;

		CubeAxis.GetRollAxis(transform, direction, out right, out up, out forward);
		SelectCubes(cube, right, up, forward);

		rollAxis = up;
		
		bool isHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
		if (isHorizontal)
		{
			rollAngle = (direction.x * CubeAxis.Axis2Direction(transform, up).y) > 0 ? -90 : 90;
		}
		else
		{
			rollAngle = (direction.y * CubeAxis.Axis2Direction(transform, up).x) > 0 ? 90 : -90;
		}
	}

	private void SelectCubes(CubeItem cube, CubeAxis.Axis right, CubeAxis.Axis up, CubeAxis.Axis forward)
	{
		m_TriggerCollider.gameObject.SetActive(false);
		enableCollision = true;

		m_TriggerCollider.transform.position = cube.transform.position;
		m_TriggerCollider.transform.forward = CubeAxis.Axis2Direction(transform, forward);
		m_TriggerCollider.transform.right = CubeAxis.Axis2Direction(transform, right);
		m_TriggerCollider.transform.up = CubeAxis.Axis2Direction(transform, up);

		float size = step * distance * 4;
		m_TriggerCollider.size = new Vector3(size, distance * 0.5f, size);
		m_TriggerCollider.gameObject.SetActive(true);
	}
	
	private void StopRoll()
	{
		m_RollStartTime = int.MinValue;
		m_SelectDict.Clear();
	}

	private void OnColorFadeout()
	{
		List<CubeItem> cubeList = m_CubeLists[lastLayer];
		for (int i = cubeList.Count; --i >= 0;)
		{
			CubeItem cube = cubeList[i];
			
			cube.renderer.enabled = false;
		}
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
			m_TriggerCollider.gameObject.SetActive(false);
			enableCollision = false;
			m_RollStartTime = Time.time;
		}
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
}