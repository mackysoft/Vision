using UnityEngine;
using UnityEditor;

using UnityObject = UnityEngine.Object;
using System;

namespace MackySoft.Vision.Editor {

	/// <summary>
	/// Automatically create <see cref="VisionSettings"/>.
	/// </summary>
	class VisionSettingsGenerator {

		const string k_ParentFolderName = "MackySoft.Vision";
		
		[InitializeOnLoadMethod]
		static void Initialize () {
			CreateSettings();
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

				// unitypackage: Assets/MackySoft/MackySoft.Vision/Runtime/VisionSettings.cs
				// Open UPM: Packages/com.mackysoft.vision/Runtime/VisionSettings.cs
				string settingsScriptPath = AssetDatabase.GetAssetPath(settingsScript);

				string parentFolderPath = null;
				if (settingsScriptPath.StartsWith("Assets/")) {
					int lastIndex = settingsScriptPath.IndexOf(k_ParentFolderName);
					parentFolderPath = settingsScriptPath.Remove(lastIndex + k_ParentFolderName.Length);
				} else if (settingsScriptPath.StartsWith("Packages/")) {
					parentFolderPath = "Assets";
				} else {
					throw new InvalidOperationException("Unexpected Vision package path.");
				}

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