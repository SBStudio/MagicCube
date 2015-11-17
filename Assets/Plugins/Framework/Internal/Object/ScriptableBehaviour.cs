using UnityEngine;

public abstract class ScriptableBehaviour : ScriptableObject
{
	public virtual void OnGUI()
	{
	}

	public virtual void OnUpdate()
	{
	}
	
	public virtual void OnLateUpdate()
	{
	}
	
	public virtual void OnFixedUpdate()
	{
	}

	public virtual void OnDestroy()
	{
	}
}