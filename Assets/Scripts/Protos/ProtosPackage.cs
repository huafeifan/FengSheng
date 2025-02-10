using System;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    [Serializable]
    public class ProtosPackage
    {
        /// <summary>
        /// 协议号
        /// </summary>
        [SerializeField]
        private uint mCmd;

        public uint Cmd {  get { return mCmd; } }

        /// <summary>
        /// 事件回调
        /// </summary>
        private List<Action<byte[]>> mCallBack = new List<Action<byte[]>>();

        /// <summary>
        /// 已注册的事件列表
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
