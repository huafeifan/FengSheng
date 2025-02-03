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
        private SerializedProperty mListeners;

        private void OnEnable()
        {
            //mListeners = serializedObject.FindProperty("mListeners");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //EditorGUILayout.PropertyField(mListeners, new GUIContent("事件列表"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
