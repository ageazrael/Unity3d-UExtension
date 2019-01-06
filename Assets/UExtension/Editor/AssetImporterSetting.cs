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
    }
}