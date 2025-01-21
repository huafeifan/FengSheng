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
        /// 网络列表
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
        /// 连接
        /// </summary>
        /// <param name="name">网络名称</param>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
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
        /// 关闭网络
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
        /// 发送消息
        /// </summary>
        /// <param name="netName">网络名称</param>
        /// <param name="cmd">协议号</param>
        /// <param name="data">数据</param>
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
