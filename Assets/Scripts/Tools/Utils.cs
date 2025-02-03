using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FengSheng
{
    public class Utils
    {
        /// <summary>
        /// 获取manifest文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetResourcesManifestPath()
        {
            return Path.Combine(GetReleasePath(), "manifest.txt");
        }

        /// <summary>
        /// 获取OutputABs/Release目录
        /// </summary>
        public static string GetReleasePath()
        {
            return Path.Combine(GetOutputABsPath(), "Release");
        }

        /// <summary>
        /// 获取OutputABs目录
        /// </summary>
        /// <returns></returns>
        public static string GetOutputABsPath()
        {
            return Path.Combine(Path.GetDirectoryName(Application.dataPath), "OutputABs");
        }

        /// <summary>
        /// 获取Release lua代码文件目录
        /// </summary>
        /// <returns></returns>
        public static string GetReleaseLuaPath()
        {
            return Path.Combine(GetReleasePath(), "lua");
        }

        /// <summary>
        /// 获取Editor lua代码文件目录
        /// </summary>
        /// <returns></returns>
        public static string GetProjectLuaPath()
        {
            return Path.Combine(Application.dataPath, "Resources", "lua");
        }

    }
}
