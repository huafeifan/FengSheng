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
                TriggerLoadingProgressChange(100, "success");
                yield break;
            }

            //��ȡ�����ļ��б�
            UnityWebRequest filesListRequest = UnityWebRequest.Get(Path.Combine(Utils.GetUpdateAddress(), Utils.HotfixFilesList));
            yield return filesListRequest.SendWebRequest();
            string filesListString = filesListRequest.downloadHandler.text.Replace("\\", "/");
            string[] filesList = filesListString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            TriggerLoadingProgressChange(0, "���ڼ��������Դ��С...");

            //��������
            List<HotfixFileInfo> updateList = new List<HotfixFileInfo>();

            for (int i = 0; i < filesList.Length; i++)
            {
                //�ж�lua����
                if (mTargetVersion.LuaVersion > mCurrentVersion.LuaVersion && filesList[i].Contains("lua"))
                {
                    string[] files = filesList[i].Split(",");
                    updateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0],
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.Lua
                    });
                }
                //�ж���Դ����
                else if (mTargetVersion.ResourcesVersion > mCurrentVersion.ResourcesVersion && filesList[i].Contains("ui"))
                {
                    string[] files = filesList[i].Split(",");
                    updateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0],
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.UI
                    });
                }
                //�ж���Դ����
                else if (mTargetVersion.ResourcesVersion > mCurrentVersion.ResourcesVersion && filesList[i].Contains("prefab"))
                {
                    string[] files = filesList[i].Split(",");
                    updateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0],
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.Prefab
                    });
                }
                //�ж�Э�����
                else if (mTargetVersion.ProtosVersion > mCurrentVersion.ProtosVersion && filesList[i].Contains("protos"))
                {
                    string[] files = filesList[i].Split(",");
                    updateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0].Replace(".txt", string.Empty),
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.Protos
                    });
                }
                else if (filesList[i].Contains("manifest.txt"))
                {
                    string[] files = filesList[i].Split(",");
                    updateList.Add(new HotfixFileInfo()
                    {
                        FilePath = files[0],
                        FileSize = long.Parse(files[1]),
                        FileType = HotfixFileEnum.Manifest
                    });
                }
            }

            //�����ļ���С
            long updateSize = 0;//��Ҫ���µĴ�С

            for (int i = 0; i < updateList.Count; i++)
            {
                updateSize += updateList[i].FileSize;
            }
            string updateSizeString = Utils.GetSize(updateSize);

            //��ʼ����
            long updatedSize = 0;//�Ѹ��µĴ�С
            for (int i = 0; i < updateList.Count; i++) 
            {
                TriggerLoadingProgressChange((float)updatedSize / updateSize, $"������...{Utils.GetSize(updatedSize)}/{updateSizeString}");
                
                UnityWebRequest fileContentRequest = UnityWebRequest.Get(Path.Combine(Utils.GetUpdateAddress(), updateList[i].FilePath));
                fileContentRequest.SendWebRequest();
                while (fileContentRequest.isDone == false)
                {
                    yield return null;
                    long tempUpdateSize = updatedSize + (long)fileContentRequest.downloadedBytes;
                    TriggerLoadingProgressChange((float)tempUpdateSize / updateSize, $"������...{Utils.GetSize(tempUpdateSize)}/{updateSizeString}");
                }

                if (updateList[i].FileType == HotfixFileEnum.Lua ||
                    updateList[i].FileType == HotfixFileEnum.Manifest)
                {
                    string fileContentString = fileContentRequest.downloadHandler.text;
                    string filePath = Path.Combine(Utils.GetReleasePath(), updateList[i].FilePath);
                    Utils.WriteFile(fileContentString, filePath);
                }
                else
                {
                    byte[] fileContentBytes = fileContentRequest.downloadHandler.data;
                    string filePath = Path.Combine(Utils.GetReleasePath(), updateList[i].FilePath);
                    Utils.WriteFile(fileContentBytes, filePath);
                }

                updatedSize += updateList[i].FileSize;
            }
            TriggerLoadingProgressChange((float)updatedSize / updateSize, $"������...{Utils.GetSize(updatedSize)}/{updateSizeString}");

            mCurrentVersion.LuaVersion = mTargetVersion.LuaVersion;
            mCurrentVersion.ResourcesVersion = mTargetVersion.ResourcesVersion;
            mCurrentVersion.ProtosVersion = mTargetVersion.ProtosVersion;
            Utils.WriteFile(mCurrentVersion.GetWrite(), Utils.GetVersionInfoPath());

            TriggerLoadingProgressChange(100, "success");
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
                mTargetVersion.ResourcesVersion > mCurrentVersion.ResourcesVersion;
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
                Name = Loading.Progress_Hotfix,
                Progress = progress,
                Tips = tip
            });
        }

    }
}

