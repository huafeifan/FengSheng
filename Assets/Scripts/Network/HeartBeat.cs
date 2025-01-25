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
        /// Э���
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
        /// �������
        /// </summary>
        private int mInterval;

        /// <summary>
        /// �������͵�Ĭ������
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
        /// ������Ϣ������
        /// </summary>
        /// <param name="sender"></param>
        public void SetSender(MessageSender sender)
        {
            mSender = sender;
        }

        /// <summary>
        /// ���ü�ʱ��
        /// </summary>
        /// <param name="interval">ʱ����</param>
        public void SetTimer(int interval = 1000)
        {
            mInterval = interval;
            mTimer.Elapsed += OnTimedEvent;
            mTimer.AutoReset = true;
        }

        /// <summary>
        /// �������Ӷ���
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
                UnityEngine.Debug.Log("HeartBeat����ͣ");
                mTimer.Stop(); 
                mTimer.Elapsed -= OnTimedEvent;
            }
        }

        public void Start()
        {
            mTimer.Interval = mInterval;
            mTimer.Enabled = true;
            mTimer.Start();
            Debug.Log("�����������ѿ���");
        }

        public void Close()
        {
            mTimer.Stop();
            mTimer.Enabled= false;
            mTimer.Elapsed -= OnTimedEvent;
        }

    }
}
