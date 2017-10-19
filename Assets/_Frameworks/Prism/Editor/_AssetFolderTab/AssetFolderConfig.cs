using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Prism
{
    public class AssetFolderConfig
    {       
        private static string configSavePath
        {
            get
            {
                var _dataPath = System.IO.Path.GetFullPath(".");
                _dataPath = _dataPath.Replace("\\", "/");
                _dataPath += "/Library/AssetBundleFolderConfig.dat";
                return _dataPath;
            }
        }
        private static ConfigData _data;
        public static List<SelectedFolder> selectedFolders
        {
            get
            {
                CheckData();
                return _data.selectedFolders;
            }
        }

        public static BuildConfig buildConfig
        {
            get
            {
                CheckData();
                return _data.buildConfig;
            }
        }

        private static void CheckData()
        {
            if (_data == null)
            {
                if (File.Exists(configSavePath))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(configSavePath, FileMode.Open);
                    _data = bf.Deserialize(file) as ConfigData;
                    file.Close();
                }
                if (_data == null)
                {
                    _data = new ConfigData();
                }
            }
        }

        public static void SaveData()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(configSavePath);
            bf.Serialize(file, _data);
            file.Close();
        }
    }

    [System.Serializable]
    public class ConfigData
    {
        public List<SelectedFolder> selectedFolders = new List<SelectedFolder>();
        public BuildConfig buildConfig = new BuildConfig();
    }

    [System.Serializable]
    public class SelectedFolder
    {
        public bool enable = true;
        public bool coverOldBunld = false;
        public string path = string.Empty;
        public string Description = string.Empty;
        public List<string> validExtensions = new List<string>(){"prefab"};
        public List<string> excludeFolders = new List<string>();
        public List<string> excludeKeywords = new List<string>();
        public string fullPath
        {
            get
            {
                return Application.dataPath + path;
            }
        }
    }

    [System.Serializable]
    public class BuildConfig
    {
        public Dictionary<BuildTarget, bool> validBuildTargets = new Dictionary<BuildTarget, bool>() {
            {BuildTarget.Android , false},
            {BuildTarget.iOS , false},
            {BuildTarget.StandaloneWindows64 , false},
            {BuildTarget.StandaloneOSXUniversal , false},
            {BuildTarget.WebGL , false},
            {BuildTarget.XboxOne , false},
            {BuildTarget.Switch , false},
            {BuildTarget.N3DS , false}
        };
        public string outputPath = "";
        public string version = "0.0.0";
        public bool clearOutputFolder = false;
        public bool copy2SteamingAssets = false;
        public bool excludeTypeInfo = false;
        public bool forceRebuild = false;
        public bool igonreTypeTreeChanges = false;
        public bool appendHash = false;
        public bool strictMode = false;
        public bool dryRunBuild = false;
    }
}