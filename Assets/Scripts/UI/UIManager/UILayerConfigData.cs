using System;
using UnityEngine;

namespace FengSheng
{
    [Serializable]
    public class UILayerConfigData
    {
        [SerializeField]
        public string Path { get; set; }

        [SerializeField]
        public int SiblingIndex { get; set; }

        [SerializeField]
        public UILayer UILayer { get; set; }
    }
}
