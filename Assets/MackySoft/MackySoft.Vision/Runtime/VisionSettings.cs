using System.Collections.Generic;
using UnityEngine;

namespace MackySoft.Vision {

	/// <summary>
	/// <para> VisionSettings is where you can configure settings related to Vision.</para>
	/// <para> It can be opened from the "Tools/Vision/Open Settings" menu. </para>
	/// </summary>
	// NOTE: VisionSettings will be created automatically.
	// [CreateAssetMenu(fileName = nameof(VisionSettings),menuName = "MackySoft/Vision/Vision Settings")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class VisionSettings : ScriptableObject {

		static VisionSettings m_Instance;

		/// <summary>
		/// <para> A singleton of VisionSettings. </para>
		/// <para> This instance will be loaded from "Resources/VisionSettings". </para>
		/// </summary>
		public static VisionSettings Instance {
			get {
				if (m_Instance == null) {
					m_Instance = Resources.Load<VisionSettings>(nameof(VisionSettings));
				}
				return m_Instance;
			}
		}

		[Tooltip(
			"Definition list of culling group key.\n" +
			"Used to display the CullingGroupKey as a popup in the inspector."
		)]
		[SerializeField]
		List<CullingGroupKeyDefinition> m_GroupKeyDefinitions = new List<CullingGroupKeyDefinition> {
			new CullingGroupKeyDefinition { Name = "Main" }
		};

		/// <summary>
		/// <para> Definition list of culling group key. </para>
		/// <para> Used to display the <see cref="CullingGroupKey"/> as a popup in the inspector. </para>
		/// </summary>
		public IReadOnlyList<CullingGroupKeyDefinition> GroupKeyDefinitions => m_GroupKeyDefinitions;

	}
}