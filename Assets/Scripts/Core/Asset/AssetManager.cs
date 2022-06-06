using UnityEngine;
using Core.Utils;

namespace Core.Asset
{
    public class AssetManager : Singleton<AssetManager>
    {
        private AbstractAsset _GetAbstractAsset()
        {
#if UNITY_EDITOR
            if (AssetWorkMode.UseSimulate)
            {
                return new AssetBySimulate();
            }
            else
            {
                return new AssetByAb();
            }
#else
            return new AssetByAb();
#endif
        }
        
        // AssetManager提供了两个加载接口，GetAsset、GetAllAssets
        // GetAsset     加载bundleName和assetName所指定的唯一资源
        // GetAllAssets 加载bundleName下所有资源。
        public AbstractAsset GetAsset(string bundleName, string assetName)
        {
            AbstractAsset asset = _GetAbstractAsset();
            asset.SetData(bundleName, assetName, true);
            return asset;
        }
        
        // 加载同一Bundle内所有资源的接口。
        // 1、一个Bundle里有多个资源文件。比如Nova的Scenario，就是很多txt打成一个Bundle。再比如ShaderVariantCollection
        // 2、一个Bundle里有一个文件，但是文件下面还带着子文件。比如Texture大图，但是它底下有一张Sprite。
        public AbstractAsset GetAllAssets(string bundleName)
        {
            AbstractAsset asset = _GetAbstractAsset();
            asset.SetData(bundleName, string.Empty, false);
            return asset;
        }
        
        // 有一种特殊的类型，一个Texture纹理，然后底下有一张Sprite。Sprite的范围又和Texture重叠。。。
        // 这种资源就很无语啊，如果是单张Texture走GetAsset就搞定了。现在就必须用LoadAllAsset这种接口，
        // 把Texture和Sprite都加载出来。然后呢，在asset[]数组里，虽然Texture是主资源，但是并不能保证
        // Texture是第一个资源，也就是的索引为0。而且两个资源的名字大概率是相同的。所以弄出了一个特殊的接口
        // GetTextureSpriteAsset，当加载完成后，可以调用AbstractAsset的LoadOrderXXX来加载Texture或者Sprite
        // 主要就是遍历assets，返回第一个指定的类型。目前Nova的SpriteController就是这么用的。
        public AbstractAsset GetTextureSpriteAsset(string bundleName, string assetName = "")
        {
            return GetAllAssets(bundleName);
        }

    }
}