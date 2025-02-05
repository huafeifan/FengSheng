using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    [Serializable]
    public class VersionInfo 
    {
        /// <summary>
        /// lua代码版本号
        /// </summary>
        [SerializeField]
        public int LuaVersion = 0;

        /// <summary>
        /// 协议版本号
        /// </summary>
        [SerializeField]
        public int ProtosVersion = 0;

        /// <summary>
        /// 资源文件版本号(预制体和图片纹理)
        /// </summary>
        [SerializeField]
        public int ResourcesVersion = 0;

        public void Analysis(string versionInfo)
        {
            string[] infos = versionInfo.Split(new[] { Environment.NewLine, ":" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < infos.Length; i++)
            {
                switch (infos[i])
                {
                    case "lua version":
                        LuaVersion = int.Parse(infos[++i]);
                        break;
                    case "protos version":
                        ProtosVersion = int.Parse(infos[++i]);
                        break;
                    case "resources version":
                        ResourcesVersion = int.Parse(infos[++i]);
                        break;
                }
            }
        }

        public string GetWrite()
        {
            return string.Format("{0}:{1}\r\n{2}:{3}\r\n{4}:{5}", "lua version", LuaVersion, "protos version", ProtosVersion, "resources version", ResourcesVersion);
        }

    }
}
