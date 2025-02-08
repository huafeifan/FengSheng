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

            TriggerLoadingProgressChange(0, "���ڶ�ȡ�汾��Ϣ...");

            //��ȡ���ذ汾��Ϣ
            string localVersionInfoPath = Utils.GetVersionInfoPath();
            if (File.Exists(localVersionInfoPath))
            {
                string localVersionInfo = File.ReadAllText(localVersionInfoPath);
                mCurrentVersion.Analysis(localVersionInfo);
            }
            else
            {
                Debug.LogError($"���ذ汾��Ϣ�ļ�������,·��{localVersionInfoPath}");
            }

            //��ȡ���°汾��Ϣ
            UnityWebRequest versionInfoRequest = UnityWebRequest.Get(Path.Combine(Utils.GetUpdateAddress(), Utils.VersionInfo));
            yield return versionInfoRequest.SendWebRequest();
            string versionInfo = versionInfoRequest.downloadHandler.text;
            mTargetVersion.Analysis(versionInfo);

            TriggerLoadingProgressChange(0, "���ڼ�����...");

            if (IsNeedUpdate() == false)
            {
                TriggerLoadingProgressChange(1, "���ڼ�����...");
                yield break;
            }

            //��ȡ�����ļ��б�
            UnityWebRequest filesListRequest = UnityWebRequest.Get(Path.Combine(Utils.GetUpdateAddress(), Utils.HotfixFilesList));
            yield return filesListRequest.SendWebRequest();
            string filesListString = filesListRequest.downloadHandler.text.Replace("\\", "/");
            string[] filesList = filesListString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            TriggerLoadingProgressChange(0, "���ڼ��������Դ��С...");

            //��������
            mUpdateList.Clear();

            for (int i = 0; i < filesList.Length; i++)
            {
                //�ж�lua����
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
                //�ж�ͼƬ������Դ����
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
                //�ж�Ԥ������Դ����
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
                //�ж�Э�����
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

            //�����ļ���С
            long updateSize = 0;//��Ҫ���µĴ�С

            for (int i = 0; i < mUpdateList.Count; i++)
            {
                updateSize += mUpdateList[i].FileSize;
            }
            string updateSizeString = Utils.GetSize(updateSize);

            //��ʼ����
            long updatedSize = 0;//�Ѹ��µĴ�С
            for (int i = 0; i < mUpdateList.Count; i++) 
            {
                TriggerLoadingProgressChange((float)updatedSize / updateSize, $"������...{Utils.GetSize(updatedSize)}/{updateSizeString}");
                
                UnityWebRequest fileContentRequest = UnityWebRequest.Get(Path.Combine(Utils.GetUpdateAddress(), mUpdateList[i].FilePath));
                fileContentRequest.SendWebRequest();
                while (fileContentRequest.isDone == false)
                {
                    yield return null;
                    long tempUpdateSize = updatedSize + (long)fileContentRequest.downloadedBytes;
                    TriggerLoadingProgressChange((float)tempUpdateSize / updateSize, $"������...{Utils.GetSize(tempUpdateSize)}/{updateSizeString}");
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
            TriggerLoadingProgressChange((float)updatedSize / updateSize, $"������...{Utils.GetSize(updatedSize)}/{updateSizeString}");

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
        /// ֪ͨ������
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

