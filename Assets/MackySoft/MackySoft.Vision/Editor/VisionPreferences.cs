#pragma warning disable IDE0051 // 使用されていないプライベート メンバーを削除する

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace MackySoft.Vision.Editor {

	[FilePath("VisionPreferences.asset",FilePathAttribute.Location.PreferencesFolder)]
	public class VisionPreferences : ScriptableSingleton<VisionPreferences> {

		[SerializeField]
		Color m_BoundingDistanceHandleColor = Color.cyan * 0.8f;

		[SerializeField]
		Color m_IdlingSphereRadiusHandleColor = Color.white;

		[SerializeField]
		Color m_VisibleSphereRadiusHandleColor = Color.green;

		[SerializeField]
		Color m_InvisibleSphereRadiusHandleColor = Color.red;

		[SerializeField]
		Color m_DisabledSphereRadiusHandleColor = Color.white * 0.8f;

		public Color BoundingDistanceHandleColor { get => m_BoundingDistanceHandleColor; set => m_BoundingDistanceHandleColor = value; }
		public Color IdlingSphereRadiusHandleColor { get => m_IdlingSphereRadiusHandleColor; set => m_IdlingSphereRadiusHandleColor = value; }
		public Color VisibleSphereRadiusHandleColor { get => m_VisibleSphereRadiusHandleColor; set => m_VisibleSphereRadiusHandleColor = value; }
		public Color InvisibleSphereRadiusHandleColor { get => m_InvisibleSphereRadiusHandleColor; set => m_InvisibleSphereRadiusHandleColor = value; }
		public Color DisabledSphereRadiusHandleColor { get => m_DisabledSphereRadiusHandleColor; set => m_DisabledSphereRadiusHandleColor = value; }

		void OnDisable () {
			Save();
		}

		public void Save () {
			Save(true);
		}

		internal SerializedObject GetSerializedObject () {
			return new SerializedObject(this);
		}

	}

	class VisionPreferencesProvider : SettingsProvider {

		static readonly Lazy<GUIStyle> k_SettingsStyle = new Lazy<GUIStyle>(() => {
			var style = new GUIStyle {
				margin = new RectOffset(10,10,0,0)
			};
			return style;
		});

		SerializedObject m_SerializedObject;
		SerializedProperty m_BoundingDistanceHandleColor;
		SerializedProperty m_IdlingSphereRadiusHandleColor;
		SerializedProperty m_VisibleSphereRadiusHandleColor;
		SerializedProperty m_InvisibleSphereRadiusHandleColor;
		SerializedProperty m_DisabledSphereRadiusHandleColor;

		class Styles {
			public static readonly GUIContent BoundingDistanceHandleColorLabel = EditorGUIUtility.TrTextContent("Bounding Distance Handle Color");
			public static readonly GUIContent IdlingSphereRadiusHandleColorLabel = EditorGUIUtility.TrTextContent("Idling Sphere Radius Handle Color");
			public static readonly GUIContent VisibleSphereRadiusHandleColorLabel = EditorGUIUtility.TrTextContent("Visible Sphere Radius Handle Color");
			public static readonly GUIContent InvisibleSphereRadiusHandleColorLabel = EditorGUIUtility.TrTextContent("invisible Sphere Radius Handle Color");
			public static readonly GUIContent DisabledSphereRadiusHandleColorLabel = EditorGUIUtility.TrTextContent("Disabled Sphere Radius Handle Color");
		}

		public VisionPreferencesProvider (string path,SettingsScope scopes,IEnumerable<string> keywords = null) : base(path,scopes,keywords) {
		}

		public override void OnActivate (string searchContext,VisualElement rootElement) {
			VisionPreferences.instance.Save();
			m_SerializedObject = VisionPreferences.instance.GetSerializedObject();
			m_BoundingDistanceHandleColor = m_SerializedObject.FindProperty("m_BoundingDistanceHandleColor");
			m_IdlingSphereRadiusHandleColor = m_SerializedObject.FindProperty("m_IdlingSphereRadiusHandleColor");
			m_VisibleSphereRadiusHandleColor = m_SerializedObject.FindProperty("m_VisibleSphereRadiusHandleColor");
			m_InvisibleSphereRadiusHandleColor = m_SerializedObject.FindProperty("m_InvisibleSphereRadiusHandleColor");
			m_DisabledSphereRadiusHandleColor = m_SerializedObject.FindProperty("m_DisabledSphereRadiusHandleColor");
		}

		public override void OnGUI (string searchContext) {
			EditorGUILayout.BeginVertical(k_SettingsStyle.Value);

			m_SerializedObject.Update();
			EditorGUI.BeginChangeCheck();
			
			m_BoundingDistanceHandleColor.colorValue = EditorGUILayout.ColorField(Styles.BoundingDistanceHandleColorLabel,m_BoundingDistanceHandleColor.colorValue);
			m_IdlingSphereRadiusHandleColor.colorValue = EditorGUILayout.ColorField(Styles.IdlingSphereRadiusHandleColorLabel,m_IdlingSphereRadiusHandleColor.colorValue);
			m_VisibleSphereRadiusHandleColor.colorValue = EditorGUILayout.ColorField(Styles.VisibleSphereRadiusHandleColorLabel,m_VisibleSphereRadiusHandleColor.colorValue);
			m_InvisibleSphereRadiusHandleColor.colorValue = EditorGUILayout.ColorField(Styles.InvisibleSphereRadiusHandleColorLabel,m_InvisibleSphereRadiusHandleColor.colorValue);
			m_DisabledSphereRadiusHandleColor.colorValue = EditorGUILayout.ColorField(Styles.DisabledSphereRadiusHandleColorLabel,m_DisabledSphereRadiusHandleColor.colorValue);

			if (EditorGUI.EndChangeCheck()) {
				m_SerializedObject.ApplyModifiedProperties();
				VisionPreferences.instance.Save();
			}

			EditorGUILayout.EndVertical();
		}

		[SettingsProvider]
		public static SettingsProvider CreateSettingsProvider () {
			return new VisionPreferencesProvider(VisionEditorUtility.k_PreferencesPath,SettingsScope.User,GetSearchKeywordsFromGUIContentProperties<Styles>());
		}

	}

}