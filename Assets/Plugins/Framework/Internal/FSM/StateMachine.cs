using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework
{
	public sealed class StateMachine
	{
		private Dictionary<Type, IState> m_StateDict = new Dictionary<Type, IState>();

		public IState globalState { get; private set; }
		public IState state { get; private set; }
		public IState prevState { get; private set; }
		public bool isRunning { get; private set; }

		private void OnEnable()
		{
			Run();
		}

		public void OnUpdate()
		{
			if (null != state && isRunning)
			{
				if (null != globalState)
				{
					globalState.OnUpdate();
				}

				state.OnUpdate();
			}
		}

		private void OnDestroy()
		{
			if (null != globalState)
			{
				globalState.OnExit();
			}

			foreach (IState state in m_StateDict.Values)
			{
				state.OnExit();
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
			state.stateMachine = this;

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

		public void Run()
		{
			isRunning = true;
		}

		public void Pause()
		{
			isRunning = false;
		}
	}
}