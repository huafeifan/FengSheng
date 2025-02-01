using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FengSheng {

    public class ProtosManager : FengShengManager
    {
        [SerializeField]
        private List<ProtosPackage> mListeners = new List<ProtosPackage>();

        [SerializeField]
        private Queue<ProtosTriggerCache> mCache = new Queue<ProtosTriggerCache>();

        private static ProtosManager mInstance;
        public static ProtosManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        public override void Register()
        {
            IsDisposing = false;
            mInstance = this;
        }

        public override void Unregister()
        {
            IsDisposing = true;

            mListeners.Clear();
            mCache.Clear();

            IsDisposing = false;
        }

        private void Update()
        {
            if (mCache.Count > 0)
            {
                ProtosTriggerCache cache = mCache.Dequeue();
                ProtosPackage eventPackage = GetProtosPackage(cache.Cmd);
                if (eventPackage != null)
                {
                    eventPackage.TriggerEvent(cache.Bytes);
                }
            }
        }

        public ProtosPackage GetProtosPackage(uint cmd)
        {
            for (int i = 0; i < mListeners.Count; i++)
            {
                if (mListeners[i].Cmd == cmd)
                {
                    return mListeners[i];
                }
            }
            return null;
        }

        public void AddListener(uint cmd, Action<byte[]> callBack, string actionName)
        {
            ProtosPackage eventPackage = GetProtosPackage(cmd);
            if (eventPackage == null)
            {
                eventPackage = new ProtosPackage(cmd);
                mListeners.Add(eventPackage);
            }
            eventPackage.AddEvent(callBack, actionName);
        }

        public void RemoveListener(uint cmd, Action<byte[]> callBack)
        {
            ProtosPackage eventPackage = GetProtosPackage(cmd);
            if (eventPackage != null)
            {
                eventPackage.RemoveEvent(callBack);
            }
        }

        public void TriggerEvent(uint cmd, byte[] bytes)
        {
            mCache.Enqueue(new ProtosTriggerCache()
            {
                Cmd = cmd,
                Bytes = bytes
            });
        }

        /// <summary>
        /// 根据路径加载协议bytes文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] LoadProtoFile(string path)
        {
            var proto = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (proto != null)
            {
                byte[] bytes = proto.bytes;
                if (bytes == null)
                {
                    bytes = Encoding.UTF8.GetBytes(proto.text);
                }
                return bytes;
            }
            return null;
        }

    }

}

