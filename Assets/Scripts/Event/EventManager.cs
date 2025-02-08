using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public class EventManager : FengShengManager
    {
        public const string Event_Connect = "Event_Connect";
        public const string Event_Exit = "Event_Exit";
        public const string Event_Restart = "Event_Restart";

        public static readonly string Event_LoadingProgress = "Event_LoadingProgress";//lua中需要用到，故改用static readonly

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

        private Queue<EventTriggerCache> mCache = new Queue<EventTriggerCache>();

        public override IEnumerator Register()
        {
            mInstance = this;
            yield return null;
        }

        public override IEnumerator Unregister()
        {
            mListeners.Clear();
            mCache.Clear();
            yield return null;
        }

        private void Update()
        {
            if (mCache.Count > 0)
            {
                EventTriggerCache cache = mCache.Dequeue();
                EventPackage eventPackage = GetEventPackage(cache.EventName);
                if (eventPackage != null)
                {
                    eventPackage.TriggerEvent(cache.EventPackage);
                }
            }
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
            mCache.Enqueue(new EventTriggerCache() 
            {
                EventName = eventName,
                EventPackage = arg
            });
        }

    }
}
