using UnityEngine;
using System;

namespace UExtension
{
    using Object = UnityEngine.Object;

    [System.Serializable]
    public class AssetRefBase
    {
		public string       AssetPath;
        public string       AssetGUID;
        public string       AssetTypeFullName;

        [SerializeField]
        protected   int     LoadedAssetInstanceID;  // Editor Debugger Value
        private     Type    mAssetType;

        public Type AssetType
        {
            get {
                if (null == mAssetType)
                    mAssetType = Type.GetType(this.AssetTypeFullName);

                return mAssetType;
            }
        }
    }

}