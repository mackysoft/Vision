#pragma warning disable CA1819 // Properties should not return arrays

using UnityEngine;

namespace MackySoft.Vision.Utilities {

	/// <summary>
	/// A simple component that enable/disable the specified renderer's depending on the visibility of the attached <see cref="ICullingTarget"/>.
	/// </summary>
	[AddComponentMenu("MackySoft/Vision/Culling Target Renderers")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class CullingTargetRenderers : MonoBehaviour {

		[Tooltip("A renderer's to enable/disable.")]
		[SerializeField]
		Renderer[] m_Renderers;

		ICullingTarget m_CullingTarget;

		/// <summary>
		/// A renderer's to enable/disable.
		/// </summary>
		public Renderer[] Renderers {
			get => m_Renderers;
			set => m_Renderers = value;
		}

		void Awake () {
			m_CullingTarget = GetComponent<ICullingTarget>();
			m_CullingTarget.OnStateChanged += OnStateChanged;
		}

		void OnDestroy () {
			m_CullingTarget.OnStateChanged -= OnStateChanged;
		}

		void OnStateChanged (CullingGroupEvent ev) {
			for (int i = 0;m_Renderers.Length > i;i++) {
				m_Renderers[i].enabled = ev.isVisible;
			}
		}

#if UNITY_EDITOR
		void Reset () {
			m_Renderers = GetComponentsInChildren<Renderer>();
		}
#endif

	}
}