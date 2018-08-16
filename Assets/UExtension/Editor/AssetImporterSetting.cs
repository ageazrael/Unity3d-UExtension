using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UExtension
{

    public class AssetImporterSetting : ScriptableObject
    {
        [Serializable]
        public class AISTextureImporter
        {
            public string RegexpMatch;

            public TextureImporterType TextureType;
        }



        public AISTextureImporter[] TextureImporter;

        [Readonly]
        public int Value;

        [MinMax(0, 1)]
        public float S1;

        [MinMax(0, 200)]
        public int S2;

        [MinMax(0, 100, IsSlider = true)]
        public Vector2 V2;

        [ResizableTextArea]
        public string Text;

        [Dropdown("")]
        public string SelectType;
    }
}