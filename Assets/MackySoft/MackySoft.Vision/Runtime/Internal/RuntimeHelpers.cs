#pragma warning disable CA1810 // Initialize reference type static fields inline

using System;
using UnityEngine;

namespace MackySoft.Vision.Internal {
	internal static class RuntimeHelpers {

		public static bool IsWellKnownNoReferenceContainsType<T> () {
			return WellKnownNoReferenceContainsType<T>.IsWellKnownType;
		}

		static bool WellKnownNoReferenceContainsTypeInitialize (Type t) {
			if (t.IsPrimitive) {
				return true;
			}
			if (t.IsEnum) { return true; }
			if (t == typeof(DateTime)) { return true; }
			if (t == typeof(DateTimeOffset)) { return true; }
			if (t == typeof(Guid)) { return true; }
			if (t == typeof(decimal)) { return true; }

			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)) {
				return WellKnownNoReferenceContainsTypeInitialize(t.GetGenericArguments()[0]);
			}

			if (t == typeof(Vector2)) { return true; }
			if (t == typeof(Vector3)) { return true; }
			if (t == typeof(Vector4)) { return true; }
			if (t == typeof(Vector2Int)) { return true; }
			if (t == typeof(Vector3Int)) { return true; }
			if (t == typeof(Rect)) { return true; }
			if (t == typeof(RectInt)) { return true; }
			if (t == typeof(Bounds)) { return true; }
			if (t == typeof(BoundsInt)) { return true; }
			if (t == typeof(Quaternion)) { return true; }
			if (t == typeof(Color)) { return true; }

			return false;
		}

		static class WellKnownNoReferenceContainsType<T> {

			public static readonly bool IsWellKnownType;

			static WellKnownNoReferenceContainsType () {
				IsWellKnownType = WellKnownNoReferenceContainsTypeInitialize(typeof(T));
			}
		}
	}
}