using UnityEngine;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;

public sealed class MagicCube : MonoBehaviour
{
	public float colorTime = 0.5f;
	public List<CubeItem>[] cubeLists { get; private set; }
	public int lastLayer { get; private set; }
	public int maxLayer { get; private set; }

	private TimerBehaviour m_FadeTimer;

	public int layer
	{
		get { return m_Layer; }
		set
		{
			value = Mathf.Clamp(value, 0, maxLayer);
			if (value == m_Layer)
			{
				return;
			}
			
			lastLayer = m_Layer;
			m_Layer = value;
			
			List<CubeItem> cubeList = cubeLists[lastLayer];
			for (int i = cubeList.Count; --i >= 0;)
			{
				CubeItem cube = cubeList[i];
				cube.Fade(colorTime, false);
			}
			
			cubeList = cubeLists[layer];
			for (int i = cubeList.Count; --i >= 0;)
			{
				CubeItem cube = cubeList[i];
				cube.Fade(colorTime, true);
			}
			
			if (null != m_FadeTimer)
			{
				m_FadeTimer.Stop();
			}
			m_FadeTimer = TimerUtil.Begin(OnFadeTimer, colorTime, 0, 0, lastLayer);
		}
	}
	private int m_Layer;

	public bool enableCollision
	{
		get { return m_EnableCollision; }
		set
		{
			if (m_EnableCollision == value)
			{
				return;
			}

			m_EnableCollision = value;

			for (int i = cubeLists.Length; --i >= 0;)
			{
				List<CubeItem> cubeList = cubeLists[i];
				for (int j = cubeList.Count; --j >= 0;)
				{
					CubeItem cube = cubeList[j];
					cube.collider.enabled = value;
				}
			}
		}
	}
	private bool m_EnableCollision = true;
	
	public bool enableRenderer
	{
		get { return m_EnableRenderer; }
		set
		{
			if (m_EnableRenderer == value)
			{
				return;
			}
			
			m_EnableRenderer = value;
			
			for (int i = cubeLists.Length; --i >= 0;)
			{
				List<CubeItem> cubeList = cubeLists[i];
				for (int j = cubeList.Count; --j >= 0;)
				{
					CubeItem cube = cubeList[j];

					cube.enableRenderer = value;
				}
			}
		}
	}
	private bool m_EnableRenderer = true;
	
	public List<CubeItem> this[int layer]
	{
		get { return cubeLists[layer]; }
	}

	public void Generate(int step, float size, float space, float distance)
	{
		if (0 >= step)
		{
			return;
		}

		maxLayer = (step - 1) / 2;
		cubeLists = new List<CubeItem>[maxLayer + 1];

		float offset = (1 - step) * 0.5f;
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
			Transform parent = transform.FindChild(layer.ToString()) ?? new GameObject(layer.ToString()).transform;
			parent.SetParent(transform);
			cube.name = i.ToString();
			cube.gameObject.layer = LayerDefine.CUBE;
			cube.transform.SetParent(parent);
			cube.transform.localPosition = position;
			cube.layer = layer;
			cube.size = size;
			cube.collider.size = Vector3.one * (1 + space);

			if (null == cubeLists[layer])
			{
				cubeLists[layer] = new List<CubeItem>();
			}
			cubeLists[layer].Add(cube);
		}

		for (int i = cubeLists.Length; --i >= 0;)
		{
			List<CubeItem> cubeList = cubeLists[i];
			for (int j = cubeList.Count; --j >= 0;)
			{
				CubeItem cube = cubeList[j];
				cube.Generate(RandomCube(cube));
			}

			for (int j = cubeList.Count; --j >= 0;)
			{
				CubeItem cube = cubeList[j];
				cube.collider.enabled = false;
			}
		}
	}

	public void Init()
	{
		enableCollision = false;
		enableRenderer = false;

		layer = maxLayer;
	}

	private Dictionary<AxisType, ItemType> RandomCube(CubeItem cube)
	{
		Dictionary<AxisType, ItemType> itemDict = new Dictionary<AxisType, ItemType>();
		
		AxisType[] axisTypes = Enum.GetValues(typeof(AxisType)) as AxisType[];
		ItemType[] itemTypes = Enum.GetValues(typeof(ItemType)) as ItemType[];
		for (int i = axisTypes.Length; --i >= 0;)
		{
			AxisType axis = axisTypes[i];
			Vector3 direction = AxisUtil.Axis2Direction(cube.transform, axis);
			if (Physics.Linecast(cube.transform.position,
			                     cube.transform.position + direction * (cube.collider.size.x * 1.5f),
			                     1 << LayerDefine.CUBE))
			{
				continue;
			}
			
			ItemType itemType = itemTypes[UnityEngine.Random.Range(0, itemTypes.Length)];
			itemDict[axis] = itemType;
		}

		return itemDict;
	}

	private void OnFadeTimer(params object[] args)
	{
		int layer = (int)args[0];
		List<CubeItem> cubeList = cubeLists[layer];
		for (int i = cubeList.Count; --i >= 0;)
		{
			CubeItem cube = cubeList[i];
			cube.enableRenderer = false;
		}
	}
}