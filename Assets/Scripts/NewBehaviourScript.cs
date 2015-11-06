using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Framework.FPSInfo fpsInfo = Framework.DebugUtil.Add<Framework.FPSInfo>();
		fpsInfo.showDetail = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
