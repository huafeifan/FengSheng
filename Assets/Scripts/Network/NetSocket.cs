using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace FengSheng
{
    public class NetSocket
    {
        /// <summary>
        /// ��������
        /// </summary>
        private string mNetName;

        /// <summary>
        /// ������ip
        /// </summary>
        private string mServerIp;

        /// <summary>
        /// �������˿�
        /// </summary>
        private int mServerPort;

        /// <summary>
        /// ��ʱ
        /// </summary>
        private int mTimeout = 10000;

        /// <summary>
        /// ����������
        /// </summary>
        private HeartBeat mHeartBeat;

        /// <summary>
        /// ��Ϣ������
        /// </summary>
        private MessageSender mSender;
        public MessageSender Sender { get { return mSender; } }

        /// <summary>
        /// ��Ϣ������
        /// </summary>
        private MessageReceiver mReceiver;

        /// <summary>
        /// ����״̬
        /// </summary>
        public enum State
        {
            Null,
            Busy,
            Connected,
            Error
        }

        /// <summary>
        /// ��ǰ����״̬
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
        /// ����׼��
        /// </summary>
        public void Connect()
        {
            if (mState == State.Connected)
            {
                TriggerConnectEvent(State.Error, "������");
                Debug.Log("������");
                return;
            }
            else if (mState == State.Busy)
            {
                TriggerConnectEvent(State.Busy, "�������ӵ�������...");
                Debug.Log("����һ���������ڽ�����...");
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

                TriggerConnectEvent(State.Busy, "�������ӵ�������...");
                Debug.Log("�������ӵ�������...");

                bool success = result.AsyncWaitHandle.WaitOne(mTimeout, false);

                if (success)
                {
                    try
                    {
                        mClient.EndConnect(result);
                        mState = State.Connected;

                        //��Ϣ��������ʼ��
                        mSender.SetTcpClient(mClient);

                        //��Ϣ��������ʼ��
                        mReceiver.SetTcpClient(mClient);

                        //������������ʼ��
                        mHeartBeat.SetSender(mSender);
                        mHeartBeat.SetTcpClient(mClient);
                        mHeartBeat.SetTimer(1000);

                        TriggerConnectEvent(State.Connected, "�����ӵ�������");
                        Debug.Log("�����ӵ�������");

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
                    TriggerConnectEvent(State.Error, "���ӳ�ʱ��");
                    Debug.Log("���ӳ�ʱ��");
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
                    Debug.Log($"����{mNetName}�����ѹر�");
                }

                if (mHeartBeat != null)
                {
                    mHeartBeat.Close();
                    Debug.Log($"����{mNetName}�����������ѹر�");
                }

                if (mSender != null)
                {
                    mSender.Close();
                    Debug.Log($"����{mNetName}��Ϣ�������ѹر�");
                }

                if (mReceiver != null)
                {
                    mReceiver.Close();
                    Debug.Log($"����{mNetName}��Ϣ�������ѹر�");
                }

                if (mThread != null)
                {
                    mThread.Join();
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception during TcpClient close: {ex.Message}");
            }

            mState = State.Null;
            TriggerConnectEvent(State.Null, "�ͻ����ѹر�");
            Debug.Log("�ͻ����ѹر�");

        }

        /// <summary>
        /// ���������¼�
        /// </summary>
        /// <param name="state"></param>
        /// <param name="msg"></param>
        public void TriggerConnectEvent(State state, string msg)
        {
            switch (state)
            {
                case State.Busy:
                case State.Error:
                case State.Null:
                    EventManager.Instance.TriggerEvent(NetManager.Event_Connect, new NetworkEventPackage()
                    {
                        NetName = mNetName,
                        ConnectResult = false,
                        ConnectMessage = msg
                    });
                    break;
                case State.Connected:
                    EventManager.Instance.TriggerEvent(NetManager.Event_Connect, new NetworkEventPackage()
                    {
                        NetName = mNetName,
                        ConnectResult = true,
                        ConnectMessage = msg
                    });
                    break;
                default:
                    break;
            }
        }

    }
}
