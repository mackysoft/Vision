using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MackySoft.Vision.Editor {

	[CustomEditor(typeof(VisionSettings))]
	public class VisionSettingsInspector : UnityEditor.Editor {

		SerializedProperty m_GroupKeyDefinitions;
		ReorderableList m_GroupKeyDefinitionsList;

		void OnEnable () {
			m_GroupKeyDefinitions = serializedObject.FindProperty("m_GroupKeyDefinitions");

			m_GroupKeyDefinitionsList = new ReorderableList(serializedObject,m_GroupKeyDefinitions,false,true,true,true);
			m_GroupKeyDefinitionsList.drawHeaderCallback += rect => EditorGUI.LabelField(rect,m_GroupKeyDefinitions.displayName);
			m_GroupKeyDefinitionsList.drawElementCallback += (rect,index,isActive,isFocused) => {
				var element = m_GroupKeyDefinitions.GetArrayElementAtIndex(index);
				rect.height -= 4;
				rect.y += 2;
				EditorGUI.PropertyField(rect,element,new GUIContent("Key " + index));
			};
			m_GroupKeyDefinitionsList.onAddCallback = list => {
				SerializedProperty array = list.serializedProperty;
				array.arraySize++;
				
				SerializedProperty definition = array.GetArrayElementAtIndex(array.arraySize - 1);
				definition.FindPropertyRelative("m_Name").stringValue = "Key " + (array.arraySize - 1).ToString();
			};
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			m_GroupKeyDefinitionsList.DoLayoutList();

			serializedObject.ApplyModifiedProperties();
		}

	}
}