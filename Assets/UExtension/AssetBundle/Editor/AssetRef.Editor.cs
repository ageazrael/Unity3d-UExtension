using UnityEditor;
using UnityEditor.Animations;
using System;


/// <summary>
/// 由于携程无法在编辑器模式下较好的运行、以下类型的资源我看你们还是别用比较好。 - 编辑器资源动态加载个毛线啊？
/// </summary>
namespace UExtension
{
    [Serializable] public class RAvatarMask         : TAssetRef<AvatarMask>         { }
    [Serializable] public class RAnimatorController : TAssetRef<AnimatorController> { }
    [Serializable] public class RSceneAsset         : TAssetRef<SceneAsset>         { }
    [Serializable] public class RLightingDataAssett : TAssetRef<LightingDataAsset>  { }
}