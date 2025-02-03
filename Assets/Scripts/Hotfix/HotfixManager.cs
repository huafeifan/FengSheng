using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public class HotfixManager : FengShengManager
    {
        private static HotfixManager mInstance;
        public static HotfixManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        public override IEnumerator Register()
        {
            mInstance = this;
            yield return null;
        }

        public override IEnumerator Unregister()
        {
            yield return null;
        }

        private void Update()
        {
            
        }

    }
}
