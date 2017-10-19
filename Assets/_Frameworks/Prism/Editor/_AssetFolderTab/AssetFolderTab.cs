using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace Prism
{
    public class AssetFolderTab
    {        
        /*[SerializeField]
        private Vector2 m_ScrollPosition;*/
        [SerializeField]
        private TreeViewState _bundleTreeState;

        private AssetFolderTree _folderTreeView;
        private EditorWindow _parent;
        private Rect _panelRange;

        public void OnEnable(Rect pos, EditorWindow parent)
        {
            _parent = parent;
            _afi = new AssetFolderInfo();
            _panelRange = pos;
            if (_bundleTreeState == null)
                _bundleTreeState = new TreeViewState();

            _folderTreeView = new AssetFolderTree(_bundleTreeState, this);
            if (AssetFolderConfig.selectedFolders.Count > 0)
            {
                _folderTreeView.Reload();
            }
        }
        
        public void OnDisable()
        {
            //ClearData  
            {
                _afi.SetData(null);
            }
            AssetFolderConfig.SaveData();
        }

        AssetFolderInfo _afi;
        public void SetFolderData(SelectedFolder sf)
        {
            _afi.SetData(sf);
        }
        
        public void OnGUI(Rect pos)
        {
            _panelRange = pos;
            if (AssetFolderConfig.selectedFolders.Count > 0)
            {
                _folderTreeView.OnGUI(new Rect(_panelRange.x, _panelRange.y, _panelRange.width * 0.45f, _panelRange.height - 30));
            }

            var r = new Rect(_panelRange.x,_panelRange.y + _panelRange.height - 30,_panelRange.width *0.225f,30);
            if (GUI.Button(r, "-"))
            {
                _folderTreeView.RemoveSelectedItems();
                //FolderAnalyzer.BuildBundle();
            }            

            r = new Rect(_panelRange.x + _panelRange.width *0.225f ,_panelRange.y + _panelRange.height - 30,_panelRange.width*0.225f,30);
            if (GUI.Button(r, "+"))
            {
                string dataPath = Application.dataPath;
                string selectedPath = EditorUtility.OpenFolderPanel("path", dataPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    if (selectedPath.StartsWith(dataPath))
                    {
                        var folder = new SelectedFolder();
                        folder.path = selectedPath.Replace(dataPath, "");
                        AssetFolderConfig.selectedFolders.Add(folder);
                        _folderTreeView.Reload();
                    }
                    else
                    {
                        _parent.ShowNotification(new GUIContent("无法将工程之外的目录作为资源目录！"));
                    }
                }
            }

            r = new Rect(_panelRange.x + _panelRange.width * 0.45f, _panelRange.y + _panelRange.height - 30, _panelRange.width * 0.55f, 30f);

            if(GUI.Button(r, "Run Analys"))
            {
                FolderAnalyzer.Fire();
            }
            _afi.OnGUI(new Rect(_panelRange.width * 0.45f + 5f, _panelRange.y, _panelRange.width* 0.55f - 10f, _panelRange.height - 30f));
        }
    }
}