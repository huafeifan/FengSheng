using System.IO;
using UnityEditor;

public class EditorConfig : Editor
{
    [MenuItem("Assets/Create/New lua", false, 1)]
    public static void CreateLua()
    {
        string selectedPath = "Assets";
        if (Selection.assetGUIDs.Length > 0)
        {
            selectedPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            if (Directory.Exists(selectedPath))
            {
                selectedPath = Path.Combine(selectedPath, "NewLuaFile.lua.txt");
            }
            else
            {
                selectedPath = Path.Combine(Path.GetDirectoryName(selectedPath), "NewLuaFile.lua.txt");
            }
        }
        File.WriteAllText(selectedPath, string.Empty);
        AssetDatabase.Refresh();
    }
}
