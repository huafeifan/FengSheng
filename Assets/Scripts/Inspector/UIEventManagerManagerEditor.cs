using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FengSheng
{
#if UNITY_EDITOR
    [CustomEditor(typeof(UIEventManager))]
    public class UIEventManagerEditor : Editor
    {
        private SerializedProperty mListener;

        private void OnEnable()
        {
            mListener = serializedObject.FindProperty("mListener");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mListener, new GUIContent("事件列表"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
