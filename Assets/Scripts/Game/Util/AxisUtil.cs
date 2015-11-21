using UnityEngine;
using System.Collections.Generic;

public static class AxisUtil
{
	public static void GetRollAxis(Transform transform,
	                               Vector3 direction,
	                               out AxisType rightAxis,
	                               out AxisType upAxis,
	                               out AxisType forwardAxis)
	{
		Vector3 right = Vector3.zero, up = Vector3.zero, forward = Vector3.zero;
		List<Vector3> axisList = new List<Vector3>()
		{
			transform.right,
			transform.up,
			transform.forward
		};
		
		for (int i = axisList.Count; --i >= 0;)
		{
			float project1 = Vector3.Project(direction, right).magnitude;
			float project2 = Vector3.Project(direction, axisList[i]).magnitude;
			right = project1 > project2 ? right : axisList[i];
		}
		axisList.Remove(right);
		
		for (int i = axisList.Count; --i >= 0;)
		{
			float project1 = Vector3.Project(Vector3.forward, forward).magnitude;
			float project2 = Vector3.Project(Vector3.forward, axisList[i]).magnitude;
			forward = project1 > project2 ? forward : axisList[i];
		}
		axisList.Remove(forward);
		
		up = axisList[0];
		
		rightAxis = Direction2Axis(transform, right);
		upAxis = Direction2Axis(transform, up);
		forwardAxis = Direction2Axis(transform, forward);
	}
	
	public static Vector3 Axis2Direction(Transform transform, AxisType axisType)
	{
		if (AxisType.UP == axisType)
		{
			return transform.up;
		}
		else if (AxisType.DOWN == axisType)
		{
			return -transform.up;
		}
		else if (AxisType.LEFT == axisType)
		{
			return -transform.right;
		}
		else if (AxisType.RIGHT == axisType)
		{
			return transform.right;
		}
		else if (AxisType.FORWARD == axisType)
		{
			return transform.forward;
		}
		
		return -transform.forward;
	}
	
	public static AxisType Direction2Axis(Transform transform, Vector3 direction)
	{
		if (transform.up == direction)
		{
			return AxisType.UP;
		}
		else if (-transform.up == direction)
		{
			return AxisType.DOWN;
		}
		else if (-transform.right == direction)
		{
			return AxisType.LEFT;
		}
		else if (transform.right == direction)
		{
			return AxisType.RIGHT;
		}
		else if (transform.forward == direction)
		{
			return AxisType.FORWARD;
		}
		
		return AxisType.BACK;
	}
}