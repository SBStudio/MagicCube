﻿using UnityEngine;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;

public sealed class MagicCube : MonoBehaviour
{
	private static DataSystem<ItemData> itemDatabase
	{
		get
		{
			if (null == s_ItemDatabase)
			{
				s_ItemDatabase = DataSystem<ItemData>.instance;
				s_ItemDatabase.Init(ItemData.DATABASE, ItemData.TABLE, ItemData.FIELDS);
			}

			return s_ItemDatabase;
		}
	}
	private static DataSystem<ItemData> s_ItemDatabase;

	public float colorTime = 0.5f;
	public int id { get; private set; }
	public int step { get; private set; }
	public int num { get; private set; }
	public float size { get; private set; }
	public float space { get; private set; }
	public float distance { get; private set; }
	public CubeItem destCube { get; private set; }
	public List<CubeItem>[] cubeLists { get; private set; }

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
	public int lastLayer { get; private set; }
	public int maxLayer { get; private set; }

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

	public void Load(MapData mapData)
	{
		Generate(mapData.id, mapData.step, mapData.size, mapData.space, mapData.destId, mapData.cubeItems);
	}

	public void Generate(int id, int step, float size, float space, int destId, Dictionary<AxisType, int>[] cubeItems = null)
	{
		for (int i = transform.childCount; --i >= 0;)
		{
			Transform child = transform.GetChild(i);
#if UNITY_EDITOR
			if (Application.isEditor)
			{
				DestroyImmediate(child.gameObject);
			}
			else
			{
				Destroy(child.gameObject);
			}
#else
			Destroy(child.gameObject);
#endif
		}

		step = Mathf.Max(1, step);

		this.id = id;
		this.step = step;
		this.size = size;
		this.space = space;

		distance = size + space;
		num = step * step * step;
		maxLayer = (step - 1) / 2;
		cubeLists = new List<CubeItem>[maxLayer + 1];
		float offset = (1 - step) * 0.5f;

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
			cube.id = i;
			cube.layer = layer;
			cube.size = size;
			cube.collider.size = Vector3.one * (1 + space);

			if (destId == i)
			{
				destCube = cube;
			}

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
				cube.Generate(null == cubeItems ? RandomCube(cube) : cubeItems[cube.id]);
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

	private Dictionary<AxisType, int> RandomCube(CubeItem cube)
	{
		Dictionary<AxisType, int> itemDict = new Dictionary<AxisType, int>();
		
		AxisType[] axisTypes = Enum.GetValues(typeof(AxisType)) as AxisType[];
		ItemData[] itemDatas = itemDatabase.GetAll();
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
			
			ItemData itemData = itemDatas[UnityEngine.Random.Range(0, itemDatas.Length)];
			itemDict[axis] = itemData.id;
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