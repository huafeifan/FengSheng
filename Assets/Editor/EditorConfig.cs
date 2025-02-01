using System.IO;
using UnityEditor;

public class EditorConfig : Editor
{
    [MenuItem("Assets/Create/New Lua/Scene", false, 1)]
    public static void CreateLua_Scene()
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
        File.WriteAllText(selectedPath, GetNewLuaText(NewLuaText.Scene));
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Create/New Lua/Layer", false, 1)]
    public static void CreateLua_Layer()
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
        File.WriteAllText(selectedPath, GetNewLuaText(NewLuaText.Layer));
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Create/New Lua/Class", false, 1)]
    public static void CreateLua_Class()
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
        File.WriteAllText(selectedPath, GetNewLuaText(NewLuaText.Class));
        AssetDatabase.Refresh();
    }

    public enum NewLuaText
    {
        None,
        Scene,
        Layer,
        Class
    }

    private static string GetNewLuaText(NewLuaText type)
    {
        switch (type)
        {
            case NewLuaText.None:
                return string.Empty;
            case NewLuaText.Scene:
                return
@"local ui = {}
                        
local InitUIReference = function()

end

local InitUI = function()

end

local AddUIEvent = function()

end

local RemoveUIEvent = function()

end

local AddListener = function()

end

local RemoveListener = function()

end

function awake()
	InitUIReference()
end

function onenable()
    InitUI()
	AddUIEvent()
	AddListener()
end

function ondisable()
	RemoveUIEvent()
	RemoveListener()
end

function ondestroy()
	RemoveUIEvent()
	RemoveListener()
end";
            case NewLuaText.Layer:
            return
@"local ui = {}
                        
local InitUIReference = function()

end

local InitUI = function()

end

local AddUIEvent = function()

end

local RemoveUIEvent = function()

end

local AddListener = function()

end

local RemoveListener = function()

end

function awake()
	InitUIReference()
end

function onenable()
    InitUI()
	AddUIEvent()
	AddListener()
end

function ondisable()
	RemoveUIEvent()
	RemoveListener()
end

function ondestroy()
	RemoveUIEvent()
	RemoveListener()
end";
                case NewLuaText.Class:
                    return 
@"NewClassName = {}
NewClassName.__index = NewClassName

function NewClassName:New(gameObject)
	local item = {}
	item.gameObject = gameObject
	item.transform = gameObject.transform
	item.ui = {}
	
	setmetatable(item, NewClassName)
	return item
end";
                default:
                    return string.Empty;
        }
    }


}
