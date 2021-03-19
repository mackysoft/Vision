using UnityEngine;

namespace MackySoft.Vision.Example {

	[AddComponentMenu("MackySoft/Vision/Example/Material Switcher")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	[RequireComponent(typeof(CullingTargetBehaviour))]
    public class MaterialSwitcher : MonoBehaviour {

        [SerializeField]
        Material[] m_Materials;

		ICullingTarget m_CullingTarget;
		Renderer[] m_Renderers;

		void Awake () {
			m_CullingTarget = GetComponent<ICullingTarget>();
			m_Renderers = GetComponentsInChildren<Renderer>();
		}

		void OnEnable () {
			m_CullingTarget.OnStateChanged += OnStateChanged;
		}

		void OnDisable () {
			m_CullingTarget.OnStateChanged -= OnStateChanged;
		}

		void OnStateChanged (CullingGroupEvent ev) {
			Material targetMaterial = m_Materials[ev.currentDistance];
			for (int i = 0;m_Renderers.Length > i;i++) {
				m_Renderers[i].sharedMaterial = targetMaterial;
			}
		}

	}
}