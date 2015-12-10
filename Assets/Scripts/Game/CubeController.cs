using UnityEngine;
using Framework;
using System;
using System.Collections.Generic;

public sealed class CubeController : MonoBehaviour
{
	public float viewDistance = 4;
	public float viewLerp = 2;
	public float viewSensitivity = 50;
	
	public float rollError = 0;
	public float rollTime = 0.5f;
	public AxisType rollAxis;
	public float rollAngle;
	public float testTime = 0.5f;
	public float moveTime = 0.25f;
	public float itemTime = 0.25f;
	
	public Camera camera { get; private set; }
	public MagicCube magicCube { get; private set; }
	public Player player { get; private set; }
	public MapData mapData { get; private set; }
	public StateMachine stateMachine { get; private set; }
	public Trigger cubeTrigger { get; private set; }
	public BoxCollider cubeCollider { get; private set; }
	public Dictionary<CubeItem, SelectCube> selectDict { get; private set; }

	private DataSystem<MapData> mapDatabase
	{
		get
		{
			if (null == m_MapDatabase)
			{
				m_MapDatabase = DataSystem<MapData>.instance;
				m_MapDatabase.Init(MapData.DATABASE, MapData.TABLE, MapData.FIELDS);
			}

			return m_MapDatabase;
		}
	}
	private DataSystem<MapData> m_MapDatabase;
	 
	private void Awake()
	{
		DebugUtil.Add<FPSInfo>();
		LogUtil.printType = LogUtil.PrintType.Screen;
		InputModule inputModule = InputModule.instance;
		Application.targetFrameRate = GameDefine.FPS;

		selectDict = new Dictionary<CubeItem, SelectCube>();

		if (null == camera)
		{
			camera = Camera.main;
		}

		GameObject gameObject = new GameObject(typeof(MagicCube).Name);
		gameObject.transform.SetParent(transform);
		magicCube = gameObject.AddComponent<MagicCube>();

		gameObject = Instantiate(Resources.Load<GameObject>(ResourceDefine.PLAYER));
		gameObject.name = typeof(Player).Name;
		gameObject.transform.SetParent(transform);
		player = gameObject.AddComponent<Player>();
		
		gameObject = new GameObject(typeof(Trigger).Name);
		gameObject.transform.SetParent(transform);
		gameObject.SetActive(false);
		
		cubeTrigger = gameObject.AddComponent<Trigger>();
		cubeCollider = gameObject.AddComponent<BoxCollider>();
		cubeCollider.isTrigger = true;
		
		Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
		rigidBody.isKinematic = true;
		
		stateMachine = this.gameObject.AddComponent<StateMachine>();
		stateMachine.Add<GlobalState>().controller = this;
		stateMachine.Add<IdleState>().controller = this;
		stateMachine.Add<TestState>().controller = this;
		stateMachine.Add<RollState>().controller = this;
		stateMachine.Add<MoveState>().controller = this;
		stateMachine.Add<ItemState>().controller = this;
		stateMachine.EnterGlobal<GlobalState>();
		stateMachine.Enter<IdleState>();
	}

	private void Start()
	{
		mapData = mapDatabase.Get(0);
		magicCube.Load(mapData);
		magicCube.Init();

		player.Spawn(magicCube.destCube, mapData.spawnInfo);
	}
}
