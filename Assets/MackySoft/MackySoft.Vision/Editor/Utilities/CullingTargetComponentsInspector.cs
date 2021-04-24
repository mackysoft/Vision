using UnityEngine;
using UnityEditor;

namespace MackySoft.Vision.Utilities.Editor {

	[CustomEditor(typeof(CullingTargetComponents))]
	public class CullingTargetComponentsInspector : UnityEditor.Editor {

		const string k_SelfCullingTargetTargetingMessage = "Targets the attached CullingTarget. Once the CullingTarget is disabled, this component will also stop working.";

		Behaviour m_SelfCullingTarget;
		SerializedProperty m_Components;

		void OnEnable () {
			m_SelfCullingTarget = ((CullingTargetComponents)target).GetComponent<ICullingTarget>() as Behaviour;
			m_Components = serializedObject.FindProperty("m_Components");
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			for (int i = 0;m_Components.arraySize > i;i++) {
				if ((m_SelfCullingTarget != null) && (m_SelfCullingTarget == m_Components.GetArrayElementAtIndex(i).objectReferenceValue)) {
					EditorGUILayout.HelpBox(k_SelfCullingTargetTargetingMessage,MessageType.Warning);
					break;
				}
			}
			EditorGUILayout.PropertyField(m_Components);

			serializedObject.ApplyModifiedProperties();
		}

	}
}