using UnityEngine;
using System.Collections.Generic;

public sealed class CubeItem : MonoBehaviour
{
	private static readonly Dictionary<ItemType, Color> s_ColorDict = new Dictionary<ItemType, Color>()
	{
		{ ItemType.NONE, new Color(1, 1, 1) },
		{ ItemType.TURN_LEFT, new Color(0, 0, 1) },
		{ ItemType.TURN_RIGHT, new Color(0, 1, 0) },
		{ ItemType.TURN_BACK, new Color(1, 0.5f, 0) },
		{ ItemType.TURN_UP, new Color(1, 0, 1) },
		{ ItemType.TURN_DOWN, new Color(0, 0, 0) },
		{ ItemType.STOP, new Color(1, 0, 0) },
	};

	public int layer { get; set; }
	public float size
	{
		get { return transform.localScale.x; }
		set { transform.localScale = Vector3.one * value; }
	}
	public Dictionary<AxisType, ItemType> itemDict { get; private set; }
	private Dictionary<AxisType, Renderer> m_RendererDict;
	private Dictionary<AxisType, Material> m_MaterialDict;

	public ItemType this[AxisType axisType]
	{
		get { return itemDict[axisType]; }
		set
		{
			if (!itemDict.ContainsKey(axisType))
			{
				return;
			}

			itemDict[axisType] = value;
			Material material = m_MaterialDict[axisType];
			Color color = s_ColorDict[value];
			material.color = color;
		}
	}

	public BoxCollider collider
	{
		get
		{
			if (null == m_Collider)
			{
				m_Collider = GetComponent<BoxCollider>();
			}

			return m_Collider;
		}
	}
	private BoxCollider m_Collider;

	public Renderer renderer
	{
		get
		{
			if (null == m_Renderer)
			{
				m_Renderer = GetComponent<Renderer>();
			}

			return m_Renderer;
		}
	}
	private Renderer m_Renderer;
	
	public Material material
	{
		get
		{
			if (null == m_Material)
			{
				m_Material = new Material(renderer.sharedMaterial);
				renderer.material = m_Material;
			}

			return m_Material;
		}
	}
	private Material m_Material;

	public bool enableRenderer
	{
		get { return m_EnableRenderer; }
		set
		{
			renderer.enabled = value;
			if (!value)
			{
				Color color = this.renderer.material.color;
				color.a = 0;
				material.color = color;
			}

			foreach (AxisType axisType in itemDict.Keys)
			{
				m_RendererDict[axisType].enabled = value;
				if (!value)
				{
					Color color = m_MaterialDict[axisType].color;
					color.a = 0;
					m_MaterialDict[axisType].color = color;
				}
			}
		}
	}
	private bool m_EnableRenderer = true;

	public void Generate(Dictionary<AxisType, ItemType> itemDict)
	{
		this.itemDict = itemDict;
		m_RendererDict = new Dictionary<AxisType, Renderer>();
		m_MaterialDict = new Dictionary<AxisType, Material>();

		foreach (KeyValuePair<AxisType, ItemType> itemInfo in itemDict)
		{
			GameObject gameObject = Instantiate(Resources.Load<GameObject>(ResourceDefine.CUBE_FACE));
			gameObject.name = itemInfo.Key + "_" + itemInfo.Value;
			gameObject.transform.SetParent(transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.forward = AxisUtil.Axis2Direction(transform, itemInfo.Key);
			gameObject.transform.localScale = Vector3.one;

			Renderer renderer = gameObject.GetComponent<Renderer>();
			Material material = new Material(renderer.sharedMaterial);
			renderer.material = material;
			Color color = s_ColorDict[itemInfo.Value];
			material.color = color;
			m_RendererDict[itemInfo.Key] = renderer;
			m_MaterialDict[itemInfo.Key] = material;
		}
	}
	
	public void Fade(float time, bool display)
	{
		renderer.enabled = true;

		Color color = material.color;
		color.a = display ? 1 : 0;
		iTween.ColorTo(gameObject, iTween.Hash("color", color, "time", time, "includechildren", false));
		
		foreach (AxisType axisType in itemDict.Keys)
		{
			m_RendererDict[axisType].enabled = true;
			color = m_MaterialDict[axisType].color;
			color.a = display ? 1 : 0;
			iTween.ColorTo(renderer.gameObject, iTween.Hash("color", color, "time", time, "includechildren", false));
		}
	}
}