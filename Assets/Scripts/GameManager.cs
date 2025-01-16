using System;
using System.Collections;
using System.Collections.Generic;
using FengSheng;
using UnityEngine;

namespace FengSheng
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager mInstance;
        public static GameManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        private List<IManager> mManagerList = new List<IManager>();

        private void Awake()
        {
            mInstance = this;

            Init();
        }

        private void OnDestroy()
        {
            Close();
        }

        private void OnApplicationQuit()
        {
            Close();
        }

        private void Init()
        {
            Register<ProtosManager>();
            Register<UIManager>();
            Register<NetManager>();
            Register<EventManager>();

            Register<LuaManager>();
        }

        private void Register<T>() where T : Component, IManager
        {
            var component = transform.GetComponentInChildren<T>();
            mManagerList.Add(component);
            component.Register();
        }

        private void Unregister(IManager manager)
        {
            if (mManagerList.Contains(manager))
            {
                manager.Unregister();
                mManagerList.Remove(manager);
            }
        }

        private void Close()
        {
            for (int i = 0; i < mManagerList.Count; i++)
            {
                mManagerList[i].Unregister();
            }
            mManagerList.Clear();
        }

    }
}
