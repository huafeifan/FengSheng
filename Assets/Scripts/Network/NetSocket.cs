using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace FengSheng
{
    [Serializable]
    public class NetSocket
    {
        /// <summary>
        /// 网络名称
        /// </summary>
        [SerializeField]
        private string mNetName;
        public string NetName { get { return mNetName; } }

        /// <summary>
        /// 服务器ip
        /// </summary>
        [SerializeField]
        private string mServerIp;

        /// <summary>
        /// 服务器端口
        /// </summary>
        [SerializeField]
        private int mServerPort;

        /// <summary>
        /// 超时
        /// </summary>
        private int mTimeout = 10000;

        /// <summary>
        /// 心跳处理器
        /// </summary>
        private HeartBeat mHeartBeat;
        public HeartBeat HeartBeat { get { return mHeartBeat; } }

        /// <summary>
        /// 消息发送器
        /// </summary>
        private MessageSender mSender;
        public MessageSender Sender { get { return mSender; } }

        /// <summary>
        /// 消息接收器
        /// </summary>
        private MessageReceiver mReceiver;

        /// <summary>
        /// 连接状态
        /// </summary>
        public enum State
        {
            Null,
            Busy,
            Connected,
            Error
        }

        /// <summary>
        /// 当前连接状态
        /// </summary>
        private State mState = State.Null;

        private TcpClient mClient;
        private Thread mThread;

        public NetSocket(string netName, string serverIp, int serverPort)
        {
            mNetName = netName;
            mServerIp = serverIp;
            mServerPort = serverPort;
            mState = State.Null;
            mHeartBeat = new HeartBeat();
            mSender = new MessageSender();
            mReceiver = new MessageReceiver();
        }

        /// <summary>
        /// 连接准备
        /// </summary>
        public void Connect()
        {
            if (mState == State.Connected)
            {
                TriggerConnectEvent(State.Error, "已连接");
                Debug.Log("已连接");
                return;
            }
            else if (mState == State.Busy)
            {
                TriggerConnectEvent(State.Busy, "正在连接到服务器...");
                Debug.Log("已有一个连接正在建立中...");
                return;
            }
            mClient = new TcpClient();
            mThread = new Thread(new ThreadStart(BeginConnect));
            mThread.Start();
        }

        public void BeginConnect()
        {
            try
            {
                IAsyncResult result = mClient.BeginConnect(mServerIp, mServerPort, null, null);
                mState = State.Busy;

                TriggerConnectEvent(State.Busy, "正在连接到服务器...");
                Debug.Log("正在连接到服务器...");

                bool success = result.AsyncWaitHandle.WaitOne(mTimeout, false);

                if (success)
                {
                    try
                    {
                        mClient.EndConnect(result);
                        mState = State.Connected;

                        //消息发送器初始化
                        mSender.SetTcpClient(mClient);

                        //消息接收器初始化
                        mReceiver.SetNetSocket(this);
                        mReceiver.SetTcpClient(mClient);

                        //心跳处理器初始化
                        mHeartBeat.SetSender(mSender);
                        mHeartBeat.SetTcpClient(mClient);
                        mHeartBeat.SetTimer(1000);

                        TriggerConnectEvent(State.Connected, "已连接到服务器");
                        Debug.Log("已连接到服务器");

                        mSender.Start();
                        mReceiver.Start();
                        mHeartBeat.Start();
                    }
                    catch (SocketException ex)
                    {
                        TriggerConnectEvent(State.Error, $"SocketException:{ex.Message}");
                        Debug.Log($"SocketException:{ex.Message}");
                        //mState = State.Error;
                        Close();
                    }
                    catch (Exception socketEx)
                    {
                        TriggerConnectEvent(State.Error, $"SocketException:{socketEx.Message}");
                        Debug.Log($"Exception during connection completion: {socketEx.Message}");
                        //mState = State.Error;
                        Close();
                    }
                }
                else
                {
                    TriggerConnectEvent(State.Error, "连接超时！");
                    Debug.Log("连接超时！");
                    //mState = State.Error;
                    Close();
                }
            }
            catch (SocketException socketEx)
            {
                TriggerConnectEvent(State.Error, $"SocketException during connection attempt: {socketEx.Message}");
                Debug.Log($"SocketException during connection attempt: {socketEx.Message}");
                //mState = State.Error;
                Close();
            }
            catch (Exception ex)
            {
                TriggerConnectEvent(State.Error, $"Exception during connection attempt: {ex.Message}");
                Debug.Log($"Exception during connection attempt: {ex.Message}");
                //mState = State.Error;
                Close();
            }
        }

        public void Close()
        {
            try
            {
                if (mClient != null)
                {
                    mClient.Close();
                    mClient.Dispose();
                    mClient = null;
                    Debug.Log($"网络{NetName}连接已关闭");
                }

                if (mHeartBeat != null)
                {
                    mHeartBeat.Close();
                    Debug.Log($"网络{NetName}心跳处理器已关闭");
                }

                if (mSender != null)
                {
                    mSender.Close();
                    Debug.Log($"网络{NetName}消息发送器已关闭");
                }

                if (mReceiver != null)
                {
                    mReceiver.Close();
                    Debug.Log($"网络{NetName}消息接受器已关闭");
                }

                if (mThread != null)
                {
                    mThread.Abort();
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception during TcpClient close: {ex.Message}");
            }

            mState = State.Null;
            TriggerConnectEvent(State.Null, "客户端已关闭");
            Debug.Log("客户端已关闭");

        }

        /// <summary>
        /// 触发连接事件
        /// </summary>
        /// <param name="state"></param>
        /// <param name="msg"></param>
        public void TriggerConnectEvent(State state, string msg)
        {
            EventManager.Instance.TriggerEvent(EventManager.Event_Connect, new NetworkEventPackage()
            {
                NetName = NetName,
                IsConnecting = state == State.Busy,
                ConnectResult = state == State.Connected,
                ConnectMessage = msg
            });

        }

    }
}
