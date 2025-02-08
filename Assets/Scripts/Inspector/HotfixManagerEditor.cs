using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FengSheng
{
#if UNITY_EDITOR
    [CustomEditor(typeof(HotfixManager))]
    public class HotfixManagerEditor : Editor
    {
        private SerializedProperty mCurrentVersion;
        private SerializedProperty mTargetVersion;
        private SerializedProperty mUpdateList;

        private void OnEnable()
        {
            mCurrentVersion = serializedObject.FindProperty("mCurrentVersion");
            mTargetVersion = serializedObject.FindProperty("mTargetVersion");
            mUpdateList = serializedObject.FindProperty("mUpdateList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mCurrentVersion, new GUIContent("���ذ汾��"));
            EditorGUILayout.PropertyField(mTargetVersion, new GUIContent("���°汾��"));
            EditorGUILayout.PropertyField(mUpdateList, new GUIContent("���θ����ļ��б�"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
