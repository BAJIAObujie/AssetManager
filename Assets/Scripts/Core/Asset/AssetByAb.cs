using System;
using UnityEngine;

namespace Core.Asset
{
    public class AssetByAb : AbstractAsset
    {
        private Bundle bundle;
        private AssetBundle ab;
        
        public override void LoadAsync()
        {
            isUsing = true;

            bundle = BundleManager.Instance.GetBundle(bundleName);
            bundle.LoadAsync((value) =>
            {
                ab = value;
                OnBundleLoaded();
            });
        }

        private void OnBundleLoaded()
        {
            if (!isUsing)
            {
                UnloadBundleAndAsset();
                return;
            }
            LoadAsset();
        }
        
        private void LoadAsset()
        {
             AssetBundleRequest assetRequest;
             if (bLoadMainAsset)
             {
                 assetRequest = ab.LoadAssetAsync(assetName);
                 assetRequest.completed += operation =>
                 {
                     asset = assetRequest.asset;
                     OnAssetLoaded();
                 };
             }
             else
             {
                 assetRequest = ab.LoadAllAssetsAsync();
                 assetRequest.completed += operation =>
                 {
                     assets = assetRequest.allAssets;
                     OnAssetLoaded();
                 };
             }
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
            bundle.Unload();
            bundle = null;
            ab = null;
            ClearFields();
        }

        public override void LoadSync()
        {
            isUsing = true;
            bundle = BundleManager.Instance.GetBundle(bundleName);
            ab = bundle.LoadSync();
            if (bLoadMainAsset)
                asset = ab.LoadAsset(assetName);
            else
                assets = ab.LoadAllAssets();
            isLoaded = true;
            InvokeCallback();
        }
    }
}