using UnityEngine;

public sealed class InputEvent
{
	public enum InputType
	{
		InputStart,
		InputMove,
		InputEnd,
	}
	
	public int inputId;
	public InputType inputType;
	public Vector2 lastPosition;
	public Vector2 position;
	public Vector2 deltaPosition;
	public float deltaTime;
}