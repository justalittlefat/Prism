using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Prism
{
    public partial class BundleLoader
    {
        public static Dictionary<string, BundleLoader> allLoader = new Dictionary<string, BundleLoader>();
        private string bundleFilePath;        
        public string bundleName;        
        public BundleLoader[] depLoaders;
        public Action<BundleLoader> onComplete;
        public LoadState loadState = LoadState.None;

        public static BundleLoader CreateLoader(string resPath, BundleLoader theLoaderDependsOnMe = null)
        {
            string bundleName = resPath.ToLower();
            BundleLoader loader = null;
            if (!allLoader.TryGetValue(bundleName, out loader))
            {
                loader = new BundleLoader(bundleName);
                allLoader[bundleName] = loader;
            }
            return loader;
        }

        private BundleLoader(string name)
        {
            bundleName = name;            
            bundleFilePath = Path.Combine(BundleUtility.LocalAssetBundlePath, bundleName);            
            var deps = BundleManager.instance.mainManifest.GetDirectDependencies(bundleName);
            depLoaders = new BundleLoader[deps.Length];
            for (int i = 0; i < deps.Length; i++)
            {
                depLoaders[i] = CreateLoader(deps[i],this);
            }
        }

        public GameObject Instantiate(bool enable = true)
        {
            if (mainObject == null) return null;
            if (!(mainObject is GameObject)) return null;
            GameObject instance = GameObject.Instantiate(mainObject) as GameObject;
            instance.SetActive(enable);
            instance.name = mainObject.name;
            WeakReference wr = new WeakReference(instance);
            instanceReferences.Add(wr);            
            return instance;
        }

        private void OnLoadComplete()
        {
            if (assetBundle != null)
            {
                _readyTime = Time.time;
                BundleManager.instance.completeLoaders.Add(this);
                if (onComplete != null)
                {
                    onComplete(this);
                    onComplete = null;
                }
                loadState = LoadState.Complete;
            }
        }

        public void Unload()
        {
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
            }
            mainObject = null;
            assetBundle = null;
            foreach(var loader in depLoaders)
            {
                loader.LoadersDependOnMe.Remove(this);
            }
            loadState = LoadState.None;
        }
    }
}