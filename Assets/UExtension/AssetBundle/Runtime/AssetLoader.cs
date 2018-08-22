using UnityEngine;
using UnityEngine.SceneManagement;
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
        public Object Asset => this.Assets[0];
        public Object[] Assets;

        public override bool keepWaiting => !this.IsDone;
        public bool IsDone;

        public bool IsAsset = true;
        public LoadSceneMode SceneMode;
        public bool IsActiveScene = false;

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
        public new T Asset => Assets[0] ? (Assets[0] as T) : default(T);
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
        public static AssetLoaderRequest LoadSceneAsync(string rSceneName, LoadSceneMode rSceneMode, bool bIsActiveScene = false)
        {
            if (AssetBundleLoader.Instance.ContainsAsset(rSceneName))
                return AssetBundleLoader.Instance.LoadSceneAsync(rSceneName, rSceneMode, bIsActiveScene);
            else
                return LoadInSceneAsync(rSceneName, rSceneMode, bIsActiveScene);
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
        static AssetLoaderRequest LoadInSceneAsync(string rSceneName, LoadSceneMode rSceneMode, bool bIsActiveScene)
        {
            var rRequest = new AssetLoaderRequest() { IsAsset = false, SceneMode = rSceneMode, IsActiveScene = bIsActiveScene };
            CoroutineManager.Start(HandleLoadInResourceAsync(rSceneName, typeof(Object), rRequest));
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
            if (rRequest.IsAsset)
            {
                var rLoadAsyncRequest = Resources.LoadAsync(rAssetPath, rAssetType);
                yield return rLoadAsyncRequest;

                rRequest.LoadCompleted(rLoadAsyncRequest.asset);
            }
            else
            {
                yield return SceneManager.LoadSceneAsync(rAssetPath, rRequest.SceneMode);
                if (rRequest.IsActiveScene)
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(rAssetPath));

                rRequest.LoadCompleted(null);
            }
        }
    }
#endregion
}