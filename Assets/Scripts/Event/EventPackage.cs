using System;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    [Serializable]
    public class EventPackage
    {
        /// <summary>
        /// �¼�����
        /// </summary>
        [SerializeField]
        private string mName;

        public string Name {  get { return mName; } }

        /// <summary>
        /// �¼��ص�
        /// </summary>
        private List<Action<System.Object>> mCallBack = new List<Action<object>>();

        /// <summary>
        /// ��ע����¼��б�
        /// </summary>
        [SerializeField]
        private List<string> mCallBackNameList = new List<string>();
        
        public EventPackage(string name) 
        {
            mName = name;
        }

        public void AddEvent(Action<System.Object> action)
        {
            mCallBack.Add(action);
            mCallBackNameList.Add(action.Method.ToString());
        }

        public void AddEvent(Action<System.Object> action, string actionName)
        {
            mCallBack.Add(action);
            mCallBackNameList.Add(actionName);
        }

        public void RemoveEvent(Action<System.Object> action)
        {
            for (int i = 0; i < mCallBack.Count; i++) 
            {
                if (mCallBack[i] == action) 
                {
                    mCallBack.RemoveAt(i);
                    mCallBackNameList.RemoveAt(i);
                }
            }
        }

        public void TriggerEvent(object arg)
        {
            Action<object>[] action = mCallBack.ToArray();
            for (int i = 0; i < action.Length; i++) 
            {
                action[i].Invoke(arg);
            }
        }
    }
}
