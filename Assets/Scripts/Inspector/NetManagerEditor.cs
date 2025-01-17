using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FengSheng
{
#if UNITY_EDITOR
    [CustomEditor(typeof(NetManager))]
    public class NetManagerEditor : Editor
    {
        private SerializedProperty mNetList;

        private void OnEnable()
        {
            mNetList = serializedObject.FindProperty("mNetList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mNetList, new GUIContent("ÍøÂçÁÐ±í"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
