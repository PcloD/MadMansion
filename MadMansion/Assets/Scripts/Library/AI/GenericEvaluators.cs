using UnityEngine;
using System.Collections;

namespace MadMansion.AI
{
	public static class GenericEvaluators {
		public static void ConstantEvaluator (float[] mapToModify, CombiningMethod combiningMethod, float value = 1f) {
			int count = mapToModify.Length;
			for (int i = 0; i < count; i++) {
				ContextMap.CombineMapValues(mapToModify, value, i, combiningMethod);
			}
		}

		public static void TargetEvaluator(float[] mapToModify, CombiningMethod combiningMethod,
		                                   Vector3 origin, Vector3 target) {
			int count = mapToModify.Length;
			Vector3[] directions = ContextMap.GetDiscreteUnitCircleDirections (count);
			for (int i = 0; i < count; i++) {
				float newVal = Vector3.Dot(directions[i].normalized, (target - origin).normalized);
				newVal = Mathf.Clamp(newVal, 0.3f, 1f);
				ContextMap.CombineMapValues(mapToModify, newVal, i, combiningMethod);
			}
			ContextMap.NormalizeMap(mapToModify);
		}
	}
}