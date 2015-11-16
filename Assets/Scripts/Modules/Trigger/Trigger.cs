using UnityEngine;

public class Trigger : MonoBehaviour
{
	public delegate void TriggerCallback(Collider collider);
	
	public event TriggerCallback onTriggerEnter;
	public event TriggerCallback onTriggerStay;
	public event TriggerCallback onTriggerExit;

	protected virtual void OnTriggerEnter(Collider collider)
	{
		if (null != onTriggerEnter)
		{
			onTriggerEnter(collider);
		}
	}

	protected virtual void OnTriggerStay(Collider collider)
	{
		if (null != onTriggerStay)
		{
			onTriggerStay(collider);
		}
	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		if (null != onTriggerExit)
		{
			onTriggerExit(collider);
		}
	}
	
	protected virtual void OnCollisionEnter(Collision collision)
	{
		if (null != onTriggerEnter)
		{
			onTriggerEnter(collision.collider);
		}
	}
	
	protected virtual void OnCollisionStay(Collision collision)
	{
		if (null != onTriggerStay)
		{
			onTriggerStay(collision.collider);
		}
	}
	
	protected virtual void OnCollisionExit(Collision collision)
	{
		if (null != onTriggerExit)
		{
			onTriggerExit(collision.collider);
		}
	}
}