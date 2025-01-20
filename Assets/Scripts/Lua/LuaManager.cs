using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XLua;

namespace FengSheng
{
    public class LuaManager : FengShengManager
    {
        private static LuaManager mInstance;
        public static LuaManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// ����һ��Lua�����������ȫ��Ψһ
        /// </summary>
        public LuaEnv luaEnv;
        private float lastGCTime = 0;
        private const float GCInterval = 1;//1s
        private float luaEnvDisposeDelay;
        private bool luaEnvDisposeFlag;

        [SerializeField]
        private List<LuaBehaviour> mLuaBehaviourList = new List<LuaBehaviour>();

        public override void Register()
        {
            mInstance = this;
            LuaEnvInit();
        }

        public override void Unregister()
        {
            for (int i = 0; i < mLuaBehaviourList.Count; i++)
            {
                mLuaBehaviourList[i]?.Clear();
            }
            mLuaBehaviourList.Clear();

            luaEnvDisposeFlag = true;

        }

        private void Update()
        {
            if (luaEnv == null) return;

            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                lastGCTime = Time.time;
            }

            if (!luaEnvDisposeFlag) return;
            luaEnvDisposeDelay += Time.deltaTime;
            if (luaEnvDisposeDelay >= 0.2f)
            {
                if (luaEnv != null)
                {
                    //luaEnv.DoString(@"
                    //            local util = require 'XLua/Resources/xlua/util'
                    //            util.print_func_ref_by_csharp()");

                    luaEnv.Dispose();
                    luaEnv = null;
                }
                luaEnvDisposeDelay = 0;
                luaEnvDisposeFlag = false;
            }
        }

        public void LuaEnvInit()
        {
            luaEnvDisposeDelay = 0;
            luaEnvDisposeFlag = false;

            //��ȡlink�ļ�����
            string[] filePaths = GetLinkData(Application.dataPath + "/Resources/lua/link.txt");

            //����lua�����
            luaEnv = new LuaEnv();

            //���luaȫ���ļ�����·��
            AddLoader();

            //ִ��luaȫ���ļ�,���� main��lua�ű�������ڣ�
            LuaDoString(filePaths);

        }

        private string[] GetLinkData(string filePath)
        {
            string link = File.ReadAllText(filePath);
            return link.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }

        private void AddLoader()
        {
            luaEnv.AddLoader((ref string fileName) =>
            {
                string filePath = Application.dataPath + "/" + fileName + ".lua.txt";
                return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath));
            });
        }

        private void LuaDoString(string[] filePaths)
        {
            //luaEnv.DoString($@"require 'Resources/lua/main'");
            for (int i = 0; i < filePaths.Length; i++)
            {
                luaEnv.DoString($@"require '{filePaths[i]}'");
            }
        }

        public void AddLuaBehaviour(LuaBehaviour luaBehaviour)
        {
            mLuaBehaviourList.Add(luaBehaviour);
        }

        public void RemoveLuaBehaviour(LuaBehaviour luaBehaviour)
        {
            mLuaBehaviourList.Remove(luaBehaviour);
        }

    }
}
