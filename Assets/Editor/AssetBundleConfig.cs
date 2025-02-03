using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using FengSheng;
using System.Linq;

public class AssetBundleConfig : EditorWindow
{
    private static string mCurrentCatalogue = "Release";

    [MenuItem("Build/打包窗口")]
    private static void ShowWindow()
    {
        GetWindow<AssetBundleConfig>();
    }

    private string abName = string.Empty;
    private string folderPath = string.Empty;
    private void OnGUI()
    {
        GUILayout.Label("AssetBundle Name", EditorStyles.boldLabel);
        abName = EditorGUILayout.TextField("包名", abName);

        GUILayout.Label("Folder Path", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("路径", folderPath);

        if (GUILayout.Button("设置包名并打资源"))
        {
            string outputPath = Path.Combine(Utils.GetOutputABsPath(), mCurrentCatalogue);

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            string[] guids = AssetDatabase.FindAssets("t:Texture2D t:Prefab t:TextAsset", new string[] { folderPath });
            string[] assetPaths = new string[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                AssetImporter importer = AssetImporter.GetAtPath(assetPaths[i]);
                if (importer != null)
                {
                    Debug.Log(assetPaths[i]);
                    importer.assetBundleName = abName;
                }
            }

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = abName;
            buildMap[0].assetNames = assetPaths;

            BuildPipeline.BuildAssetBundles(outputPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

            AssetDatabase.Refresh();
            Debug.Log(@$"Build Windows64 AssetBundles Completed
<color=green>folderPath</color>:{folderPath}
<color=green>abName</color>:{abName}
<color=green>outputPath</color>:{outputPath}");
        }

    }

    [MenuItem("Build/清空所有包名")]
    private static void ClearAllABsLabel()
    {
        try
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in allAssets)
            {
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    continue;
                }

                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (importer != null)
                {
                    if (!string.IsNullOrEmpty(importer.assetBundleVariant))
                    {
                        importer.assetBundleVariant = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(importer.assetBundleName))
                    {
                        importer.assetBundleName = string.Empty;
                    }
                }
            }
            AssetDatabase.Refresh();
            Debug.Log("清空所有包名完成");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// 生成AB包映射文件
    /// </summary>
    [MenuItem("Build/生成AB包映射文件(ui,prefab,protos)")]
    private static void GenerateAssetBundleManifest()
    {
        try
        {
            string filePath = Utils.GetResourcesManifestPath();
            string outputPath = Path.Combine(Utils.GetOutputABsPath(), mCurrentCatalogue);
            string[] guids = AssetDatabase.FindAssets("t:Texture2D t:Prefab t:TextAsset", new string[] { "Assets/Resources" });

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            File.WriteAllText(filePath, string.Empty);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (path.Contains("lua")) continue;
                AssetImporter importer = AssetImporter.GetAtPath(path);
                if (string.IsNullOrEmpty(importer.assetBundleName))
                {
                    Debug.LogError($"{path}未设置AB包名");
                }
                else
                {
                    string content = $"{path},{importer.assetBundleName},{Path.GetFileName(path)}\r\n";
                    File.AppendAllText(filePath, content);
                }

            }
            Debug.Log("AB包映射文件生成成功");
        }
        catch (Exception e) 
        {
            Debug.LogError(e);
        }
    }

    [MenuItem("Build/生成Release lua文件")]
    private static void GenerateLuaFiles()
    {
        string sourceDir = Utils.GetProjectLuaPath();
        string targetDir = Utils.GetReleaseLuaPath();

        try
        {
            CopyDirectory(sourceDir, targetDir);
            Debug.Log("生成lua文件成功");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        //拷贝文件
        string[] files = Directory.GetFiles(sourceDir);
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileName(files[i]);
            if (fileName.EndsWith(".meta")) continue;

            string sourceFile = files[i];
            string targetFile = Path.Combine(targetDir, fileName);

            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }

            File.Copy(sourceFile, targetFile);
            Debug.Log($"{targetFile}文件已复制");
        }

        //拷贝文件夹
        string[] dirs = Directory.GetDirectories(sourceDir);
        for (int i = 0; i < dirs.Length; i++)
        {
            string sourceDir2 = dirs[i];
            string targetDir2 = Path.Combine(targetDir, Path.GetFileName(dirs[i]));
            CopyDirectory(sourceDir2, targetDir2);
        }
    }

}
