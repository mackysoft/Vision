using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;
using System;

namespace MackySoft.Vision.Editor {

	/// <summary>
	/// Automatically create <see cref="VisionSettings"/>.
	/// </summary>
	class VisionSettingsInitializer {

		const string k_ParentFolderName = "MackySoft.Vision";
		
		[InitializeOnLoadMethod]
		static void Initialize () {
			EditorApplication.projectChanged -= CreateSettings;
			EditorApplication.projectChanged += CreateSettings;
		}

		static void CreateSettings () {
#if !VISION_DISABLE_GENERATE_SETTINGS
			var settings = VisionSettings.Instance;
			if (settings != null) {
				return;
			}

			settings = ScriptableObject.CreateInstance<VisionSettings>();
			try {
				MonoScript settingsScript = MonoScript.FromScriptableObject(settings);

				// Assets/MackySoft/MackySoft.Vision/Runtime/VisionSettings.cs
				string settingsScriptPath = AssetDatabase.GetAssetPath(settingsScript);
				int lastIndex = settingsScriptPath.IndexOf(k_ParentFolderName);
				string parentFolderPath = settingsScriptPath.Remove(lastIndex + k_ParentFolderName.Length);

				if (!AssetDatabase.IsValidFolder(parentFolderPath + "/Resources")) {
					AssetDatabase.CreateFolder(parentFolderPath,"Resources");
				}

				AssetDatabase.CreateAsset(settings,$"{parentFolderPath}/Resources/{nameof(VisionSettings)}.asset");
				AssetDatabase.Refresh();
			} catch (Exception e) {
				UnityObject.DestroyImmediate(settings);
				throw e;
			}
#endif
		}

	}
}