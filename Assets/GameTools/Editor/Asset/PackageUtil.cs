using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Core.Asset;

namespace GameTools.Package
{
    public static class PackageUtil
    {
        #region MenuItem
        
        [MenuItem("GameTools/Asset/构建Win64-AssetBundle")]
        public static void PackageWin64()
        {
            Package(BuildTarget.StandaloneWindows64);
        }
        
        [MenuItem("GameTools/Asset/构建Win64-AssetBundle", true)]
        public static bool PackageWin64Check()
        {
            return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64;
        }

        #endregion
        
        private static void Package(BuildTarget target)
        {
            string bundlePath = AssetConfig.StreamingBundlePath;
            
            // delete old folder
            if (Directory.Exists(bundlePath))
            {
                Directory.Delete(bundlePath, true);
            }
            Directory.CreateDirectory(bundlePath);
            
            // Copy Lua File Then Change Extension
            // string luaSrcFolderPath = Path.Combine(Application.dataPath, "../LuaScript");
            // string luaTargetFolderPath = Path.Combine(Application.dataPath, "LuaScript");
            // CopyFolder(luaSrcFolderPath, luaTargetFolderPath);
            // ReNameFileExtensionRecursive(luaTargetFolderPath, "bytes");
            
            // //Mark LuaFile ABName
            // AssetDatabase.Refresh();
            // string[] searchPaths = {"Assets/LuaScript"};
            // foreach (string fileGuid in AssetDatabase.FindAssets(String.Empty, searchPaths))
            // {
            //     string assetPath = AssetDatabase.GUIDToAssetPath(fileGuid);
            //     if (AssetDatabase.IsValidFolder(assetPath))
            //         continue;
            //     AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            //     importer.assetBundleName = "LuaScriptBundle".ToLower();
            // }
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();

            // build bundle
            BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
            BuildPipeline.BuildAssetBundles(bundlePath, options, target);
            
            // Delete Lua Folder
            // Directory.Delete(luaTargetFolderPath, true);
        }
        
    }
}