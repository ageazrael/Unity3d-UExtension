using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace UExtension.Example
{
    public class UIChild : MonoBehaviour
    {
        [Serializable]
        public class ChildInfo
        {
            public string       ChildType;
            public string       ChildPath;
            public UIBehaviour  UIElement;

            public Type         ChildRealType
            {
                get
                {
                    if (null == mChildRealType)
                        mChildRealType = Type.GetType(this.ChildType);
                    return mChildRealType;
                }
            }

            protected Type      mChildRealType;
        }

        public List<ChildInfo> ChildCache;

        public T Get<T>(string rPath, bool bOnlyCache = true)
            where T : UIBehaviour
        {
            if (null != this.ChildCache)
            {
                foreach (var rChildInfo in this.ChildCache)
                {
                    if (rChildInfo.ChildPath == rPath && rChildInfo.ChildRealType == typeof(T))
                        return (T)rChildInfo.UIElement;
                }
            }
            if (!bOnlyCache)
            {
                foreach(var rChild in this.GetComponentsInChildren<T>())
                {
                    if (rChild.transform.GetPath(this.transform) == rPath)
                        return rChild;
                }
            }
            return default(T);
        }

        [ContextMenu("Build ChildCache")]
        public void BuildChildCache()
        {
            if (null == this.ChildCache)
                this.ChildCache = new List<ChildInfo>();
            this.ChildCache.Clear();

            foreach (var rChild in this.GetComponentsInChildren<UIBehaviour>())
            {
                this.ChildCache.Add(new ChildInfo() {
                    ChildType = rChild.GetType().AssemblyQualifiedName,
                    ChildPath = rChild.transform.GetPath(this.transform),
                    UIElement = rChild
                });
            }
        }
    }
}