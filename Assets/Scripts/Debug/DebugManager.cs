using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public class DebugManager : FengShengManager
    {
        private List<string> mLogMessage = new List<string>();
        public bool DisplayLog = false;
        private GUIStyle mLogStyle;

        private static DebugManager mInstance;
        public static DebugManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        public override IEnumerator Register()
        {
            if (GameManager.Instance.IsOpenDebug)
            {
                mLogStyle = new GUIStyle();
                mLogStyle.fontSize = 25;
                mLogStyle.normal.textColor = Color.white;
                //Application.logMessageReceived += Log;
                Application.logMessageReceivedThreaded += Log;
            }
            mInstance = this;
            yield return null;
        }

        public override IEnumerator Unregister()
        {
            if (GameManager.Instance.IsOpenDebug)
            {
                mLogMessage.Clear();
                mLogStyle = null;
                //Application.logMessageReceived -= Log;
                Application.logMessageReceivedThreaded -= Log;
            }
            yield return null;
        }

        private void Update()
        {
            
        }

        private void Log(string logString, string stack, LogType type)
        {
            mLogMessage.Add(logString);
            if (mLogMessage.Count > 10)
            {
                mLogMessage.RemoveAt(0);
            }
        }

        private void OnGUI()
        {
            if (DisplayLog == false || mLogStyle == null) return;
            int start = Mathf.Max(0, mLogMessage.Count - 10);
            for (int i = start; i < mLogMessage.Count; i++)
            {
                GUI.Label(new Rect(10, 10 + (i - start) * 30, 1000, 30), mLogMessage[i], mLogStyle);
            }
        }

        public void SetDisPlayLog(bool status)
        {
            if (GameManager.Instance.IsOpenDebug)
            {
                DisplayLog = status;
            }
        }

    }
}
