using System.Collections.Generic;
using System.IO;
using FengSheng;
using UnityEditor;
using UnityEngine;

public class CustomBuildWin64 : EditorWindow
{
    public static void BuildGame()
    {
        string[] scenes = { "Assets/Scenes/FengSheng.unity" };
        string outputPath = "E:/FengSheng/Release";
        BuildTarget target = BuildTarget.StandaloneWindows64;
        BuildOptions options = BuildOptions.None;

        BuildPipeline.BuildPlayer(scenes, outputPath, target, options);


        Debug.Log("Build Completed");
    }


    [MenuItem("Build/Build Win64")]
    private static void ShowWindow()
    {
        GetWindow<CustomBuildWin64>();
    }

    private static List<string> scenes = new List<string>(){
        "Assets/Scenes/FengSheng.unity" };

    private static string projectOutputPath = "E:/FengSheng/FengSheng/Build/Release_Win64/FengSheng.exe";

    private static List<string> unpackFolderPaths = new List<string>(){
        "E:/FengSheng/FengSheng/Assets/Resources/hotfix"};

    private static List<string> unpackFilePaths = new List<string>(){
        "E:/FengSheng/FengSheng/Assets/Resources/hotfix.meta"};

    private static string tempCachePath = "E:/FengSheng/FengSheng/Build/FilesCache";

    private void OnGUI()
    {
        GUILayout.Label("需要打包的场景", EditorStyles.boldLabel);
        for (int i = 0; i < scenes.Count; i++)
        {
            scenes[i] = EditorGUILayout.TextField(string.Empty, scenes[i]);
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("添加", GUILayout.Width(50), GUILayout.Height(25)))
        {
            scenes.Add(string.Empty);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("删除", GUILayout.Width(50), GUILayout.Height(25)) && scenes.Count > 1)
        {
            scenes.RemoveAt(scenes.Count - 1);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.Label("包输出路径", EditorStyles.boldLabel);
        projectOutputPath = EditorGUILayout.TextField(string.Empty, projectOutputPath);

        GUILayout.Space(5);

        GUILayout.Label("Assets目录下打包时需要移除的资源", EditorStyles.boldLabel);
        GUILayout.Label("Folder", EditorStyles.label);
        for (int i = 0; i < unpackFolderPaths.Count; i++)
        {
            unpackFolderPaths[i] = EditorGUILayout.TextField(string.Empty, unpackFolderPaths[i]);
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("添加", GUILayout.Width(50), GUILayout.Height(25)))
        {
            unpackFolderPaths.Add(string.Empty);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("删除", GUILayout.Width(50), GUILayout.Height(25)) && unpackFolderPaths.Count > 0)
        {
            unpackFolderPaths.RemoveAt(unpackFolderPaths.Count - 1);
        }

        GUILayout.EndHorizontal();

        GUILayout.Label("File", EditorStyles.label);
        for (int i = 0; i < unpackFilePaths.Count; i++)
        {
            unpackFilePaths[i] = EditorGUILayout.TextField(string.Empty, unpackFilePaths[i]);
        }

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("添加", GUILayout.Width(50), GUILayout.Height(25)))
        {
            unpackFilePaths.Add(string.Empty);
        }

        GUILayout.Space(5);

        if (GUILayout.Button("删除", GUILayout.Width(50), GUILayout.Height(25)) && unpackFilePaths.Count > 0)
        {
            unpackFilePaths.RemoveAt(unpackFilePaths.Count - 1);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.Label("临时文件存放路径", EditorStyles.boldLabel);
        tempCachePath = EditorGUILayout.TextField(string.Empty, tempCachePath);

        GUILayout.Space(20);
        if (GUILayout.Button("Build", GUILayout.Width(100), GUILayout.Height(100)))
        {
            for (int i = 0; i < unpackFolderPaths.Count; i++)
            {
                Utils.CopyDirectory(unpackFolderPaths[i], Path.Combine(tempCachePath, Path.GetFileName(unpackFolderPaths[i])));
                Utils.DeleteDirectory(unpackFolderPaths[i]);
            }

            for (int i = 0; i < unpackFilePaths.Count; i++)
            {
                Utils.CopyFile(unpackFilePaths[i], Path.Combine(tempCachePath, Path.GetFileName(unpackFilePaths[i])));
                Utils.DeleteFile(unpackFilePaths[i]);
            }

            CSObjectWrapEditor.Generator.ClearAll();
            CSObjectWrapEditor.Generator.GenAll();

            BuildPipeline.BuildPlayer(scenes.ToArray(), projectOutputPath, BuildTarget.StandaloneWindows64, BuildOptions.None);

            for (int i = 0; i < unpackFolderPaths.Count; i++)
            {
                string sourceDir = Path.Combine(tempCachePath, Path.GetFileName(unpackFolderPaths[i]));
                Utils.CopyDirectory(sourceDir, unpackFolderPaths[i]);
                Utils.DeleteDirectory(sourceDir);
            }

            for (int i = 0; i < unpackFilePaths.Count; i++)
            {
                string sourceFile = Path.Combine(tempCachePath, Path.GetFileName(unpackFilePaths[i]));
                Utils.CopyFile(sourceFile, unpackFilePaths[i]);
                Utils.DeleteFile(sourceFile);
            }

            Debug.Log("Build Win64 Completed");
        }
        

    }
}