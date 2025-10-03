using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MigalhaSystem.Pool
{
	[CustomEditor(typeof(PoolManager))]
	public class PoolManagerCustomEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayoutOption[] buttonLayout = new[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.MinWidth(75),
                GUILayout.MinHeight(35)
            };
            PoolManager manager = target as PoolManager;
            if (manager == null) return;

            GUILayout.Space(10);

            SerializedProperty removeEmptyPoolsProperty = serializedObject.FindProperty("m_removeEmptyPool");

            GUI.backgroundColor = removeEmptyPoolsProperty.boolValue ? Color.green : Color.red;
            if (GUILayout.Button("Remove Empty Pools", buttonLayout))
            {
                manager.m_RemoveEmptyPool = !manager.m_RemoveEmptyPool;
            }
            GUI.backgroundColor = Color.white;

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(this);

            GUILayout.Space(10);


            if (manager.m_Pools != null && manager.m_Pools.Count > 0)
            {
                if (manager.m_Pools.FindAll(x => x.m_PoolData == null)?.Count != manager.m_Pools?.Count) 
                {
                    if (GUILayout.Button("Remove All Pools", buttonLayout))
                    {
                        for (int i = manager.m_Pools.Count - 1; i >= 0; i--)
                        {
                            if (manager.m_Pools[i].m_PoolData != null)
                            {
                                manager.DeletePool(manager.m_Pools[i].m_PoolData);
                            }
                            else
                            {
                                manager.m_Pools.RemoveAt(i);
                            }
                        }
                    }
                }


                if (manager.m_Pools.Exists(x => x.m_PoolData == null))
                {
                    if (GUILayout.Button("Remove All Empty Pools", buttonLayout))
                    {
                        manager.m_Pools.RemoveAll(x => x.m_PoolData == null);
                    }
                }
                GUILayout.Space(10);
            }

            

            if (manager.m_PoolCollections == null || manager.m_PoolCollections.Count <= 0) return;

            List<PoolCollection> nullPoolCollections = manager.m_PoolCollections.FindAll(x => x == null);

            if (nullPoolCollections.Count != manager.m_PoolCollections.Count)
            {
                if (GUILayout.Button("Add Pool Collections", buttonLayout))
                {
                    manager.AddPoolCollections();
                }
            }

            if (nullPoolCollections.Count > 0)
            {
                if (GUILayout.Button("Remove All Empty Pool Collections", buttonLayout))
                {
                    foreach (PoolCollection poolCollection in nullPoolCollections) {
                        manager.m_PoolCollections.Remove(poolCollection);
                    }
                }
            }

            if (GUILayout.Button("Remove All Pool Collections", buttonLayout))
            {
                for (int i = manager.m_PoolCollections.Count - 1; i >= 0; i--)
                {
                    manager.m_PoolCollections.RemoveAt(i);
                }
            }
        }
    }
}