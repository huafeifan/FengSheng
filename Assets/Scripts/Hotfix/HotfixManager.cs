using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace FengSheng
{
    public class HotfixManager : FengShengManager
    {
        [SerializeField]
        private VersionInfo mCurrentVersion = new VersionInfo();

        [SerializeField]
        private VersionInfo mTargetVersion = new VersionInfo();

        [SerializeField]
        private List<HotfixFileInfo> mUpdateList = new List<HotfixFileInfo>();

        private static HotfixManager mInstance;
        public static HotfixManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        public override IEnumerator Register()
        {
            mInstance = this;

            TriggerLoadingProgressChange(0, "正在读取版本信息...");

            //读取本地版本信息
            string localVersionInfoPath = Utils.GetVersionInfoPath();
            if (File.Exists(localVersionInfoPath))
            {
                string localVersionInfo = File.ReadAllText(localVersionInfoPath);
                mCurrentVersion.Analysis(localVersionInfo);
            }
            else
            {
                Debug.LogError($"本地版本信息文件不存在,路径{localVersionInfoPath}");
            }

            //获取最新版本信息
            UnityWebRequest versionInfoRequest = UnityWebRequest.Get(Path.Combine(Utils.GetUpdateAddress(), Utils.VersionInfo));
            yield return versionInfoRequest.SendWebRequest();
            string versionInfo = versionInfoRequest.downloadHandler.text;
            mTargetVersion.Analysis(versionInfo);

            TriggerLoadingProgressChange(0, "正在检查更新...");

            if (IsNeedUpdate() == false)
            {
                TriggerLoadingProgressChange(1, "正在检查更新...");
                yield break;
            }

            //获取最新文件列表
            UnityWebRequest filesListRequest = UnityWebRequest.Get(Path.Combine(Utils.GetUpdateAddress(), Utils.HotfixFilesList));
            yield return filesListRequest.SendWebRequest();
            string filesListString = filesListRequest.downloadHandler.text.Replace("\\", "/");
            string[] filesList = filesListString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            TriggerLoadingProgressChange(0, "正在计算更新资源大小...");

            //更新内容
            mUpdateList.Clear();

            for (int i = 0; i < filesList.Length; i++)
            {
                //判断lua更新
                if (mTargetVersion.LuaVersion > mCurrentVersion.LuaVersion && 
                    filesList[i].Contains(Utils.Lua) &&
                    !filesList[i].Contains(Utils.abEnd))
                {
                    string[] files = filesList[i].Split(",");
                    mUpdateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0],
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.Lua
                    });
                }
                //判断图片纹理资源更新
                else if (mTargetVersion.TextureVersion > mCurrentVersion.TextureVersion && 
                    filesList[i].Contains(Utils.Texture + "." + Utils.abEnd))
                {
                    string[] files = filesList[i].Split(",");
                    mUpdateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0],
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.Texture
                    });
                }
                //判断预制体资源更新
                else if (mTargetVersion.PrefabVersion > mCurrentVersion.PrefabVersion && 
                    filesList[i].Contains(Utils.Prefab + "." + Utils.abEnd))
                {
                    string[] files = filesList[i].Split(",");
                    mUpdateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0],
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.Prefab
                    });
                }
                //判断协议更新
                else if (mTargetVersion.ProtosVersion > mCurrentVersion.ProtosVersion && 
                    filesList[i].Contains(Utils.Protos + "." + Utils.abEnd))
                {
                    string[] files = filesList[i].Split(",");
                    mUpdateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0].Replace(".txt", string.Empty),
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.Protos
                    });
                }
                else if (filesList[i].Contains(Utils.Manifest))
                {
                    string[] files = filesList[i].Split(",");
                    mUpdateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0],
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.Manifest
                    });
                }
            }

            //计算文件大小
            long updateSize = 0;//需要更新的大小

            for (int i = 0; i < mUpdateList.Count; i++)
            {
                updateSize += mUpdateList[i].FileSize;
            }
            string updateSizeString = Utils.GetSize(updateSize);

            //开始更新
            long updatedSize = 0;//已更新的大小
            for (int i = 0; i < mUpdateList.Count; i++) 
            {
                TriggerLoadingProgressChange((float)updatedSize / updateSize, $"更新中...{Utils.GetSize(updatedSize)}/{updateSizeString}");
                
                UnityWebRequest fileContentRequest = UnityWebRequest.Get(Path.Combine(Utils.GetUpdateAddress(), mUpdateList[i].FilePath));
                fileContentRequest.SendWebRequest();
                while (fileContentRequest.isDone == false)
                {
                    yield return null;
                    long tempUpdateSize = updatedSize + (long)fileContentRequest.downloadedBytes;
                    TriggerLoadingProgressChange((float)tempUpdateSize / updateSize, $"更新中...{Utils.GetSize(tempUpdateSize)}/{updateSizeString}");
                }

                if (mUpdateList[i].FileType == HotfixFileEnum.Lua ||
                    mUpdateList[i].FileType == HotfixFileEnum.Manifest)
                {
                    string fileContentString = fileContentRequest.downloadHandler.text;
                    string filePath = Path.Combine(Utils.GetReleasePath(), mUpdateList[i].FilePath);
                    Utils.WriteFile(fileContentString, filePath);
                }
                else
                {
                    byte[] fileContentBytes = fileContentRequest.downloadHandler.data;
                    string filePath = Path.Combine(Utils.GetReleasePath(), mUpdateList[i].FilePath);
                    Utils.WriteFile(fileContentBytes, filePath);
                }

                updatedSize += mUpdateList[i].FileSize;
            }
            TriggerLoadingProgressChange((float)updatedSize / updateSize, $"更新中...{Utils.GetSize(updatedSize)}/{updateSizeString}");

            mCurrentVersion.LuaVersion = mTargetVersion.LuaVersion;
            mCurrentVersion.PrefabVersion = mTargetVersion.PrefabVersion;
            mCurrentVersion.TextureVersion = mTargetVersion.TextureVersion;
            mCurrentVersion.ProtosVersion = mTargetVersion.ProtosVersion;
            Utils.WriteFile(mCurrentVersion.GetWrite(), Utils.GetVersionInfoPath());
        }

        public override IEnumerator Unregister()
        {
            yield return null;
        }

        private void Update()
        {

        }

        private bool IsNeedUpdate()
        {
            return mTargetVersion.LuaVersion > mCurrentVersion.LuaVersion ||
                mTargetVersion.ProtosVersion > mCurrentVersion.ProtosVersion ||
                mTargetVersion.TextureVersion > mCurrentVersion.TextureVersion ||
                mTargetVersion.PrefabVersion > mCurrentVersion.PrefabVersion;
        }

        /// <summary>
        /// 通知进度条
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="tip"></param>
        private void TriggerLoadingProgressChange(float progress, string tip)
        {
            EventManager.Instance.TriggerEvent(EventManager.Event_LoadingProgress, new LoadingEventPackage()
            {
                Progress = progress,
                Tips = tip
            });
        }

    }
}

