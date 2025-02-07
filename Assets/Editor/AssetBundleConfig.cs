using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using FengSheng;
using System.Collections.Generic;

public class AssetBundleConfig : EditorWindow
{
    [MenuItem("Build/���/��Դ")]
    private static void ShowWindow()
    {
        GetWindow<AssetBundleConfig>();
    }

    private string abName = string.Empty;
    private string endName = string.Empty;
    private string folderPath = string.Empty;
    private void OnGUI()
    {
        GUILayout.Label("AssetBundle Name", EditorStyles.boldLabel);
        abName = EditorGUILayout.TextField("����", abName);

        GUILayout.Label("End Name", EditorStyles.boldLabel);
        endName = EditorGUILayout.TextField("��׺��", endName);

        GUILayout.Label("Folder Path", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("·��", folderPath);

        if (GUILayout.Button("���ð���������Դ"))
        {
            string outputPath = Utils.GetReleasePath();

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            string[] guids = AssetDatabase.FindAssets("t:Texture2D t:Prefab t:TextAsset", new string[] { folderPath });
            string[] assetPaths = new string[guids.Length];

            string fullName = string.IsNullOrEmpty(endName) ? abName : abName + "." + endName;
            for (int i = 0; i < guids.Length; i++)
            {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                AssetImporter importer = AssetImporter.GetAtPath(assetPaths[i]);
                if (importer != null)
                {
                    Debug.Log(assetPaths[i]);
                    importer.assetBundleName = fullName;
                }
            }

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = fullName;
            buildMap[0].assetNames = assetPaths;

            BuildPipeline.BuildAssetBundles(outputPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

            AssetDatabase.Refresh();
            Debug.Log(@$"Build Windows64 AssetBundles Completed
<color=green>folderPath</color>:{folderPath}
<color=green>abName</color>:{fullName}
<color=green>outputPath</color>:{outputPath}");
        }

    }

    [MenuItem("Build/���/������а���")]
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
            Debug.Log("������а������");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    [MenuItem("Build/���/�����ļ�")]
    private static void GenerateConfigFiles()
    {
        GenerateAssetBundleManifest();
        GenerateHotfixFilesList();
    }

    /// <summary>
    /// ����AB��ӳ���ļ�
    /// </summary>
    [MenuItem("Build/����/����AB��ӳ���ļ�(ui,prefab,protos)")]
    private static void GenerateAssetBundleManifest()
    {
        try
        {
            string filePath = Utils.GetResourcesManifestPath();
            string outputPath = Utils.GetReleasePath();
            string[] guids = AssetDatabase.FindAssets("t:Texture2D t:Prefab t:TextAsset", new string[] { "Assets/Resources/hotfix" });

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            if (!File.Exists(filePath))
            {
                var file = File.Create(filePath);
                file.Dispose();
            }

            File.WriteAllText(filePath, string.Empty);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    if (path.Contains("lua")) continue;

                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    if (string.IsNullOrEmpty(importer.assetBundleName))
                    {
                        Debug.LogError($"{path}δ����AB����");
                    }
                    else
                    {
                        string content = $"{path},{importer.assetBundleName},{Path.GetFileName(path)}";
                        writer.WriteLine(content);
                    }

                }
            }
            Debug.Log("AB��ӳ���ļ����ɳɹ�");
        }
        catch (Exception e) 
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// �����ȸ��ļ��б��ļ�
    /// </summary>
    [MenuItem("Build/����/�����ȸ��ļ��б�")]
    private static void GenerateHotfixFilesList()
    {
        try
        {
            string filePath = Utils.GetHotfixFilesListPath();
            string dir = Utils.GetReleasePath();
            string head = string.Empty;

            if (!File.Exists(filePath))
            {
                var file = File.Create(filePath);
                file.Dispose();
            }

            File.WriteAllText(filePath, string.Empty);
            List<string> writes = new List<string>();
            WriteHotfixFilesList(writes, dir, head);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < writes.Count; i++)
                {
                    writer.WriteLine(writes[i]);
                }
            }
            Debug.Log("�ȸ��ļ��б��ļ����ɳɹ�");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private static void WriteHotfixFilesList(List<string> writes, string dir, string head)
    {
        string[] files = Directory.GetFiles(dir);
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo fileInfo = new FileInfo(files[i]);
            string write = $"{Path.Combine(head, Path.GetFileName(files[i]))},{fileInfo.Length}";
            writes.Add(write);
        }

        string[] dirs = Directory.GetDirectories(dir);
        for (int i = 0; i < dirs.Length; i++)
        {
            WriteHotfixFilesList(writes, dirs[i], Path.Combine(head, Path.GetFileName(dirs[i])));
        }
    }

    [MenuItem("Build/���/Lua����")]
    private static void GenerateLuaFiles()
    {
        string sourceDir = Utils.GetProjectLuaPath();
        string targetDir = Utils.GetReleaseLuaPath();

        try
        {
            CopyDirectory(sourceDir, targetDir);
            Debug.Log("����lua�ļ��ɹ�");
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

        //�����ļ�
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
            Debug.Log($"{targetFile}�ļ��Ѹ���");
        }

        //�����ļ���
        string[] dirs = Directory.GetDirectories(sourceDir);
        for (int i = 0; i < dirs.Length; i++)
        {
            string sourceDir2 = dirs[i];
            string targetDir2 = Path.Combine(targetDir, Path.GetFileName(dirs[i]));
            CopyDirectory(sourceDir2, targetDir2);
        }
    }

}
