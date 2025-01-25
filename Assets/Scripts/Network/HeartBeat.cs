using System;
using System.Net.Sockets;
using System.Timers;
using UnityEngine;

namespace FengSheng
{
    public class HeartBeat
    {
        private bool isGetCmd = false;
        private uint mCmd;
        /// <summary>
        /// 协议号
        /// </summary>
        public uint Cmd
        {
            get
            {
                if (isGetCmd == false)
                {
                    mCmd = LuaManager.Instance.luaEnv.Global.GetExtension<uint>("Protos.Cmd.HeartBeat");
                    isGetCmd = true;
                }
                return mCmd;
            }
        }

        /// <summary>
        /// 心跳间隔
        /// </summary>
        private int mInterval;

        /// <summary>
        /// 心跳发送的默认数据
        /// </summary>
        private byte[] defaultData = new byte[0];

        private MessageSender mSender;
        private System.Timers.Timer mTimer;
        private TcpClient mTcpClient;

        public HeartBeat()
        {
            mTimer = new System.Timers.Timer();
        }

        /// <summary>
        /// 设置消息发送器
        /// </summary>
        /// <param name="sender"></param>
        public void SetSender(MessageSender sender)
        {
            mSender = sender;
        }

        /// <summary>
        /// 设置计时器
        /// </summary>
        /// <param name="interval">时间间隔</param>
        public void SetTimer(int interval = 1000)
        {
            mInterval = interval;
            mTimer.Elapsed += OnTimedEvent;
            mTimer.AutoReset = true;
        }

        /// <summary>
        /// 传入连接对象
        /// </summary>
        /// <param name="tcpClient"></param>
        public void SetTcpClient(TcpClient tcpClient)
        {
            mTcpClient = tcpClient;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (mTcpClient.Connected)
            {
                mSender.SendMessage(Cmd, defaultData, false);
            }
            else
            {
                UnityEngine.Debug.Log("HeartBeat已暂停");
                mTimer.Stop(); 
                mTimer.Elapsed -= OnTimedEvent;
            }
        }

        public void Start()
        {
            mTimer.Interval = mInterval;
            mTimer.Enabled = true;
            mTimer.Start();
            Debug.Log("心跳处理器已开启");
        }

        public void Close()
        {
            mTimer.Stop();
            mTimer.Enabled= false;
            mTimer.Elapsed -= OnTimedEvent;
        }

    }
}
