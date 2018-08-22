using System.Collections.Generic;
using UnityEngine;
using System;

namespace UExtension
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeSearchAttribute : PropertyAttribute
    {
        public Type TypeSearchType;

        public TypeSearchAttribute(Type rTypeSearch)
        {
            this.TypeSearchType = rTypeSearch;
        }

        public List<string> TypeFullNames => TypeSearchBase.GetTypeFullNames(TypeSearchType);
    }
}