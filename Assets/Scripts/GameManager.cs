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

        private bool mGameCloseFlag = false;
        private int mGameCloseCount = 0;

        private void Awake()
        {
            mInstance = this;

            Register();
        }

        private void Update()
        {
            if (mGameCloseFlag)
            {
                TryClose();
            }
        }

        private void OnDestroy()
        {
            Unresgister();
        }

        public override void Register()
        {
            Register<ProtosManager>();
            Register<UIManager>();
            Register<NetManager>();
            Register<EventManager>();

            Register<LuaManager>();

            EventManager.Instance.AddListener(EventManager.Event_Exit, OnExit, "CSharp.GameManager.OnExit");
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
                    mManagerList.RemoveAt(i);
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

        private void TryClose()
        {
            mGameCloseCount++;
            if (mGameCloseCount >= 10)
            {
                Unresgister();
            }
        }

        public void Unresgister()
        {
            Register<ProtosManager>();
            Register<UIManager>();
            Register<NetManager>();
            Register<EventManager>();

            Register<LuaManager>();

            mGameCloseCount = 0;
            mGameCloseFlag = false;
        }

        private void OnExit(object obj)
        {
            mGameCloseFlag = true;
        }

    }
}
