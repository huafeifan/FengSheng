using System.IO;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

namespace FengSheng
{
    [Serializable]
    public class MessageReceiver
    {
        private NetSocket mNetSocket;
        private Stream mStream;
        private CancellationTokenSource mCts;
        [SerializeField]
        private List<byte[]> mBufferPool = new List<byte[]>();
        private Queue<byte[]> mReceivedData = new Queue<byte[]>();

        public MessageReceiver()
        {
            
        }

        /// <summary>
        /// 传入连接对象
        /// </summary>
        /// <param name="tcpClient"></param>
        public void SetTcpClient(TcpClient tcpClient)
        {
            mStream = tcpClient.GetStream();
        }

        /// <summary>
        /// 传入网络对象
        /// </summary>
        /// <param name="netSocket"></param>
        public void SetNetSocket(NetSocket netSocket)
        {
            mNetSocket = netSocket;
        }

        public void Start()
        {
            mCts = new CancellationTokenSource();
            _ = Task.Run(() => ReceiveDataAsync(mCts.Token));
            Debug.Log("消息接受器已开启");
        }

        public void Close()
        {
            mBufferPool.Clear();
            mReceivedData.Clear();
            mCts.Cancel();
        }

        public void FixedUpdate()
        {
            if (mReceivedData.Count > 0)
            {
                int count = Math.Min(mReceivedData.Count, 100);
                for (int i = 0; i < count; i++)
                {
                    DealReceiveData(mReceivedData.Dequeue());
                }
            }
        }

        private byte[] GetBuffer()
        {
            if (mBufferPool.Count == 0)
            {
                return new byte[1024];
            }

            int index = mBufferPool.Count - 1;
            var result = mBufferPool[index];
            mBufferPool.RemoveAt(index);
            return result;
        }

        private async Task ReceiveDataAsync(CancellationToken ct)
        {
            int bytesRead;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var buffer = GetBuffer();
                    bytesRead = await mStream.ReadAsync(buffer, 0, 1024, ct);
                    if (bytesRead > 0)
                    {
                        mReceivedData.Enqueue(buffer);
                    }
                    else
                    {
                        mNetSocket.Close();
                    }
                }
                catch (IOException ex)
                {
                    // 处理连接断开等IO异常
                    Debug.Log("接收数据时发生IOException：" + ex.Message);
                    mNetSocket.Close();
                    break;
                }
                catch (OperationCanceledException)
                {
                    // 正常情况下，当取消令牌被请求时，会抛出此异常
                    break;
                }
            }
        }

        private void DealReceiveData(byte[] buffer)
        {
            uint length = (uint)(buffer[0] << 8) + (uint)buffer[1];
            uint cmd = (uint)(buffer[2] << 8) + (uint)buffer[3];
            if (cmd != mNetSocket.HeartBeat.Cmd)
            {
                Debug.Log($"<color=green> Receive 0x{cmd:x4}, Length {length} </color>");

                uint len = length - 4;
                byte[] data = new byte[len];
                for (int i = 0, j = 4; i < len; i++, j++)
                {
                    data[i] = buffer[j];
                }

                mBufferPool.Add(buffer);
                ProtosManager.Instance.TriggerEvent(cmd, data);
            }
        }
    }
}
