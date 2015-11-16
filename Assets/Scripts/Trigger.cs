using UnityEngine;

public class Trigger : MonoBehaviour
{
	public delegate void TriggerCallback(Collider collider);
	
	public event TriggerCallback onTriggerEnter;
	public event TriggerCallback onTriggerStay;
	public event TriggerCallback onTriggerExit;

	protected virtual void OnTriggerEnter(Collider collider)
	{
		onTriggerEnter(collider);
	}

	protected virtual void OnTriggerStay(Collider collider)
	{
		onTriggerStay(collider);
	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		onTriggerExit(collider);
	}
	
	protected virtual void OnCollisionEnter(Collision collision)
	{
		onTriggerEnter(collision.collider);
	}
	
	protected virtual void OnCollisionStay(Collision collision)
	{
		onTriggerStay(collision.collider);
	}
	
	protected virtual void OnCollisionExit(Collision collision)
	{
		onTriggerExit(collision.collider);
	}
}