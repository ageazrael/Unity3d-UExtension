﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UExtension
{
    /// <summary>
    /// 使运行时任何地方都能使用携程
    /// </summary>
    public static class CoroutineManager
    {
        public static Coroutine Start(IEnumerator rEnumerator)
        {
            if (!GCoroutineObject)
            {
                var go = new GameObject("CoroutineObject") { hideFlags = HideFlags.HideAndDontSave };
                GCoroutineObject = go.AddComponent<CoroutineObject>();
            }

            return GCoroutineObject?.StartCoroutine(rEnumerator);
        }
        public static void Stop(Coroutine rCoroutine) => GCoroutineObject?.StopCoroutine(rCoroutine);
        
        private static CoroutineObject GCoroutineObject;
    }

    public class CoroutineHandle
    {
        public CoroutineHandle(IEnumerator rEnumerator)
        {
            mCoroutine = CoroutineManager.Start(rEnumerator);
        }
        public void Stop()
        {
            if (null != mCoroutine)
                CoroutineManager.Stop(mCoroutine);
        }
        public Coroutine WaitCoroutine => this.mCoroutine;

        Coroutine mCoroutine;
    }
}