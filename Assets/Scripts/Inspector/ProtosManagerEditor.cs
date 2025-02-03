using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FengSheng
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ProtosManager))]
    public class ProtosManagerEditor : Editor
    {
        private SerializedProperty mListeners;

        private void OnEnable()
        {
            mListeners = serializedObject.FindProperty("mListeners");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mListeners, new GUIContent("协议监听列表"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
