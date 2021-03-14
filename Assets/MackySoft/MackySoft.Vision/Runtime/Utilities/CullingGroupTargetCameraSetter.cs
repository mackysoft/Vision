using UnityEngine;

namespace MackySoft.Vision.Utilities {

	[AddComponentMenu("MackySoft/Vision/Culling Group Target Camera Setter")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class CullingGroupTargetCameraSetter : MonoBehaviour {

		[SerializeField]
		CullingGroupKey m_GroupKey = CullingGroupKey.None;

		[SerializeField]
		CameraReference m_TargetCamera = new CameraReference(CameraSourceMode.MainCamera);

		void Start () {
			if ((m_TargetCamera.Camera != null) && CullingGroupProxy.TryGetGroup(m_GroupKey,out CullingGroupProxy group)) {
				group.TargetCamera = m_TargetCamera;
			}
		}

#if UNITY_EDITOR
		void Reset () {
			if (TryGetComponent(out CullingGroupProxy group)) {
				m_GroupKey = group.Key;
			}
			if (TryGetComponent(out Camera camera)) {
				m_TargetCamera.Mode = CameraSourceMode.Custom;
				m_TargetCamera.CustomCamera = camera;
			}
		}
#endif

	}
}