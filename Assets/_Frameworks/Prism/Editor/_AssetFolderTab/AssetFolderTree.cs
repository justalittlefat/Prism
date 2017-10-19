using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Prism
{
    public class AssetFolderTree : TreeView
    {
        AssetFolderTab _folderTab;
        public AssetFolderTree(TreeViewState s, AssetFolderTab parent) : base(s)
        {
            _folderTab = parent;
            showBorder = true;
        }

        protected override TreeViewItem BuildRoot()
        {            
            var root = new TreeViewItem(-1, -1);
            foreach (var b in AssetFolderConfig.selectedFolders)
            {
                root.AddChild(new FolderTreeItem(b, _folderTab));
            }
            return root;
        }        

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                SetSelection(new int[0], TreeViewSelectionOptions.FireSelectionChanged);
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);

            if (selectedIds.Count > 0)
            {
                var it = FindItem(selectedIds[0], rootItem) as FolderTreeItem;
                 _folderTab.SetFolderData(it.sf);            
            }
            else
            {
                _folderTab.SetFolderData(null);
            }
        }

        protected override void KeyEvent()
        {
            if (Event.current.keyCode == KeyCode.Delete)
            {
                RemoveSelectedItems();
            }
        }
        
        public void RemoveSelectedItems()
        {
            if(GetSelection().Count > 0)
            {
                foreach (var nodeID in GetSelection())
                {
                    var item = FindItem(nodeID, rootItem) as FolderTreeItem;
                    if (item == null) continue;                   
                    AssetFolderConfig.selectedFolders.Remove(item.sf);
                }
            }
            _folderTab.SetFolderData(null);           
            if (AssetFolderConfig.selectedFolders.Count > 0)
            {
                Reload();
            }
            else
            {
                Repaint();
            }
        }        
    }

    public class FolderTreeItem : TreeViewItem
    {
        public SelectedFolder sf;
        public FolderTreeItem(SelectedFolder sf, AssetFolderTab folderTab) : base(sf.path.GetHashCode(), 0, sf.path)
		{
            this.sf = sf;
        }
    }
}