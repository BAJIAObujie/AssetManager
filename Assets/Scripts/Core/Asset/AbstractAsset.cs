using System;
using UnityEngine;

namespace Core.Asset
{
    public abstract class AbstractAsset
    {
        protected string bundleName;
        protected string assetName;
        protected bool bLoadMainAsset; // true mean LoadAsset, false mean LoadAllAssets
        
        protected bool isUsing;
        protected bool isLoaded;
        private Action onComplete;
        
        public UnityEngine.Object asset;
        public UnityEngine.Object[] assets;
        
        public void SetData(string _bundleName, string _assetName, bool bLoadMainAsset)
        {
            this.bundleName = _bundleName;
            this.assetName = _assetName;
            this.bLoadMainAsset = bLoadMainAsset;
        }
        
        #region 回调
        
        public void SetCallback(Action onComplete)
        {
            this.onComplete = onComplete;
        }

        protected void InvokeCallback()
        {
            onComplete?.Invoke();
        }
        
        #endregion

        #region 加载接口

        public abstract void LoadSync();
        
        public abstract void LoadAsync();

        protected abstract void UnloadBundleAndAsset();
        
        #endregion
        
        public void Release()
        {
            if (!isUsing) { return; }
            isUsing = false;
            
            onComplete = null;
            if (isLoaded)
                UnloadBundleAndAsset();
        }
        
        protected void ClearFields()
        {
            bundleName = null;
            assetName = null;
            isUsing = false;
            isLoaded = false;
            onComplete = null;
            asset = null;
            assets = null;
        }

        
        #region 一些特殊的读取接口（完成加载后才能调用
        
        // 下面是处理一张Texture贴图里有单个或者多个Sprite的情况。
        // 这种情况下 UnityEngine.Object[] assets 包含一个Texture以及单个或者多个Sprite。
        // LoadTexture遍历所有返回唯一的Texture
        // LoadSprite
        // 对于Texture下多个Sprite的情况，要求要传入spriteName，会遍历所有返回spriteName匹配的Sprite。
        // 对于Texture下单个Sprite的情况，可以让spriteName为空，返回唯一的一个sprite。
        public Texture LoadTexture()
        {
            foreach (var value in assets)
            {
                if (value is Texture ret)
                {
                    return ret;
                }
            }
            return null;
        }

        public Sprite LoadSprite(string spriteName = "")
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                foreach (var value in assets)
                {
                    if (value is Sprite ret)
                    {
                        return ret;
                    }
                }
            }
            else
            {
                foreach (var value in assets)
                {
                    if (value.name == spriteName && value is Sprite ret)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }
        
        #endregion
        

    }
}