using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UExtension
{
    [CustomEditor(typeof(DefaultAsset))]
    public class UserDataInspector : Editor
    {
        void OnEnable() 
        {
        }
        void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            mUserDataInspector.OnGUI(this);
        }


        protected UserDataInspectorGUI mUserDataInspector = new UserDataInspectorGUI();
    }


    public class UserDataInspectorGUI
    {
        public void OnGUI(Editor rEditor)
        {
            var rAssetPaths = new string[rEditor.targets.Length];
            for (int nIndex = 0; nIndex < rEditor.targets.Length; ++ nIndex)
            {
                var rAsset = rEditor.targets[nIndex];
                if (!rAsset)
                    continue;

                var rAssetPath = AssetDatabase.GetAssetPath(rAsset);
                if (string.IsNullOrEmpty(rAssetPath))
                    return; // 所有对象是资源时才有效

                rAssetPaths[nIndex] = rAssetPath;
            }
                
            var rUserDataText = AssetImporter.GetAtPath(rAssetPaths[0]).userData;
            foreach(var rAssetPath in rAssetPaths)
            {
                var rImporter = AssetImporter.GetAtPath(rAssetPath);
                if (rUserDataText != rImporter.userData)
                    rUserDataText = @"---";
            }

            var bEnabled = GUI.enabled;
            GUI.enabled = true;
            {
                EditorGUILayout.LabelField("UserData:");
                var rNewUserDataText = EditorGUILayout.TextArea(rUserDataText);
                if (rNewUserDataText != rUserDataText)
                {
                    foreach(var rAssetPath in rAssetPaths)
                        AssetImporter.GetAtPath(rAssetPath).userData = rNewUserDataText;
                }
            }
            GUI.enabled = bEnabled;
        }
    }

}