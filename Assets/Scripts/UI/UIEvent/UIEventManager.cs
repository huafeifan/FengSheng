using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace FengSheng
{
    public class UIEventManager : FengShengManager
    {
        private static UIEventManager mInstance;
        public static UIEventManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        [SerializeField]
        private List<UIEventPackage> mListener = new List<UIEventPackage>();

        public override IEnumerator Register()
        {
            mInstance = this;
            yield return null;
        }

        public override IEnumerator Unregister()
        {
            //mListenerName.Clear();
            yield return null;
        }

        private void Update()
        {
            
        }

        #region btn
        private UIEventPackage GetUIEventPackage(Button btn)
        {
            for (int i = 0; i < mListener.Count; i++)
            {
                if (mListener[i].Button == btn)
                {
                    return mListener[i];
                }
            }
            return null;
        }

        private string GetActionFullName(UnityEngine.Events.UnityAction action, string actionName)
        {
            return actionName + "_" + action.GetHashCode();
        }

        public void AddListener(Button btn, UnityEngine.Events.UnityAction action, string actionName)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(btn);
            if (uiEventPackage == null)
            {
                uiEventPackage = new UIEventPackage(btn);
                mListener.Add(uiEventPackage);
            }
            uiEventPackage.AddListener(action, GetActionFullName(action, actionName));
        }

        public UnityEngine.Events.UnityAction AddListener(Button btn, UnityEngine.Events.UnityAction<LuaTable> action, LuaTable lua, string actionName)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(btn);
            if (uiEventPackage == null)
            {
                uiEventPackage = new UIEventPackage(btn);
                mListener.Add(uiEventPackage);
            }
            UnityEngine.Events.UnityAction newAction = () => action(lua);
            uiEventPackage.AddListener(newAction, GetActionFullName(newAction, actionName));
            return newAction;
        }

        public void RemoveListener(Button btn, UnityEngine.Events.UnityAction action, string actionName)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(btn);
            if (uiEventPackage != null)
            {
                uiEventPackage.RemoveListener(action, GetActionFullName(action, actionName));
                if (uiEventPackage.GetCallBackCount() == 0)
                {
                    mListener.Remove(uiEventPackage);
                }
            }
        }

        public void RemoveAllListeners(Button btn)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(btn);
            if (uiEventPackage != null)
            {
                uiEventPackage.RemoveAllListeners();
                mListener.Remove(uiEventPackage);
            }
        }
        #endregion

        #region inputField
        private UIEventPackage GetUIEventPackage(InputField inputField)
        {
            for (int i = 0; i < mListener.Count; i++)
            {
                if (mListener[i].InputField == inputField)
                {
                    return mListener[i];
                }
            }
            return null;
        }

        private string GetActionFullName(UnityEngine.Events.UnityAction<string> action, string actionName)
        {
            return actionName + "_" + action.GetHashCode();
        }

        public void AddListener(InputField inputField, UnityEngine.Events.UnityAction<string> action, string actionName)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(inputField);
            if (uiEventPackage == null)
            {
                uiEventPackage = new UIEventPackage(inputField);
                mListener.Add(uiEventPackage);
            }
            uiEventPackage.AddListener(action, GetActionFullName(action, actionName));
        }

        public void RemoveListener(InputField inputField, UnityEngine.Events.UnityAction<string> action, string actionName)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(inputField);
            if (uiEventPackage != null)
            {
                uiEventPackage.RemoveListener(action, GetActionFullName(action, actionName));
                if (uiEventPackage.GetCallBackCount() == 0)
                {
                    mListener.Remove(uiEventPackage);
                }
            }
        }

        public void RemoveAllListeners(InputField inputField)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(inputField);
            if (uiEventPackage != null)
            {
                uiEventPackage.RemoveAllListeners();
                mListener.Remove(uiEventPackage);
            }
        }

        #endregion

        #region toggle
        private UIEventPackage GetUIEventPackage(Toggle toggle)
        {
            for (int i = 0; i < mListener.Count; i++)
            {
                if (mListener[i].Toggle == toggle)
                {
                    return mListener[i];
                }
            }
            return null;
        }

        private string GetActionFullName(UnityEngine.Events.UnityAction<bool> action, string actionName)
        {
            return actionName + "_" + action.GetHashCode();
        }

        public void AddListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> action, string actionName)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(toggle);
            if (uiEventPackage == null)
            {
                uiEventPackage = new UIEventPackage(toggle);
                mListener.Add(uiEventPackage);
            }
            uiEventPackage.AddListener(action, GetActionFullName(action, actionName));
        }

        public void RemoveListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> action, string actionName)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(toggle);
            if (uiEventPackage != null)
            {
                uiEventPackage.RemoveListener(action, GetActionFullName(action, actionName));
                if (uiEventPackage.GetCallBackCount() == 0)
                {
                    mListener.Remove(uiEventPackage);
                }
            }
        }

        public void RemoveAllListeners(Toggle toggle)
        {
            UIEventPackage uiEventPackage = GetUIEventPackage(toggle);
            if (uiEventPackage != null)
            {
                uiEventPackage.RemoveAllListeners();
                mListener.Remove(uiEventPackage);
            }
        }
        #endregion
    }
}
