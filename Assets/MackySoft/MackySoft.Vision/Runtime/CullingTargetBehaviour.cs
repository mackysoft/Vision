using UnityEngine;

namespace MackySoft.Vision {

	/// <summary>
	/// A component that implement <see cref="ICullingTarget"/>, which can be registered to <see cref="CullingGroupProxy"/> with a key.
	/// </summary>
	[AddComponentMenu("MackySoft/Vision/Culling Target Behaviour")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class CullingTargetBehaviour : MonoBehaviour, ICullingTarget {

		#region Variables

		[Tooltip("A key of CullingGroupProxy to register this CullingTargetBehaviour.")]
		[SerializeField]
		CullingGroupKey m_GroupKey = CullingGroupKey.None;

		[Tooltip("When to update the bounding sphere transform.")]
		[SerializeField]
		TransformUpdateMode m_BoundingSphereUpdateMode = TransformUpdateMode.Dynamic;

		[Tooltip("A radius of bounding sphere.")]
		[SerializeField]
		float m_Radius = 1f;

		Transform m_Transform;
		CullingGroupProxy m_Group;
		BoundingSphere m_BoundingSphere;
		bool m_HasStarted;

		#endregion

		#region Properties

		/// <summary>
		/// A key of <see cref="CullingGroupProxy"/> to register this <see cref="CullingTargetBehaviour"/>.
		/// </summary>
		public CullingGroupKey GroupKey {
			get => m_GroupKey;
			set {
				if (m_GroupKey != value) {
					OnDisable();

					m_GroupKey = value;

					OnEnableOrStart();
				}
			}
		}

		/// <summary>
		/// <para> When to update the bounding sphere transform. </para>
		/// <para> To apply this value change, you need to call the update method of <see cref="CullingGroupProxy"/>. </para>
		/// <para> See: <see cref="CullingGroupProxy.UpdateTargets"/>, <see cref="CullingGroupProxy.UpdateDynamicTargets"/> </para>
		/// </summary>
		public TransformUpdateMode BoundingSphereUpdateMode {
			get => m_BoundingSphereUpdateMode;
			set => m_BoundingSphereUpdateMode = value;
		}

		/// <summary>
		/// A radius of bounding sphere.
		/// </summary>
		public float Radius { get => m_Radius; set => m_Radius = value; }

		/// <summary>
		/// <para> BoundingSphere with transform position and radius applied. </para>
		/// <para> If you want to get the latest state, use <see cref="UpdateAndGetBoundingSphere"/>. </para>
		/// </summary>
		public BoundingSphere BoundingSphere => m_BoundingSphere;

		/// <summary>
		/// The group to which this culling target belongs.
		/// </summary>
		public CullingGroupProxy Group => m_Group;

		#endregion

		#region Events

		public CullingGroup.StateChanged OnStateChanged { get; set; }

		#endregion

		#region Behaviour

		void Awake () {
			m_Transform = transform;
		}

		void Start () {
			m_HasStarted = true;
			OnEnableOrStart();
		}

		void OnEnable () {
			if (m_HasStarted) {
				OnEnableOrStart();
			}
		}

		void OnEnableOrStart () {
			if (CullingGroupProxy.TryGetGroup(m_GroupKey,out var group)) {
				m_Group = group;
				m_Group.Add(this);
			}
		}

		void OnDisable () {
			if (m_Group != null) {
				m_Group.Remove(this);
				m_Group = null;
			}
		}

		#endregion

		public bool IsVisible () {
			return (m_Group == null) || m_Group.IsVisible(this);
		}

		public int GetDistance () {
			return (m_Group != null) ? m_Group.GetDistance(this) : -1;
		}

		/// <summary>
		/// <para> Update the <see cref="BoundingSphere"/> to the latest state and returns it. </para>
		/// </summary>
		/// <returns> The <see cref="BoundingSphere"/> updated to the latest state. </returns>
		public BoundingSphere UpdateAndGetBoundingSphere () {
			m_BoundingSphere.position = m_Transform.position;
			m_BoundingSphere.radius = m_Radius;
			return m_BoundingSphere;
		}

	}
}