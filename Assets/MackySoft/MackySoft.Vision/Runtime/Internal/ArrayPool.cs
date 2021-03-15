#pragma warning disable CA1810 // Initialize reference type static fields inline
#pragma warning disable IDE0066 // switch ステートメントを式に変換します

using System;
using System.Threading;
using System.Collections.Generic;

namespace MackySoft.Vision.Internal {

	internal static class ArrayPool<T> {

		const int k_DefaultMaxNumberOfArraysPerBucket = 50;

		static readonly T[] m_Empty = Array.Empty<T>();
		static readonly Stack<T[]>[] m_Pool;
		static readonly SpinLock[] m_Locks;

		static ArrayPool () {
			m_Pool = new Stack<T[]>[18];
			m_Locks = new SpinLock[18];
			for (int i = 0;m_Pool.Length > i;i++) {
				m_Locks[i] = new SpinLock(false);
			}
		}
		
		/// <summary>
		/// The array length is not always accurate.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static T[] Rent (int minimumLength) {
			if (minimumLength < 0) {
				throw new ArgumentOutOfRangeException(nameof(minimumLength));
			}
			else if (minimumLength == 0) {
				return m_Empty;
			}

			int size = CalculateArraySize(minimumLength);
			int poolIndex = GetPoolIndex(size);

			if (m_Pool[poolIndex] == null) {
				m_Pool[poolIndex] = new Stack<T[]>();
			}

			var pool = m_Pool[poolIndex];
			bool lockTaken = false;
			try {
				m_Locks[poolIndex].Enter(ref lockTaken);
				if (pool.Count != 0) {
					return pool.Pop();
				}
			}
			finally {
				if (lockTaken) {
					m_Locks[poolIndex].Exit(false);
				}
			}
			
			return new T[size];
		}

		/// <summary>
		/// <para> Return the array to the pool. </para>
		/// <para> The length of the array must be greater than or equal to 8 and a power of 2. </para>
		/// </summary>
		/// <param name="array"> The length of the array must be greater than or equal to 8 and a power of 2. </param>
		public static void Return (T[] array,bool clearArray = false) {
			if ((array == null) || (array.Length == 0)) {
				return;
			}

			int poolIndex = GetPoolIndex(array.Length);
			if (poolIndex == -1) {
				return;
			}

			if (m_Pool[poolIndex] == null) {
				m_Pool[poolIndex] = new Stack<T[]>();
			}

			if (clearArray) {
				Array.Clear(array,0,array.Length);
			}

			var pool = m_Pool[poolIndex];
			bool lockTaken = false;
			try {
				m_Locks[poolIndex].Enter(ref lockTaken);

				if (pool.Count > k_DefaultMaxNumberOfArraysPerBucket) {
					return;
				}
				pool.Push(array);
			}
			finally {
				if (lockTaken) {
					m_Locks[poolIndex].Exit(false);
				}
			}
		}

		/// <summary>
		/// <para> Return the array to the pool and set array reference to null. </para>
		/// <para> The length of the array must be greater than or equal to 8 and a power of 2. </para>
		/// </summary>
		/// <param name="array"> The length of the array must be greater than or equal to 8 and a power of 2. </param>
		public static void Return (ref T[] array,bool clearArray = false) {
			Return(array,clearArray);
			array = null;
		}

		static int CalculateArraySize (int size) {
			size--;
			size |= size >> 1;
			size |= size >> 2;
			size |= size >> 4;
			size |= size >> 8;
			size |= size >> 16;
			size += 1;

			if (size < 8) {
				size = 8;
			}
			return size;
		}

		static int GetPoolIndex (int length) {

			switch (length) {
				case 8: return 0;
				case 16: return 1;
				case 32: return 2;
				case 64: return 3;
				case 128: return 4;
				case 256: return 5;
				case 512: return 6;
				case 1024: return 7;
				case 2048: return 8;
				case 4096: return 9;
				case 8192: return 10;
				case 16384: return 11;
				case 32768: return 12;
				case 65536: return 13;
				case 131072: return 14;
				case 262144: return 15;
				case 524288: return 16;
				case 1048576: return 17;
				default: return -1;
			}
		}

	}

	internal static class ArrayPoolExtensions {

		/// <summary>
		/// <para> Convert enumerable to array. Array are returned from <see cref="ArrayPool{T}"/>. </para>
		/// <para> The array length is not always accurate. </para>
		/// </summary>
		public static T[] ToArrayFromPool<T> (this IEnumerable<T> source) {
			return ToArrayFromPool(source,out _);
		}

		/// <summary>
		/// <para> Convert enumerable to array. Array are returned from <see cref="ArrayPool{T}"/>. </para>
		/// <para> The array length is not always accurate. </para>
		/// </summary>
		/// <param name="count"> Number of elements in source. </param>
		public static T[] ToArrayFromPool<T> (this IEnumerable<T> source,out int count) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}

			// Tries to cast source to the collection interfaces.
			if (source is IList<T> list) {
				count = list.Count;
				return ToArrayFromPoolInternal(list);
			}
			if (source is IReadOnlyList<T> readonlyList) {
				count = readonlyList.Count;
				return ToArrayFromPoolInternal(readonlyList);
			}
			if (source is ICollection<T> collection) {
				count = collection.Count;
				return ToArrayFromPoolInternal(collection);
			}
			if (source is IReadOnlyCollection<T> readonlyCollection) {
				count = readonlyCollection.Count;
				return ToArrayFromPoolInternal(readonlyCollection);
			}

			T[] array = ArrayPool<T>.Rent(32);
			count = 0;
			foreach (T item in source) {
				ArrayPoolUtility.EnsureCapacity(ref array,count);
				array[count] = item;
				count++;
			}
			return array;
		}

		static T[] ToArrayFromPoolInternal<T> (IReadOnlyList<T> source) {
			T[] array = ArrayPool<T>.Rent(source.Count);
			for (int i = 0;source.Count > i;i++) {
				array[i] = source[i];
			}
			return array;
		}

		static T[] ToArrayFromPoolInternal<T> (IList<T> source) {
			T[] array = ArrayPool<T>.Rent(source.Count);
			source.CopyTo(array,0);
			return array;
		}

		static T[] ToArrayFromPoolInternal<T> (IReadOnlyCollection<T> source) {
			T[] array = ArrayPool<T>.Rent(source.Count);
			int i = 0;
			foreach (T item in source) {
				array[i] = item;
				i++;
			}
			return array;
		}

		static T[] ToArrayFromPoolInternal<T> (ICollection<T> source) {
			T[] array = ArrayPool<T>.Rent(source.Count);
			source.CopyTo(array,0);
			return array;
		}

	}
}