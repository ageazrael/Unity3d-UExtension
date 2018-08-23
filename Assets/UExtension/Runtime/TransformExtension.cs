using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UExtension
{
    public static class TransformExtension
    {
        public static string GetPath(this Transform rTransform, Transform rEndParent = null)
        {
            if (rTransform.parent != rEndParent)
                return GetPath(rTransform.parent, rEndParent) + "." + rTransform.name;
            else
                return rTransform.name;
        }
    }
}