using System;
using System.Collections;
using System.Collections.Generic;

namespace MackySoft.Vision.Internal {

	/// <summary>
	/// <para> Temporary array without allocation. </para>
	/// <para> This struct use <see cref="ArrayPool{T}"/> internally to avoid allocation and can be used just like a normal array. </para>
	/// <para> After using it, please call the Dispose(). </para>
	/// </summary>
	internal struct TemporaryArray<T> : IEnumerable<T>, IDisposable {

		/// <summary>
		/// Create a temporary array of the specified length.
		/// </summary>
		public static TemporaryArray<T> Create (int length) {
			return new TemporaryArray<T>(ArrayPool<T>.Rent(length),length);
		}

		/// <summary>
		/// <para> Create a temporary array with a length of 0. </para>
		/// <para> The length can be increased by using the <see cref="Add(T)"/>. </para>
		/// </summary>
		/// <param name="prepare"> Length of the internal array to be prepared. </param>
		public static TemporaryArray<T> CreateAsList (int prepare) {
			return new TemporaryArray<T>(ArrayPool<T>.Rent(prepare),0);
		}

		/// <summary>
		/// Create a temporary array from the elements of <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public static TemporaryArray<T> From (IEnumerable<T> source) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}

			T[] array = source.ToArrayFromPool(out int count);
			return new TemporaryArray<T>(array,count);
		}

		public static TemporaryArray<T> From (TemporaryArray<T> source) {
			var result = Create(source.Length);
			for (int i = 0;source.Length > i;i++) {
				result[i] = source[i];
			}
			return result;
		}

		T[] m_Array;
		int m_Length;

		public int Length => m_Length;

		/// <summary>
		/// Length of internal array.
		/// </summary>
		public int Capacity => m_Array.Length;

		/// <summary>
		/// <para> Internal array. </para>
		/// <para> The length of internal array is always greater than or equal to <see cref="Length"/> property. </para>
		/// </summary>
		public T[] Array => m_Array;

		public T this[int index] {
			get => index >= 0 && index < m_Length ? m_Array[index] : throw new IndexOutOfRangeException();
			set => m_Array[index] = value;
		}

		public TemporaryArray (T[] array,int length) {
			m_Array = array;
			m_Length = length;
		}

		public bool Contains (T item) {
			if (item == null) {
				for (int i = 0;i < m_Length;i++) {
					if (m_Array[i] == null) {
						return true;
					}
				}
				return false;
			} else {
				var comparer = EqualityComparer<T>.Default;
				for (int i = 0;i < m_Length;i++) {
					if (comparer.Equals(m_Array[i],item)) {
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Set item to current length and increase length.
		/// </summary>
		public void Add (T item) {
			ArrayPoolUtility.EnsureCapacity(ref m_Array,m_Length);
			m_Array[m_Length] = item;
			m_Length++;
		}

		public bool RemoveAt (int index) {
			if (index >= m_Length) {
				return false;
			}
			m_Length--;
			for (int i = index;i < m_Length;i++) {
				m_Array[i] = m_Array[i + 1];
			}
			return true;
		}

		public void Clear (bool clearArray = false) {
			ArrayPool<T>.Return(m_Array,clearArray);

			m_Array = ArrayPool<T>.Rent(0);
			m_Length = 0;
		}

		public void Dispose () {
			Dispose(!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());
		}

		public void Dispose (bool clearArray) {
			ArrayPool<T>.Return(ref m_Array,clearArray);
			m_Length = 0;
		}

		public IEnumerator<T> GetEnumerator () {
			for (int i = 0;m_Length > i;i++) {
				yield return m_Array[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return GetEnumerator();
		}
	}

	internal static class TemporaryArrayExtensions {

		/// <summary>
		/// Create a temporary array from the elements of <see cref="IEnumerable{T}"/>.
		/// </summary>
		public static TemporaryArray<T> ToTemporaryArray<T> (this IEnumerable<T> source) {
			return TemporaryArray<T>.From(source);
		}

	}
}