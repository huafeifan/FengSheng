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

        public const string Resources = "Resources";
        public const string Hotfix = "hotfix";
        public const string Lua = "lua";
        public const string Prefab = "prefab";
        public const string Texture = "ui";
        public const string Protos = "protos";
        /// <summary>
        /// ab包后缀名
        /// </summary>
        public const string abEnd = "asset";

        /// <summary>
        /// 获取manifest文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetResourcesManifestPath()
        {
            return Path.Combine(GetReleasePath(), Manifest);
        }

        /// <summary>
        /// 获取热更文件列表文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetHotfixFilesListPath()
        {
            return Path.Combine(GetReleasePath(), HotfixFilesList);
        }

        /// <summary>
        /// 获取本地版本信息文件路径
        /// </summary>
        /// <returns></returns>
        public static string GetVersionInfoPath()
        {
            return Path.Combine(GetReleasePath(), VersionInfo);
        }

        /// <summary>
        /// 获取本地版本信息
        /// </summary>
        /// <returns></returns>
        public static VersionInfo GetCurrentVersion()
        {
            //读取本地版本信息
            string localVersionInfoPath = Utils.GetVersionInfoPath();
            if (File.Exists(localVersionInfoPath))
            {
                string localVersionInfo = File.ReadAllText(localVersionInfoPath);
                VersionInfo versionInfo = new VersionInfo();
                versionInfo.Analysis(localVersionInfo);
                return versionInfo;
            }
            else
            {
                Debug.LogError($"本地版本信息文件不存在,路径{localVersionInfoPath}");
                return null;
            }
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
            return Path.Combine(GetReleasePath(), Utils.Lua);
        }

        /// <summary>
        /// 获取指定Release lua代码文件
        /// </summary>
        /// <param name="filePathInLua">lua目录下的文件路径</param>
        /// <returns></returns>
        public static string GetReleaseLuaPath(string filePathInLua)
        {
            return Path.Combine(GetReleaseLuaPath(), filePathInLua);
        }

        /// <summary>
        /// 获取Editor lua代码文件目录
        /// </summary>
        /// <returns></returns>
        public static string GetProjectLuaPath()
        {
            return Path.Combine(Application.dataPath, Utils.Resources, Utils.Hotfix, Utils.Lua);
        }

        /// <summary>
        /// 获取服务器更新地址
        /// </summary>
        /// <returns></returns>
        public static string GetUpdateAddress()
        {
            return "http://119.91.49.126/Release";
        }

        /// <summary>
        /// 写入
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
        /// 写入
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

        public static void CopyDirectory(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                CreateDirectory(targetDir);
            }

            //拷贝文件
            string[] files = Directory.GetFiles(sourceDir);
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                string sourceFile = files[i];
                string targetFile = Path.Combine(targetDir, fileName);

                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                }

                File.Copy(sourceFile, targetFile);
            }

            //拷贝文件夹
            string[] dirs = Directory.GetDirectories(sourceDir);
            for (int i = 0; i < dirs.Length; i++)
            {
                string sourceDir2 = dirs[i];
                string targetDir2 = Path.Combine(targetDir, Path.GetFileName(dirs[i]));
                CopyDirectory(sourceDir2, targetDir2);
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                if (files == null || files.Length == 0) return;

                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }

                string[] dirs = Directory.GetDirectories(path);
                for (int i = 0; i < dirs.Length; i++)
                {
                    DeleteDirectory(dirs[i]);
                }
                Directory.Delete(path);
            }
        }

        public static void CopyFile(string sourceFile, string targetFile)
        {
            if (!File.Exists(targetFile))
            {
                string targetDir = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(targetDir))
                {
                    CreateDirectory(targetDir);
                }
            }
            File.Copy(sourceFile, targetFile);
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

    }
}
