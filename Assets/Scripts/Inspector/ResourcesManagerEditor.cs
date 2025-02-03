using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FengSheng
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ResourcesManager))]
    public class ResourcesManagerEditor : Editor
    {
        private SerializedProperty mResourcesPackageList;

        private void OnEnable()
        {
            mResourcesPackageList = serializedObject.FindProperty("mResourcesPackageList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mResourcesPackageList, new GUIContent("��Դ��ӳ���"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
