using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;
using UExtension;
using System.Collections;

namespace UnitTest
{

    public class UnitTestAssetBundleLoader
    {
        [Test]
        public void EditorTest()
        {
            CoroutineManager.Start(TestDomain());
        }

        IEnumerator TestDomain()
        {
            yield return CoroutineManager.Start(HandleDomain(true));

            yield return CoroutineManager.Start(HandleDomain(false));

        }

        IEnumerator HandleDomain(bool bIsUrl)
        {
            var rUnitTestURL = PathExtension.Combine(System.Environment.CurrentDirectory.Replace('\\', '/'), "UnitTestAssetBundle");

            Object rAssetA = null;
            Object rAssetB = null;
            if (bIsUrl)
            {
                yield return AssetBundleLoader.Instance.AttachAssetBundleInfoByURL("file:///" + rUnitTestURL);

                var rAssetARequest = AssetBundleLoader.Instance.LoadAssetAsync("Object/A", typeof(Texture2D));
                yield return rAssetARequest;
                rAssetA = rAssetARequest.Asset;

                var rAssetBRequest = AssetBundleLoader.Instance.LoadAssetAsync("Object/B", typeof(Texture2D));
                yield return rAssetBRequest;
                rAssetB = rAssetBRequest.Asset;
            }
            else
            {
                AssetBundleLoader.Instance.AttachAssetBundleInfoByFilePath(rUnitTestURL);

                rAssetA = AssetBundleLoader.Instance.LoadAsset("Object/A", typeof(Texture2D));
                rAssetB = AssetBundleLoader.Instance.LoadAsset("Object/B", typeof(Texture2D));
            }

            Debug.LogFormat("Object/A => {0}", rAssetA);
            Debug.LogFormat("Object/B => {0}", rAssetB);

            Debug.Log("Completed");

            yield return 0;
        }
    }

}