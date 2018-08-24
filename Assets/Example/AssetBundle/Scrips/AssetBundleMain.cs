using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UExtension.Example
{
    public class AssetBundleMain : MonoBehaviour
    {
        public GameObject   GUI;
        public ScrollRect   Scrollview;
        public UIChild      TemplateItem;

        private CharacterSettings Settings;

        public void Awake()
        {
            var rAssetBundlePath = Application.dataPath.Replace("/Assets", "/AssetBundle");
            AssetBundleLoader.Instance.AttachAssetBundleInfoByFilePath(PathExtension.Combine(rAssetBundlePath, "AddonLevel"));
            AssetBundleLoader.Instance.AttachAssetBundleInfoByFilePath(PathExtension.Combine(rAssetBundlePath, "Character"));

            this.GUI.SetActive(false);
            this.StartCoroutine(this.HandlerInitialize());
        }

        IEnumerator HandlerInitialize()
        {
            var rChartSettingsRequest = AssetLoader.LoadAssetAsync<CharacterSettings>("Settings/CharacterSettings");
            yield return rChartSettingsRequest;

            this.Settings = rChartSettingsRequest.Asset;

            TemplateItem.gameObject.SetActive(false);
            foreach (var rChar in this.Settings.Characters)
            {
                var rItem = Object.Instantiate(TemplateItem.gameObject, TemplateItem.transform.parent);
                var rChild = rItem.GetComponent<UIChild>();

                yield return rChar.HeadIcon.LoadAsync();

                rChild.Get<Text>("Text").text = rChar.Name;
                rChild.Get<RawImage>("Image").texture = rChar.HeadIcon.Asset;
                rChild.Get<Button>("Button").onClick.AddListener(() =>
                {
                    this.StartCoroutine(HandlerOnSelectCharacter(
                        Path.GetFileNameWithoutExtension(rChar.CharacterAsset.AssetPath)));
                });

                rItem.SetActive(true);
            }

            var rContent = TemplateItem.transform.parent.transform as RectTransform;
            var rGrid = TemplateItem.transform.parent.GetComponent<GridLayoutGroup>();
            if (rGrid)
            {
                rContent.sizeDelta = new Vector2(rContent.sizeDelta.x, 
                    rGrid.cellSize.y * this.Settings.Characters.Length);
            }

            this.GUI.SetActive(true);
        }

        IEnumerator HandlerOnSelectCharacter(string rCharacterName)
        {
            yield return AssetLoader.LoadSceneAsync("Level0", LoadSceneMode.Additive, true);

            var rCharacterRequest = AssetLoader.LoadAssetAsync<GameObject>(rCharacterName);
            yield return rCharacterRequest;

            var rPlayStart = GameObject.FindGameObjectWithTag("PlayStart");

            var rCharacter = Instantiate<GameObject>(rCharacterRequest.Asset, null);
            rCharacter.transform.position = rPlayStart.transform.position;

            rCharacter.AddComponent<Character>();

            var rTPCamera = Camera.main.gameObject.AddComponent<ThirdPersonCamera>();
            if (rTPCamera)
                rTPCamera.TrackTarget = rCharacter.transform;

            this.GUI.SetActive(false);
        }
    }
}