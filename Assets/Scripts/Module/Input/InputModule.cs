using UnityEngine;
using Framework;
using System.Collections.Generic;

public sealed class InputModule : MonoSingleton<InputModule>
{
	private const int INPUT_ID_MOUSE = 0;
	private Dictionary<int, Gesture> m_InputDict = new Dictionary<int, Gesture>();

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
				Gesture gesture = new Gesture();
				gesture.inputId = touch.fingerId;
				gesture.position = touch.position;

				InputStartEvent evt = new InputStartEvent();
				evt.gesture = gesture;
				evt.time = Time.time;
				evt.deltaTime = Time.deltaTime;
				
				EventSystem<InputStartEvent>.Broadcast(evt);
				
				m_InputDict[gesture.inputId] = evt.gesture;
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				Gesture gesture = m_InputDict[touch.fingerId];
				if (touch.position == gesture.position)
				{
					return;
				}

				gesture.lastPosition = gesture.position;
				gesture.position = touch.position;
				gesture.deltaPosition = gesture.position - gesture.lastPosition;

				InputMoveEvent evt = new InputMoveEvent();
				evt.gesture = gesture;
				evt.time = Time.time;
				evt.deltaTime = Time.deltaTime;
				
				EventSystem<InputMoveEvent>.Broadcast(evt);
			}
			else if (touch.phase == TouchPhase.Ended
			         || touch.phase == TouchPhase.Canceled)
			{
				Gesture gesture = m_InputDict[touch.fingerId];
				gesture.lastPosition = gesture.position;
				gesture.position = touch.position;

				InputEndEvent evt = new InputEndEvent();
				evt.gesture = gesture;
				evt.time = Time.time;
				evt.deltaTime = Time.deltaTime;
				
				EventSystem<InputEndEvent>.Broadcast(evt);
				
				m_InputDict.Remove(gesture.inputId);
			}
		}
#else
		if (Input.GetMouseButtonDown(INPUT_ID_MOUSE))
		{
			Gesture gesture = new Gesture();
			gesture.inputId = INPUT_ID_MOUSE;
			gesture.position = Input.mousePosition;
			
			InputStartEvent evt = new InputStartEvent();
			evt.gesture = gesture;
			evt.time = Time.time;
			evt.deltaTime = Time.deltaTime;
			
			EventSystem<InputStartEvent>.Broadcast(evt);
			
			m_InputDict[gesture.inputId] = gesture;
		}
		else if (Input.GetMouseButton(INPUT_ID_MOUSE))
		{
			Gesture gesture = m_InputDict[INPUT_ID_MOUSE];

			Vector2 position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			if (position == gesture.position)
			{
				return;
			}

			gesture.lastPosition = gesture.position;
			gesture.position = position;
			gesture.deltaPosition = gesture.position - gesture.lastPosition;
			
			InputMoveEvent evt = new InputMoveEvent();
			evt.gesture = gesture;
			evt.time = Time.time;
			evt.deltaTime = Time.deltaTime;
			
			EventSystem<InputMoveEvent>.Broadcast(evt);
		}
		else if (Input.GetMouseButtonUp(INPUT_ID_MOUSE))
		{
			Gesture gesture = m_InputDict[INPUT_ID_MOUSE];
			gesture.lastPosition = gesture.position;
			gesture.position = Input.mousePosition;
			
			InputEndEvent evt = new InputEndEvent();
			evt.gesture = gesture;
			evt.time = Time.time;
			evt.deltaTime = Time.deltaTime;
			
			EventSystem<InputEndEvent>.Broadcast(evt);
			
			m_InputDict.Remove(gesture.inputId);
		}
#endif
	}
}