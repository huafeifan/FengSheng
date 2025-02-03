using System;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    [Serializable]
    public class ResourcesPackage
    {
        /// <summary>
        /// Asset目录下的路径
        /// </summary>
        [SerializeField]
        public string ResourcesPath;

        /// <summary>
        /// 所属AB包名
        /// </summary>
        [SerializeField]
        public string AssetBundleName;

        /// <summary>
        /// 资源名
        /// </summary>
        [SerializeField]
        public string ResourecesName;

        /// <summary>
        /// AB包对象
        /// </summary>
        [SerializeField]
        public AssetBundle AssetBundle;
    }
}
