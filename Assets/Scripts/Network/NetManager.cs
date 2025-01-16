using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public class NetManager : MonoBehaviour, IManager
    {
        public const string Event_Connect = "Event_Connect";

        private static NetManager mInstance;
        public static NetManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// �����б�
        /// </summary>
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

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="ip">������ip</param>
        /// <param name="port">�������˿�</param>
        public void Connect(string name, string ip, int port)
        {
            NetSocket netSocket = null;
            if (mNetDict.ContainsKey(name))
            {
                netSocket = mNetDict[name];
            }
            else
            {
                netSocket = new NetSocket(name, ip, port);
                mNetDict.Add(name, netSocket);
            }

            netSocket.Connect();
        }

        /// <summary>
        /// �ر�����
        /// </summary>
        /// <param name="name"></param>
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

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="netName">��������</param>
        /// <param name="cmd">Э���</param>
        /// <param name="data">����</param>
        public void Send(string netName, uint cmd, byte[] data)
        {
            if (mNetDict.ContainsKey(netName))
            {
                mNetDict[netName].Sender.SendMessage(cmd, data);
            }
        }

    }
}
