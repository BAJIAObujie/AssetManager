using System;
using System.Collections.Generic;
using UnityEngine;
using Core.Utils;

namespace Core.Asset
{
    public class BundleManager : Singleton<BundleManager>
    {
        private Dictionary<string, Bundle> name2Bundles = new Dictionary<string, Bundle>();
        private AssetBundleManifest _abManifest;
        private HashSet<string> _abNameSet;

        public void Init()
        {
            if (AssetWorkMode.UseSimulate) return;
            
            AssetBundle ab = AssetBundle.LoadFromFile(AssetConfig.AbManifestPath);
            _abManifest = ab.LoadAsset<AssetBundleManifest>(AssetConfig.AbManifestName);
            _abNameSet = new HashSet<string>();
            foreach (var abName in _abManifest.GetAllAssetBundles())
            { 
                _abNameSet.Add(abName);
            }
        }

        public Bundle GetBundle(string bundleName)
        {
            if (!_abNameSet.Contains(bundleName))
            {
                throw new Exception($"找不到对应的Bundle BundleName:{bundleName}");
            }
            if (name2Bundles.TryGetValue(bundleName, out Bundle ret))
            {
                return ret;
            }
            ret = new Bundle(bundleName);
            name2Bundles[bundleName] = ret;
            string[] deps = _abManifest.GetAllDependencies(bundleName);
            List<Bundle> depBundles = new List<Bundle>(deps.Length);
            foreach (var dep in deps)
            {
                depBundles.Add(GetBundle(dep));
            }
            ret.SetDepBundles(depBundles);
            return ret;
        }

    }
}