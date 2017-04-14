using UnityEngine;
using System.Collections;
using UExtension;

namespace UnitTest
{
    [System.Serializable]
    public class EventX : UnityEngine.Events.UnityEvent<string, int, int>
    {
    }
    public class UnitTestAssetRef : MonoBehaviour
    {
        public RObject      RefObject;
        public RTexture     RefTexture;
        public RTexture2D   RefTexture2D;
        public RTexture3D   RefTexture3D;

        public RSprite      RefSprite;

        public RSprite      LoadAssetRef;


        public EventX       What;


        public void Print(string v1, int v2, int v3)
        {
        }
        public void EXD(string v1)
        {
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