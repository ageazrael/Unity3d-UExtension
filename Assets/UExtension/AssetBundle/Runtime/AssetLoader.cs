using UnityEngine;
using System.Collections;
using System;

namespace UExtension
{
    using Object = UnityEngine.Object;

    /// <summary>
    /// AssetLoaderRequest
    /// </summary>
    public class AssetLoaderRequest : CustomYieldInstruction
    {
        public Object Asset
        {
            get { return Assets[0]; }
        }
        public Object[] Assets;

        public override bool keepWaiting
        {
            get { return !IsDone; }
        }
        public bool IsDone;

        public event Action<AssetLoaderRequest> OnLoadCompleted;

        public void LoadCompleted(params Object[] rAssets)
        {
            this.Assets = rAssets;
            this.IsDone = true;
            if (null != this.OnLoadCompleted)
                this.OnLoadCompleted(this);
        }
    }
    /// <summary>
    /// TAssetLoaderRequest<T>
    /// </summary>
    public class TAssetLoaderRequest<T> : AssetLoaderRequest
        where T : UnityEngine.Object
    {
        public new T Asset
        {
            get { return Assets[0] ? (Assets[0] as T) : default(T); }
        }
    }

    /// <summary>
    /// AssetLoader
    /// </summary>
    public class AssetLoader
    {
        public static AssetLoaderRequest LoadAssetAsync(string rAssetPath, System.Type rAssetType)
        {
            if (AssetBundleLoader.Instance.ContainsAsset(rAssetPath))
                return AssetBundleLoader.Instance.LoadAssetAsync(rAssetPath, rAssetType);
            else
                return LoadInResourceAsync(rAssetPath, rAssetType);
        }
        public static TAssetLoaderRequest<T> LoadAssetAsync<T>(string rAssetPath)
            where T : UnityEngine.Object
        {
            if (AssetBundleLoader.Instance.ContainsAsset(rAssetPath))
                return AssetBundleLoader.Instance.LoadAssetAsync<T>(rAssetPath);
            else
                return LoadInResourceAsync<T>(rAssetPath);
        }
        public static Object LoadAsset(string rAssetPath, System.Type rAssetType)
        {
            if (AssetBundleLoader.Instance.ContainsAsset(rAssetPath))
                return AssetBundleLoader.Instance.LoadAsset(rAssetPath, rAssetType);
            else
                return Resources.Load(rAssetPath, rAssetType);
        }
#region Async 
        static AssetLoaderRequest LoadInResourceAsync(string rAssetPath, System.Type rAssetType)
        {
            var rRequest = new AssetLoaderRequest();
            CoroutineManager.Start(HandleLoadInResourceAsync(rAssetPath, rAssetType, rRequest));
            return rRequest;
        }
        static TAssetLoaderRequest<T> LoadInResourceAsync<T>(string rAssetPath)
            where T : UnityEngine.Object
        {
            var rRequest = new TAssetLoaderRequest<T>();
            CoroutineManager.Start(HandleLoadInResourceAsync(rAssetPath, typeof(T), rRequest));
            return rRequest;
        }
        static IEnumerator HandleLoadInResourceAsync(string rAssetPath, System.Type rAssetType, AssetLoaderRequest rRequest)
        {
            var rLoadAsyncRequest = Resources.LoadAsync(rAssetPath, rAssetType);
            yield return rLoadAsyncRequest;

            rRequest.LoadCompleted(rLoadAsyncRequest.asset);
        }
    }
#endregion
}