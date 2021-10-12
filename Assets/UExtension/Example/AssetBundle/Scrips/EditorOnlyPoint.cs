using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UExtension.Example
{
    public class EditorOnlyPoint : MonoBehaviour
    {
        public Color SphereColor;
        public float Size;

        void OnDrawGizmos()
        {
            Gizmos.color = this.SphereColor;
            Gizmos.DrawSphere(transform.position, this.Size);
        }
    }
}