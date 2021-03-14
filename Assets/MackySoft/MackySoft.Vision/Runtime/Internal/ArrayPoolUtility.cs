using System;

namespace MackySoft.Vision.Internal {
	internal static class ArrayPoolUtility {

		public static void EnsureCapacity<T> (ref T[] array,int index) {
			if (array.Length <= index) {
				int newSize = array.Length * 2;
				T[] newArray = ArrayPool<T>.Rent((index < newSize) ? newSize : (index * 2));
				Array.Copy(array,0,newArray,0,array.Length);

				ArrayPool<T>.Return(array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());

				array = newArray;
			}
		}

	}
}