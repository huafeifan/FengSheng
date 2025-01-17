using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FengSheng
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LuaManagerEditor))]
    public class LuaManagerEditor : Editor
    {
        private SerializedProperty mLuaBehaviourList;

        private void OnEnable()
        {
            mLuaBehaviourList = serializedObject.FindProperty("mLuaBehaviourList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mLuaBehaviourList, new GUIContent("LuaBehaviour¡–±Ì"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
