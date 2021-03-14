using UnityEngine;

namespace MackySoft.Vision {

	public enum TransformUpdateMode {
		Dynamic = 0,
		Static = 1
	}

	public interface ICullingTarget {

		/// <summary>
		/// <para> When to update the bounding sphere transform. </para>
		/// </summary>
		TransformUpdateMode BoundingSphereUpdateMode { get; }

		/// <summary>
		/// <para> Returns the bounding sphere that was last updated. </para>
		/// <para> To get the latest bounding sphere, use <see cref="UpdateAndGetBoundingSphere"/> instead. </para>
		/// </summary>
		BoundingSphere BoundingSphere { get; }

		/// <summary>
		/// Sets the callback that will be called when a sphere visibility and/or distance state has changed.
		/// </summary>
		CullingGroup.StateChanged OnStateChanged { get; set; }

		/// <summary>
		/// <para> Update the bounding sphere to the latest state and returns it. </para>
		/// </summary>
		/// <returns> The bounding sphere updated to the latest state. </returns>
		BoundingSphere UpdateAndGetBoundingSphere ();

		/// <summary>
		/// Returns true if the bounding sphere is currently visible from any of the contributing cameras.
		/// </summary>
		/// <returns> True if the bounding sphere is visible, false if it is invisible. </returns>
		bool IsVisible ();

		/// <summary>
		/// Get the current distance band index of the bounding sphere.
		/// </summary>
		/// <returns> The sphere current distance band index. </returns>
		int GetDistance ();

	}

}