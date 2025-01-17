using Google.Protobuf;
using UnityEngine;

namespace FengSheng {

    public class ProtosManager : FengShengManager
    {
        private static ProtosManager mInstance;
        public static ProtosManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        public override void Register()
        {
            mInstance = this;
        }

        public override void Unregister()
        {
            
        }

        /// <summary>
        /// 0x0001 µÇÂ¼
        /// </summary>
        /// <param name="name"></param>
        public void Send_Login(string netName, string name)
        {
            LoginServer.Login.C2S.Login login = new LoginServer.Login.C2S.Login()
            {
                Name = name,
            };
            NetManager.Instance.Send(netName, 0x0001, login.ToByteArray());
        }
    }

}

