using DG.Tweening;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XLua;
using XLua.LuaDLL;

namespace FengSheng
{
    public class LuaManager : MonoBehaviour, IManager
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
        public static LuaEnv luaEnv;
        public static float lastGCTime = 0;
        public const float GCInterval = 1;//1s

        public static LinkedList<LuaBehaviour> mLuaList = new LinkedList<LuaBehaviour>();

        public void Register()
        {
            mInstance = this;
            LuaEnvInit();
        }

        public void Unregister()
        {
            if (luaEnv != null)
            {
                while (mLuaList.Count > 0)
                {
                    mLuaList.First.Value.Clear();
                    mLuaList.RemoveFirst();
                }
                //luaEnv.DoString(@"
                //                local util = require 'XLua/Resources/xlua/util'
                //                util.print_func_ref_by_csharp()");

                luaEnv.Dispose();
            }
        }

        private void Update()
        {
            if (luaEnv == null) return;

            if (Time.time - lastGCTime > GCInterval)
            {
                luaEnv.Tick();
                lastGCTime = Time.time;
            }
        }

        public void LuaEnvInit()
        {
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
            for (int i = 0; i < filePaths.Length; i++)
            {
                luaEnv.DoString($@"require '{filePaths[i]}'");
            }
        }

        public void PushLuaBehaviour(LuaBehaviour lua)
        {
            mLuaList.AddLast(lua);
        }

        public void PopLuaBehaviour(LuaBehaviour lua) 
        {
            mLuaList.Remove(lua);
        }

    }
}
