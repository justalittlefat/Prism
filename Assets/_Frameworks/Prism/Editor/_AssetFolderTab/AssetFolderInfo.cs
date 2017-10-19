using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Prism
{
    public class AssetFolderInfo
    {
        SerializedProperty _validExtensions;
        SerializedProperty _excludeFolders;
        SerializedProperty _excludeKeywords;
        SerializedObject _agent;
        SelectedFolder _selectedFolder;
        StringArray sa;
        float uiSpace = 2f;
        Vector2 scrollPos = Vector2.zero;

        public void SetData(SelectedFolder sf)
        {
            _selectedFolder = sf;
            if (_selectedFolder == null)
            {
                scrollPos = Vector2.zero;
                return;
            }
            sa = ScriptableObject.CreateInstance<StringArray>();
            sa.validExtensions = _selectedFolder.validExtensions;
            sa.excludeFolders = _selectedFolder.excludeFolders;
            sa.excludeKeywords = _selectedFolder.excludeKeywords;
            _agent = new SerializedObject(sa);

            _validExtensions = _agent.FindProperty("validExtensions");
            _excludeFolders = _agent.FindProperty("excludeFolders");
            _excludeKeywords = _agent.FindProperty("excludeKeywords");
        }

        public void OnGUI(Rect rect)
        {
            if (_selectedFolder == null) return;
            _agent.Update();
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginArea(rect);
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.Button(new GUIContent("Rechoose Folder", ""));
            
            GUILayout.Space(uiSpace);
            GUI.enabled = false;
            EditorGUILayout.TextField(new GUIContent("Folder Path", ""), _selectedFolder.path);
            GUI.enabled = true;

            GUILayout.Space(uiSpace);
            _selectedFolder.Description = EditorGUILayout.TextField(new GUIContent("Description", ""), _selectedFolder.Description);

            GUILayout.Space(uiSpace);
            _selectedFolder.enable = EditorGUILayout.Toggle(new GUIContent("Enable", ""), _selectedFolder.enable);

            GUILayout.Space(uiSpace);
            _selectedFolder.coverOldBunld = EditorGUILayout.Toggle(new GUIContent("Cover Old Bunld", ""), _selectedFolder.coverOldBunld);

            GUILayout.Space(uiSpace*3);
            EditorGUILayout.PropertyField(_validExtensions, true);

            GUILayout.Space(uiSpace);
            EditorGUILayout.PropertyField(_excludeFolders, true);

            GUILayout.Space(uiSpace);
            EditorGUILayout.PropertyField(_excludeKeywords, true);
            
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            if (EditorGUI.EndChangeCheck())
            {
                _agent.ApplyModifiedProperties();
            }
        }
    }

    public class StringArray : ScriptableObject
    {
        public List<string> validExtensions;
        public List<string> excludeFolders;
        public List<string> excludeKeywords;
    }
}
