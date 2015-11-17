using UnityEngine;

public static class LayerDefine
{
	public const int CUBE = 8;
}

public static class GameDefine
{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
	public const int FPS = 30;
#else
	public const int FPS = 60;
#endif
}