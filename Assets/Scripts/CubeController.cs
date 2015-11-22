using UnityEngine;
using Framework;
using System;
using System.Collections.Generic;

public sealed class CubeController : MonoBehaviour
{
	public struct SelectCube
	{
		public CubeItem cube;
		public Vector3 position;
		public Quaternion rotation;
	}

	public float viewDistance = 4;
	public float viewLerp = 2;
	public float viewSensitivity = 50;
	
	public int step = 5;
	public float size = 1;
	public float space = 0.05f;
	public float distance;
	public float rollError = 0;
	public float rollTime = 0.5f;
	public AxisType rollAxis;
	public float rollAngle;
	public float testTime = 0.5f;
	public float moveTime = 0.5f;
	
	public Camera camera { get; private set; }
	public MagicCube magicCube { get; private set; }
	public Player player { get; private set; }
	public StateMachine stateMachine { get; private set; }
	public Trigger cubeTrigger { get; private set; }
	public BoxCollider cubeCollider { get; private set; }
	public Dictionary<CubeItem, SelectCube> selectDict { get; private set; }
	 
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

		GameObject gameObject = Instantiate(Resources.Load<GameObject>(ResourceDefine.MAGIC_CUBE));
		gameObject.transform.SetParent(transform);
		magicCube = gameObject.GetComponent<MagicCube>() ?? gameObject.AddComponent<MagicCube>();

		gameObject = Instantiate(Resources.Load<GameObject>(ResourceDefine.PLAYER));
		gameObject.transform.SetParent(transform);
		player = gameObject.GetComponent<Player>() ?? gameObject.AddComponent<Player>();
		
		gameObject = new GameObject("Trigger");
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
		stateMachine.EnterGlobal<GlobalState>();
		stateMachine.Enter<IdleState>();
	}

	private void Start()
	{
		distance = size + space;
		magicCube.Init(step, size, space, distance);

		List<CubeItem> cubeList = magicCube[magicCube.layer];
		CubeItem cube = cubeList[UnityEngine.Random.Range(0, cubeList.Count)];
		
		List<AxisType> axisTypes = new List<AxisType>(cube.axisDict.Keys);
		int index = UnityEngine.Random.Range(0, cube.axisDict.Count);
		AxisType upAxis = axisTypes[index];

		axisTypes = new List<AxisType>(Enum.GetValues(typeof(AxisType)) as AxisType[]);

		axisTypes.Remove(upAxis);
		axisTypes.Remove((AxisType)(-(int)upAxis));

		index = UnityEngine.Random.Range(0, axisTypes.Count);
		AxisType rightAxis = axisTypes[index];

		axisTypes.Remove(rightAxis);
		axisTypes.Remove((AxisType)(-(int)rightAxis));

		index = UnityEngine.Random.Range(0, axisTypes.Count);
		AxisType forwardAxis = axisTypes[index];

		player.SetCube(cube, rightAxis, upAxis, forwardAxis);
	}
}
