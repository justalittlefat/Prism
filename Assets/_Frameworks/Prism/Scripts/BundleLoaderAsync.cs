using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Prism
{
    public partial class BundleLoader
    {
        public void LoadAsync()
        {
            LoadDependsAsync();
            if (loadState != LoadState.None) return;
            loadState = LoadState.Loading;
            BundleManager.instance.StartCoroutine(LoadSelfAsync());
        }

        private bool IsAllDependsReady()
        {
            foreach (var loader in depLoaders)
            {
                if (loader.loadState != LoadState.Complete) return false;
            }
            return true;
        }

        private void LoadDependsAsync()
        {
            for (int i = 0; i < depLoaders.Length; i++)
            {                
                depLoaders[i].LoadersDependOnMe.Add(this);
                depLoaders[i].LoadAsync();
            }
        }
        
        private IEnumerator LoadSelfAsync()
        {
            yield return new WaitUntil(IsAllDependsReady);
            //Debug.Log(bundleFilePath);
            //yield return new WaitForSeconds(Random.value);
            var bundleRequest = AssetBundle.LoadFromFileAsync(bundleFilePath);
            yield return bundleRequest;
            assetBundle = bundleRequest.assetBundle;
            var assetRequest = assetBundle.LoadAllAssetsAsync();
            yield return assetRequest;
            mainObject = assetRequest.allAssets[0];            
            OnLoadComplete();
        }
    }
}