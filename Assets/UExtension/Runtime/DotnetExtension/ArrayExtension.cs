using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UExtension
{
    public static class ArrayExtension
    {
        public static bool IsValid<T>(this T[] rArray, int nIndex) => rArray.Length > nIndex && 0 <= nIndex;
    }

}