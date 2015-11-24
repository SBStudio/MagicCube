using UnityEngine;
using System;
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
	public BoxCollider collider { get; private set; }
	public Renderer renderer { get; private set; }
	public bool enableRenderer
	{
		get { return m_EnableRenderer; }
		set
		{
			this.renderer.enabled = value;
			foreach (Renderer renderer in faceDict.Values)
			{
				renderer.enabled = value;
			}
		}
	}
	private bool m_EnableRenderer = false;

	private void Awake()
	{
		collider = GetComponent<BoxCollider>();
		renderer = GetComponent<Renderer>();
	}

	public void Init()
	{
		itemDict = new Dictionary<AxisType, ItemType>();
		faceDict = new Dictionary<AxisType, Renderer>();

		Color color = this.renderer.material.color;
		color.a = 0;
		this.renderer.material.color = color;
		this.renderer.enabled = false;

		AxisType[] axisTypes = Enum.GetValues(typeof(AxisType)) as AxisType[];
		ItemType[] itemTypes = Enum.GetValues(typeof(ItemType)) as ItemType[];
		for (int i = axisTypes.Length; --i >= 0;)
		{
			AxisType axis = axisTypes[i];
			Vector3 direction = AxisUtil.Axis2Direction(transform, axis);
			if (Physics.Linecast(transform.position,
			                     transform.position + direction * (collider.size.x * 1.5f),
			                     1 << LayerDefine.CUBE))
			{
				continue;
			}

			ItemType itemType = itemTypes[UnityEngine.Random.Range(0, itemTypes.Length)];
			itemDict[axis] = itemType;

			GameObject gameObject = Instantiate(Resources.Load<GameObject>(ResourceDefine.CUBE_FACE));
			gameObject.name = axis + "_" + itemType;
			gameObject.transform.SetParent(transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.forward = AxisUtil.Axis2Direction(transform, axis);
			gameObject.transform.localScale = Vector3.one;

			Renderer renderer = gameObject.GetComponent<Renderer>();

			color = s_ColorDict[itemType];
			color.a = 0;
			renderer.material.color = color;
			renderer.enabled = false;
			faceDict[axis] = renderer;
		}
	}

	public void FadeIn(float time)
	{
		this.renderer.enabled = true;
		Color color = this.renderer.material.color;
		color.a = 0;
		iTween.ColorTo(gameObject, iTween.Hash("color", color, "time", time, "includechildren", false));

		foreach (Renderer renderer in faceDict.Values)
		{
			renderer.enabled = true;
			color = renderer.material.color;
			color.a = 0;
			iTween.ColorTo(renderer.gameObject, iTween.Hash("color", color, "time", time, "includechildren", false));
		}
	}
	
	public void FadeOut(float time)
	{
		this.renderer.enabled = true;

		Color color = this.renderer.material.color;
		color.a = 1;
		iTween.ColorTo(gameObject, iTween.Hash("color", color, "time", time, "includechildren", false));

		foreach (Renderer renderer in faceDict.Values)
		{
			renderer.enabled = true;
			color = renderer.material.color;
			color.a = 1;
			iTween.ColorTo(renderer.gameObject, iTween.Hash("color", color, "time", time, "includechildren", false));
		}
	}
}