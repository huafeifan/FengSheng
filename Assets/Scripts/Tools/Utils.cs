using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FengSheng
{
    public class Utils
    {
        /// <summary>
        /// ��ȡmanifest�ļ�·��
        /// </summary>
        /// <returns></returns>
        public static string GetResourcesManifestPath()
        {
            return Path.Combine(GetReleasePath(), "manifest.txt");
        }

        /// <summary>
        /// ��ȡOutputABs/ReleaseĿ¼
        /// </summary>
        public static string GetReleasePath()
        {
            return Path.Combine(GetOutputABsPath(), "Release");
        }

        /// <summary>
        /// ��ȡOutputABsĿ¼
        /// </summary>
        /// <returns></returns>
        public static string GetOutputABsPath()
        {
            return Path.Combine(Path.GetDirectoryName(Application.dataPath), "OutputABs");
        }

        /// <summary>
        /// ��ȡRelease lua�����ļ�Ŀ¼
        /// </summary>
        /// <returns></returns>
        public static string GetReleaseLuaPath()
        {
            return Path.Combine(GetReleasePath(), "lua");
        }

        /// <summary>
        /// ��ȡEditor lua�����ļ�Ŀ¼
        /// </summary>
        /// <returns></returns>
        public static string GetProjectLuaPath()
        {
            return Path.Combine(Application.dataPath, "Resources", "lua");
        }

    }
}
