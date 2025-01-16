using System.IO;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace FengSheng
{
    public class MessageReceiver
    {
        private TcpClient mTcpClient;
        private Stream mStream;
        private CancellationTokenSource mCts;

        public MessageReceiver()
        {
            mCts = new CancellationTokenSource();
        }

        /// <summary>
        /// �������Ӷ���
        /// </summary>
        /// <param name="tcpClient"></param>
        public void SetTcpClient(TcpClient tcpClient)
        {
            mTcpClient = tcpClient;
            mStream = tcpClient.GetStream();
        }

        public void Start()
        {
            _ = Task.Run(() => ReceiveDataAsync(mCts.Token));
            Debug.Log("��Ϣ�������ѿ���");
        }

        public void Close()
        {
            mCts.Cancel();
        }

        private async Task ReceiveDataAsync(CancellationToken ct)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    bytesRead = await mStream.ReadAsync(buffer, 0, 1024, ct);
                    if (bytesRead > 0)
                    {
                        uint length = (uint)(buffer[0] << 8) + (uint)buffer[1];
                        uint cmd = (uint)(buffer[2] << 8) + (uint)buffer[3];
                        Debug.Log($"<color=green> Receive 0x{cmd:x4}, Length {length} </color>");
                    }
                }
                catch (IOException ex)
                {
                    // �������ӶϿ���IO�쳣
                    Debug.Log("��������ʱ����IOException��" + ex.Message);
                    break;
                }
                catch (OperationCanceledException)
                {
                    // ��������£���ȡ�����Ʊ�����ʱ�����׳����쳣
                    break;
                }
            }
        }

    }
}
