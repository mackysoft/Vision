using UnityEngine;
using UnityEditor;

namespace MackySoft.Vision.Utilities.Editor {

	[CustomPropertyDrawer(typeof(CameraReference))]
	public class CameraReferenceDrawer : PropertyDrawer {

		public override void OnGUI (Rect position,SerializedProperty property,GUIContent label) {
			EditorGUI.BeginProperty(position,label,property);

			position = EditorGUI.PrefixLabel(position,label);

			SerializedProperty mode = property.FindPropertyRelative("m_Mode");

			if (!mode.hasMultipleDifferentValues) {
				switch ((CameraReferenceMode)mode.enumValueIndex) {
					case CameraReferenceMode.TaggedCamera:
						position.width *= 0.5f;
						EditorGUI.PropertyField(new Rect(position.xMin,position.yMin,position.width,position.height),mode,GUIContent.none);
						SerializedProperty tag = property.FindPropertyRelative("m_Tag");

						EditorGUI.showMixedValue = tag.hasMultipleDifferentValues;
						tag.stringValue = EditorGUI.TagField(new Rect(position.xMin + position.width,position.yMin,position.width,position.height),GUIContent.none,tag.stringValue);
						EditorGUI.showMixedValue = false;
						break;
					case CameraReferenceMode.Custom:
						position.width *= 0.5f;
						EditorGUI.PropertyField(new Rect(position.xMin,position.yMin,position.width,position.height),mode,GUIContent.none);
						SerializedProperty customCamera = property.FindPropertyRelative("m_CustomCamera");

						EditorGUI.showMixedValue = customCamera.hasMultipleDifferentValues;
						EditorGUI.PropertyField(new Rect(position.xMin + position.width,position.yMin,position.width,position.height),customCamera,GUIContent.none);
						EditorGUI.showMixedValue = false;
						break;
					default:
						EditorGUI.PropertyField(position,mode,GUIContent.none);
						break;
				}
			} else {
				EditorGUI.PropertyField(position,mode,GUIContent.none);
			}

			EditorGUI.EndProperty();
		}

	}
}