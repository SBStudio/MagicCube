using UnityEngine;
using Framework;

public class NewBehaviourScript : MonoBehaviour
{
	private void Start()
	{
		FPSInfo fpsInfo = DebugUtil.Add<FPSInfo>();

		TimerBehaviour timer = TimerUtil.Begin(OnTimer, 0, 1, 0, "asdf");

		LogUtil.printType = LogUtil.PrintType.Screen;
	}

	private void OnTimer(object args)
	{
		LogUtil.LogError(args.ToString() + Time.time);
	}
}