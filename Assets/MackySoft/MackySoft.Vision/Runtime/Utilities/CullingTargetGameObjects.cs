#pragma warning disable CA1819 // Properties should not return arrays

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MackySoft.Vision.Utilities {

	/// <summary>
	/// <para> A simple component that enable/disable the specified component's depending on the visibility of the attached <see cref="ICullingTarget"/>. </para>
	/// <para> WARN: If you target your self game object, CullingTarget will also be disabled. </para>
	/// </summary>
	[AddComponentMenu("MackySoft/Vision/Utilities/Culling Target GameObjects")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class CullingTargetGameObjects : MonoBehaviour {

		[Tooltip("A GameObject's to active/inactive.")]
		[SerializeField]
		GameObject[] m_GameObjects;

		ICullingTarget m_CullingTarget;

		/// <summary>
		/// <para> A GameObjects's to active/inactive. </para>
		/// <para> WARN: If you target your self game object, CullingTarget will also be disabled. </para>
		/// </summary>
		public GameObject[] GameObjects {
			get => m_GameObjects;
			set => m_GameObjects = value;
		}

		void Awake () {
			m_CullingTarget = GetComponent<ICullingTarget>();
			m_CullingTarget.OnStateChanged += OnStateChanged;
		}

		void OnDestroy () {
			m_CullingTarget.OnStateChanged -= OnStateChanged;
		}

		void OnStateChanged (CullingGroupEvent ev) {
			if (m_GameObjects != null) {
				for (int i = 0;m_GameObjects.Length > i;i++) {
					m_GameObjects[i].SetActive(ev.isVisible);
				}
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Add Children")]
		void AddChildren () {
			var set = (m_GameObjects != null) ? new HashSet<GameObject>(m_GameObjects) : new HashSet<GameObject>();
			foreach (Transform child in transform) {
				set.Add(child.gameObject);
			}
			m_GameObjects = set.ToArray();
		}
#endif

	}
}