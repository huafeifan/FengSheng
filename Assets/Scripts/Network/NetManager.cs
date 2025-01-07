using System.Collections;
using System.Collections.Generic;
using FengSheng;
using UnityEngine;

namespace FengSheng
{
    public class NetManager : MonoBehaviour, IManager
    {
        private static NetManager mInstance;
        public static NetManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        private Dictionary<string, NetSocket> mNetDict = new Dictionary<string, NetSocket>();

        public void Register()
        {
            mInstance = this;
        }

        public void Unregister()
        {
            CloseAll();
            mNetDict.Clear();
        }

        private void OnDestroy()
        {
            Unregister();
        }

        public void Connect(string name, string ip, int port)
        {
            NetSocket netSocket = null;
            if (mNetDict.ContainsKey(name))
            {
                netSocket = mNetDict[name];
            }
            else
            {
                netSocket = new NetSocket(ip, port);
                mNetDict.Add(name, netSocket);
            }

            netSocket.Connect();
        }

        public void Close(string name)
        {
            if (mNetDict.ContainsKey(name))
            {
                mNetDict[name].Close();
            }
        }

        public void CloseAll()
        {
            if (mNetDict.Count > 0)
            {
                foreach (var net in mNetDict)
                {
                    Close(net.Key);
                }
            }
        }

    }
}
