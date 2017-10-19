using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Prism
{
    public enum LoadState
    {
        None = 0,
        Loading = 1,
        Error = 2,
        Complete = 3
    }

    public partial class BundleManager : MonoBehaviour
    {
        public static BundleManager instance;
        public static bool AB_MODE = true;
        public static bool EditorMode = false;
        private const int MAX_REQUEST = 100;
        public AssetBundleManifest mainManifest;                
        public HashSet<BundleLoader> completeLoaders = new HashSet<BundleLoader>();
        public HashSet<BundleLoader> runningLoaders = new HashSet<BundleLoader>();
        //public List<BundleLoader> waittingLoaders = new List<BundleLoader>();
        private bool enableLog = true;
        /*private bool isLoading
        {
            get
            {
                return runningLoaders.Count > 0 || waittingLoaders.Count > 0;
            }
        }*/
        
        protected void Awake()
        {
            instance = this;
            InvokeRepeating("CheckUnusedBundle", 0, 3);
        }

        private Action _initCallback;
        public void Init(Action callback = null)
        {
            _initCallback = callback;
            if (AB_MODE)
            {
                StartCoroutine(LoadMainManifest());
            }
            else
            {
                InitComplete();
            }
        }
        void InitComplete()
        {
            if (_initCallback != null)
            {
                _initCallback();
            }
            _initCallback = null;
        }
        void CheckUnusedBundle()
        {
            //Debug.Log("check");
            UnloadUnusedBundle();
        }
        private IEnumerator LoadMainManifest()
        {
            yield return null;
            string manifest = Path.Combine(BundleUtility.LocalAssetBundlePath, BundleUtility.MainManifest);
            Debug.Log(manifest);
            Debug.Log("Prism-读取配置文件：" + File.Exists(manifest));
            if (File.Exists(manifest))
            {
                AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(manifest);
                yield return bundleRequest;
                AssetBundleRequest assetRequest = bundleRequest.assetBundle.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                yield return assetRequest;
                mainManifest = assetRequest.asset as AssetBundleManifest;
                bundleRequest.assetBundle.Unload(false);
            }
            InitComplete();
        }        

        private HashSet<BundleLoader> tempLoaders = new HashSet<BundleLoader>();
        public void UnloadUnusedBundle(bool forceUnload = false)
        {
            //if (isLoading && !forceUnload) return;
            tempLoaders.Clear();
            foreach(var loader in completeLoaders)
            {
                tempLoaders.Add(loader);
            }
            int unloadLimit = 20;//单帧卸载上限
            int unloadCount = 0;
            foreach (var loader in tempLoaders)
            {
                //if (isLoading && !forceUnload) break;
                if (unloadCount > unloadLimit) break;
                if (loader.isUsed) continue;
                loader.Unload();
                completeLoaders.Remove(loader);
                unloadCount++;
            }
#if UNITY_EDITOR            
            if (unloadCount > 0 && enableLog)
            {
                Debug.Log("当前内存资源数：" + completeLoaders.Count);
                //Debug.Log(Time.time + "===>> Unload Count: " + unloadCount);
            }
#endif
        }
        
        public GameObject LoadPrefab(string path, bool enable = true)
        {
            if (!AB_MODE && EditorMode)
            {
                path = path + ".prefab";
            }
            return LoadAsset<GameObject>(path, enable);
        }

        public T LoadAsset<T>(string path, bool enable = true) where T : UnityEngine.Object
        {
            BundleLoader loader = BundleLoader.CreateLoader(path);
            if(loader.loadState== LoadState.Loading)
            {                
                Debug.Log("can not load asset while it is async loading");
                Debug.Log("asset name:" + path);
                return null;                
            }
            loader.Load();
            if (loader.mainObject is T)
            {
                if (loader.mainObject is GameObject)
                {
                    return loader.Instantiate(enable) as T;
                }
                else
                {
                    return loader.mainObject as T;
                }
            }
            else
            {
                return null;
            }
        }
    }
}