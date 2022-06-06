using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Core.Asset
{
    // 2022.4.12改动
    // Bundle增加同步加载的接口。总共有三种加载情况
    // 1、异步加载（加载完成后，request和data都有值
    // 2、异步加载过程中调用同步加载接口（加载完成后，request和data都有值
    //    ref: https://www.1024sou.com/article/268142.html
    //    ref: https://www.cnblogs.com/yptianma/p/11781124.html
    // 3、同步加载（加载完成后，data存在，但request为空

    public class Bundle
    {
        private readonly string bundleName;
        private List<Bundle> depBundles;
        private int depCount;
        private AssetBundleCreateRequest request;
        private AssetBundle data;
        private List<Action<AssetBundle>> callbacks;

        public Bundle(string name)
        {
            bundleName = name;
            depCount = 0;
        }

        public void SetDepBundles(List<Bundle> depBundles)
        {
            this.depBundles = depBundles;
        }

        public AssetBundle LoadSync()
        {
            AddSelfAndDepRef();
            if (IsReadyLoadAsset())
            {
                return data;
            }
            LoadSelfAndDepSync();
            return data;
        }
        
        public void LoadAsync(Action<AssetBundle> onBundleLoaded)
        {
            AddSelfAndDepRef();
            if (IsReadyLoadAsset())
            {
                onBundleLoaded(data);
                return;
            }
            AddCallback(onBundleLoaded);
            LoadSelfAndDepAsync();
        }
        
        // 调用这个接口前必须保证已经加载完成。。。应该有改进的地方。
        // 与Asset设计不一样的原因在于，Asset是new出来的，直接调用Release就是调用对应的卸载方法。
        // Bundle是常驻唯一的，可能被多个Asset引用。调用Unload就只能在所有外部Asset都已经使用完成后，调用Asset.Unload之后
        // 才能调用Bundle.Unload。但其实只需要卸载对应的回调即可。这真是个缺点，因为调用Unload让外部确保是已经加载完成的状态。
        // AssetByAb倒是没问题，但是比如FGUI或者Scene不走AssetByAb，直接用Bundle的，就需要考虑这个问题了。。。。。。
        // 所以建议！！！在LoadAsync的回调里，设置为加载完成的状态。

        public void Unload()
        {
            SubtractSelfAndDepRef();
            UnloadSelfAndDep();
        }

        private bool IsSelfLoaded() => data != null;
        
        private bool IsReadyLoadAsset()
        {
            if (!IsSelfLoaded())
            {
                return false;
            }
            foreach (var depBundle in depBundles)
            {
                if (!depBundle.IsSelfLoaded())
                {
                    return false;
                }
            }
            return true;
        }

        #region 依赖引用计数
        
        private void AddSelfAndDepRef()
        {
            ++depCount;
            foreach (var depBundle in depBundles)
            {
                ++depBundle.depCount;
            }
        }

        private void SubtractSelfAndDepRef()
        {
            --depCount;
            foreach (var depBundle in depBundles)
            {
                --depBundle.depCount;
            }
        }
        #endregion
        
        #region 异步加载部分
        
        private void LoadSelfAndDepAsync()
        {
            LoadSelfAsync(this);
            foreach (var depBundle in depBundles)
            {
                depBundle.LoadSelfAsync(this);
            }
        }

        private void LoadSelfAsync(Bundle notifyWhichWhenSelfLoaded)
        {
            if (IsSelfLoaded())
            {
                OnSelfOrDepBundleLoaded(notifyWhichWhenSelfLoaded);
                return;
            }
            if (request == null)
            {
                string persistentPath = AssetConfig.GetPersistentABPath(bundleName);
                if (File.Exists(persistentPath))
                {
                    request = AssetBundle.LoadFromFileAsync(persistentPath);
                }
                else
                {
                    string streamingPath = AssetConfig.GetStreamingABPath(bundleName);
                    request = AssetBundle.LoadFromFileAsync(streamingPath);
                }
            }
            if (request.isDone)
                OnSelfOrDepBundleLoaded(notifyWhichWhenSelfLoaded);
            else
                request.completed += (AsyncOperation o) =>
                {
                    data = request.assetBundle;
                    OnSelfOrDepBundleLoaded(notifyWhichWhenSelfLoaded);
                };
        }
        
        private void OnSelfOrDepBundleLoaded(Bundle notifyWhichWhenSelfLoaded)
        {
            if (notifyWhichWhenSelfLoaded.IsReadyLoadAsset())
            {
                InvokeCallbacks();
            }
        }

        #endregion 
        
        #region 同步加载部分
        
        private void LoadSelfAndDepSync()
        {
            data = LoadSelfSync();
            foreach (var depBundle in depBundles)
            {
                depBundle.data = depBundle.LoadSelfSync();
            }
        }
        
        private AssetBundle LoadSelfSync()
        {
            if (IsSelfLoaded())
            {
                return data;
            }
            
            //正在异步加载，那么尝试转同步加载，直接调用 request.assetBundle即可。参考Bundle.cs开头的注释
            if (request != null)
            {
                return request.assetBundle;
            }
            
            // 如果没有异步加载，那么直接使用同步加载，先尝试Persistent路径，再尝试Streaming路径
            string persistentPath = AssetConfig.GetPersistentABPath(bundleName);
            if (File.Exists(persistentPath))
            {
                return AssetBundle.LoadFromFile(persistentPath);
            }
            string streamingPath = AssetConfig.GetStreamingABPath(bundleName);
            return AssetBundle.LoadFromFile(streamingPath);
        }

        #endregion 

        #region 卸载部分
        
        private void UnloadSelfAndDep()
        {
            UnloadSelf();
            UnloadDeps();
        }

        //每次加载会在对应的Bundle以及依赖的Bundle上自增计数。
        //如果依赖计数归零，表示此Bundle不再被需要，可卸载。
        private void UnloadSelf()
        {
            if (depCount > 0)
            {
                return;
            }
            // 如果是1、2两种加载情况，异步或者异步转同步
            if (request != null)
            {
                request.assetBundle.Unload(true);
                request = null;
                data = null;
            }
            //如果是3同步加载的情况
            if (data != null)
            {
                data.Unload(true);
                data = null;
            }
        }

        private void UnloadDeps()
        {
            foreach (var depBundle in depBundles)
            {
                depBundle.UnloadSelf();
            }
        }

        #endregion
        
        #region 回调
        
        private void AddCallback(Action<AssetBundle> onCompleted)
        {
            if (callbacks == null)
            {
                callbacks = new List<Action<AssetBundle>>();
            }
            callbacks.Add(onCompleted);
        }
        
        private void InvokeCallbacks()
        {
            if (callbacks != null && callbacks.Count != 0)
            {
                foreach (var action in callbacks)
                {
                    action.Invoke(data);
                }
                callbacks.Clear();
            }
        }

        #endregion
        
        public string GetInfo()
        {
            bool requestIsNull;
            bool assetBundleIsNull;
            if (request == null)
            {
                requestIsNull = true;
                assetBundleIsNull = true;
            }
            else
            {
                requestIsNull = false;
                assetBundleIsNull = request.assetBundle == null;
            }
            return $"bundleName:{bundleName}, depCount:{depCount}, requestIsNull:{requestIsNull}, assetBundleIsNull:{assetBundleIsNull}";
        }
        
    }
}