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
	public Vector3 lastPosition;
	public Vector3 position;
	public Vector3 deltaPosition;
}