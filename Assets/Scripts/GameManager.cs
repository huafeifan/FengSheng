using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FengSheng
{
    public class GameManager : FengShengManager
    {
        public enum Status
        {
            /// <summary>
            /// 运行中
            /// </summary>
            Run = 1,

            /// <summary>
            /// 正在注册中
            /// </summary>
            Registering = 2,

            /// <summary>
            /// 正在注销中
            /// </summary>
            Unregistering = 3,
        }

        public bool IsEditorMode = true;

        private Status mStatus;

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

        private Loading mLoading;

        private void Awake()
        {
            mInstance = this;
            mStatus = Status.Run;

            mLoading = new Loading(GameObject.Find("UIRoot/Bg").transform);
            StartCoroutine(Register());
        }

        private void Update()
        {
            
        }
        
        public override IEnumerator Register()
        {
            if (mStatus != Status.Run) 
                yield break;

            mStatus = Status.Registering;

            yield return Register<EventManager>();
            mLoading.AddListener();

            EventManager.Instance.TriggerEvent(EventManager.Event_LoadingProgress, new LoadingEventPackage()
            {
                Progress = 0,
                Tips = "start"
            });

            if (!IsEditorMode)
            {
                yield return Register<HotfixManager>();
            }
            yield return Register<ResourcesManager>();
            yield return Register<ProtosManager>();
            yield return Register<UIManager>();
            yield return Register<NetManager>();

            yield return Register<LuaManager>();

            AddListener();
            mStatus = Status.Run;
        }

        public override IEnumerator Unregister()
        {
            mStatus = Status.Unregistering;
            RemoveListener();

            if (!IsEditorMode)
            {
                yield return Unregister<HotfixManager>();
            }
            yield return Unregister<ProtosManager>();
            yield return Unregister<UIManager>();
            yield return Unregister<NetManager>();
            yield return Unregister<EventManager>();

            yield return Unregister<LuaManager>();
            yield return Unregister<ResourcesManager>();

            mLoading.Dispose();
            mStatus = Status.Run;
        }

        private IEnumerator Register<T>() where T : FengShengManager
        {
            var component = transform.GetComponentInChildren<T>();
            mManagerList.Add(component);
            yield return component.Register();
        }

        private IEnumerator Unregister<T>() where T : FengShengManager
        {
            var component = transform.GetComponentInChildren<T>();
            mManagerList.Remove(component);
            yield return component.Unregister();
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
            //Unregister();
        }

        private void OnExit(object obj)
        {
            if (mStatus == Status.Run)
                StartCoroutine(Exit());
        }

        private void OnRestart(object obj)
        {
            if (mStatus == Status.Run)
                StartCoroutine(Restart());
        }

        private IEnumerator Restart()
        {
            yield return Unregister();

            StartCoroutine(Register());
        }

        private IEnumerator Exit()
        {
            yield return Unregister();

            Application.Quit();
        }


    }
}
