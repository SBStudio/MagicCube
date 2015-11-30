using UnityEngine;

public sealed class InputStartEvent
{
	public Gesture gesture;
	public float time;
	public float deltaTime;
}

public sealed class InputMoveEvent
{
	public Gesture gesture;
	public float time;
	public float deltaTime;
}

public sealed class InputEndEvent
{
	public Gesture gesture;
	public float time;
	public float deltaTime;
}

public sealed class Gesture
{
	public int inputId;
	public Vector2 lastPosition;
	public Vector2 position;
	public Vector2 deltaPosition;
}