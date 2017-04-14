using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UExtension
{
    public enum SingletonSettingType
    {
        Editor,
        Runtime,
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonSettingAttribute : Attribute
    {
        public string               AssetPath;
        public SingletonSettingType SettingType;

        public SingletonSettingAttribute(SingletonSettingType rSettingType, string rAssetPath)
        {
            this.AssetPath      = rAssetPath;
            this.SettingType    = rSettingType;
        }
    }

    [TSIgnore]
    public class SingletonSetting : ScriptableObject
    {
        public static T Load<T>()
            where T : SingletonSetting
        {
            var rAttribute = typeof(T).GetCustomAttribute<SingletonSettingAttribute>(false);
            if (null == rAttribute)
            {
                Debug.LogErrorFormat("{0} need add SingletonSettingAttribute attribute.");
                return default(T);
            }

            if (rAttribute.SettingType == SingletonSettingType.Editor)
                return LoadEditor(typeof(T), rAttribute) as T;
            else if (rAttribute.SettingType == SingletonSettingType.Runtime)
                return LoadRuntime(typeof(T), rAttribute) as T;

            return default(T);
        }

        protected static ScriptableObject LoadRuntime(Type rType, SingletonSettingAttribute rAttribute)
        {
            var ResourcesFolderName = "Resources";
            var nFindIndex = rAttribute.AssetPath.LastIndexOf(ResourcesFolderName);
            if (-1 == nFindIndex)
            {
                rAttribute.AssetPath.Contains("");
                Debug.LogErrorFormat("{0} SingletonSettingAttribute.AssetPath need in `Resources` folder!", rType.FullName);
                return default(ScriptableObject);
            }
            var rAssetResourcePath = rAttribute.AssetPath.Substring(nFindIndex + ResourcesFolderName.Length);

            return Resources.Load(PathExtension.GetPathWithoutExtension(rAssetResourcePath), rType) as ScriptableObject;
        }
        protected static ScriptableObject LoadEditor(Type rType, SingletonSettingAttribute rAttribute)
        {
            #if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath(rAttribute.AssetPath, rType) as ScriptableObject;
            #else
            return default(ScriptableObject);
            #endif
        }

        public virtual void OnDestroy() { }
    }
    public class SingletonSettingTypes : TypeSearchDefault<SingletonSetting>{ }

    [TSIgnore]
    public class TSingletonSetting<T> : SingletonSetting
        where T : TSingletonSetting<T>
    {
        public static T Instance
        {
            get
            {
                if (null == GInstance)
                    return CreateInstance();
                return GInstance;
            }
        }

        public static T CreateInstance()
        {
            if (null == GInstance)
            {
                lock (GInstanceLock)
                {
                    if (null == GInstance)
                        GInstance = Load<T>();
                }
            }
            return GInstance;
        }
        public static void DestroyInstance()
        {
            if (null != GInstance)
            {
                lock (GInstanceLock)
                {
                    if (null != GInstance)
                    {
                        GInstance.OnDestroy();
                        GInstance = default(T);
                    }
                }
            }
        }

        #region protected Field
        protected static T      GInstance;
        protected static object GInstanceLock = new object();
        #endregion
    }
}