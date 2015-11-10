using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	public sealed class StateMachine : MonoBehaviour
	{
		private Dictionary<Type, IState> m_StateDict = new Dictionary<Type, IState>();

		public IState globalState { get; private set; }
		public IState state { get; private set; }
		public IState prevState { get; private set; }

		private void Update()
		{
			if (null != globalState)
			{
				globalState.OnUpdate();
			}

			if (null != state)
			{
				
				state.OnUpdate();
			}
		}
		
		private void LateUpdate()
		{
			if (null != globalState)
			{
				globalState.OnLateUpdate();
			}

			if (null != state)
			{
				state.OnLateUpdate();
			}
		}
		
		private void FixedUpdate()
		{
			if (null != globalState)
			{
				globalState.OnFixedUpdate();
			}

			if (null != state)
			{
				
				state.OnFixedUpdate();
			}
		}

		private void OnGUI()
		{
			if (null != globalState)
			{
				globalState.OnGUI();
			}

			if (null != state)
			{
				
				state.OnGUI();
			}
		}

		private void OnDestroy()
		{
			if (null != globalState)
			{
				globalState.OnDestroy();
				globalState = null;
			}

			foreach (IState state in m_StateDict.Values)
			{
				state.OnDestroy();
			}

			m_StateDict.Clear();
			m_StateDict = null;
		}

		public bool Enter<T>() where T : IState
		{
			if (!Contains<T>())
			{
				return false;
			}
			
			Type type = typeof(T);
			IState nextState = m_StateDict[type];
			if (null != state)
			{
				if (!state.OnCondition(nextState))
				{
					return false;
				}
				
				state.OnExit();
			}

			prevState = state;
			state = nextState;
			state.OnEnter();
			
			return true;
		}

		public void Add<T>() where T : IState
		{
			if (Contains<T>())
			{
				return;
			}
			
			Type type = typeof(T);
			IState state = ScriptableObject.CreateInstance<T>();

			m_StateDict.Add(type, state);
		}

		public T Get<T>() where T : IState
		{
			if (!Contains<T>())
			{
				return null;
			}
			
			Type type = typeof(T);
			T state = m_StateDict[type] as T;

			return state;
		}

		public void Remove<T>() where T : IState
		{
			if (!Contains<T>())
			{
				return;
			}

			Type type = typeof(T);
			IState state = m_StateDict[type];

			state.OnExit();
			
			if (this.state == state)
			{
				this.state = null;
			}
			
			m_StateDict.Remove(type);
		}

		public bool Contains<T>() where T : IState
		{
			Type type = typeof(T);

			return m_StateDict.ContainsKey(type);
		}
	}
}