using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Prism
{
    public partial class BundleManager
    {
        /*void Update()
        {
            if (true)
            {
                //CheckNewLoaders();
                CheckQueue();
            }
        }

        void CheckQueue()
        {
            while (waittingLoaders.Count > 0 && runningLoaders.Count < MAX_REQUEST)
            {
                LoadBundleAsync(waittingLoaders[0]);
                runningLoaders.Add(waittingLoaders[0]);
                waittingLoaders.RemoveAt(0);
            }
        }*/

        void LoadBundleAsync(BundleLoader loader)
        {
            if (!loader.mainObject)
            {
                loader.LoadAsync();
            }
        }

        public void LoadAsync(string path, Action<BundleLoader> onLoadAssetComplete = null)
        {
            BundleLoader loader = BundleLoader.CreateLoader(path);
            if (loader.mainObject)
            {
                onLoadAssetComplete(loader);
            }
            else
            {
                loader.onComplete += onLoadAssetComplete;
            }
            loader.LoadAsync();
        }

        public void LoadPrefabAsync(string path, Action<GameObject> onLoadPrefabComplete)
        {
            if (!AB_MODE && EditorMode)
            {
                path = path + ".prefab";
            }
            LoadAssetAsync<GameObject>(path, onLoadPrefabComplete);
        }

        public void LoadAssetAsync<T>(string path, Action<T> onLoadComplete) where T : UnityEngine.Object
        {
            LoadAsync(path, (loader) =>
            {
                if (loader.mainObject is T)
                {
                    if (loader.mainObject is GameObject)
                    {
                        onLoadComplete(loader.Instantiate() as T);
                    }
                    else
                    {
                        onLoadComplete(loader.mainObject as T);
                    }
                }
            });
        }
    }
}

