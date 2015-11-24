using UnityEngine;
using Framework;
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
				cube.FadeIn(colorTime);
			}
			
			cubeList = cubeLists[layer];
			for (int i = cubeList.Count; --i >= 0;)
			{
				CubeItem cube = cubeList[i];
				cube.FadeOut(colorTime);
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
	private bool m_EnableCollision = false;
	
	public List<CubeItem> this[int layer]
	{
		get { return cubeLists[layer]; }
	}

	public void Init(int step, float size, float space, float distance)
	{
		float offset = (1 - step) * 0.5f;
		
		maxLayer = (step - 1) / 2;

		cubeLists = new List<CubeItem>[maxLayer + 1];
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
			cube.transform.SetParent(transform);
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
				
				cube.Init();
			}

			for (int j = cubeList.Count; --j >= 0;)
			{
				CubeItem cube = cubeList[j];
				
				cube.collider.enabled = false;
			}
		}
		
		this.layer = maxLayer;
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