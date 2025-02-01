using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public class GameManager : FengShengManager
    {
        private static GameManager mInstance;
        public static GameManager Instance
        {
            get
            {
                return mInstance;
            }

        }

        [SerializeField]
        private List<FengShengManager> mManagerList = new List<FengShengManager>();

        private bool mIsRestart = false;

        private void Awake()
        {

            mInstance = this;

            Register();
        }

        private void Update()
        {
            if (IsDisposing)
            {
                for (int i = mManagerList.Count - 1; i > 0; i--)
                {
                    if (mManagerList[i].IsDisposing == false)
                    {
                        mManagerList.RemoveAt(i);
                    }
                    else
                    {
                        return;
                    }
                }
                IsDisposing = false;

                if (mIsRestart)
                {
                    Register();
                }
            }
        }

        public override void Register()
        {
            IsDisposing = false;
            mIsRestart = false;

            Register<ProtosManager>();
            Register<UIManager>();
            Register<NetManager>();
            Register<EventManager>();

            Register<LuaManager>();

            AddListener();
        }

        public override void Unregister()
        {
            RemoveListener();

            Unregister<ProtosManager>();
            Unregister<UIManager>();
            Unregister<NetManager>();
            Unregister<EventManager>();

            Unregister<LuaManager>();

            IsDisposing = true;
        }

        private void Register<T>() where T : FengShengManager
        {
            var component = transform.GetComponentInChildren<T>();
            mManagerList.Add(component);
            component.Register();
        }

        private void Unregister<T>() where T : FengShengManager
        {
            for (int i = 0; i < mManagerList.Count; i++)
            {
                if (mManagerList[i] is T)
                {
                    mManagerList[i].Unregister();
                    break;
                }
            }
        }

        public FengShengManager GetManager<T>() where T : FengShengManager
        {
            for (int i = 0; i < mManagerList.Count; i++)
            {
                if (mManagerList[i] is T)
                {
                    return mManagerList[i];
                }
            }
            return null;
        }

        private void AddListener()
        {
            EventManager.Instance.AddListener(EventManager.Event_Exit, OnExit, "CSharp.GameManager.OnExit");
            EventManager.Instance.AddListener(EventManager.Event_Restart, OnRestart, "CSharp.GameManager.OnRestart");
        }

        private void RemoveListener()
        {
            EventManager.Instance.RemoveListener(EventManager.Event_Exit, OnExit);
            EventManager.Instance.RemoveListener(EventManager.Event_Restart, OnRestart);
        }

        private void OnDestroy()
        {
            Unregister();
        }

        private void OnExit(object obj)
        {
            Unregister();
        }

        private void OnRestart(object obj)
        {
            Unregister();

            mIsRestart = true;
        }


    }
}
