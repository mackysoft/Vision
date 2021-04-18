#pragma warning disable CA1802 // Use literals where appropriate

using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using UnityObject = UnityEngine.Object;

namespace MackySoft.Vision.Editor {
	
	[CustomEditor(typeof(CullingGroupProxy))]
	[CanEditMultipleObjects]
	public class CullingGroupProxyInspector : UnityEditor.Editor {

		public static void DrawHandles (CullingGroupProxy group) {
			if ((group == null) || !group.enabled || (group.DistanceReferencePoint == null) || (group.BoundingDistances == null)) {
				return;
			}
			
			for (int i = 0;group.BoundingDistances.Length > i;i++) {
				EditorGUI.BeginChangeCheck();

				Handles.color = VisionPreferences.instance.BoundingDistanceHandleColor;
				float distance = Handles.RadiusHandle(
					group.DistanceReferencePoint.rotation,
					group.DistanceReferencePoint.position,
					group.BoundingDistances[i]
				);
				Handles.Label(group.DistanceReferencePoint.position + new Vector3(group.BoundingDistances[i] + 0.1f,0f,0f),"Level " + i.ToString(),EditorStyles.boldLabel);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(group,$"{nameof(CullingGroupProxy)} \"{group.name}\" distances");

					float minDistance = 0f;
					float maxDistance = float.MaxValue;
					if (i > 0) {
						minDistance = group.BoundingDistances[i - 1] + 0.1f;
					}
					if ((i + 1) < group.BoundingDistances.Length) {
						maxDistance = group.BoundingDistances[i + 1] - 0.1f;
					}

					group.BoundingDistances[i] = Mathf.Clamp(distance,minDistance,maxDistance);
				}
			}
		}

		const string k_InfoStateKey = "Vision_CullingGroupProxy_Info";

		CullingGroupProxy[] m_AllGroups;
		CullingGroupProxy[] m_Groups;

		SerializedProperty m_Key;
		SerializedProperty m_KeyIndex;
		SerializedProperty m_TargetsUpdateMode;
		SerializedProperty m_TargetCamera;
		SerializedProperty m_DistanceReferencePoint;
		SerializedProperty m_BoundingDistances;

		ReorderableList m_BoundingDistancesList;
		ReorderableList m_TargetsList;
		bool m_IsInfoExpanded;

		void OnEnable () {
			m_AllGroups = FindObjectsOfType<CullingGroupProxy>();

			m_Groups = new CullingGroupProxy[targets.Length];
			for (int i = 0;m_Groups.Length > i;i++) {
				m_Groups[i] = (CullingGroupProxy)targets[i];
			}

			m_Key = serializedObject.FindProperty("m_Key");
			m_KeyIndex = m_Key.FindPropertyRelative("m_Index");
			m_TargetsUpdateMode = serializedObject.FindProperty("m_TargetsUpdateMode");
			m_TargetCamera = serializedObject.FindProperty("m_TargetCamera");
			m_DistanceReferencePoint = serializedObject.FindProperty("m_DistanceReferencePoint");
			m_BoundingDistances = serializedObject.FindProperty("m_BoundingDistances");

			m_BoundingDistancesList = new ReorderableList(serializedObject,m_BoundingDistances,false,true,true,true) {
				drawHeaderCallback = rect => EditorGUI.LabelField(rect,m_BoundingDistances.displayName),
				drawElementCallback = (rect,index,isActive,isFocused) => {
					var distance = m_BoundingDistances.GetArrayElementAtIndex(index);

					rect.height -= 4f;
					rect.y += 2f;

					EditorGUI.BeginChangeCheck();

					EditorGUI.showMixedValue = distance.hasMultipleDifferentValues;
					float editedDistance = EditorGUI.FloatField(
						rect,
						new GUIContent("Level " + index.ToString()),
						distance.floatValue
					);
					EditorGUI.showMixedValue = false;

					if (EditorGUI.EndChangeCheck()) {
						float minDistance = 0f;
						float maxDistance = float.MaxValue;
						if (index > 0) {
							SerializedProperty previous = m_BoundingDistances.GetArrayElementAtIndex(index - 1);
							minDistance = previous.floatValue + 0.1f;
						}
						if ((index + 1) < m_BoundingDistances.arraySize) {
							SerializedProperty next = m_BoundingDistances.GetArrayElementAtIndex(index + 1);
							maxDistance = next.floatValue - 0.1f;
						}

						distance.floatValue = Mathf.Clamp(editedDistance,minDistance,maxDistance);
					}
				}
			};

			m_TargetsList = new ReorderableList((IList)m_Groups[0].Targets,typeof(CullingTargetBehaviour),false,true,false,false) {
				drawHeaderCallback = (rect) => EditorGUI.LabelField(rect,"Targets"),
				drawElementCallback = (rect,index,isActive,isFocused) => {
					rect.height -= 4f;
					rect.y += 2f;

					ICullingTarget target = m_Groups[0].Targets[index];
					string label = "Target " + index.ToString();
					if (target is UnityObject unityObject) {
						EditorGUI.ObjectField(rect,label,unityObject,typeof(UnityObject),true);
					} else {
						EditorGUI.LabelField(rect,label,target.ToString());
					}
				}
			};
			m_IsInfoExpanded = SessionState.GetBool(k_InfoStateKey,false);
		}

		void OnSceneGUI () {
			CullingGroupProxy group = (CullingGroupProxy)target;
			DrawHandles(group);

			if ((group.TargetCamera != null) && !Selection.Contains(group.TargetCamera)) {
				CameraEditorUtils.DrawFrustumGizmo(group.TargetCamera);
			}
			
			for (int i = 0;group.Targets.Count > i;i++) {
				CullingTargetBehaviourInspector.DrawHandles(group.Targets[i]);
			}
		}

		static readonly string k_KeyNotSetMessage = $"The key is not set. In order for {nameof(CullingTargetBehaviour)} and {nameof(CullingGroupProxy)}.{nameof(CullingGroupProxy.GetGroup)} method to find this {nameof(CullingGroupProxy)}, you should set the key.";
		static readonly string k_IsDuplicateKeyMessage = $"The key is a duplicate of other {nameof(CullingGroupProxy)}.";

		public override void OnInspectorGUI () {
			serializedObject.Update();

			if (m_KeyIndex.intValue < 0) {
				EditorGUILayout.HelpBox(k_KeyNotSetMessage,MessageType.Error);
			}
			else if (IsDuplicateKey(m_KeyIndex.intValue)) {
				EditorGUILayout.HelpBox(k_IsDuplicateKeyMessage,MessageType.Error);
			}
			EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
			EditorGUILayout.PropertyField(m_Key);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(m_TargetsUpdateMode);

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(m_TargetCamera);
			EditorGUILayout.PropertyField(m_DistanceReferencePoint);

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			m_BoundingDistancesList.DoLayoutList();
			if (EditorGUI.EndChangeCheck()) {
				for (int i = 0;m_Groups.Length > i;i++) {
					m_Groups[i].BoundingDistances = m_Groups[i].BoundingDistances;
				}
			}

			if (EditorApplication.isPlaying) {
				bool isInfoExpanded = EditorGUILayout.Foldout(m_IsInfoExpanded,"Info",true);
				if (isInfoExpanded != m_IsInfoExpanded) {
					SessionState.SetBool(k_InfoStateKey,isInfoExpanded);
					m_IsInfoExpanded = isInfoExpanded;
				}

				if (m_IsInfoExpanded) {
					if (!serializedObject.isEditingMultipleObjects) {
						m_TargetsList.DoLayoutList();
					} else {
						EditorGUI.indentLevel++;
						EditorGUILayout.HelpBox("Is editing multiple objects.",MessageType.Info);
						EditorGUI.indentLevel--;
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		bool IsDuplicateKey (int keyIndex) {
			bool exists = false;
			for (int i = 0;m_AllGroups.Length > i;i++) {
				if (m_AllGroups[i].Key != keyIndex) {
					continue;
				}
				if (exists) {
					return true;
				} else {
					exists = true;
				}
			}
			return false;
		}

	}
}