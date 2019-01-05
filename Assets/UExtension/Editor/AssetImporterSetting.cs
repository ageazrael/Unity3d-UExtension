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

            [Dropdown("骑士", 1, "盗贼", 2, PreviewValue = true)]
            public string SelectType;

            [Dropdown("GMoveCurve", PreviewValue = true)]
            public AnimationCurve CharMoveCurve;
            public static Dictionary<string, AnimationCurve> GMoveCurve = new Dictionary<string, AnimationCurve>()
            {
                {"直线移动", AnimationCurve.Linear(0, 0, 2, 1) },
                {"瞎鸡巴动", AnimationCurve.EaseInOut(0, 0, 2, 1) },
            };

            [Dropdown("GCharIcons", PreviewValue = true, Readonly = true)]
            public Texture2D CharIcons;
            public static Dictionary<string, Texture2D> GCharIcons
            {
                get
                {
                    if (_GCharIcons == null)
                    {
                        _GCharIcons = new Dictionary<string, Texture2D>()
                        {
                            {"None", null },
                            {"UISkinBackground", Resources.Load<Texture2D>("Folder/Total/A") },
                        };
                    }
                    return _GCharIcons;
                }
            }
            static Dictionary<string, Texture2D> _GCharIcons = null;
        }


        public AISTextureImporter[] TextureImporter;

        [Readonly]
        public int Value;

        [MinMax(0, 1)]
        public float S1;

        [MinMax(0, 200)]
        public int S2;

        [MinMax(0, 100, IsSlider = true, IsVisibleControllerValue = "S2IsValid")]
        public Vector2 V2;

        [ResizableTextArea(IsEnableControllerValue = "S1IsValid")]
        public string Text;


        private bool S1IsValid => this.S1 > 0;
        private bool S2IsValid => this.S2 > 0;
    }
}