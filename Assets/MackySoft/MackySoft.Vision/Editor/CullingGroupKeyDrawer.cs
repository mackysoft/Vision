using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MackySoft.Vision {

	[CustomPropertyDrawer(typeof(CullingGroupKey))]
	public class CullingGroupKeyDrawer : PropertyDrawer {

		static readonly GUIContent k_None = new GUIContent("<none>");
		static readonly GUIContent k_AddKey = new GUIContent("<add key>");

		GUIContent[] m_DisplayContents;

		public override void OnGUI (Rect position,SerializedProperty property,GUIContent label) {
			EditorGUI.BeginProperty(position,label,property);
			
			if (m_DisplayContents == null) {
				m_DisplayContents = GetDisplayContents().ToArray();
			}

			SerializedProperty index = property.FindPropertyRelative("m_Index");

			bool previousShowMixedValue = EditorGUI.showMixedValue;
			EditorGUI.showMixedValue = index.hasMultipleDifferentValues;

			EditorGUI.BeginChangeCheck();
			int selectedIndex = (index.intValue == -1) ? 0 : index.intValue + 2;
			selectedIndex = EditorGUI.Popup(position,label,selectedIndex,m_DisplayContents);

			if (EditorGUI.EndChangeCheck()) {
				if (selectedIndex == 1) {
					// Add Key
					VisionEditorUtility.OpenSettings();
				} else if (selectedIndex == 0) {
					// None
					index.intValue = -1;
				} else {
					index.intValue = selectedIndex - 2;
				}
			}

			EditorGUI.showMixedValue = previousShowMixedValue;

			EditorGUI.EndProperty();
		}

		IEnumerable<GUIContent> GetDisplayContents () {
			yield return k_None;
			yield return k_AddKey;

			var definitions = VisionSettings.Instance.GroupKeyDefinitions;
			for (int i = 0;definitions.Count > i;i++) {
				yield return new GUIContent(definitions[i].Name);
			}
		}

	}
}