using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadMansion.Utils.Math;

namespace MadMansion.AI
{
	public enum CombiningMethod
	{
		KeepLargest,
		KeepSmallest,
		Add,
		Average,
		Multiply,
		Set
	}

	/*
	 * ContextMap.cs
	 *   Provides static functions to manipulate context maps for AI. See:
	 *   http://andrewfray.wordpress.com/2013/03/26/context-behaviours-know-how-to-share/
	 *   http://www.gamedev.net/page/resources/_/technical/artificial-intelligence/smarter-steering-using-context-for-direction-r3149
	 *
	 *   In our implementation, we use float[] arrays for the context maps. We support multiple levels of detail
	 *   for the maps so that we can make distant objects do less work but still behave somewhat reasonably.
	 *
	 *   To implement LOD, make sure to ChangeMapLOD first, so that there isn't a weird jump during the first
	 *   update that the LOD is different
	 */
	public static class ContextMap
	{
		// Caches directionCount 2D unit vectors in a radial pattern starting with <0,1>
		private static Dictionary<int, Vector3[]> contextMapLODToDirections_ = new Dictionary<int, Vector3[]> ();
		public static Vector3[] GetDiscreteUnitCircleDirections (int directionCount) {
			if (contextMapLODToDirections_.ContainsKey (directionCount)) {
				return contextMapLODToDirections_ [directionCount];
			}
			Vector3[] directions = new Vector3[directionCount];

			// Adapted from this optimized OpenGL cicle-drawing routine: http://slabode.exofire.net/circle_draw.shtml
			float theta = 2f * Mathf.PI / (float)directionCount;
			float tangetialFactor = Mathf.Tan (theta);
			float radialFactor = Mathf.Cos (theta);

			//we start at angle = 0
			float x = 1f; // radius is 1
			float y = 0;

			float tx, ty;
			for (int slotIndex = 0; slotIndex < directionCount; slotIndex++) {
				directions [slotIndex] = (new Vector3 (x, 0f, y)).normalized; /* normalize since vectors aren't quite
																		 prior example magnitude: 0.9999991 */

				// calculate the tangential vector
				// the radial vector is (x, y)
				// to get the tangential vector we flip those coordinates and negate one of them
				tx = -y;
				ty = x;

				// add the tangential vector, moving around but away from the circle
				x += tx * tangetialFactor;
				y += ty * tangetialFactor;

				// correct using the radial factor, pulling back to the circle center
				x *= radialFactor;
				y *= radialFactor;
			}

			contextMapLODToDirections_ [directionCount] = directions;
			return directions;
		}

		private static void ChangeMapLOD (float[] sourceMap, float[] mapToModify) {
			// For Normal Distribution
			const float stdDev = 1f;
			const float variance = stdDev * stdDev;
			const float mean = 0.5f;
			const float denom = (2f * variance);
			const float sqrt2PI = 2.506628f;
			const float factor = 1f / (stdDev * sqrt2PI);

			int sourceLOD = sourceMap.Length;
			int destLOD = mapToModify.Length;
			float scale = ((float)sourceLOD) / ((float)destLOD);
			for (int i = 0; i < destLOD; i++) {
				int lowIndex = Mathf.FloorToInt (scale * i);
				int highIndex = Mathf.CeilToInt (scale * i);

				float weight = 0f;
				float weightSum = 0f;
				float weightedTotal = 0f;
				if (highIndex == lowIndex) {
					mapToModify [i] = sourceMap [MathUtils.Modulo (lowIndex, sourceLOD)];
					continue;
				}
				for (int j = lowIndex; j <= highIndex; j++) {
					float numSqrt = (j - lowIndex) - mean;
					// Normal Distribution (http://en.wikipedia.org/wiki/Normal_distribution)
					weight = factor * Mathf.Exp (-(numSqrt * numSqrt) / denom);
					weightSum += weight;
					weightedTotal += sourceMap [MathUtils.Modulo (j, sourceLOD)] * weight;
				}
				mapToModify [i] = weightedTotal / weightSum;
			}
		}

		public static void CombineMapValues (float[] mapToModify, float newValue, int slotIndex, CombiningMethod method) {
			switch (method) {
			case CombiningMethod.Add:
				mapToModify [slotIndex] += newValue;
				break;
			case CombiningMethod.Average:
				mapToModify [slotIndex] = (newValue + mapToModify [slotIndex]) / 2f;
				break;
			case CombiningMethod.KeepLargest:
				mapToModify [slotIndex] = Mathf.Max (newValue, mapToModify [slotIndex]);
				break;
			case CombiningMethod.KeepSmallest:
				mapToModify [slotIndex] = Mathf.Min (newValue, mapToModify [slotIndex]);
				break;
			case CombiningMethod.Multiply:
				mapToModify [slotIndex] *= newValue;
				break;
			case CombiningMethod.Set:
				mapToModify [slotIndex] = newValue;
				break;
			}
		}

		public static void InvertMap (float[] mapToModify) {
			int count = mapToModify.Length;
			for (int i = 0; i < count; i++) {
				mapToModify [i] = -mapToModify [i];
			}
		}

		public static void NormalizeMap (float[] mapToModify) {
			int count = mapToModify.Length;
			float maxVal = 0;
			float currVal;
			for (int i = 0; i < count; i++) {
				currVal = Mathf.Abs (mapToModify [i]);
				if (currVal > maxVal) {
					maxVal = currVal;
				}
			}
			if (maxVal == 0f) {
				return; // Prevent div by zero
			}

			for (int i = 0; i < count; i++) {
				mapToModify [i] /= maxVal;
			}
		}

		public static void BlendMaps (float[] lastMap, float[] currMap, float[] mapToOverwrite) {
			for (int i = 0; i < currMap.Length; i++) {
				mapToOverwrite [i] = (lastMap [i] + currMap [i]) / 2f;
			}
		}

		public static void ComponentMultiplyMaps (float[] mapA, float[] mapB, float[] mapToOverwrite) {
			for (int i = 0; i < mapToOverwrite.Length; i++) {
				mapToOverwrite [i] = mapA [i] * mapB [i];
			}
		}

		public static void ComponentAddMaps (float[] mapA, float[] mapB, float[] mapToOverwrite) {
			for (int i = 0; i < mapToOverwrite.Length; i++) {
				mapToOverwrite [i] = Mathf.Clamp01 (mapA [i] + mapB [i]);
			}
		}

		public static void VisualizeMap (float[] mapToVisualize, Vector3 center, Color color, float scale = 1f) {
			if (mapToVisualize != null && mapToVisualize.Length > 0) {
				int count = mapToVisualize.Length;
				Vector3[] directions = ContextMap.GetDiscreteUnitCircleDirections(count);
				for (int i = 0; i < count; i++) {
					Debug.DrawLine(center, center + (mapToVisualize [i] * directions [i] * scale), color);
				}
			}
		}
	}
}