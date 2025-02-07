using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FengSheng
{
    public class Utils
    {
        public const string VersionInfo = "versionInfo.txt";
        public const string Manifest = "manifest.txt";
        public const string HotfixFilesList = "hotfixFilesList.txt";

        /// <summary>
        /// ��ȡmanifest�ļ�·��
        /// </summary>
        /// <returns></returns>
        public static string GetResourcesManifestPath()
        {
            return Path.Combine(GetReleasePath(), Manifest);
        }

        /// <summary>
        /// ��ȡ�ȸ��ļ��б��ļ�·��
        /// </summary>
        /// <returns></returns>
        public static string GetHotfixFilesListPath()
        {
            return Path.Combine(GetReleasePath(), HotfixFilesList);
        }

        /// <summary>
        /// ��ȡ���ذ汾��Ϣ�ļ�·��
        /// </summary>
        /// <returns></returns>
        public static string GetVersionInfoPath()
        {
            return Path.Combine(GetReleasePath(), VersionInfo);
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
        /// ��ȡָ��Release lua�����ļ�
        /// </summary>
        /// <param name="filePathInLua">luaĿ¼�µ��ļ�·��</param>
        /// <returns></returns>
        public static string GetReleaseLuaPath(string filePathInLua)
        {
            return Path.Combine(GetReleaseLuaPath(), filePathInLua);
        }

        /// <summary>
        /// ��ȡEditor lua�����ļ�Ŀ¼
        /// </summary>
        /// <returns></returns>
        public static string GetProjectLuaPath()
        {
            return Path.Combine(Application.dataPath, "Resources", "hotfix", "lua");
        }

        /// <summary>
        /// ��ȡ���������µ�ַ
        /// </summary>
        /// <returns></returns>
        public static string GetUpdateAddress()
        {
            return "http://119.91.49.126/Release";
        }

        /// <summary>
        /// д��
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        public static void WriteFile(string content, string path)
        {
            string directoryPath = Path.GetDirectoryName(path);
            CreateDirectory(directoryPath);
            if (!File.Exists(path)) 
            {
                var file = File.Create(path);
                file.Dispose();
            }

            File.WriteAllText(path, content);
        }

        /// <summary>
        /// д��
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        public static void WriteFile(byte[] content, string path)
        {
            string directoryPath = Path.GetDirectoryName(path);
            CreateDirectory(directoryPath);
            if (!File.Exists(path))
            {
                var file = File.Create(path);
                file.Dispose();
            }

            File.WriteAllBytes(path, content);
        }


        private static void CreateDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath) || Directory.Exists(directoryPath))
            {
                return;
            }

            string parentDirectoryPath = Directory.GetParent(directoryPath).FullName;
            CreateDirectory(parentDirectoryPath);
            Directory.CreateDirectory(directoryPath);
        }

        public static string GetSize(long size)
        {
            if (size < 1024)
            {
                return $"{size.ToString()}B";
            }

            float result = size / 1024.0f;
            if (result < 1024)
            {
                return $"{result.ToString("F2")}KB";
            }

            result /= 1024.0f;
            if (result < 1024)
            {
                return $"{result.ToString("F2")}MB";
            }

            result /= 1024.0f;
            return $"{result.ToString("F2")}GB";
        }

    }
}
