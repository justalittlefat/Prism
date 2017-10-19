using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Security.Cryptography; 

namespace Prism
{
    public class BundleUtility
    {
        public static string LocalAssetBundlePath = Application.persistentDataPath + "/Bundle";
        public static string MainManifest = "Bundle";

        public static string EditorResourcesPath = "Assets/ResourcesTest";
        //public static string ResourcesFolderName = "Resources";
        public static string VersionFileName = "version";
        public static string versions = "versions.txt";
        public static string ZipFileName = "Resources.zip";
        public static string PatchListFileName = "PatchesList.txt";
        public static string SecretKey = "12345678";
        public static string abExtension = ".ab";

        

        public static bool ResolveDecryptedVersionData(byte[] bytes, out string versionID, out string error)
        {
            try
            {
                string text = Encoding.UTF8.GetString(bytes);
                string[] items = text.Split('\n');
                versionID = items[0];
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = string.Format("Failed resolving version data: {0}", ex.ToString());
                versionID = "0.0.0.0";
                return false;
            }
        }

        /// <summary>
        /// 解析版本号列表
        /// </summary>
        /// <param name="bytes">文件流</param>
        /// <param name="versionInfos">版本号列表</param>
        /// <param name="error">错误</param>
        /// <returns>是否成功</returns>
        public static bool ResolveVersionInfos(string text, ref List<string> versionInfos, out string error)
        {
            try
            {
                if (versionInfos == null)
                {
                    versionInfos = new List<string>();
                }
                else
                {
                    versionInfos.Clear();
                }

                string[] items = text.Split('\n');
                var e = items.GetEnumerator();
                while (e.MoveNext())
                {
                    if (!string.IsNullOrEmpty(e.Current.ToString()) && !versionInfos.Contains(e.Current.ToString()))
                    {
                        versionInfos.Add(e.Current.ToString().Trim());
                    }
                }
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = string.Format("Failed resolving version data: {0}", ex.ToString());
                return false;
            }
        }
        

#if UNITY_EDITOR
        /// <summary>
        /// 获取打包平台名
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                case BuildTarget.WebGL:
                    return "WebGL";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }
#endif

        /// <summary>
        /// runtime获取AB所需平台名
        /// </summary>
        /// <param name="platform">runtime平台</param>
        /// <returns>平台名</returns>
        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }

        #region Android Java获取persistentDataPath以解决Unity获取路径为空的问题

        private static string[] _persistentDataPaths;

        public static bool IsDirectoryWritable(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return false;
                string file = Path.Combine(path, Path.GetRandomFileName());
                using (FileStream fs = File.Create(file, 1)) { }
                File.Delete(file);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetPersistentDataPath(params string[] components)
        {
            try
            {
                string path = Path.DirectorySeparatorChar + string.Join("" + Path.DirectorySeparatorChar, components);
                if (!Directory.GetParent(path).Exists) return null;
                if (!Directory.Exists(path))
                {
                    Debug.Log("creating directory: " + path);
                    Directory.CreateDirectory(path);
                }
                if (!IsDirectoryWritable(path))
                {
                    Debug.LogWarning("persistent data path not writable: " + path);
                    return null;
                }
                return path;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        public static string persistentDataPathInternal
        {
#if UNITY_ANDROID
            get
            {
                if (Application.isEditor || !Application.isPlaying) return Application.persistentDataPath;
                string path = null;
                if (string.IsNullOrEmpty(path)) path = GetPersistentDataPath("storage", "emulated", "0", "Android", "data", Application.identifier, "files");
                if (string.IsNullOrEmpty(path)) path = GetPersistentDataPath("data", "data", Application.identifier, "files");
                return path;
            }
#else
    get { return Application.persistentDataPath; }
#endif
        }

        public static string persistentDataPathExternal
        {
#if UNITY_ANDROID
            get
            {
                if (Application.isEditor || !Application.isPlaying) return null;
                string path = null;
                if (string.IsNullOrEmpty(path)) path = GetPersistentDataPath("storage", "sdcard0", "Android", "data", Application.identifier, "files");
                if (string.IsNullOrEmpty(path)) path = GetPersistentDataPath("storage", "sdcard1", "Android", "data", Application.identifier, "files");
                if (string.IsNullOrEmpty(path)) path = GetPersistentDataPath("mnt", "sdcard", "Android", "data", Application.identifier, "files");
                return path;
            }
#else
    get { return null; }
#endif
        }

        public static string[] persistentDataPaths
        {
            get
            {
                if (_persistentDataPaths == null)
                {
                    List<string> paths = new List<string>();
                    if (!string.IsNullOrEmpty(persistentDataPathInternal)) paths.Add(persistentDataPathInternal);
                    if (!string.IsNullOrEmpty(persistentDataPathExternal)) paths.Add(persistentDataPathExternal);
                    if (!string.IsNullOrEmpty(Application.persistentDataPath) && !paths.Contains(Application.persistentDataPath)) paths.Add(Application.persistentDataPath);
                    _persistentDataPaths = paths.ToArray();
                }
                return _persistentDataPaths;
            }
        }

        public static string GetPersistentFile(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return null;
            foreach (string path in persistentDataPaths)
            {
                if (FileExists(path, relativePath)) return Path.Combine(path, relativePath);
            }
            return null;
        }

        public static bool SaveData(string relativePath, byte[] data)
        {
            string path = GetPersistentFile(relativePath);
            if (string.IsNullOrEmpty(path))
            {
                return SaveData(relativePath, data, 0);
            }
            else
            {
                try
                {
                    File.WriteAllBytes(path, data);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("couldn't save data to: " + path);
                    Debug.LogException(ex);
                    // try to delete file again
                    if (File.Exists(path)) File.Delete(path);
                    return SaveData(relativePath, data, 0);
                }
            }
        }

        private static bool SaveData(string relativePath, byte[] data, int pathIndex)
        {
            if (pathIndex < persistentDataPaths.Length)
            {
                string path = Path.Combine(persistentDataPaths[pathIndex], relativePath);
                try
                {
                    string dir = Path.GetDirectoryName(path);
                    if (!Directory.Exists(dir))
                    {
                        Debug.Log("creating directory: " + dir);
                        Directory.CreateDirectory(dir);
                    }
                    File.WriteAllBytes(path, data);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("couldn't save data to: " + path);
                    Debug.LogException(ex);
                    if (File.Exists(path)) File.Delete(path);       // try to delete file again
                    return SaveData(relativePath, data, pathIndex + 1); // try next persistent path
                }
            }
            else
            {
                Debug.LogWarning("couldn't save data to any persistent data path");
                return false;
            }
        }

        public static bool FileExists(string path, string relativePath)
        {
            return Directory.Exists(path) && File.Exists(Path.Combine(path, relativePath));
        }
        #endregion
    }
}