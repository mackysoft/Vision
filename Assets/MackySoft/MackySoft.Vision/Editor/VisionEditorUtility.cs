using UnityEngine;
using UnityEditor;

namespace MackySoft.Vision.Editor {
	public static class VisionEditorUtility {

		const string k_VisionMenuPath = "Tools/Vision/";

		public const string k_SettingsPath = "Project/MackySoft/Vision";
		public const string k_PreferencesPath = "Preferences/MackySoft/Vision";

		[MenuItem(k_VisionMenuPath + "Create New CullingGroupProxy",priority = 0)]
		public static void CreateNewCullingGroupProxy () {
			var go = new GameObject("New " + nameof(CullingGroupProxy),typeof(CullingGroupProxy));
			Selection.activeGameObject = go;
			EditorGUIUtility.PingObject(go);
			Undo.RegisterCreatedObjectUndo(go,"Created " + nameof(CullingGroupProxy));
		}

		[MenuItem(k_VisionMenuPath + "Open Settings",priority = 11)]
		public static void OpenSettings () {
			SettingsService.OpenProjectSettings(k_SettingsPath);
		}

		[MenuItem(k_VisionMenuPath + "Open Preferences",priority = 12)]
		public static void OpenPreferences () {
			SettingsService.OpenUserPreferences(k_PreferencesPath);
		}

		[MenuItem(k_VisionMenuPath + "Open Documentation",priority = 13)]
		public static void OpenDocumentation () {
			Application.OpenURL("https://github.com/mackysoft/Vision");
		}

	}
}