using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XLua;

namespace FengSheng
{
    public class UIManager : MonoBehaviour, IManager
    {
        private static UIManager mInstance;
        public static UIManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        public Transform UIRoot;
        //UICache存储
        private Dictionary<string, UICache> mUICache = new Dictionary<string, UICache>();
        //UICache存在数量下限
        private int mUICacheCountLower;

        public void Register()
        {
            mInstance = this;
            mUICacheCountLower = 10;
        }

        public void Unregister()
        {
            foreach(var cache in mUICache)
            {
                cache.Value.Destory();
            }
            mUICache.Clear();
        }

        private void FixedUpdate()
        {
            if (mUICache.Count > mUICacheCountLower)
            {
                List<string> list = null;
                foreach (var cache in mUICache)
                {
                    if (cache.Value.IsOverTime())
                    {
                        if (list == null) list = new List<string>();
                        list.Add(cache.Key);
                    }
                }
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        mUICache[list[i]].Destory();
                        mUICache.Remove(list[i]);
                    }
                }
            }

        }

        private void OnDestroy()
        {
            Unregister();
        }

        //一种类型窗口只有一个
        public bool OpenUI(string path)
        {


            if (mUICache != null && mUICache.ContainsKey(path))
            {
                mUICache[path].GameObject.SetActive(true);
            }
            else
            {
                Object obj = Resources.Load(path);
                if (obj == null) return false;
                GameObject gameObject = (GameObject)GameObject.Instantiate(obj, UIRoot);
                UICache uICache = new UICache(Time.time, gameObject);
                mUICache.Add(path, uICache);
            }

            return true;
        }

    }
}
