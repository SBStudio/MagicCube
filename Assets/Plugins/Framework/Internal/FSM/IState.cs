using UnityEngine;

namespace Framework
{
	public abstract class IState : ScriptableObject
	{
		public StateMachine stateMachine;

		public virtual void OnEnter()
		{
		}

		public virtual void OnExit()
		{
		}

		public virtual void OnUpdate()
		{
		}

		public virtual bool OnCondition(IState nextState)
		{
			return true;
		}
	}
}