using Core.Asset;
using UnityEngine.Assertions;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/UITexture(UGUI)")]
    public class UITexture : RawImage
    {
        private AbstractAsset _texAsset;

        public void SetTexture(string bundleName, string assetName)
        {
            _texAsset?.Release();
            _texAsset = AssetManager.Instance.GetAsset(bundleName, assetName);
            _texAsset.SetCallback(OnAssetLoaded);
            _texAsset.LoadAsync();
        }

        public void SetTexture(Texture tex)
        {
            _texAsset?.Release();
            texture = tex;
        }

        private void OnAssetLoaded()
        {
            texture = _texAsset.asset as Texture;
        }
    }
}