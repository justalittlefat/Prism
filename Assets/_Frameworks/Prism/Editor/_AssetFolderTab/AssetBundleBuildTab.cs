using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Prism
{
    public class AssetBundleBuildTab
    {
        private BuildConfig buildConfig;
        CompressOptions m_Compression;
        int[] compressionValues = { 0, 1, 2 };
        GUIContent content_compressionDesc = new GUIContent("Compression", "Choose no compress, standard (LZMA), or chunk based (LZ4)");
        GUIContent content_outputPath = new GUIContent("Output Path", "");
        GUIContent content_version = new GUIContent("Version", "");
        GUIContent content_clearFolders = new GUIContent("Clear Destination Folder", ""); 
        GUIContent content_copy2StreamingAssets = new GUIContent("Copy 2 StreamingAssets", "");        
        GUIContent[] content_compressionOptions =
        {
            new GUIContent("No Compression"),
            new GUIContent("Standard Compression (LZMA)"),
            new GUIContent("Chunk Based Compression (LZ4)")
        };
        GUIContent content_excludeTypeInfo = new GUIContent("Exclude Type Info", "");
        GUIContent content_forceRebuild = new GUIContent("Force Rebuild", "");
        GUIContent content_ingonreTypeTreeChanges = new GUIContent("Igonre Type Tree", "");
        GUIContent content_appendHash = new GUIContent("Append Hash", "");
        GUIContent content_strictMode = new GUIContent("Strict Mode", "");
        GUIContent content_dryRunBuild = new GUIContent("Dry Run Build", "");

        public void OnEnable(Rect pos, EditorWindow parent)
        {
            buildConfig = AssetFolderConfig.buildConfig;
        }
        public void OnDisable()
        {
            AssetFolderConfig.SaveData();
        }

        public void OnGUI(Rect pos)
        {
            var rect = new Rect(pos.x, pos.y, pos.width * 0.4f, pos.height - 30);
            DrawTargetPlatform(rect);
            rect = new Rect(pos.x+pos.width*0.4f + 3, pos.y, pos.width * 0.6f-3, pos.height - 30);
            DrawBuildConfig(rect);
            rect = new Rect(pos.x, pos.y + pos.height - 28, pos.width, 28);
            if(GUI.Button(rect, "Build"))
            {
                Fire();
            }            
        }

        public void Fire()
        {
            if (string.IsNullOrEmpty(buildConfig.outputPath))
            {
                Debug.Log("Output path can not be null!");
                return;
            }
            EnsureFolderExists(buildConfig.outputPath);
            BuildAssetBundleOptions opt = BuildAssetBundleOptions.None;
            if (buildConfig.forceRebuild)
                opt |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            if (buildConfig.igonreTypeTreeChanges)
                opt |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            if (buildConfig.appendHash)
                opt |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
            if (buildConfig.strictMode)
                opt |= BuildAssetBundleOptions.StrictMode;
            if (buildConfig.dryRunBuild)
                opt |= BuildAssetBundleOptions.DryRunBuild;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            foreach (var target in buildConfig.validBuildTargets)
            {
                if (!target.Value) continue;

                var platformFolder = Path.Combine(buildConfig.outputPath, target.Key.ToString()).Replace("\\", "/");
                if(Directory.Exists(platformFolder) && buildConfig.clearOutputFolder)
                {
                    Directory.Delete(platformFolder, true);
                }
                EnsureFolderExists(platformFolder);
                var bundlePath = platformFolder + "/Bundle";
                EnsureFolderExists(bundlePath);
                Debug.Log(target + "start:" + sw.ElapsedMilliseconds);
                BuildPipeline.BuildAssetBundles(bundlePath, opt, target.Key);                
                Debug.Log(target + "end:" + sw.ElapsedMilliseconds);
                if (buildConfig.validBuildTargets.Count > 1)
                {
                    //同时打包多平台时的缓冲时间
                    Thread.Sleep(1000);
                }
            }
            sw.Stop();
        }

        private void EnsureFolderExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        private float space = 4f; 
        private void DrawTargetPlatform(Rect r)
        {
            RectUtils.DrawOutline(r, 1);
            GUILayout.BeginArea(new Rect(r.x+15,r.y+8,r.width-30,r.height-16));
            var targets = buildConfig.validBuildTargets; 
            var keys = new BuildTarget[targets.Count];
            int index = -1;
            foreach(var key in targets.Keys)
            {
                index++;
                keys[index] = key;
            }
            for(int i = 0; i < keys.Length; i++)
            {
                targets[keys[i]] = GUILayout.Toggle(targets[keys[i]], keys[i].ToString());
                GUILayout.Space(space);
            }
            GUILayout.EndArea();
        }

        GUIContent content_chooseOutputPath = new GUIContent("Choose Output Path", "");
        private void DrawBuildConfig(Rect r)
        {
            RectUtils.DrawOutline(r, 1);
            GUILayout.BeginArea(new Rect(r.x + 15, r.y + 8, r.width - 30, r.height - 16));
            if (GUILayout.Button(content_chooseOutputPath))
            {
                string dataPath = Application.dataPath;
                string selectedPath = EditorUtility.OpenFolderPanel("path", dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    buildConfig.outputPath = selectedPath;
                }
            }
            GUI.enabled = false;
            buildConfig.outputPath = EditorGUILayout.TextField(content_outputPath, buildConfig.outputPath);
            GUI.enabled = true;
            GUILayout.Space(space);
            buildConfig.version = EditorGUILayout.TextField(content_version, buildConfig.version);
            GUILayout.Space(space);
            buildConfig.clearOutputFolder = EditorGUILayout.Toggle(content_clearFolders, buildConfig.clearOutputFolder);
            GUILayout.Space(space);
            buildConfig.copy2SteamingAssets = EditorGUILayout.Toggle(content_copy2StreamingAssets, buildConfig.copy2SteamingAssets);
            GUILayout.Space(space);
            GUILayout.Space(space);

            m_Compression = (CompressOptions)EditorGUILayout.IntPopup(
                        content_compressionDesc,
                        (int)m_Compression,
                        content_compressionOptions,
                        compressionValues);
            GUILayout.Space(space);

            buildConfig.excludeTypeInfo = EditorGUILayout.Toggle(content_excludeTypeInfo, buildConfig.excludeTypeInfo);
            GUILayout.Space(space);
            buildConfig.forceRebuild = EditorGUILayout.Toggle(content_forceRebuild, buildConfig.forceRebuild);
            GUILayout.Space(space);
            buildConfig.igonreTypeTreeChanges = EditorGUILayout.Toggle(content_ingonreTypeTreeChanges, buildConfig.igonreTypeTreeChanges);
            GUILayout.Space(space);
            buildConfig.appendHash = EditorGUILayout.Toggle(content_appendHash, buildConfig.appendHash);
            GUILayout.Space(space);
            buildConfig.strictMode = EditorGUILayout.Toggle(content_strictMode, buildConfig.strictMode);
            GUILayout.Space(space);
            buildConfig.dryRunBuild = EditorGUILayout.Toggle(content_dryRunBuild, buildConfig.dryRunBuild);
            GUILayout.Space(space);

            GUILayout.EndArea();
        }      
    }

    public enum CompressOptions
    {
        Uncompressed = 0,
        StandardCompression = 1,
        ChunkBasedCompression = 2,
    }
}
