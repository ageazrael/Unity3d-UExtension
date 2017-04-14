using UnityEngine;
using System.Collections;
using System;

namespace UExtension
{
    public static class Debugger
    {
        public static bool Assert(bool bCondition, string format, params object[] args)
        {
            if (!bCondition)
                Debug.LogError(string.Format(null, format, args));

            return bCondition;
        }
        public static void AssertE(bool bCondition, string format, params object[] args)
        {
            if (!bCondition)
                throw new UnityException(string.Format(null, format, args));
        }
        public static void ValidValueE(string value, string format, params object[] args)
        {
            if(string.IsNullOrEmpty(value))
                throw new UnityException(string.Format(null, format, args));
        }
        public static void ValidValueE<T>(T value, string format, params object[] args)
            where T : class
        {
            if(null == value)
                throw new UnityException(string.Format(null, format, args));
        }
    }
}