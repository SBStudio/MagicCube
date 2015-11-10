using UnityEngine;

namespace Framework
{
	public abstract class IState : ScriptableBehaviour
	{
		public virtual void OnEnter()
		{
		}

		public virtual void OnExit()
		{
		}

		public virtual bool OnCondition(IState nextState)
		{
			return this != nextState;
		}
	}
}