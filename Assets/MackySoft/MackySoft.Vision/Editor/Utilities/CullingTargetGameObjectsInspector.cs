using UnityEngine;
using UnityEditor;

namespace MackySoft.Vision.Utilities.Editor {

	[CustomEditor(typeof(CullingTargetGameObjects))]
	public class CullingTargetGameObjectsInspector : UnityEditor.Editor {

		const string k_SelfGameObjectTargetingMessage = "It targets its self GameObject. Once the GameObject is inactive, the attached CullingTarget will also become disabled, so this component will also stop working.";

		GameObject m_SelfGameObject;
		SerializedProperty m_GameObjects;

		void OnEnable () {
			m_SelfGameObject = ((CullingTargetGameObjects)target).gameObject;
			m_GameObjects = serializedObject.FindProperty("m_GameObjects");
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			for (int i = 0;m_GameObjects.arraySize > i;i++) {
				if (m_SelfGameObject == m_GameObjects.GetArrayElementAtIndex(i).objectReferenceValue) {
					EditorGUILayout.HelpBox(k_SelfGameObjectTargetingMessage,MessageType.Warning);
					break;
				}
			}
			EditorGUILayout.PropertyField(m_GameObjects);

			serializedObject.ApplyModifiedProperties();
		}

	}
}