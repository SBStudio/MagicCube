using UnityEngine;
using System.Collections.Generic;

public static class CubeAxis
{
	public enum Axis
	{
		UP,
		DOWN,
		LEFT,
		RIGHT,
		FORWARD,
		BACK
	}
	
	public static void GetRollAxis(Transform transform, Vector3 direction, out Axis right, out Axis up, out Axis forward)
	{
		Vector3 r = Vector3.zero, u = Vector3.zero, f = Vector3.zero;
		List<Vector3> axisList = new List<Vector3>()
		{
			transform.right,
			transform.up,
			transform.forward
		};
		
		for (int i = axisList.Count; --i >= 0;)
		{
			float project1 = Vector3.Project(direction, r).magnitude;
			float project2 = Vector3.Project(direction, axisList[i]).magnitude;
			r = project1 > project2 ? r : axisList[i];
		}
		axisList.Remove(r);
		
		for (int i = axisList.Count; --i >= 0;)
		{
			float project1 = Vector3.Project(Vector3.forward, f).magnitude;
			float project2 = Vector3.Project(Vector3.forward, axisList[i]).magnitude;
			f = project1 > project2 ? f : axisList[i];
		}
		axisList.Remove(f);
		
		u = axisList[0];
		
		right = Direction2Axis(transform, r);
		up = Direction2Axis(transform, u);
		forward = Direction2Axis(transform, f);
	}
	
	public static Vector3 Axis2Direction(Transform transform, Axis axis)
	{
		if (Axis.UP == axis)
		{
			return transform.up;
		}
		else if (Axis.DOWN == axis)
		{
			return -transform.up;
		}
		else if (Axis.LEFT == axis)
		{
			return -transform.right;
		}
		else if (Axis.RIGHT == axis)
		{
			return transform.right;
		}
		else if (Axis.FORWARD == axis)
		{
			return transform.forward;
		}
		
		return -transform.forward;
	}
	
	public static Axis Direction2Axis(Transform transform, Vector3 direction)
	{
		if (transform.up == direction)
		{
			return Axis.UP;
		}
		else if (-transform.up == direction)
		{
			return Axis.DOWN;
		}
		else if (-transform.right == direction)
		{
			return Axis.LEFT;
		}
		else if (transform.right == direction)
		{
			return Axis.RIGHT;
		}
		else if (transform.forward == direction)
		{
			return Axis.FORWARD;
		}
		
		return Axis.BACK;
	}
}