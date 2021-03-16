#pragma warning disable CA2235 // Mark all non-serializable fields

using System;
using UnityEngine;

namespace MackySoft.Vision {

	/// <summary>
	/// <para> Definition of <see cref="CullingGroupKey"/>. It can be defined in <see cref="VisionSettings"/>. </para>
	/// <para> Basically, it is not used directly by the user. </para>
	/// </summary>
	[Serializable]
	public class CullingGroupKeyDefinition {

		[SerializeField]
		string m_Name;

		public string Name { get => m_Name; set => m_Name = value; }

	}

	/// <summary>
	/// <para> Key to find the <see cref="CullingGroupProxy"/>. The essence of a key is an index. </para>
	/// <para> The keys are defined in <see cref="VisionSettings"/>. </para>
	/// </summary>
	[Serializable]
	public struct CullingGroupKey : IEquatable<CullingGroupKey> {

		/// <summary>
		/// Key with index -1.
		/// </summary>
		public static readonly CullingGroupKey None = new CullingGroupKey(-1);

		[SerializeField]
		int m_Index;

		public CullingGroupKey (int index) {
			m_Index = index;
		}

		public bool Equals (CullingGroupKey other) {
			return m_Index == other.m_Index;
		}

		public override bool Equals (object obj) {
			return (obj != null) && (obj is CullingGroupKey other) && Equals(other);
		}

		public override int GetHashCode () {
			return m_Index.GetHashCode();
		}

		public static bool operator == (CullingGroupKey left,CullingGroupKey right) {
			return left.Equals(right);
		}

		public static bool operator != (CullingGroupKey left,CullingGroupKey right) {
			return !(left == right);
		}

		public static implicit operator int (CullingGroupKey source) {
			return source.m_Index;
		}

		public static implicit operator CullingGroupKey (int source) {
			return new CullingGroupKey(source);
		}

	}

}