using UnityEngine;
using System.Collections;
using UExtension;

namespace UnitTest
{
    public class UnitTestAssetRef : MonoBehaviour
    {
        public RObject      RefObject;
        public RTexture     RefTexture;
        public RTexture2D   RefTexture2D;
        public RTexture3D   RefTexture3D;

        public RSprite      RefSprite;

        public RSprite      LoadAssetRef;

        private void Awake()
        {
            AssetBundleLoader.Instance.AttachAssetBundleInfoByFilePath("UnitTestAssetBundle");
        }


        [ContextMenu("Begin load")]
        void MCLoadAssetRef()
        {
            this.StartCoroutine(HandleLoadAssetRef());
        }

        IEnumerator HandleLoadAssetRef()
        {
            var rRequest = LoadAssetRef.LoadAsync();
            yield return rRequest;

            if (rRequest.Asset)
                Debug.LogErrorFormat("Load Completed {0}", rRequest.Asset.ToString());
            else
                Debug.LogErrorFormat("Load Error");
        }
    }
}