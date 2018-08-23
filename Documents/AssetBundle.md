# AssetBundle
* AssetBundle��Դ������

![Asset Bundle.Character](AssetBundle.Character.png)

* ����AssetBundle��Դ
```cs
AssetBundleLoader.Instance.AttachAssetBundleInfoByFilePath("AssetBundle/Character");

// :Assets/Example/AssetBundle/Character/Resources/Character0.prefab
// :AssetBundle/Character/Windows/character0.unity3d:Character0
// Assets/Example/AssetBundle/Scripts/AssetBundleMain.cs
var rRequest = AssetLoader.LoadAsync<GameObject>("Character0");
yield return rRequest;

var rCharacterInstance = Instantiate(rRequest.Asset, null);
```

* �ؼ�AssetBundle�е���Դ��Resources�е���Դ
![Asset Ref.Character Settings](AssetRef.CharacterSettings.png)
```cs
// Assets/Example/AssetBundle/Scripts/CharacterSettings.cs
// RGameObject CharacterAsset;

var rRequest = CharacterAsset.LoadAsync();
yield return rRequest;

var rCharacterInstance = Instantiate(rRequest.Asset, null);
```


# Bug
* `AssetLoader.LoadAsset("Character0", typeof(GameObject))`����ü���AssetBundle��֧�ֶ�μ���Prefab