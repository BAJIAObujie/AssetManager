#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Assertions;

namespace Core.Asset
{
    public class AssetBySimulate : AbstractAsset
    {
        public override void LoadAsync()
        {
            isUsing = true;
            if (false)
                EditorApplication.delayCall += () => LoadAsset();
            else
                LoadAsset();
        }

        private void LoadAsset()
        {
            if (bLoadMainAsset)
                LoadMainAsset();
            else
                LoadAllAsset();
        }

        private void LoadMainAsset()
        {
            //在同一个Bundle下，不允许出现同名资源！
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
            Assert.IsTrue(assetPaths.Length == 1, $"assetPaths.Length:{assetPaths.Length}, bundleName:{bundleName}, assetName:{assetName} ");
            string assetPath = assetPaths[0];
            asset = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
            OnAssetLoaded();
        }
        
        private void LoadAllAsset()
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

            List<UnityEngine.Object> allAssets = new List<UnityEngine.Object>(assetPaths.Length);
            foreach (var assetPath in assetPaths)
            {
                var assetsUnderPath = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                allAssets.AddRange(assetsUnderPath);
            }
            assets = allAssets.ToArray();
            OnAssetLoaded();
        }

        private void OnAssetLoaded()
        {
            isLoaded = true;
            if (!isUsing)
            {
                UnloadBundleAndAsset();
                return;
            }
            InvokeCallback();
        }

        protected override void UnloadBundleAndAsset()
        {
            ClearFields();
        }

        public override void LoadSync()
        {
            isUsing = true;
            LoadAsset();
        }
    }
}

#endif