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

	public int layer;
	public float size
	{
		get { return transform.localScale.x; }
		set { transform.localScale = Vector3.one * value; }
	}
	public Dictionary<AxisType, ItemType> itemDict { get; private set; }
	public Dictionary<AxisType, Renderer> faceDict { get; private set; }

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
	public BoxCollider m_Collider;

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
	public Renderer m_Renderer;

	public bool enableRenderer
	{
		get { return m_EnableRenderer; }
		set
		{
			this.renderer.enabled = value;
			if (!value)
			{
				Color color = this.renderer.material.color;
				color.a = 0;
				this.renderer.material.color = color;
			}

			foreach (Renderer renderer in faceDict.Values)
			{
				renderer.enabled = value;
				if (!value)
				{
					Color color = renderer.material.color;
					color.a = 0;
					renderer.material.color = color;
				}
			}
		}
	}
	private bool m_EnableRenderer = true;

	public void Generate(Dictionary<AxisType, ItemType> itemDict)
	{
		this.itemDict = itemDict;
		faceDict = new Dictionary<AxisType, Renderer>();

		foreach (KeyValuePair<AxisType, ItemType> itemInfo in itemDict)
		{
			Vector3 direction = AxisUtil.Axis2Direction(transform, itemInfo.Key);
			if (Physics.Linecast(transform.position,
			                     transform.position + direction * (collider.size.x * 1.5f),
			                     1 << LayerDefine.CUBE))
			{
				continue;
			}

			GameObject gameObject = Instantiate(Resources.Load<GameObject>(ResourceDefine.CUBE_FACE));
			gameObject.name = itemInfo.Key + "_" + itemInfo.Value;
			gameObject.transform.SetParent(transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.forward = AxisUtil.Axis2Direction(transform, itemInfo.Key);
			gameObject.transform.localScale = Vector3.one;

			Renderer renderer = gameObject.GetComponent<Renderer>();
			Color color = s_ColorDict[itemInfo.Value];
			renderer.material.color = color;
			faceDict[itemInfo.Key] = renderer;
		}
	}
	
	public void Fade(float time, bool display)
	{
		this.renderer.enabled = true;

		Color color = this.renderer.material.color;
		color.a = display ? 1 : 0;
		iTween.ColorTo(gameObject, iTween.Hash("color", color, "time", time, "includechildren", false));

		foreach (Renderer renderer in faceDict.Values)
		{
			renderer.enabled = true;
			color = renderer.material.color;
			color.a = display ? 1 : 0;
			iTween.ColorTo(renderer.gameObject, iTween.Hash("color", color, "time", time, "includechildren", false));
		}
	}
}