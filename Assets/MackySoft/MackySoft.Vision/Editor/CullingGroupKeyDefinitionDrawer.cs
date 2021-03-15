using UnityEngine;
using UnityEditor;

namespace MackySoft.Vision.Editor {

	[CustomPropertyDrawer(typeof(CullingGroupKeyDefinition))]
	public class CullingGroupKeyDefinitionDrawer : PropertyDrawer {

		public override void OnGUI (Rect position,SerializedProperty property,GUIContent label) {
			EditorGUI.BeginProperty(position,label,property);

			SerializedProperty name = property.FindPropertyRelative("m_Name");
			EditorGUI.PropertyField(position,name,label);

			EditorGUI.EndProperty();
		}

	}
}