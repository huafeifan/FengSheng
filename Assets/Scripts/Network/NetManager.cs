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
        /// 网络列表
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
        /// 连接
        /// </summary>
        /// <param name="name">网络名称</param>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">服务器端口</param>
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
        /// 关闭网络
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
        /// 发送消息
        /// </summary>
        /// <param name="netName">网络名称</param>
        /// <param name="cmd">协议号</param>
        /// <param name="data">数据</param>
        public void Send(string netName, uint cmd, byte[] data)
        {
            if (mNetDict.ContainsKey(netName))
            {
                mNetDict[netName].Sender.SendMessage(cmd, data);
            }
        }

    }
}
