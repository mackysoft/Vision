using UnityEngine;

namespace MackySoft.Vision.Utilities {

	/// <summary>
	/// A useful component for linking <see cref="CullingGroupProxy"/> and camera that exist in separate scenes.
	/// </summary>
	[AddComponentMenu("MackySoft/Vision/Utilities/Culling Group Target Camera Setter")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class CullingGroupTargetCameraSetter : MonoBehaviour {

		[Tooltip("Key to reference the CullingGroupProxy")]
		[SerializeField]
		CullingGroupKey m_GroupKey = CullingGroupKey.None;

		[Tooltip("A reference to the camera you want to set to CullingGroupProxy.TargetCamera")]
		[SerializeField]
		CameraReference m_TargetCamera = new CameraReference(CameraReferenceMode.MainCamera);

		[SerializeField]
		bool m_LinkOnEnableOrStart = true;

		/// <summary>
		/// Key to reference the <see cref="CullingGroupProxy"/>.
		/// </summary>
		public CullingGroupKey GroupKey { get => m_GroupKey; set => m_GroupKey = value; }

		/// <summary>
		/// A reference to the camera you want to set to <see cref="CullingGroupProxy.TargetCamera"/>.
		/// </summary>
		public CameraReference TargetCamera { get => m_TargetCamera; set => m_TargetCamera = value; }

		public bool LinkOnEnableOrStart { get => m_LinkOnEnableOrStart; set => m_LinkOnEnableOrStart = value; }

		/// <summary>
		/// <para> Set the specified camera to the <see cref="CullingGroupProxy.TargetCamera"/> of the specified <see cref="CullingGroupProxy"/>. </para>
		/// <para> See: <see cref="GroupKey"/>, <see cref="TargetCamera"/> </para>
		/// </summary>
		public void Link () {
			if ((m_TargetCamera.Camera != null) && CullingGroupProxy.TryGetGroup(m_GroupKey,out CullingGroupProxy group)) {
				group.TargetCamera = m_TargetCamera;
			}
		}

		void OnEnable () {
			Link();
		}

		void Start () {
			Link();
		}

#if UNITY_EDITOR
		void Reset () {
			if (TryGetComponent(out CullingGroupProxy group)) {
				m_GroupKey = group.Key;
			}
			if (TryGetComponent(out Camera camera)) {
				m_TargetCamera.Mode = CameraReferenceMode.Custom;
				m_TargetCamera.CustomCamera = camera;
			}
		}
#endif

	}
}