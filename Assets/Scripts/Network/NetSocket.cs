using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

namespace FengSheng
{
    public class NetSocket
    {
        private const int BufferSize = 1024;
        private string mServerIp;
        private int mServerPort;
        private int mTimeout = 10000;

        public enum State
        {
            Null,
            Busy,
            Connected,
            Error
        }

        private State mState = State.Null;
        private TcpClient mClient;
        private Thread mThread;
        private NetworkStream mStream;
        private CancellationTokenSource mCts;

        public NetSocket(string serverIp, int serverPort)
        {
            mServerIp = serverIp;
            mServerPort = serverPort;

            mState = State.Null;
        }

        public void Connect()
        {
            if (mState == State.Connected)
            {
                Debug.Log("已连接");
                return;
            }
            else if (mState == State.Busy)
            {
                Debug.Log("已有一个连接正在建立中...");
                return;
            }
            mClient = new TcpClient();
            mThread = new Thread(new ThreadStart(BeginConnect));
            mThread.Start();
        }

        public async void BeginConnect() 
        {
            try
            {
                IAsyncResult result = mClient.BeginConnect(mServerIp, mServerPort, null, null);
                mState = State.Busy;
                Debug.Log("正在连接到服务器...");

                bool success = result.AsyncWaitHandle.WaitOne(mTimeout, false);

                if (success)
                {
                    try
                    {
                        mClient.EndConnect(result);
                        mState = State.Connected;
                        Debug.Log("已连接到服务器");

                        mStream = mClient.GetStream();
                        mCts = new CancellationTokenSource();

                        string message = "Hello, Server!";
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        mStream.Write(data, 0, data.Length);
                        Debug.Log("已发送消息");

                        _ = Task.Run(() => ReceiveDataAsync(mCts.Token));

                        //while (true)
                        //{
                        //    string s = "";
                        //    if (s == "exit") break;
                        //}
                    }
                    catch (SocketException ex)
                    {
                        Debug.Log($"SocketException:{ex.Message}");
                        //mState = State.Error;
                        Close();
                    }
                    catch (Exception socketEx) 
                    {
                        Debug.Log($"Exception during connection completion: {socketEx.Message}");
                        //mState = State.Error;
                        Close();
                    }
                }
                else
                {
                    Debug.Log("连接超时！");
                    //mState = State.Error;
                    Close();
                }
            }
            catch (SocketException socketEx)
            {
                Debug.Log($"SocketException during connection attempt: {socketEx.Message}");
                //mState = State.Error;
                Close();
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception during connection attempt: {ex.Message}");
                //mState = State.Error;
                Close();
            }
        }

        private async Task ReceiveDataAsync(CancellationToken ct)
        {
            byte[] buffer = new byte[BufferSize];
            int bytesRead;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    bytesRead = await mStream.ReadAsync(buffer, 0, BufferSize, ct);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log("收到服务端的消息: " + message);
                    }
                }
                catch (IOException ex)
                {
                    // 处理连接断开等IO异常
                    Debug.Log("接收数据时发生IOException：" + ex.Message);
                    break;
                }
                catch (OperationCanceledException)
                {
                    // 正常情况下，当取消令牌被请求时，会抛出此异常
                    break;
                }
            }
        }

        public void Close()
        {
            try
            {
                if (mClient != null)
                {
                    mClient.Close();
                    mClient = null;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception during TcpClient close: {ex.Message}");
            }

            mState = State.Null;
            Debug.Log("客户端已关闭");
        }

    }
}
