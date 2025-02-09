using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FengSheng
{
#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
        private SerializedProperty mManagerList;
        private SerializedProperty mIsEditorMode;
        private SerializedProperty mIsOpenDebug;

        private void OnEnable()
        {
            mManagerList = serializedObject.FindProperty("mManagerList");
            mIsEditorMode = serializedObject.FindProperty("IsEditorMode");
            mIsOpenDebug = serializedObject.FindProperty("IsOpenDebug");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mIsEditorMode, new GUIContent("EditorMode"));
            EditorGUILayout.PropertyField(mIsOpenDebug, new GUIContent("IsOpenDebug"));
            EditorGUILayout.PropertyField(mManagerList, new GUIContent("管理器列表"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
