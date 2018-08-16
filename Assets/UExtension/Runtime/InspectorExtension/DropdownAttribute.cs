using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UExtension
{
    public struct AttributeValue
    {
        public string Key;
        public object Value;

        public AttributeValue(string inKey, object inValue)
        {
            this.Key = inKey;
            this.Value = inValue;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class DropdownAttribute : PropertyAttribute
    {
        public string MappingValueName;

        public DropdownAttribute(string rMappingValueName)
        {
            this.MappingValueName = rMappingValueName;
        }
    }
}