using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UExtension;

namespace UExtension.Example
{

    [CreateAssetMenu(menuName = "UExtension/Example/AssetBundle/Character Settings")]
    public class CharacterSettings : ScriptableObject
    {
        [System.Serializable]
        public class CharacterInfo
        {
            public string      Name;

            public RGameObject CharacterAsset;

            public RTexture2D HeadIcon;

            public float HPBase;
            public float Atkbase;
        }

        public AnimationCurve HPCurve;
        public AnimationCurve AtkCurve;

        public CharacterInfo[] Characters;
    }
}