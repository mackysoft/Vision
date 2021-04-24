using UnityEditor;

namespace MackySoft.Vision.Utilities.Editor {

	[CustomEditor(typeof(CullingTargetRenderers))]
	public class CullingTargetRenderersInspector : UnityEditor.Editor {

		SerializedProperty m_Renderers;

		void OnEnable () {
			m_Renderers = serializedObject.FindProperty("m_Renderers");
		}

		public override void OnInspectorGUI () {
			serializedObject.Update();

			EditorGUILayout.PropertyField(m_Renderers);

			serializedObject.ApplyModifiedProperties();
		}

	}
}