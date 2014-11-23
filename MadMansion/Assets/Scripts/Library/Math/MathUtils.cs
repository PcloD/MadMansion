using UnityEngine;
using System.Collections;

namespace MadMansion.Utils.Math
{
	public static class MathUtils
	{
		public static int Modulo (int x, int m)
		{
			return x < 0 ? ((x % m) + m) % m : x % m;
		}

		// In 2D, we usually want to use transform.up for fromDir
		// The result can be multiplied by a torqueForce for rotation
		public static float GetSignedAngleFromDirToDir (Vector3 fromDir, Vector3 targetDir)
		{
			var angle = Vector3.Angle (fromDir, targetDir);
			Vector3 perp = Vector3.Cross (fromDir, targetDir);
			int direction = (int)perp.normalized.z;
			return angle * direction;
		}

		// Slightly faster, but not quite as nice in that it doesn't return 0 as often
		public static float GetSignedAngleFromDirToDirFast (Vector3 fromDir, Vector3 targetDir)
		{
			var angle = Vector3.Angle (fromDir, targetDir);
			int direction = System.Math.Sign (((fromDir.x * targetDir.y) - (fromDir.y * targetDir.x)));
			return angle * direction;
		}

		//take an angle that is represented from 0-360 and return its -180-180 equivalent
		public static float Convert360AngleTo180(float angle)
		{
			if(-180.0f < angle && angle < 180.0f)
				return angle;
			else
				return angle - 360.0f;
		}
	}
}