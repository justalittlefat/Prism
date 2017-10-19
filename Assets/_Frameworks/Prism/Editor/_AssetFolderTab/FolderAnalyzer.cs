using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Prism
{
    public class FolderAnalyzer
    {
        public static void Fire()
        {
            EditorUtility.DisplayProgressBar("Analys Folder Assets", "", 1);
            var conifgs = AssetFolderConfig.selectedFolders;
            if (conifgs.Count == 0)
            {
                Debug.Log("当前不存在bundle folder配置信息");
                EditorUtility.ClearProgressBar();
                return;
            }
            var fileList = new List<string>();
            foreach (var folderInfo in conifgs)
            {
                if (!folderInfo.enable) continue;
                CollectValidFiles(folderInfo,fileList);
            }
            for(int i = 0; i < fileList.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Analys Folder Assets", fileList[i], (float)i/fileList.Count);
                SetBundleName(fileList[i]);
            }
            EditorUtility.ClearProgressBar();
        }

        public static void SetBundleName(string path)
        {
            AssetImporter ai = AssetImporter.GetAtPath(path);
            string bundleName = path.Replace("Assets/", "").Replace(Path.GetExtension(path), "");
            ai.assetBundleName = bundleName;
            HashSet<string> depsList = GetDependencies(path);
            foreach (var dep in depsList)
            {
                SetBundleName(dep);
            }
        }

        static HashSet<string> GetDependencies(string path)
        {
            Object asset = AssetDatabase.LoadMainAssetAtPath(path);
            Object[] deps = EditorUtility.CollectDependencies(new Object[] { asset });
            var pathList = new HashSet<string>();
            for (int i = 0; i < deps.Length; i++)
            {
                Object o = deps[i];
                if (o is MonoScript || o is LightingDataAsset) continue;
                var p = AssetDatabase.GetAssetPath(o);
                if (File.Exists(p) && p != path) pathList.Add(p);
            }
            return pathList;
        }

        public static void CollectValidFiles(SelectedFolder sf, List<string> fileList)
        {            
            var targetFiles = Directory.GetFiles(sf.fullPath, "*.*",SearchOption.AllDirectories);
            for (int i = 0; i < targetFiles.Length; i++)
            {                
                var ex = Path.GetExtension(targetFiles[i]);
                if (CheckExtension(ex, sf.validExtensions))
                {
                    fileList.Add(targetFiles[i].Replace(Application.dataPath, "Assets").Replace("\\", "/"));
                }                
            }
        }

        public static bool CheckExtension(string extension,List<string> validExtensions)
        {
            if (extension == ".meta") return false;
            if (validExtensions == null || validExtensions.Count == 0) return true;
            foreach (var ve in validExtensions)
            {
                string filter = ve.StartsWith(".") ? ve : "." + ve;
                if (extension == filter) return true;
            }
            return false;
        }

        static string buildPath = "_BundleFile";
        public static void BuildBundle()
        {
            string _outputPath = Application.dataPath.Replace("Assets", "");
            _outputPath = Path.Combine(_outputPath, buildPath);
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
            BuildPipeline.BuildAssetBundles(_outputPath, 0, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}