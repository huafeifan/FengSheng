using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FengSheng
{
#if UNITY_EDITOR
    [CustomEditor(typeof(UIManager))]
    public class UIManagerEditor : Editor
    {
        private SerializedProperty UIRoot;
        private SerializedProperty mUICache;

        private void OnEnable()
        {
            UIRoot = serializedObject.FindProperty("UIRoot");
            mUICache = serializedObject.FindProperty("mUICache");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(UIRoot, new GUIContent("UI���ڵ�"));
            EditorGUILayout.PropertyField(mUICache, new GUIContent("�Ѵ���Prefab�б�"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
