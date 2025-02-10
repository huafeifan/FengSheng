using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FengSheng
{
    [Serializable]
    public class UIEventPackage
    {
        /// <summary>
        /// 按钮
        /// </summary>
        [SerializeField]
        public Button Button;

        /// <summary>
        /// 输入框
        /// </summary>
        [SerializeField]
        public InputField InputField;

        /// <summary>
        /// 选择
        /// </summary>
        [SerializeField]
        public Toggle Toggle;

        /// <summary>
        /// 已注册的事件列表
        /// </summary>
        [SerializeField]
        private List<string> mCallBackNameList = new List<string>();

        public UIEventPackage(Button btn)
        {
            Button = btn;
        }

        public UIEventPackage(InputField inputField)
        {
            InputField = inputField;
        }

        public UIEventPackage(Toggle toggle)
        {
            Toggle = toggle;
        }

        public void AddListener(UnityEngine.Events.UnityAction action, string actionName)
        {
            Button.onClick.AddListener(action);
            mCallBackNameList.Add(actionName);
        }

        public void RemoveListener(UnityEngine.Events.UnityAction action, string actionName)
        {
            Button.onClick.RemoveListener(action);
            mCallBackNameList.Remove(actionName);
        }

        public void AddListener(UnityEngine.Events.UnityAction<string> action, string actionName)
        {
            InputField.onValueChanged.AddListener(action);
            mCallBackNameList.Add(actionName);
        }

        public void RemoveListener(UnityEngine.Events.UnityAction<string> action, string actionName)
        {
            InputField.onValueChanged.RemoveListener(action);
            mCallBackNameList.Remove(actionName);
        }

        public void AddListener(UnityEngine.Events.UnityAction<bool> action, string actionName)
        {
            Toggle.onValueChanged.AddListener(action);
            mCallBackNameList.Add(actionName);
        }

        public void RemoveListener(UnityEngine.Events.UnityAction<bool> action, string actionName)
        {
            Toggle.onValueChanged.RemoveListener(action);
            mCallBackNameList.Remove(actionName);
        }

        public void RemoveAllListeners()
        {
            if (Button != null)
            {
                Button.onClick.RemoveAllListeners();
            }

            if (InputField != null)
            {
                InputField.onValueChanged.RemoveAllListeners();
            }

            if (Toggle != null)
            {
                Toggle.onValueChanged.RemoveAllListeners();
            }
            mCallBackNameList.Clear();
        }

        public int GetCallBackCount()
        {
            return mCallBackNameList.Count;
        }
    }

}
