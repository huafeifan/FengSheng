using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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

        /// <summary>
        /// UI根节点
        /// </summary>
        public Transform UIRoot;

        /// <summary>
        /// UI层级
        /// </summary>
        private Dictionary<UILayer, Transform> mUILayerRootConfig = new Dictionary<UILayer, Transform>();

        /// <summary>
        /// UI层级配置
        /// </summary>
        private Dictionary<string, UILayerConfigData> mUILayerConfig = new Dictionary<string, UILayerConfigData>();

        /// <summary>
        /// UICache存储
        /// </summary>
        private Dictionary<string, UICache> mUICache = new Dictionary<string, UICache>();

        /// <summary>
        /// UICache存在数量下限
        /// </summary>
        private int mUICacheCountLower;

        public void Register()
        {
            mInstance = this;
            mUICacheCountLower = 10;

            mUILayerRootConfig.Add(UILayer.SceneUI_Level1, UIRoot.Find("Scene/SceneUI_Level1"));
            mUILayerRootConfig.Add(UILayer.SceneParticle_Level1, UIRoot.Find("Scene/SceneParticle_Level1"));
            mUILayerRootConfig.Add(UILayer.SceneTempLayer, UIRoot.Find("Scene/SceneTempLayer"));

            mUILayerRootConfig.Add(UILayer.NormalUI_Level1, UIRoot.Find("Normal/NormalUI_Level1"));
            mUILayerRootConfig.Add(UILayer.NormalParticle_Level1, UIRoot.Find("Normal/NormalParticle_Level1"));
            mUILayerRootConfig.Add(UILayer.NormalTempLayer, UIRoot.Find("Normal/NormalTempLayer"));

            mUILayerRootConfig.Add(UILayer.SpecialUI_Level1, UIRoot.Find("Special/SpecialUI_Level1"));
            mUILayerRootConfig.Add(UILayer.SpecialParticle_Level1, UIRoot.Find("Special/SpecialParticle_Level1"));
            mUILayerRootConfig.Add(UILayer.SpecialTempLayer, UIRoot.Find("Special/SpecialTempLayer"));

            mUILayerRootConfig.Add(UILayer.TopUI_Level1, UIRoot.Find("Top/TopUI_Level1"));
            mUILayerRootConfig.Add(UILayer.TopParticle_Level1, UIRoot.Find("Top/TopParticle_Level1"));
            mUILayerRootConfig.Add(UILayer.TopTempLayer, UIRoot.Find("Top/TopTempLayer"));

            GetUILayerConfig(Application.dataPath + "/Resources/lua/global/uiConfig.lua.txt");
        }

        public void Unregister()
        {
            foreach(var cache in mUICache)
            {
                cache.Value.Destory();
            }
            mUICache.Clear();
            mUILayerRootConfig.Clear();
            mUILayerConfig.Clear();
        }

        /// <summary>
        /// 读取UI层级配置
        /// </summary>
        /// <param name="filePath"></param>
        private void GetUILayerConfig(string filePath)
        {
            string data = File.ReadAllText(filePath);
            string[] items = data.Split(new[] { Environment.NewLine, "=" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                if (!string.IsNullOrEmpty(items[i]) && items[i].Contains("_Layer") && !items[i].Contains("_LayerSiblingIndex"))
                {
                    mUILayerConfig.Add(items[i - 1].Trim().Replace("\"", string.Empty), new UILayerConfigData()
                    {
                        UILayer = (UILayer)Enum.Parse(typeof(UILayer), items[i + 1].Trim()),
                        SiblingIndex = int.Parse(items[i + 3].Trim())
                    });
                }
            }

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

        /// <summary>
        /// 打开UI界面 一种类型窗口只有一个
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GameObject OpenUI(string path)
        {
            GameObject gameObject = null;

            if (mUICache != null && mUICache.ContainsKey(path))
            {
                gameObject = mUICache[path].gameObject;
                mUICache[path].FlashTime(Time.time);
                if (gameObject != null) 
                { 
                    gameObject.SetActive(true);
                    return gameObject;
                }
            }

            return LoadUI(path, true);
        }

        /// <summary>
        /// 加载UI的Prefab并创建实例
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public GameObject LoadUI(string path, bool isActive = false)
        {
            UICache uICache = null;
            if (mUICache != null && mUICache.ContainsKey(path))
            {
                uICache = mUICache[path];
            }
            else
            {
                UnityEngine.Object obj = Resources.Load(path);
                if (obj == null) return null;
                GameObject gameObject = (GameObject)GameObject.Instantiate(obj, UIRoot);
                uICache = new UICache(Time.time, gameObject);
                mUICache.Add(path, uICache);
            }

            SetUILayer(path);
            uICache.gameObject.SetActive(isActive);
            uICache.FlashTime(Time.time);

            return uICache.gameObject;
        }

        /// <summary>
        /// 设置ui层级
        /// </summary>
        /// <param name="path"></param>
        public void SetUILayer(string path)
        {
            if (mUICache != null && mUICache.ContainsKey(path))
            {
                UILayer layer = UILayer.NormalUI_Level1;
                int siblingIndex = 0;
                if (mUILayerConfig.ContainsKey(path))
                {
                    layer = mUILayerConfig[path].UILayer;
                    siblingIndex = mUILayerConfig[path].SiblingIndex;
                }

                if (mUILayerRootConfig.ContainsKey(layer))
                {
                    Transform root = mUILayerRootConfig[layer];
                    mUICache[path].transform.SetParent(root);
                    mUICache[path].transform.SetSiblingIndex(siblingIndex);
                }

            }
            
        }

    }
}
