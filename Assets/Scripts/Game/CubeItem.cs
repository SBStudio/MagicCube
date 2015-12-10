using UnityEngine;
using Framework;
using System.Collections.Generic;

public sealed class CubeItem : MonoBehaviour
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

	public int id { get; set; }
	public int layer { get; set; }
	public float size
	{
		get { return transform.localScale.x; }
		set { transform.localScale = Vector3.one * value; }
	}
	public Dictionary<AxisType, ItemData> itemDict { get; private set; }
	private Dictionary<AxisType, Renderer> m_RendererDict;
	private Dictionary<AxisType, Material> m_MaterialDict;

	public ItemData this[AxisType axisType]
	{
		get { return itemDict[axisType]; }
		set
		{
			if (!itemDict.ContainsKey(axisType))
			{
				return;
			}

			itemDict[axisType] = value;
			m_RendererDict[axisType].name = axisType + "_" + value.name;
			Material material = m_MaterialDict[axisType];
			material.color = value.color;
		}
	}

	public new BoxCollider collider
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

	public new Renderer renderer
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

	public void Generate(Dictionary<AxisType, int> itemDict)
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

		this.itemDict = new Dictionary<AxisType, ItemData>();
		m_RendererDict = new Dictionary<AxisType, Renderer>();
		m_MaterialDict = new Dictionary<AxisType, Material>();

		foreach (KeyValuePair<AxisType, int> itemInfo in itemDict)
		{
			ItemData itemData = itemDatabase.Get(itemInfo.Value);
			this.itemDict[itemInfo.Key] = itemData;

			GameObject gameObject = Instantiate(Resources.Load<GameObject>(ResourceDefine.CUBE_FACE));
			gameObject.name = itemInfo.Key + "_" + itemData.name;
			gameObject.transform.SetParent(transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.forward = AxisUtil.Axis2Direction(transform, itemInfo.Key);
			gameObject.transform.localScale = Vector3.one;

			Renderer renderer = gameObject.GetComponent<Renderer>();
			Material material = new Material(renderer.sharedMaterial);
			renderer.material = material;

			material.color = itemData.color;
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
			iTween.ColorTo(m_RendererDict[axisType].gameObject, iTween.Hash("color", color, "time", time, "includechildren", false));
		}
	}
}