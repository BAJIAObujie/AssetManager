using System.IO;
using UnityEngine;

namespace Core.Asset
{
    public static class AssetConfig
    {
        public static readonly string StreamingBundlePath = Path.Combine(Application.streamingAssetsPath, "Bundle");
        public static readonly string PersistentBundlePath = Path.Combine(Application.persistentDataPath, "Bundle");
        
        public static readonly string AbManifestPath = Path.Combine(StreamingBundlePath, "Bundle");
        public static readonly string AbManifestName = "AssetBundleManifest";
        
        public static string GetStreamingABPath(string relativePath)
        {
            return Path.Combine(StreamingBundlePath, relativePath);
        }
        
        public static string GetPersistentABPath(string relativePath)
        {
            return Path.Combine(PersistentBundlePath, relativePath);
        }
    }
}