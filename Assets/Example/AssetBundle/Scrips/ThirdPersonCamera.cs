using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UExtension.Example
{
    [RequireComponent(typeof(Camera))]
    public class ThirdPersonCamera : MonoBehaviour
    {
        public Transform TrackTarget;

        public float Distance = 5;
        public float AxisX = 43;
        public float AxisY = 0;

        // Update is called once per frame
        void Update()
        {
            if (!this.TrackTarget)
                return;

            var rRotate = Quaternion.AngleAxis(this.AxisY, Vector3.up) *
                Quaternion.AngleAxis(this.AxisX, Vector3.right);

            this.transform.rotation = rRotate;
            this.transform.position = rRotate * (Vector3.back * this.Distance) + this.TrackTarget.position;
        }
    }
}