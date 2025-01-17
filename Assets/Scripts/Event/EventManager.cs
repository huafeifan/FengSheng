using System;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public class EventManager : FengShengManager
    {
        public const string Event_Connect = "Event_Connect";
        public const string Event_Exit = "Event_Exit";

        private static EventManager mInstance;
        public static EventManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        [SerializeField]
        private List<EventPackage> mListeners = new List<EventPackage>();

        public override void Register()
        {
            mInstance = this;
        }

        public override void Unregister()
        {
            mListeners.Clear();
        }

        public EventPackage GetEventPackage(string eventName)
        {
            for (int i = 0; i < mListeners.Count; i++)
            {
                if (mListeners[i].Name == eventName)
                {
                    return mListeners[i];
                }
            }
            return null;
        }

        public void AddListener(string eventName, Action<System.Object> callBack)
        {
            EventPackage eventPackage = GetEventPackage(eventName);
            if (eventPackage == null)
            {
                eventPackage = new EventPackage(eventName);
                mListeners.Add(eventPackage);
            }
            eventPackage.AddEvent(callBack);
        }

        public void AddListener(string eventName, Action<System.Object> callBack, string actionName)
        {
            EventPackage eventPackage = GetEventPackage(eventName);
            if (eventPackage == null)
            {
                eventPackage = new EventPackage(eventName);
                mListeners.Add(eventPackage);
            }
            eventPackage.AddEvent(callBack, actionName);
        }

        public void RemoveListener(string eventName, Action<System.Object> callBack)
        {
            EventPackage eventPackage = GetEventPackage(eventName);
            if (eventPackage != null)
            {
                eventPackage.RemoveEvent(callBack);
            }
        }

        public void TriggerEvent(string eventName, object arg)
        {
            EventPackage eventPackage = GetEventPackage(eventName);
            if (eventPackage != null)
            {
                eventPackage.TriggerEvent(arg);
            }
        }

    }
}
