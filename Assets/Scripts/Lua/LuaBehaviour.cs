/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using XLua;
using System;
using System.Collections.Generic;
using System.IO;

namespace FengSheng
{
    public class LuaBehaviour : MonoBehaviour
    {
        public string LuaScripPath;

        private Action luaStart;
        private Action luaOnEnable;
        private Action luaOnDisable;
        private Action luaUpdate;
        private Action luaOnDestroy;

        private LuaTable scriptScopeTable;

        private bool isDispose;

        void Awake()
        {
            // 为每个脚本设置一个独立的脚本域，可一定程度上防止脚本间全局变量、函数冲突
            scriptScopeTable = LuaManager.Instance.luaEnv.NewTable();

            // 设置其元表的 __index, 使其能够访问全局变量
            using (LuaTable meta = LuaManager.Instance.luaEnv.NewTable())
            {
                meta.Set("__index", LuaManager.Instance.luaEnv.Global);
                scriptScopeTable.SetMetaTable(meta);
            }

            // 将所需值注入到 Lua 脚本域中
            scriptScopeTable.Set("self", this);

            // 如果你希望在脚本内能够设置全局变量, 也可以直接将全局脚本域注入到当前脚本的脚本域中
            // 这样, 你就可以在 Lua 脚本中通过 Global.XXX 来访问全局变量
            scriptScopeTable.Set("Global", LuaManager.Instance.luaEnv.Global);

            // 执行脚本
            string luaScript = ResourcesManager.Instance.LoadLuaScript(LuaScripPath);
            string luaName = Path.GetFileName(LuaScripPath).Replace(".txt", string.Empty);
            LuaManager.Instance.luaEnv.DoString(luaScript, luaName, scriptScopeTable);

            LuaManager.Instance.AddLuaBehaviour(this);
            isDispose = false;

            // 从 Lua 脚本域中获取定义的函数
            Action luaAwake = scriptScopeTable.Get<Action>("awake");
            scriptScopeTable.Get("start", out luaStart);
            scriptScopeTable.Get("onenable", out luaOnEnable);
            scriptScopeTable.Get("ondisable", out luaOnDisable);
            scriptScopeTable.Get("update", out luaUpdate);
            scriptScopeTable.Get("ondestroy", out luaOnDestroy);

            if (luaAwake != null)
            {
                luaAwake();
            }
        }

        // Use this for initialization
        void Start()
        {
            if (luaStart != null)
            {
                luaStart();
            }
        }

        void OnEnable()
        {
            if (luaOnEnable != null)
            {
                luaOnEnable();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (luaUpdate != null)
            {
                luaUpdate();
            }
        }

        void OnDisable()
        {
            if (luaOnDisable != null)
            {
                luaOnDisable();
            }
        }

        void OnDestroy()
        {
            Clear();
        }

        public void Clear()
        {
            if (isDispose) return;
            isDispose = true;

            if (luaOnDestroy != null)
            {
                luaOnDestroy();
            }
            LuaManager.Instance.RemoveLuaBehaviour(this);
            scriptScopeTable.Dispose();
            luaOnDestroy = null;
            luaOnEnable = null;
            luaOnDisable = null;
            luaUpdate = null;
            luaStart = null;
        }

    }
}
