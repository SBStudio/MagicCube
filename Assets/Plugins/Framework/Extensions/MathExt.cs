using UnityEngine;

namespace Framework
{
	public static class MathExt
	{
		public static readonly System.Random random  = new System.Random();

		public static float RandomFloat(this System.Random random, float min, float max)
		{
			if (max < min)
			{
				return 0;
			}

			float number = (float)random.NextDouble() * (max - min) + min;

			return number;
		}

		public static Vector3 AngleToDirection(this float eulerAngle)
		{
			float radian = Mathf.Deg2Rad * (eulerAngle - 90);
			Vector3 direction = new Vector3(-Mathf.Cos(radian), 0, Mathf.Sin(radian)).normalized;

			return direction;
		}

		public static float DirectionToAngle(this Vector3 direction)
		{
			float angle = Vector3.Angle(new Vector3(direction.x, 0, direction.z), Vector3.forward) * (direction.x > 0 ? -1 : 1);

			return angle;
		}
	}
}