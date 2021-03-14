using UnityEngine;
using UnityEditor;

namespace MackySoft.Vision {
	public static class VisionEditorUtility {

		const string k_VisionMenuPath = "Tools/Vision/";

		public const string k_SettingsPath = "Project/MackySoft/Vision";
		public const string k_PreferencesPath = "Preferences/MackySoft/Vision";

		[MenuItem(k_VisionMenuPath + "Open Settings")]
		public static void OpenSettings () {
			SettingsService.OpenProjectSettings(k_SettingsPath);
		}

		[MenuItem(k_VisionMenuPath + "Open Preferences")]
		public static void OpenPreferences () {
			SettingsService.OpenUserPreferences(k_PreferencesPath);
		}

		[MenuItem(k_VisionMenuPath + "Open Documentation")]
		public static void OpenDocumentation () {
			Application.OpenURL("https://github.com/mackysoft/Vision");
		}

	}
}