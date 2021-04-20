using UnityEngine;
using UnityEditor;

namespace MackySoft.Vision.Editor {

	[CustomEditor(typeof(CullingTargetBehaviour))]
	[CanEditMultipleObjects]
	public class CullingTargetBehaviourInspector : UnityEditor.Editor {

		public static void DrawHandles (ICullingTarget cullingTarget) {
			if (cullingTarget == null) {
				return;
			}
			if (cullingTarget is CullingTargetBehaviour cullingTargetBehaviour) {
				DrawHandles(cullingTargetBehaviour);
			} else {
				BoundingSphere boundingSphere = cullingTarget.BoundingSphere;

				Handles.color = GetRadiusHandleColor(cullingTarget);
				Gizmos.DrawWireSphere(
					boundingSphere.position,
					boundingSphere.radius
				);
			}
		}

		public static void DrawHandles (CullingTargetBehaviour cullingTarget) {
			if ((cullingTarget == null) || !cullingTarget.enabled) {
				return;
			}

			EditorGUI.BeginChangeCheck();

			Transform transform = cullingTarget.transform;
			Vector3 position;
			float radius;
			int index = (cullingTarget.Group != null) ? cullingTarget.Group.IndexOf(cullingTarget) : -1;
			if (index != -1) {
				BoundingSphere boundingSphere = cullingTarget.Group.BoundingSpheres[index];
				position = boundingSphere.position;
				radius = boundingSphere.radius;
			} else {
				position = transform.position;
				radius = cullingTarget.Radius;
			}

			Handles.color = GetRadiusHandleColor(cullingTarget);
			radius = Handles.RadiusHandle(
				transform.rotation,
				position,
				radius
			);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(cullingTarget,$"{nameof(CullingTargetBehaviour)} \"{cullingTarget.name}\" radius");
				cullingTarget.Radius = radius;
			}
		}

		const string k_InfoStateKey = "Vision_CullingTargetBehaviour_Info";

		CullingTargetBehaviour[] m_CullingTargets;
		Transform[] m_Transforms;

		SerializedProperty m_GroupKey;
		SerializedProperty m_GroupKeyIndex;
		SerializedProperty m_BoundingSphereUpdateMode;
		SerializedProperty m_Radius;

		bool m_IsInfoExpanded;

		void OnEnable () {
			m_CullingTargets = new CullingTargetBehaviour[targets.Length];
			m_Transforms = new Transform[targets.Length];
			for (int i = 0;targets.Length > i;i++) {
				var cullingTarget = (CullingTargetBehaviour)targets[i];
				m_CullingTargets[i] = cullingTarget;
				m_Transforms[i] = cullingTarget.transform;
			}

			m_GroupKey = serializedObject.FindProperty("m_GroupKey");
			m_GroupKeyIndex = m_GroupKey.FindPropertyRelative("m_Index");
			m_BoundingSphereUpdateMode = serializedObject.FindProperty("m_BoundingSphereUpdateMode");
			m_Radius = serializedObject.FindProperty("m_Radius");

			m_IsInfoExpanded = SessionState.GetBool(k_InfoStateKey,false);
		}
		
		void OnSceneGUI () {
			var cullingTarget = (CullingTargetBehaviour)target;
			DrawHandles(cullingTarget);
		}

		static readonly string k_KeyNotSetMessage = $"The key is not set. To cull this CullingTarget, you need to set a key to find the {nameof(CullingGroupProxy)}.";

		public override void OnInspectorGUI () {
			serializedObject.Update();

			if (m_GroupKeyIndex.intValue < 0) {
				EditorGUILayout.HelpBox(k_KeyNotSetMessage,MessageType.Warning);
			}
			EditorGUILayout.PropertyField(m_GroupKey);

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(m_BoundingSphereUpdateMode);
			EditorGUILayout.PropertyField(m_Radius);

			if (EditorApplication.isPlaying && !serializedObject.isEditingMultipleObjects) {
				CullingTargetBehaviour cullingTarget = m_CullingTargets[0];
				if (cullingTarget.Group != null) {
					bool isInfoExpanded = EditorGUILayout.Foldout(m_IsInfoExpanded,"Info",true);
					if (isInfoExpanded != m_IsInfoExpanded) {
						SessionState.SetBool(k_InfoStateKey,isInfoExpanded);
						m_IsInfoExpanded = isInfoExpanded;
					}

					if (m_IsInfoExpanded) {
						EditorGUI.BeginDisabledGroup(true);
						EditorGUI.indentLevel++;

						EditorGUILayout.ObjectField("Group",cullingTarget.Group,typeof(CullingGroupProxy),true);
						EditorGUILayout.IntField("Index",cullingTarget.Group.IndexOf(cullingTarget));
						EditorGUILayout.Toggle("Is Visible",cullingTarget.IsVisible());
						EditorGUILayout.IntField("Distance Level",cullingTarget.GetDistance());

						EditorGUI.indentLevel--;
						EditorGUI.EndDisabledGroup();
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		static Color GetRadiusHandleColor (ICullingTarget target) {
			var preferences = VisionPreferences.instance;
			if (EditorApplication.isPlaying) {
				return target.IsVisible() ? preferences.VisibleSphereRadiusHandleColor : preferences.InvisibleSphereRadiusHandleColor;
			}
			return preferences.IdlingSphereRadiusHandleColor;
		}

		static Color GetRadiusHandleColor (CullingTargetBehaviour target) {
			var preferences = VisionPreferences.instance;
			if (EditorApplication.isPlaying) {
				if (target.enabled && target.Group) {
					return target.IsVisible() ? preferences.VisibleSphereRadiusHandleColor : preferences.InvisibleSphereRadiusHandleColor;
				} else {
					return preferences.DisabledSphereRadiusHandleColor;
				}
			}
			return preferences.IdlingSphereRadiusHandleColor;
		}

	}
}