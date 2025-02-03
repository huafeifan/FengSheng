using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FengSheng
{
    public class ResourcesManager : FengShengManager
    {
        private readonly Vector2 mDefaultSpritePivot = new Vector2(0.5f, 0.5f);

        /// <summary>
        /// 资源配置表
        /// </summary>
        [SerializeField]
        private List<ResourcesPackage> mResourcesPackageList = new List<ResourcesPackage>();

        /// <summary>
        /// AB包名 AB包
        /// </summary>
        private Dictionary<string, AssetBundle> mAssetBundleDict = new Dictionary<string, AssetBundle>();

        private static ResourcesManager mInstance;
        public static ResourcesManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        private void Update()
        {

        }

        public override void Register()
        {
            IsDisposing = false;

            mInstance = this;

            if (GameManager.Instance.IsEditorMode == false)
            {
                GetResourcesManifestConfig();
            }
        }

        public override void Unregister()
        {
            IsDisposing = true;
            mResourcesPackageList.Clear();
            mAssetBundleDict.Clear();
            if (GameManager.Instance.IsEditorMode == false)
            {
                AssetBundle.UnloadAllAssetBundles(true);
            }
            IsDisposing = false;
        }

        private void GetResourcesManifestConfig()
        {
            string filePath = Utils.GetResourcesManifestPath();

            mResourcesPackageList.Clear();

            string data = File.ReadAllText(filePath);
            string[] lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] item = lines[i].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                mResourcesPackageList.Add(new ResourcesPackage()
                {
                    ResourcesPath = item[0],
                    AssetBundleName = item[1],
                    ResourecesName = item[2]
                });
            }
        }

        public T LoadResourcesFromAssetBundle<T>(string resourcesPath) where T : UnityEngine.Object
        {
            ResourcesPackage pack = mResourcesPackageList.Find(p => p.ResourcesPath.Contains(resourcesPath));
            if (pack == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(pack.AssetBundleName))
            {
                Debug.LogError($"{pack.ResourcesPath}未配置AB包名");
                return null;
            }

            string readPath = Path.Combine(Utils.GetReleasePath(), pack.AssetBundleName);

            if (pack.AssetBundle == null)
            {
                if (!mAssetBundleDict.ContainsKey(pack.AssetBundleName))
                {
                    var ab = AssetBundle.LoadFromFile(readPath);
                    mAssetBundleDict.Add(pack.AssetBundleName, ab);
                }
                pack.AssetBundle = mAssetBundleDict[pack.AssetBundleName];
            }

            if (pack.AssetBundle == null)
            {
                Debug.LogError($"{resourcesPath} ResourcesManager.LoadResourceFromAssetBundle({pack.AssetBundleName}) is null");
                return null;
            }

            return pack.AssetBundle.LoadAsset<T>(pack.ResourecesName);
        }

        public Sprite LoadSprite(string resourcesPath)
        {
            if (GameManager.Instance.IsEditorMode)
            {
                return Resources.Load<Sprite>(resourcesPath);
            }

            Texture2D texture = LoadResourcesFromAssetBundle<Texture2D>(resourcesPath);
            if (texture != null)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), mDefaultSpritePivot);
                return sprite;
            }
            return null;
        }

        public GameObject LoadGameObject(string resourcesPath)
        {
            if (GameManager.Instance.IsEditorMode)
            {
                return Resources.Load<GameObject>(resourcesPath);
            }

            return LoadResourcesFromAssetBundle<GameObject>(resourcesPath);
        }

        public TextAsset LoadProtos(string resourcesPath)
        {
            if (GameManager.Instance.IsEditorMode)
            {
                return Resources.Load<TextAsset>(resourcesPath);
            }

            return LoadResourcesFromAssetBundle<TextAsset>(resourcesPath);
        }

        /// <summary>
        /// 加载lua文件
        /// </summary>
        public string LoadLuaScript(string resourcesPath)
        {
            if (GameManager.Instance.IsEditorMode)
            {
                return Resources.Load<TextAsset>(resourcesPath.Replace(".txt", string.Empty)).text;
            }

            return File.ReadAllText(Path.Combine(Utils.GetReleasePath(), resourcesPath));
        }

    }
}
