using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System;

namespace UExtension
{
    using Object = UnityEngine.Object;

    [Serializable]
    public class TAssetRef<T> : AssetRefBase
        where T : UnityEngine.Object
    {
        public TAssetRef()
        {
            this.AssetTypeFullName = typeof(T).AssemblyQualifiedName;
        }

        public T Asset
        {
            get {
                var rLoadAsset = AssetLoader.LoadAsset(this.AssetPath, this.AssetType);
                if (rLoadAsset)
                    this.LoadedAssetInstanceID = rLoadAsset.GetInstanceID();
                return rLoadAsset ? rLoadAsset as T : default(T);
            }
        }
        public TAssetLoaderRequest<T> LoadAsync()
        {
            var rRequest = AssetLoader.LoadAssetAsync<T>(this.AssetPath);
            rRequest.OnLoadCompleted += this.HandleLoadCompleted;
            return rRequest;
        }

        private void HandleLoadCompleted(AssetLoaderRequest rRequest)
        {
            if (rRequest.Asset)
                this.LoadedAssetInstanceID = rRequest.Asset.GetInstanceID();
        }
    }

    [Serializable] public class RObject                      : TAssetRef<Object> { }
    [Serializable] public class RTexture                     : TAssetRef<Texture> { }
    [Serializable] public class RTexture2D                   : TAssetRef<Texture2D> { }
    [Serializable] public class RTexture3D                   : TAssetRef<Texture3D> { }
    [Serializable] public class RCubemap                     : TAssetRef<Cubemap> { }
    [Serializable] public class RRenderTexture               : TAssetRef<RenderTexture> { }
    [Serializable] public class RAnimationClip               : TAssetRef<AnimationClip> { }
    [Serializable] public class RRuntimeAnimatorController   : TAssetRef<RuntimeAnimatorController> { }
    [Serializable] public class RAnimatorOverrideController  : TAssetRef<AnimatorOverrideController> { }
    [Serializable] public class RAudioClip                   : TAssetRef<AudioClip> { }
    [Serializable] public class RAudioMixer                  : TAssetRef<AudioMixer> { }
    [Serializable] public class RMesh                        : TAssetRef<Mesh> { }
    [Serializable] public class RSprite                      : TAssetRef<Sprite> { }
    [Serializable] public class RShader                      : TAssetRef<Shader> { }
    [Serializable] public class RComputeShader               : TAssetRef<ComputeShader> { }
    [Serializable] public class RShaderVariantCollection     : TAssetRef<ShaderVariantCollection> { }
    [Serializable] public class RMaterial                    : TAssetRef<Material> { }
    [Serializable] public class RFlare                       : TAssetRef<Flare> { }
    [Serializable] public class RTextAsset                   : TAssetRef<TextAsset> { }
    [Serializable] public class RPhysicMaterial              : TAssetRef<PhysicMaterial> { }
    [Serializable] public class RPhysicsMaterial2D           : TAssetRef<PhysicsMaterial2D> { }
    [Serializable] public class RFont                        : TAssetRef<Font> { }


    [Serializable] public class RScriptableObject           : TAssetRef<ScriptableObject> { }
    [Serializable] public class RGameObject                 : TAssetRef<GameObject> { }
}