using UnityEngine;
using Framework;
using System.Collections.Generic;

public sealed class InputModule : MonoSingleton<InputModule>
{
	private const int INPUT_ID_MOUSE = 0;
	private Dictionary<int, InputEvent> m_InputDict = new Dictionary<int, InputEvent>();

	private void Update()
	{
		UpdateInput();
	}

	private void UpdateInput()
	{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
		if (0 >= Input.touchCount)
		{
			return;
		}
		
		for (int i = Input.touchCount; --i >= 0;)
		{
			Touch touch = Input.GetTouch(i);
			
			if (touch.phase == TouchPhase.Began)
			{
				InputEvent evt = new InputEvent();
				evt.inputId = touch.fingerId;
				evt.inputType = InputEvent.InputType.InputStart;
				evt.position = touch.position;
				
				EventSystem<InputEvent>.Broadcast(evt);
				
				m_InputDict[evt.inputId] = evt;
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				InputEvent evt = m_InputDict[touch.fingerId];
				if (touch.position == evt.position)
				{
					return;
				}
				
				evt.inputType = InputEvent.InputType.InputMove;
				evt.lastPosition = evt.position;
				evt.position = touch.position;
				evt.deltaPosition = evt.position - evt.lastPosition;
				
				EventSystem<InputEvent>.Broadcast(evt);
			}
			else
			{
				InputEvent evt = m_InputDict[touch.fingerId];
				evt.inputType = InputEvent.InputType.InputEnd;
				evt.lastPosition = evt.position;
				evt.position = touch.position;
				
				EventSystem<InputEvent>.Broadcast(evt);
				
				m_InputDict.Remove(evt.inputId);
			}
		}
#else
		if (Input.GetMouseButtonDown(INPUT_ID_MOUSE))
		{
			InputEvent evt = new InputEvent();
			evt.inputId = INPUT_ID_MOUSE;
			evt.inputType = InputEvent.InputType.InputStart;
			evt.position = Input.mousePosition;
			
			EventSystem<InputEvent>.Broadcast(evt);
			
			m_InputDict[evt.inputId] = evt;
		}
		else if (Input.GetMouseButton(INPUT_ID_MOUSE))
		{
			InputEvent evt = m_InputDict[INPUT_ID_MOUSE];
			if (new Vector2(Input.mousePosition.x, Input.mousePosition.y) == evt.position)
			{
				return;
			}

			evt.inputType = InputEvent.InputType.InputMove;
			evt.lastPosition = evt.position;
			evt.position = Input.mousePosition;
			evt.deltaPosition = evt.position - evt.lastPosition;
			
			EventSystem<InputEvent>.Broadcast(evt);
		}
		else if (Input.GetMouseButtonUp(INPUT_ID_MOUSE))
		{
			InputEvent evt = m_InputDict[INPUT_ID_MOUSE];
			evt.inputType = InputEvent.InputType.InputEnd;
			evt.lastPosition = evt.position;
			evt.position = Input.mousePosition;
			
			EventSystem<InputEvent>.Broadcast(evt);
			
			m_InputDict.Remove(evt.inputId);
		}
#endif
	}
}