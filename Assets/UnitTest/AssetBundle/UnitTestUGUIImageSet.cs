using UnityEngine;
using System.Collections;
using UExtension;
using UnityEngine.UI;

namespace UnitTest
{

    [RequireComponent(typeof(Image))]
    public class UnitTestUGUIImageSet : MonoBehaviour
    {
        public RSprite  SpriteImage;
        public bool     AsyncLoad = false;

        public void Show()
        {
            if (this.AsyncLoad)
                this.StartCoroutine(HandleShow());
            else
                this.GetComponent<Image>().sprite = SpriteImage.Asset;
        }
        public void Hide()
        {
            this.GetComponent<Image>().sprite = null;
        }
        public void UnloadUnuse()
        {
            Resources.UnloadUnusedAssets();
        }

        IEnumerator HandleShow()
        {
            var rRequest = SpriteImage.LoadAsync();
            yield return rRequest;

            if (rRequest.Asset)
                this.GetComponent<Image>().sprite = rRequest.Asset;
        }
    }

}