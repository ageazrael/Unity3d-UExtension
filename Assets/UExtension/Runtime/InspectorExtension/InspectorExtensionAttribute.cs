using UnityEngine;
using System;

namespace UExtension
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InspectorExtensionAttribute : PropertyAttribute
    {
        public string   IsEnableControllerValue;
        public string   IsVisibleControllerValue;

        public bool     Readonly = false;
    }
}