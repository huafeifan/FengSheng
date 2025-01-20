using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace FengSheng
{
    public class UIManager : FengShengManager
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
        /// UI���ڵ�
        /// </summary>
        public Transform UIRoot;

        /// <summary>
        /// UI�㼶
        /// </summary>
        private Dictionary<UILayer, Transform> mUILayerRootConfig = new Dictionary<UILayer, Transform>();

        /// <summary>
        /// UI�㼶����
        /// </summary>
        [SerializeField]
        private List<UILayerConfigData> mUILayerConfig = new List<UILayerConfigData>();

        /// <summary>
        /// UICache�洢
        /// </summary>
        [SerializeField]
        private List<UICache> mUICache = new List<UICache>();

        /// <summary>
        /// UICache������������
        /// </summary>
        private int mUICacheCountLower;

        /// <summary>
        /// UI��������ˢ�¼��
        /// </summary>
        private float mUIFlashInterval;

        public override void Register()
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

        public override void Unregister()
        {
            foreach (var cache in mUICache)
            {
                cache.Destory();
            }
            mUICache.Clear();
            mUILayerRootConfig.Clear();
            mUILayerConfig.Clear();
        }

        /// <summary>
        /// ��ȡUI�㼶����
        /// </summary>
        /// <param name="filePath"></param>
        private void GetUILayerConfig(string filePath)
        {
            string data = File.ReadAllText(filePath);
            string[] items = data.Split(new[] { Environment.NewLine, "=" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                if (!string.IsNullOrEmpty(items[i]) && items[i].Contains("_Path"))
                {
                    mUILayerConfig.Add(new UILayerConfigData()
                    {
                        Path = items[i + 1].Trim().Replace("\"", string.Empty),
                        UILayer = (UILayer)Enum.Parse(typeof(UILayer), items[i + 3].Trim()),
                        SiblingIndex = int.Parse(items[i + 5].Trim())
                    });
                }
            }

        }

        private void FixedUpdate()
        {
            mUIFlashInterval += Time.deltaTime;
            if (mUIFlashInterval >= 1)
            {
                for (int i = mUICache.Count - 1; i >= 0; i--)
                {
                    if (mUICache[i].gameObject.activeSelf)
                    {
                        mUICache[i].FlashTime(Time.time);
                    }
                    else if (mUICache.Count > mUICacheCountLower && mUICache[i].IsOverTime())
                    {
                        mUICache[i].Destory();
                        mUICache.RemoveAt(i);
                    }
                }
                mUIFlashInterval = 0;
            }

        }

        /// <summary>
        /// ��UI���� һ�����ʹ���ֻ��һ��
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GameObject OpenUI(string path)
        {
            UICache uiCache = GetUICache(path);
            if (uiCache != null)
            {
                uiCache.FlashTime(Time.time);
                uiCache.gameObject.SetActive(true);
                return uiCache.gameObject;
            }
            return LoadUI(path, true);
        }

        /// <summary>
        /// ����UI��Prefab������ʵ��
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public GameObject LoadUI(string path, bool isActive = false)
        {
            UICache uiCache = GetUICache(path);
            if (uiCache == null)
            {
                UnityEngine.Object obj = Resources.Load(path);
                if (obj == null) return null;
                GameObject gameObject = (GameObject)GameObject.Instantiate(obj, UIRoot);
                uiCache = new UICache(path, Time.time, gameObject);
                mUICache.Add(uiCache);
            }

            SetUILayer(path);
            uiCache.gameObject.SetActive(isActive);
            uiCache.FlashTime(Time.time);

            return uiCache.gameObject;
        }

        /// <summary>
        /// ��ȡ�Ѵ�����UICache
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public UICache GetUICache(string path)
        {
            for (int i = 0; i < mUICache.Count; i++)
            {
                if (mUICache[i].Path == path)
                {
                    return mUICache[i];
                }
            }
            return null;
        }

        /// <summary>
        /// ����ui�㼶
        /// </summary>
        /// <param name="path"></param>
        public void SetUILayer(string path)
        {
            UICache uiCache = GetUICache(path);
            if (uiCache != null)
            {
                UILayer layer = UILayer.NormalUI_Level1;
                int siblingIndex = 0;

                UILayerConfigData data = GetUILayerConfigData(path);
                if (data != null)
                {
                    layer = data.UILayer;
                    siblingIndex = data.SiblingIndex;
                }

                if (mUILayerRootConfig.ContainsKey(layer))
                {
                    Transform root = mUILayerRootConfig[layer];
                    uiCache.transform.SetParent(root);
                    uiCache.transform.SetSiblingIndex(siblingIndex);
                }
            }

        }

        public UILayerConfigData GetUILayerConfigData(string path)
        {
            for (int i = 0; i < mUILayerConfig.Count; i++)
            {
                if (mUILayerConfig[i].Path == path)
                {
                    return mUILayerConfig[i];
                }
            }
            return null;
        }

    }
}
