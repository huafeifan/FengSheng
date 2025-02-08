
using System;

namespace FengSheng
{
    [Serializable]
    public class HotfixFileInfo
    {
        public HotfixFileEnum FileType;

        public string FilePath;

        public long FileSize;
    }

    public enum HotfixFileEnum
    {
        Lua = 1,
        Texture = 2,
        Protos = 3,
        Prefab = 4,
        Manifest = 5
    }
}
