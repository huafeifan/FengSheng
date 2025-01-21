using System.Text;
using UnityEditor;
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
        /// 根据路径加载协议bytes文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] LoadProtoFile(string path)
        {
            var proto = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (proto != null)
            {
                byte[] bytes = proto.bytes;
                if (bytes == null)
                {
                    bytes = Encoding.UTF8.GetBytes(proto.text);
                }
                return bytes;
            }
            return null;
        }

    }

}

