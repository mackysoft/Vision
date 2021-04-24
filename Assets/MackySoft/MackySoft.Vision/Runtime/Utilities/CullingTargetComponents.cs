#pragma warning disable CA1819 // Properties should not return arrays

using UnityEngine;

namespace MackySoft.Vision.Utilities {

	/// <summary>
	/// A simple component that enable/disable the specified component's depending on the visibility of the attached <see cref="ICullingTarget"/>.
	/// </summary>
	[AddComponentMenu("MackySoft/Vision/Utilities/Culling Target Components")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class CullingTargetComponents : MonoBehaviour {

		[Tooltip("A component's to enable/disable.")]
		[SerializeField]
		Behaviour[] m_Components;
		
		ICullingTarget m_CullingTarget;

		/// <summary>
		/// A component's to enable/disable.
		/// </summary>
		public Behaviour[] Components {
			get => m_Components;
			set => m_Components = value;
		}

		void Awake () {
			m_CullingTarget = GetComponent<ICullingTarget>();
			m_CullingTarget.OnStateChanged += OnStateChanged;
		}

		void OnDestroy () {
			m_CullingTarget.OnStateChanged -= OnStateChanged;
		}

		void OnStateChanged (CullingGroupEvent ev) {
			if (m_Components != null) {
				for (int i = 0;m_Components.Length > i;i++) {
					m_Components[i].enabled = ev.isVisible;
				}
			}
		}

	}
}