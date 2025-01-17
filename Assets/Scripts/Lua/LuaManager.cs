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
        /// 定义一个Lua虚拟机，建议全局唯一
        /// </summary>
        public LuaEnv luaEnv;
        private float lastGCTime = 0;
        private const float GCInterval = 1;//1s

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

            if (luaEnv != null)
            {
                //luaEnv.DoString(@"
                //                local util = require 'XLua/Resources/xlua/util'
                //                util.print_func_ref_by_csharp()");

                luaEnv.Dispose();
                luaEnv = null;
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
            //读取link文件数据
            string[] filePaths = GetLinkData(Application.dataPath + "/Resources/lua/link.txt");

            //创建lua虚拟机
            luaEnv = new LuaEnv();

            //添加lua全局文件加载路径
            AddLoader();

            //执行lua全局文件,包括 main（lua脚本的主入口）
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
