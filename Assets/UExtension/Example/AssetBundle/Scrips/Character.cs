using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UExtension.Example
{
    [RequireComponent(typeof(CharacterController))]
    public class Character : MonoBehaviour
    {
        public float Speed = 6.0F;
        public float JumpSpeed = 8.0F;


        protected CharacterController Controller;
        protected Camera Camera;

        protected Vector3 MoveDirection = Vector3.zero;
        void Start()
        {
            this.Controller = this.GetComponent<CharacterController>();
        }

        void Update()
        {
            if (this.Controller.isGrounded)
            {
                this.MoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                this.MoveDirection = transform.TransformDirection(this.MoveDirection);
                this.MoveDirection *= this.Speed;
                if (Input.GetButton("Jump"))
                    this.MoveDirection.y = this.JumpSpeed;

            }
            this.MoveDirection += Physics.gravity * Time.deltaTime;
            this.Controller.Move(this.MoveDirection * Time.deltaTime);
        }
    }
}