using UnityEngine;
using System;

namespace UExtension
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MinMaxAttribute : InspectorExtensionAttribute
    {
        public float MinValue;
        public float MaxValue;
        public bool IsSlider;
        public MinMaxAttribute(float min, float max)
        {
            this.MinValue = min;
            this.MaxValue = max;
        }
        public MinMaxAttribute(int min, int max)
        {
            this.MinValue = (float)min;
            this.MaxValue = (float)max;
        }
    }
}