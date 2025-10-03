using UnityEditor;
using UnityEngine;

namespace MigalhaSystem.Pool {
    [CustomEditor(typeof(PoolData))]
    public class PoolDataEditor : Editor {
        GUILayoutOption[] MyGUILayout;
        PoolData poolData;

        Color enableColor = Color.green;
        Color disableColor = Color.red;
        private void OnEnable() {
            MyGUILayout = new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.MinWidth(75),
                GUILayout.MaxWidth(300)
            };
        }

        public override void OnInspectorGUI() {
            poolData = (PoolData)target;
            DrawBasicSettings();
            DrawExtraObjects();
            DrawLifeTime();
            DrawFatherPool();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(poolData);
        }

        void DrawBasicSettings() {
            if (poolData == null) return;
            GUIStyle style = new(GUI.skin.textField);
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Prefab:");
            poolData.m_Prefab = (GameObject)EditorGUILayout.ObjectField("", poolData.m_Prefab, typeof(GameObject), false, MyGUILayout);
            GUILayout.Space(10);


            GUILayout.Label("Deafault Pool Size:");
            poolData.m_PoolSize = EditorGUILayout.IntField("", poolData.m_PoolSize, style, MyGUILayout);
            if (poolData.m_PoolSize <= 0) {
                poolData.m_PoolSize = 1;
            }
            GUILayout.Space(10);

            GUI.backgroundColor = poolData.m_ExpandablePool ? enableColor : disableColor;
            if (GUILayout.Button("Expandable Pool", MyGUILayout)) {
                poolData.m_ExpandablePool = !poolData.m_ExpandablePool;
            }
            GUI.backgroundColor = Color.white;
        }

        void DrawExtraObjects() {
            if (poolData == null) return;
            if (!poolData.m_ExpandablePool) return;
            GUI.backgroundColor = poolData.m_DestroyExtraObjectsAfterUse ? enableColor : disableColor;
            if (GUILayout.Button("Destroy Extra Objects", MyGUILayout)) {
                poolData.m_DestroyExtraObjectsAfterUse = !poolData.m_DestroyExtraObjectsAfterUse;
            }
            GUI.backgroundColor = Color.white;
            if (!poolData.m_DestroyExtraObjectsAfterUse) return;

            GUIStyle style = new(GUI.skin.textField);
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Extra Objects Allowed:");
            poolData.m_ExtraObjectsAllowed = EditorGUILayout.IntField("", poolData.m_ExtraObjectsAllowed, style, MyGUILayout);
            if (poolData.m_ExtraObjectsAllowed < 0) {
                poolData.m_ExtraObjectsAllowed = 0;
            }
        }

        void DrawLifeTime() {
            if (poolData == null) return;
            GUI.backgroundColor = poolData.m_RemoveIdlePool ? enableColor : disableColor;
            if (GUILayout.Button("Remove Idle Pool", MyGUILayout)) {
                poolData.m_RemoveIdlePool = !poolData.m_RemoveIdlePool;
            }
            GUI.backgroundColor = Color.white;
            if (!poolData.m_RemoveIdlePool) return;

            GUIStyle style = new(GUI.skin.textField);
            style.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label("Idle Time Pool Allowed in Seconds:");
            poolData.m_IdlePoolLifeTime = EditorGUILayout.FloatField("", poolData.m_IdlePoolLifeTime, style, MyGUILayout);
            if (poolData.m_IdlePoolLifeTime < 0) {
                poolData.m_IdlePoolLifeTime = 0;
            }
        }

        void DrawFatherPool() {
            if (poolData == null) return;
            if (poolData.m_PoolFather != null) poolData.m_IsFatherPool = true;
            GUI.backgroundColor = poolData.m_IsFatherPool ? enableColor : disableColor;
            if (GUILayout.Button("Add Custom Pool Father", MyGUILayout)) {
                poolData.m_IsFatherPool = !poolData.m_IsFatherPool;
            }
            GUI.backgroundColor = Color.white;
            if (!poolData.m_IsFatherPool) return;

            GUIStyle style = new(GUI.skin.textField);
            style.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label("Father Prefab:");
            poolData.m_PoolFather = (GameObject)EditorGUILayout.ObjectField("", poolData.m_PoolFather, typeof(GameObject), MyGUILayout);
        }
    }
}