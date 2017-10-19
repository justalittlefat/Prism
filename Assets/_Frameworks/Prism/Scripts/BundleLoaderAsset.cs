using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Prism
{
    public partial class BundleLoader
    {    
        public AssetBundle assetBundle;
        public Object mainObject;
        private float _readyTime;
        private float minLifeTime = 5;
        public bool isUsed
        {
            get { return LoadersDependOnMe.Count > 0 || instanceReferencesCount > 0 || Time.time - _readyTime < minLifeTime; }
        }
        private HashSet<BundleLoader> LoadersDependOnMe = new HashSet<BundleLoader>();
        private List<WeakReference> instanceReferences = new List<WeakReference>();
        private int instanceReferencesCount
        {
            get
            {
                for (int i = 0; i < instanceReferences.Count; i++)
                {
                    Object o = (Object)instanceReferences[i].Target;
                    if (!o)
                    {
                        instanceReferences.RemoveAt(i);
                        i--;
                    }
                }
                return instanceReferences.Count;
            }
        }
    }
}