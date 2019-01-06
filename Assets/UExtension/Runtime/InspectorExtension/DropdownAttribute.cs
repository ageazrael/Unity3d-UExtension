using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UExtension
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DropdownAttribute : InspectorExtensionAttribute
    {
        public string                       MappingValueName;
        public Dictionary<string, object>   MappingValues;
        public bool                         PreviewValue;

        public DropdownAttribute(string rMappingValueName)
        {
            this.MappingValueName = rMappingValueName;
        }
        #region Integer
        public DropdownAttribute(string k1, int v1)
        {
            this.MappingValues = new Dictionary<string, object>() { 
                {k1, v1}
            };
        }
        public DropdownAttribute(string k1, int v1, string k2, int v2)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
            };
        }
        public DropdownAttribute(string k1, int v1, string k2, int v2, string k3, int v3)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
            };
        }
        public DropdownAttribute(string k1, int v1, string k2, int v2, string k3, int v3, string k4, int v4)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
            };
        }
        public DropdownAttribute(string k1, int v1, string k2, int v2, string k3, int v3, string k4, int v4, string k5, int v5)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
            };
        }
        #endregion

        #region string
        public DropdownAttribute(string k1, string v1)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1}
            };
        }
        public DropdownAttribute(string k1, string v1, string k2, string v2)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
            };
        }
        public DropdownAttribute(string k1, string v1, string k2, string v2, string k3, string v3)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
            };
        }
        public DropdownAttribute(string k1, string v1, string k2, string v2, string k3, string v3, string k4, string v4)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
            };
        }
        public DropdownAttribute(string k1, string v1, string k2, string v2, string k3, string v3, string k4, string v4, string k5, string v5)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
            };
        }
        #endregion 

        #region Type
        public DropdownAttribute(string k1, Type v1)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1}
            };
        }
        public DropdownAttribute(string k1, Type v1, string k2, Type v2)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
            };
        }
        public DropdownAttribute(string k1, Type v1, string k2, Type v2, string k3, Type v3)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
            };
        }
        public DropdownAttribute(string k1, Type v1, string k2, Type v2, string k3, Type v3, string k4, Type v4)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
            };
        }
        public DropdownAttribute(string k1, Type v1, string k2, Type v2, string k3, Type v3, string k4, Type v4, string k5, Type v5)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
            };
        }
        #endregion 

        #region float
        public DropdownAttribute(string k1, float v1)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1}
            };
        }
        public DropdownAttribute(string k1, float v1, string k2, float v2)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
            };
        }
        public DropdownAttribute(string k1, float v1, string k2, float v2, string k3, float v3)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
            };
        }
        public DropdownAttribute(string k1, float v1, string k2, float v2, string k3, float v3, string k4, float v4)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
            };
        }
        public DropdownAttribute(string k1, float v1, string k2, float v2, string k3, float v3, string k4, float v4, string k5, float v5)
        {
            this.MappingValues = new Dictionary<string, object>() {
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
            };
        }
        #endregion 
    }
}