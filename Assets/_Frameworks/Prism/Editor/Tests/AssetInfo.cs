using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetInfo : Editor {

    /*[MenuItem("Prism/GetInfo", false, 101)]
    public static void GetInfo()
    {
        string path = "Assets/TestPrefab/Group.prefab";
        var list = getDeps(path);
        foreach(var l in list)
        {
            Debug.Log(l);
        }
    }*/
   

    static List<string> getDeps(string path)
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
        return pathList.ToList<string>();
    }




    static List<string> getDeps2(string path)
    {
        Object asset = AssetDatabase.LoadMainAssetAtPath(path);
        Object[] deps = EditorUtility.CollectDependencies(new Object[] { asset });

        var depList = new List<Object>();
        for (int i = 0; i < deps.Length; i++)
        {
            Object o = deps[i];
            if (o is MonoScript || o is LightingDataAsset)
                continue;
            depList.Add(o);
        }

        var res = from s in deps
                  let obj = AssetDatabase.GetAssetPath(s)
                  select obj;

        var paths = res.Distinct().ToArray();

        var realList = new List<string>();
        for (int i = 0; i < paths.Length; i++)
        {

            if (File.Exists(paths[i]) == false)
            {
                //Debug.Log("invalid:" + paths[i]);
                continue;
            }
            realList.Add(paths[i]);
        }
        return realList;
    }
}
