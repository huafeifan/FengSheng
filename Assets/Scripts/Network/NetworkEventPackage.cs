
namespace FengSheng
{
    public class NetworkEventPackage
    {
        /// <summary>
        /// 网络名称
        /// </summary>
        public string NetName {  get; set; }

        /// <summary>
        /// 连接结果
        /// </summary>
        public bool ConnectResult { get; set; }

        /// <summary>
        /// 连接消息
        /// </summary>
        public string ConnectMessage { get; set; }

        /// <summary>
        /// 是否在连接中
        /// </summary>
        public bool IsConnecting {  get; set; }


    }
}
