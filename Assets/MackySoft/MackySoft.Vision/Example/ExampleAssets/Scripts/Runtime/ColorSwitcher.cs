using UnityEngine;

namespace MackySoft.Vision.Example {

    [AddComponentMenu("MackySoft/Vision/Example/Color Switcher")]
    [HelpURL("https://github.com/mackysoft/Vision")]
    [RequireComponent(typeof(CullingTargetBehaviour))]
    public class ColorSwitcher : MonoBehaviour {

		[SerializeField]
		Color[] m_Colors;

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
			Color targetColor = m_Colors[ev.currentDistance];
			for (int i = 0;m_Renderers.Length > i;i++) {
				m_Renderers[i].material.color = targetColor;
			}
		}
	}
}