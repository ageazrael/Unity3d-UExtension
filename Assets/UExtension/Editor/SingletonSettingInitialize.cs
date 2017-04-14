using UnityEngine;
using System.Collections.Generic;
using System.IO;

using UnityEditor;

namespace UExtension
{
    [InitializeOnLoad]
    public class SingletonSettingInitialize
    {
        static SingletonSettingInitialize()
        {
            foreach (var rType in SingletonSettingTypes.Types)
            {
                var rAttribute = rType.GetCustomAttribute<SingletonSettingAttribute>(false);
                if (null == rAttribute)
                {
                    Debug.LogErrorFormat("{0} need add SingletonSettingAttribute attribute.", rType.FullName);
                    continue;
                }


                if (!File.Exists(rAttribute.AssetPath))
                {
                    var rAssetPath = Path.GetDirectoryName(rAttribute.AssetPath);
                    if (!Directory.Exists(rAssetPath))
                        Directory.CreateDirectory(rAssetPath);

                    var rAssetObject = ScriptableObject.CreateInstance(rType);
                    AssetDatabase.CreateAsset(rAssetObject, rAttribute.AssetPath);
                }
            }
        }
    }
}