
namespace FengSheng
{
    public class NetworkEventPackage
    {
        /// <summary>
        /// ��������
        /// </summary>
        public string NetName {  get; set; }

        /// <summary>
        /// ���ӽ��
        /// </summary>
        public bool ConnectResult { get; set; }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public string ConnectMessage { get; set; }

        /// <summary>
        /// �Ƿ���������
        /// </summary>
        public bool IsConnecting {  get; set; }


    }
}
