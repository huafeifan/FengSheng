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

            Register<UIManager>();
            Register<NetManager>();
            Register<LuaManager>();
        }

        private void OnDestroy()
        {
            mManagerList.Clear();
        }

        private void Register<T>() where T : Component, IManager
        {
            var component = transform.GetComponentInChildren<T>();
            mManagerList.Add(component);
            component.Register();
        }

        public void Unregister(IManager manager)
        {
            if (mManagerList.Contains(manager))
            {
                manager.Unregister();
                mManagerList.Remove(manager);
            }
        }

    }
}
