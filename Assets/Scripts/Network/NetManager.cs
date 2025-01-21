using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public class NetManager : FengShengManager
    {
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
        [SerializeField]
        private List<NetSocket> mNetList = new List<NetSocket>();

        public override void Register()
        {
            mInstance = this;
        }

        public override void Unregister()
        {
            CloseAll();
            mNetList.Clear();
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="name">��������</param>
        /// <param name="ip">������ip</param>
        /// <param name="port">�������˿�</param>
        public void Connect(string name, string ip, int port)
        {
            NetSocket netSocket = GetNetSocket(name);
            if (netSocket == null)
            {
                netSocket = new NetSocket(name, ip, port);
                mNetList.Add(netSocket);
            }

            netSocket.Connect();
        }

        /// <summary>
        /// �ر�����
        /// </summary>
        /// <param name="name"></param>
        public void Close(string name)
        {
            GetNetSocket(name)?.Close();
        }

        public void CloseAll()
        {
            if (mNetList.Count > 0)
            {
                for (int i = 0; i < mNetList.Count; i++)
                {
                    mNetList[i].Close();
                }
            }
        }

        public NetSocket GetNetSocket(string netName)
        {
            for (int i = 0; i < mNetList.Count; i++)
            {
                if (mNetList[i].NetName == netName)
                {
                    return mNetList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="netName">��������</param>
        /// <param name="cmd">Э���</param>
        /// <param name="data">����</param>
        public void Send(string netName, uint cmd, byte[] data)
        {
            NetSocket socket = GetNetSocket(netName);
            if (socket != null)
            {
                socket.Sender.SendMessage(cmd, data);
            }

        }

    }
}
