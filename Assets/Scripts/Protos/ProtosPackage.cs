using System;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    [Serializable]
    public class ProtosPackage
    {
        /// <summary>
        /// Э���
        /// </summary>
        [SerializeField]
        private uint mCmd;

        public uint Cmd {  get { return mCmd; } }

        /// <summary>
        /// �¼��ص�
        /// </summary>
        private List<Action<byte[]>> mCallBack = new List<Action<byte[]>>();

        /// <summary>
        /// ��ע����¼��б�
        /// </summary>
        [SerializeField]
        private List<string> mCallBackNameList = new List<string>();
        
        public ProtosPackage(uint cmd) 
        {
            mCmd = cmd;
        }

        public void AddEvent(Action<byte[]> action, string actionName)
        {
            mCallBack.Add(action);
            mCallBackNameList.Add(actionName);
        }

        public void RemoveEvent(Action<byte[]> action)
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

        public void TriggerEvent(byte[] arg)
        {
            Action<byte[]>[] action = mCallBack.ToArray();
            for (int i = 0; i < action.Length; i++) 
            {
                action[i].Invoke(arg);
            }
        }

        public int GetCallBackCount()
        {
            return mCallBack.Count;
        }

    }
}
