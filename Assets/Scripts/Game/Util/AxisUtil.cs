using UnityEngine;
using System.Collections.Generic;

public static class AxisUtil
{
	public static void GetRollAxis(Transform transform,
	                               Vector3 right,
	                               Vector3 forward,
	                               out AxisType rightAxis,
	                               out AxisType upAxis,
	                               out AxisType forwardAxis)
	{
		Vector3 r = default(Vector3), u = default(Vector3), f = default(Vector3);
		List<Vector3> axisList = new List<Vector3>()
		{
			transform.right,
			transform.up,
			transform.forward
		};
		
		for (int i = axisList.Count; --i >= 0;)
		{
			if (default(Vector3) == r)
			{
				r = axisList[i];

				continue;
			}

			float project1 = Vector3.Project(right, r).magnitude;
			float project2 = Vector3.Project(right, axisList[i]).magnitude;
			r = project1 > project2 ? r : axisList[i];
		}
		axisList.Remove(r);
		
		for (int i = axisList.Count; --i >= 0;)
		{
			if (default(Vector3) == f)
			{
				f = axisList[i];
				
				continue;
			}

			float project1 = Vector3.Project(forward, f).magnitude;
			float project2 = Vector3.Project(forward, axisList[i]).magnitude;
			f = project1 > project2 ? f : axisList[i];
		}
		axisList.Remove(f);
		
		u = axisList[0];
		
		rightAxis = Direction2Axis(transform, r);
		upAxis = Direction2Axis(transform, u);
		forwardAxis = Direction2Axis(transform, f);
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