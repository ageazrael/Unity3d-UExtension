using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UExtension
{
    using Object = UnityEngine.Object;

	/// <summary>
	/// Runtime platform path.
	/// </summary>
	public class RuntimePlatformPath
	{
		public static string Path
		{
			get {
				switch (Application.platform) {
				case RuntimePlatform.Android:
					return "Android";

				case RuntimePlatform.IPhonePlayer:
					return "iOS";

				case RuntimePlatform.LinuxPlayer:
					return "Linux";

				case RuntimePlatform.tvOS:
					return "tvOS";

				case RuntimePlatform.PS4:
					return "PS4";

				case RuntimePlatform.PSP2:
					return "PSP2";

				case RuntimePlatform.XboxOne:
					return "XboxOne";

				case RuntimePlatform.OSXPlayer:
				case RuntimePlatform.OSXEditor:
					return "OSX";

				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					return "Windows";

				case RuntimePlatform.WebGLPlayer:
					return "WebGL";

				case RuntimePlatform.WSAPlayerARM:
				case RuntimePlatform.WSAPlayerX64:
				case RuntimePlatform.WSAPlayerX86:
					return "WSA";

				default:
					throw new UnityException (String.Format("Not support platform {0}", Application.platform.ToString()));
				}
			}
		}
	}

    /// <summary>
    /// AssetBundleLoader
    /// </summary>
    public class AssetBundleLoader : TSingleton<AssetBundleLoader>
    {
        public class BundleRef : ReferenceObject
        {
            public BundleRef(string rName, ReferenceManager rManager)
                : base(rName, rManager)
            { }
            
            public UnityEngine.AssetBundle  Bundle;
            public BundleInfo               Info;

            public string                   BundleName
            {
                get { return Info.BundleName; }
            }
            public string                   BundleFilePath
            {
                get { return Info.BundleFilePath; }
            }
            public string BundleURL
            {
                get { return Info.BundleURL; }
            }
            public string[]                 BundleDepend
            {
                get { return Info.BundleDepend; }
            }

            public override void Discard()
            {
                if (this.Bundle)
                    this.Bundle.Unload(true);

                this.Bundle = null;
            }
        }
        public class BundleInfo
        {
            public string   BundleName;
            public string   BundleFilePath;
            public string   BundleURL;
            public string[] BundleDepend;
        }

        public bool ContainsAsset(string rAssetPath)
        {
            return mAssetToBundle.ContainsKey(rAssetPath);
        }

        public Object LoadAsset(string rAssetPath, Type rAssetType)
        {
            string rAssetBundleName = string.Empty;
            if (!mAssetToBundle.TryGetValue(rAssetPath, out rAssetBundleName))
                return null;

            var rAssetBundleRef = this.CacheAssetBundle(rAssetBundleName);
            if (null == rAssetBundleRef)
                return null;

            if (!rAssetBundleRef.Bundle)
                return null;// TODO: Bundle load failed！

            return rAssetBundleRef.Bundle.LoadAsset(mAssetToFullPath[rAssetPath], rAssetType);;
        }
        public AssetLoaderRequest LoadAssetAsync(string rAssetPath, Type rAssetType)
        {
            var rRequest = new AssetLoaderRequest();
            CoroutineManager.Start(this.HandleLoadAssetAsync(rRequest, rAssetPath, rAssetType));
            return rRequest;
        }
        public TAssetLoaderRequest<T> LoadAssetAsync<T>(string rAssetPath)
            where T : UnityEngine.Object
        {
            var rRequest = new TAssetLoaderRequest<T>();
            CoroutineManager.Start(this.HandleLoadAssetAsync(rRequest, rAssetPath, typeof(T)));
            return rRequest;
        }

        public Coroutine AttachAssetBundleInfoByURL(string rRootURLOrPath)
        {
			return CoroutineManager.Start(this.HandleAttachAssetBundleInfo(PathExtension.Combine(rRootURLOrPath, RuntimePlatformPath.Path)));
        }
        public void AttachAssetBundleInfoByFilePath(string rRootPath)
        {
			var rAssetBundleInfoPath = PathExtension.Combine(rRootPath, RuntimePlatformPath.Path, ABInfoArchive.BinaryFileName);
            if (rAssetBundleInfoPath.Contains("://"))
                return; // TODO: invalid path

			AddAssetBundleInfo(ABInfoArchive.StaticLoadArchive(rAssetBundleInfoPath), PathExtension.Combine(rRootPath, RuntimePlatformPath.Path), false);
        }
        public void UnloadUnuseAssetBundle()
        {
            mBundleManager.DiscardReferenceEmpty();
        }
        public void UnloadAll()
        {
            mAssetToBundle.Clear();
            mAssetToFullPath.Clear();
            mBundleInfos.Clear();
            mBundleManager.DiscardAllAndClear();
        }
        protected BundleRef CacheAssetBundle(string rAssetBundleName)
        {
            var rAssetBundleRef = this.GetBundleRef(rAssetBundleName);
            if (null == rAssetBundleRef)
            {
                Debug.LogErrorFormat("Cache AssetBundle, Can`t found {0}", rAssetBundleName);
                return null;
            }

            if (!rAssetBundleRef.Bundle)
            {
                rAssetBundleRef.Bundle = UnityEngine.AssetBundle.LoadFromFile(rAssetBundleRef.BundleFilePath);
                foreach(var rDependBundleName in rAssetBundleRef.Info.BundleDepend)
                    this.CacheAssetBundle(rDependBundleName);
                
                rAssetBundleRef.Increment();
            }

            return rAssetBundleRef;
        }
        protected void UncacheAssetBundle(string rAssetBundleName)
        {
            var rAssetBundleRef = this.GetBundleRef(rAssetBundleName);
            if (null == rAssetBundleRef)
                return;

            if (0 == rAssetBundleRef.Decrement())
            {
                foreach(var rDependName in rAssetBundleRef.Info.BundleDepend)
                    this.UncacheAssetBundle(rDependName);
            }
        }
        protected IEnumerator HandleLoadAssetAsync(AssetLoaderRequest rRequest, string rAssetPath, Type rAssetType)
        {
            string rAssetBundleName = string.Empty;
			if (!mAssetToBundle.TryGetValue (rAssetPath, out rAssetBundleName))
			{
				Debug.LogErrorFormat("{0} not in AssetBundleLoader", rAssetPath);
				rRequest.LoadCompleted();
				yield break;
			}

            var rAssetBundleRef = this.GetBundleRef(rAssetBundleName);
            if (null == rAssetBundleRef)
			{
				Debug.LogErrorFormat("can't found {0} AssetBundle", rAssetBundleName);
				rRequest.LoadCompleted();
                yield break;
			}

            if (!rAssetBundleRef.Bundle)
            {
                if (!string.IsNullOrEmpty(rAssetBundleRef.BundleURL))
                {
                    var www = new WWW(rAssetBundleRef.BundleURL);
                    yield return www;

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError(www.error);
                        rRequest.LoadCompleted();
                        yield break; // TODO: load assetbundle failed!
                    }

                    rAssetBundleRef.Bundle = www.assetBundle;
                }
                else if (!string.IsNullOrEmpty(rAssetBundleRef.BundleFilePath))
                {
                    var rLoader = AssetBundle.LoadFromFileAsync(rAssetBundleRef.BundleFilePath);
                    yield return rLoader;

                    rAssetBundleRef.Bundle = rLoader.assetBundle;
                }
                else
                {
                    Debug.LogErrorFormat("AssetBundle({0}) invalid URL/FilePath", rAssetBundleName);
                    rRequest.LoadCompleted();
                    yield break;
                }
                
                foreach(var rDependName in rAssetBundleRef.BundleDepend)
                    yield return CoroutineManager.Start(this.CacheBundleDependByWWW(rDependName));

                rAssetBundleRef.Decrement();
            }
            var rAssetRequest = rAssetBundleRef.Bundle.LoadAssetAsync(mAssetToFullPath[rAssetPath], rAssetType);
            yield return rAssetRequest;

			rRequest.LoadCompleted(rAssetRequest.asset);
        }
        protected IEnumerator CacheBundleDependByWWW(string rAssetBundleName)
        {
            var rAssetBundleRef = this.GetBundleRef(rAssetBundleName);
            if (null == rAssetBundleRef)
                yield break; // TODO: invalid assetbundle

            if (!rAssetBundleRef.Bundle)
            {
                var www = new WWW(rAssetBundleRef.BundleURL);
                yield return www;

                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError(www.error);
                    yield break; // TODO: load assetbundle failed!
                }

                rAssetBundleRef.Bundle = www.assetBundle;
                foreach(var rDependName in rAssetBundleRef.BundleDepend)
                    this.CacheBundleDependByWWW(rDependName);
            }

            rAssetBundleRef.Increment();
        }
        protected IEnumerator CacheAssetBundleByWWW(string rAssetBundleName)
        {
            yield return 0;
        }
        protected BundleRef GetBundleRef(string rAssetBundleName)
        {
            BundleInfo rBundleInfo = null;
            if (!mBundleInfos.TryGetValue(rAssetBundleName.ToLower(), out rBundleInfo))
                return null;

            BundleRef rBundleRef = null;
            if (mBundleManager.GetOrCreateObject(rAssetBundleName, out rBundleRef))
                rBundleRef.Info = rBundleInfo;

            return rBundleRef;
        }
        protected IEnumerator HandleAttachAssetBundleInfo(string rRootURL)
        {
            var rAssetBundleInfoPath = PathExtension.Combine(rRootURL, ABInfoArchive.BinaryFileName);
            if (!rAssetBundleInfoPath.Contains("://"))
			{
				Debug.LogErrorFormat("{0} not url", rRootURL);
                yield break;
			}

            var rAssetBundleInfo = new ABInfoArchive();
            yield return rAssetBundleInfo.LoadArchiveByWWW(rAssetBundleInfoPath);

            AddAssetBundleInfo(rAssetBundleInfo, rRootURL, true);
        }
        protected void AddAssetBundleInfo(ABInfoArchive rABInfoArchive, string rRootPath, bool bIsUrl)
        {
			foreach (var rABInfo in rABInfoArchive.Entitys)
			{
				var rBundleName = rABInfo.Key.ToLower();

				var rBundleInfo = new BundleInfo ();
				rBundleInfo.BundleName 			= rBundleName;
				rBundleInfo.BundleDepend 		= rABInfo.Value.Depend;
				if (bIsUrl)
					rBundleInfo.BundleURL 		= PathExtension.Combine(rRootPath, rABInfo.Key);
				else
					rBundleInfo.BundleFilePath 	= PathExtension.Combine(rRootPath, rABInfo.Key);

				mBundleInfos[rBundleName]		= rBundleInfo;
			}
			mAssetToBundle   = rABInfoArchive.GenerateAssetToBundle();
			mAssetToFullPath = rABInfoArchive.GenerateAssetToFullPath ();
        }
        
        Dictionary<string, string>      mAssetToBundle  = new Dictionary<string, string>();
        Dictionary<string, string>      mAssetToFullPath= new Dictionary<string, string>();
        Dictionary<string, BundleInfo>  mBundleInfos    = new Dictionary<string, BundleInfo>();
        TReferenceManager<BundleRef>    mBundleManager  = new TReferenceManager<BundleRef>();
    }
}