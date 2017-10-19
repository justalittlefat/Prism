using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Prism
{
    public partial class BundleLoader
    {        
        public void Load()
        {            
            LoadDepends();
            LoadSelf();
        }

        private void LoadSelf()
        {                        
            if (assetBundle) return;
            assetBundle = AssetBundle.LoadFromFile(bundleFilePath);
            mainObject = assetBundle.LoadAllAssets()[0] as GameObject;
            OnLoadComplete();
        }

        private void LoadDepends()
        {
            for (int i = 0; i < depLoaders.Length; i++)
            {
                depLoaders[i].Load();
                depLoaders[i].LoadersDependOnMe.Add(this);
            }
        }
    }
}