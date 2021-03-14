using System.Collections.Generic;
using UnityEngine;

namespace MackySoft.Vision {

	[CreateAssetMenu(fileName = nameof(VisionSettings),menuName = "MackySoft/Vision/Vision Settings")]
	public class VisionSettings : ScriptableObject {

		static VisionSettings m_Instance;

		public static VisionSettings Instance {
			get {
				if (m_Instance == null) {
					m_Instance = Resources.Load<VisionSettings>(nameof(VisionSettings));
				}
				return m_Instance;
			}
		}

		[SerializeField]
		List<CullingGroupKeyDefinition> m_GroupKeyDefinitions = new List<CullingGroupKeyDefinition>();

		public IReadOnlyList<CullingGroupKeyDefinition> GroupKeyDefinitions => m_GroupKeyDefinitions;

	}
}