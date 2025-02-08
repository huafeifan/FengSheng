using System;
using System.Collections;
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

        [SerializeField]
        private List<LuaBehaviour> mLuaBehaviourList = new List<LuaBehaviour>();

        public override IEnumerator Register()
        {
            mInstance = this;
            LuaEnvInit();

            yield return null;
        }

        public override IEnumerator Unregister()
        {
            for (int i = 0; i < mLuaBehaviourList.Count; i++)
            {
                mLuaBehaviourList[i]?.Clear();
            }
            mLuaBehaviourList.Clear();
            StopAllCoroutines();

            yield return new WaitForSeconds(0.2f);

            if (luaEnv != null)
            {
                //luaEnv.DoString(@"
                //            local util = require 'XLua/Resources/xlua/util'
                //            util.print_func_ref_by_csharp()");

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
            //����lua�����
            luaEnv = new LuaEnv();

            luaEnv.AddLoader((ref string fileName) =>
            {
                string filePath = string.Empty;
                if (GameManager.Instance.IsEditorMode)
                {
                    filePath = Path.Combine(Application.dataPath, Utils.Resources, Utils.Hotfix, Utils.Lua, fileName + ".lua.txt");
                }
                else
                {
                    filePath = Path.Combine(Utils.GetReleaseLuaPath(), fileName + ".lua.txt");
                }

                return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath));
            });

            //lua-protobuf
            luaEnv.AddBuildin("pb", XLua.LuaDLL.Lua.LoadPb);

            //��ȡlink�ļ�����
            string[] filePaths = GetLinkData();

            //ִ��luaȫ���ļ�,���� main��lua�ű�������ڣ�
            LuaDoString(filePaths);

        }

        private string[] GetLinkData()
        {
            string link = ResourcesManager.Instance.LoadLuaScript("lua/link.txt");
            return link.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }

        private void LuaDoString(string[] filePaths)
        {
            //luaEnv.DoString($@"require 'main'");
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

        private IEnumerator CustomIEnumerator(IEnumerator method, Action onComplete)
        {
            yield return method;
            onComplete?.Invoke();
        }

        public Coroutine AddCoroutine(IEnumerator method, Action onComplete)
        {
            return StartCoroutine(CustomIEnumerator(method, onComplete));
        }
    }
}
