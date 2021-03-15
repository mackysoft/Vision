#pragma warning disable CA1819 // Properties should not return arrays

using System;
using System.Collections.Generic;
using UnityEngine;
using MackySoft.Vision.Internal;

namespace MackySoft.Vision {

	public enum CullingTargetsUpdateMode {
		/// <summary>
		/// A targets will be updated automatically at every update.
		/// </summary>
		EveryUpdate = 0,

		/// <summary>
		/// To update the targets, you need to explicitly call the update method.
		/// </summary>
		Manual = 1
	}

	/// <summary>
	/// Relay component for easy access to the CullingGroup API.
	/// </summary>
	[AddComponentMenu("MackySoft/Vision/Culling Group Proxy")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class CullingGroupProxy : MonoBehaviour {
		
		#region Variables

		const int k_BoundingSphereArrayMinLength = 16;

		static readonly CullingGroupProxy[] m_Groups = new CullingGroupProxy[16];
		static readonly float[] k_InfinityDistances = new float[] { 0f, float.PositiveInfinity };

		[Tooltip("A key to find this CullingGroupProxy.")]
		[SerializeField]
		CullingGroupKey m_Key = CullingGroupKey.None;

		[Tooltip("When to update the targets.")]
		[SerializeField]
		CullingTargetsUpdateMode m_TargetsUpdateMode = CullingTargetsUpdateMode.EveryUpdate;

		[Tooltip("Looks the CullingGroup to a specific camera.")]
		[SerializeField]
		Camera m_TargetCamera;

		[Tooltip("A transform to measure the distance from. The transform's position will be automatically tracked.")]
		[SerializeField]
		Transform m_DistanceReferencePoint;

		[Tooltip("An array of bounding distances. The distances should be sorted in increasing order.")]
		[SerializeField]
		float[] m_BoundingDistances = Array.Empty<float>();

		CullingGroup m_CullingGroup;
		BoundingSphere[] m_BoundingSpheres = Array.Empty<BoundingSphere>();

		readonly List<ICullingTarget> m_Targets = new List<ICullingTarget>();
		readonly HashSet<ICullingTarget> m_TargetsToAdd = new HashSet<ICullingTarget>();
		readonly HashSet<ICullingTarget> m_TargetsToRemove = new HashSet<ICullingTarget>();
		TemporaryArray<int> m_DynamicTargetIndices;

		#endregion

		#region Properties

		/// <summary>
		/// A key to find this CullingGroupProxy.
		/// </summary>
		public CullingGroupKey Key {
			get => m_Key;
		}

		/// <summary>
		/// <para> When to update the targets. </para>
		/// <para> If is <see cref="CullingTargetsUpdateMode.Manual"/>, you need to call the <see cref="UpdateTargets"/> method to update the targets. </para>
		/// </summary>
		public CullingTargetsUpdateMode TargetsUpdateMode {
			get => m_TargetsUpdateMode;
			set => m_TargetsUpdateMode = value;
		}

		/// <summary>
		/// Looks the CullingGroup to a specific camera.
		/// </summary>
		public Camera TargetCamera {
			get => m_TargetCamera;
			set {
				m_TargetCamera = value;
				if (m_CullingGroup != null) {
					m_CullingGroup.targetCamera = m_TargetCamera;
				}
			}
		}

		/// <summary>
		/// A transform to measure the distance from. The transform's position will be automatically tracked.
		/// </summary>
		public Transform DistanceReferencePoint {
			get => m_DistanceReferencePoint;
			set {
				m_DistanceReferencePoint = value;
				m_CullingGroup?.SetDistanceReferencePoint(m_DistanceReferencePoint);
			}
		}

		/// <summary>
		/// An array of bounding distances. The distances should be sorted in increasing order.
		/// </summary>

		public float[] BoundingDistances {
			get => m_BoundingDistances;
			set {
				m_BoundingDistances = value;
				m_CullingGroup?.SetBoundingDistances(m_BoundingDistances?.Length > 0 ? m_BoundingDistances : k_InfinityDistances);
			}
		}

		public IReadOnlyList<ICullingTarget> Targets => m_Targets;

		public IReadOnlyList<BoundingSphere> BoundingSpheres => m_BoundingSpheres;

		#endregion

		#region Events

		/// <summary>
		/// Sets the callback that will be called when a sphere's visibility and/or distance state has changed.
		/// </summary>
		public event CullingGroup.StateChanged OnStateChanged {
			add => m_CullingGroup.onStateChanged += value;
			remove => m_CullingGroup.onStateChanged -= value;
		}

		#endregion

		#region Behaviour

		void Awake () {
			m_CullingGroup = new CullingGroup {
				targetCamera = m_TargetCamera,
				onStateChanged = ev => m_Targets[ev.index].OnStateChanged?.Invoke(ev)
			};
			m_CullingGroup.SetBoundingDistances(m_BoundingDistances?.Length > 0 ? m_BoundingDistances : k_InfinityDistances);
			m_CullingGroup.SetDistanceReferencePoint(m_DistanceReferencePoint);
		}

		void OnEnable () {
			m_CullingGroup.enabled = true;

			if (m_Key != CullingGroupKey.None) {
				CullingGroupProxy current = m_Groups[m_Key];
				if ((current != null) && (current != this)) {
					throw new InvalidOperationException($"There is already a {nameof(CullingGroupProxy)} with the same key.");
				}
				m_Groups[m_Key] = this;
			}
		}

		void OnDisable () {
			if ((m_Key != CullingGroupKey.None) && (m_Groups[m_Key] == this)) {
				m_Groups[m_Key] = null;
			}
			
			m_CullingGroup.enabled = false;
		}

		void OnDestroy () {
			m_DynamicTargetIndices.Dispose();
			m_CullingGroup.Dispose();
			m_CullingGroup = null;
		}

		void Update () {
			if (m_TargetsUpdateMode == CullingTargetsUpdateMode.EveryUpdate) {
				UpdateTargets();
			}

			UpdateDynamicBoundingSphereTransforms();
		}

#if UNITY_EDITOR
		void Reset () {
			Camera camera = GetComponentInParent<Camera>();
			if (camera == null) {
				camera = Camera.main;
			}
			TargetCamera = camera;
		}

		void OnValidate () {
			TargetCamera = TargetCamera;
			DistanceReferencePoint = DistanceReferencePoint;
		}
#endif

		#endregion

		/// <summary>
		/// <para> Add the specified target to the queue of targets to add. </para>
		/// <para> The timing at which its queue is applied depends on the <see cref="TargetsUpdateMode"/>.</para>
		/// <para> See: <see cref="CullingTargetsUpdateMode"/> </para>
		/// </summary>
		public void Add (ICullingTarget target) {
			m_TargetsToAdd.Add(target);
			m_TargetsToRemove.Remove(target);
		}

		/// <summary>
		/// <para> Remove the specified target at the queue of targets to remove. </para>
		/// <para> The timing at which its queue is applied depends on the <see cref="TargetsUpdateMode"/>.</para>
		/// <para> See: <see cref="CullingTargetsUpdateMode"/> </para>
		/// </summary>
		public void Remove (ICullingTarget target) {
			m_TargetsToRemove.Add(target);
			m_TargetsToAdd.Remove(target);
		}

		/// <summary>
		/// Returns true if the bounding sphere of target is currently visible from any of the contributing cameras.
		/// </summary>
		/// <returns> True if the target sphere is visible, false if it is invisible. </returns>
		public bool IsVisible (ICullingTarget target) {
			int index = m_Targets.IndexOf(target);
			return m_CullingGroup.IsVisible(index);
		}

		/// <summary>
		/// Get the current distance band index of a given target sphere.
		/// </summary>
		/// <returns> The sphere current distance band index. </returns>
		public int GetDistance (ICullingTarget target) {
			int index = m_Targets.IndexOf(target);
			return m_CullingGroup.GetDistance(index);
		}

		public int IndexOf (ICullingTarget target) {
			return m_Targets.IndexOf(target);
		}

		/// <summary>
		/// <para> Update the list of culling targets. </para>
		/// </summary>
		public void UpdateTargets () {
			// Remove targets.
			bool hasTargetsToRemove = (m_TargetsToRemove.Count > 0);
			if (hasTargetsToRemove) {
				foreach (ICullingTarget target in m_TargetsToRemove) {
					m_Targets.Remove(target);
				}
				m_TargetsToRemove.Clear();
			}

			// Add targets.
			bool hasTargetsToAdd = (m_TargetsToAdd.Count > 0);
			if (hasTargetsToAdd) {
				m_Targets.AddRange(m_TargetsToAdd);
				m_TargetsToAdd.Clear();
			}

			// Update bounding sphere's array.
			if (hasTargetsToAdd || hasTargetsToRemove) {
				int targetsCount = m_Targets.Count;

				// Ensure bounding spheres array.
				int nextPowerOfTwo = Mathf.NextPowerOfTwo(targetsCount);
				int length = (nextPowerOfTwo > k_BoundingSphereArrayMinLength) ? nextPowerOfTwo : k_BoundingSphereArrayMinLength;
				if (length != m_BoundingSpheres.Length) {
					m_BoundingSpheres = new BoundingSphere[length];
				}
				
				// Update sphere's and dynamic target indices.
				m_DynamicTargetIndices.Dispose();
				m_DynamicTargetIndices = TemporaryArray<int>.CreateAsList(targetsCount);
				for (int i = 0;targetsCount > i;i++) {
					ICullingTarget target = m_Targets[i];
					if (target != null) {
						m_BoundingSpheres[i] = target.UpdateAndGetBoundingSphere();

						if (target.BoundingSphereUpdateMode == TransformUpdateMode.Dynamic) {
							m_DynamicTargetIndices.Add(i);
						}
					}
				}

				// Reset the all bounding sphere's status.
				// NOTE: If the old state remains, OnStateChanged may not be called.
				m_CullingGroup.SetBoundingSphereCount(0);

				// Set bounding sphere's array and count.
				m_CullingGroup.SetBoundingSpheres(m_BoundingSpheres);
				m_CullingGroup.SetBoundingSphereCount(targetsCount);
			}
		}

		/// <summary>
		/// <para> Update the list of dynamic culling targets. </para>
		/// <para> If there is a change in the <see cref="ICullingTarget.BoundingSphereUpdateMode"/> of the culling target,  </para>
		/// <para> you need to call this method or <see cref="UpdateTargets"/>. </para>
		/// </summary>
		public void UpdateDynamicTargets () {
			m_DynamicTargetIndices.Dispose();
			m_DynamicTargetIndices = TemporaryArray<int>.CreateAsList(m_Targets.Count);
			for (int i = 0;m_Targets.Count > i;i++) {
				if (m_Targets[i].BoundingSphereUpdateMode == TransformUpdateMode.Dynamic) {
					m_DynamicTargetIndices.Add(i);
				}
			}
		}

		/// <summary>
		/// Update the all(static/dynamic) bounding sphere transform's.
		/// </summary>
		public void UpdateAllBoundingSphereTransforms () {
			for (int i = 0;m_Targets.Count > i;i++) {
				ICullingTarget target = m_Targets[i];
				m_BoundingSpheres[i] = target.UpdateAndGetBoundingSphere();
			}
		}

		/// <summary>
		/// Update only the dynamic bounding sphere transform's.
		/// </summary>
		public void UpdateDynamicBoundingSphereTransforms () {
			for (int i = 0;m_DynamicTargetIndices.Length > i;i++) {
				int index = m_DynamicTargetIndices[i];
				ICullingTarget target = m_Targets[index];
				if (target is UnityEngine.Object obj && obj != null) {
					m_BoundingSpheres[index] = target.UpdateAndGetBoundingSphere();
				}
			}
		}
		
		public static CullingGroupProxy GetGroup (CullingGroupKey key) {
			return (key >= 0) ? m_Groups[key] : null;
		}

		public static bool TryGetGroup (CullingGroupKey key,out CullingGroupProxy result) {
			result = GetGroup(key);
			return result != null;
		}

		public static void UpdateAllGroupTargets () {
			for (int i = 0;m_Groups.Length > i;i++) {
				CullingGroupProxy group = m_Groups[i];
				if (group != null) {
					group.UpdateTargets();
				}
			}
		}

	}

}