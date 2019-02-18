using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UExtension
{
    public class AssetImporterTypes : TypeSearchDefault<AssetImporter> {}

    [CreateAssetMenu(fileName = "AssetImporterSetting", menuName = "UExtension/Asset Importer Setting")]
    public class AssetImporterSetting : ScriptableObject
    {
        [Serializable]
        public class ImporterValue
        {
            public string Key;
            public string Value;
        }
        [Serializable]
        public class ImporterFieldSetting
        {
            public string   Contains;
            public bool     ImportSettingsMissing;

            [TypeSearch(typeof(AssetImporterTypes))]
            public string ImporterType;

            [Dropdown("ImporterTypeFields")]
            public string FieldName;
            public string FieldValue;

            public Type GetImporterType() => Type.GetType(this.ImporterType + ", UnityEditor");

            public Dictionary<string, string> ImporterTypeFields
            {
                get
                {
                    if (string.IsNullOrEmpty(this.ImporterType))
                    {
                        this.mFieldNames.Clear();
                        return this.mFieldNames;
                    }

                    if (null == this.mImporterType || this.mImporterType.FullName != this.ImporterType)
                    {
                        this.mFieldNames.Clear();
                        this.mImporterType = Type.GetType(this.ImporterType + ", UnityEditor");
                        if (null != this.mImporterType)
                        {
                            var rProperties = this.mImporterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                            foreach(var rPropInfo in rProperties)
                            {
                                if (rPropInfo.CanWrite)
                                {
                                    this.mFieldNames.Add(rPropInfo.Name, rPropInfo.Name);
                                }
                            }
                        }
                    }
                    return this.mFieldNames;
                }
            }

            protected Type                          mImporterType;
            protected Dictionary<string, string>    mFieldNames = new Dictionary<string, string>();
        }
        public ImporterFieldSetting[] ImporterFieldSettings;



        public Action OnValidateChanged;
        private void OnValidate() => this.OnValidateChanged?.Invoke();
    }
}