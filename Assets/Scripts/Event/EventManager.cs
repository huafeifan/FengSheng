using System;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public class EventManager : MonoBehaviour, IManager
    {
        private static EventManager mInstance;
        public static EventManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        private Dictionary<string, Action<System.Object>> mListeners = new Dictionary<string, Action<System.Object>>();

        public void Register()
        {
            mInstance = this;
        }

        public void Unregister()
        {
            mListeners.Clear();
        }

        public void AddListener(string eventName, Action<System.Object> callBack)
        {
            if (mListeners.ContainsKey(eventName))
            {
                mListeners[eventName] += callBack;
            }
            else
            {
                mListeners.Add(eventName, callBack);
            }
        }

        public void RemoveListener(string eventName, Action<System.Object> callBack)
        {
            if (mListeners.ContainsKey(eventName))
            {
                mListeners[eventName] -= callBack;
                if (mListeners[eventName] == null)
                {
                    mListeners.Remove(eventName);
                }
            }
        }

        public void TriggerEvent(string eventName, object arg)
        {
            if (mListeners.ContainsKey(eventName))
            {
                Action<System.Object> callBack = mListeners[eventName];
                callBack.Invoke(arg);
            }
        }
    }
}
