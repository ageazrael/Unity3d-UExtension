using UnityEngine;
using System;

namespace UExtension
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadonlyAttribute : PropertyAttribute { }
}