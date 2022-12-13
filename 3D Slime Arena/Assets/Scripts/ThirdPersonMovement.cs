using System.Collections;

using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor.SceneManagement
{


    public class ThirdPersonMovement : MonoBehaviour
    {
        public CharacterController controller;
        public Transform cam;

        bool colliding = false;
        public float speed = 6;
        public float gravity = -9.81f;
        public float jumpHeight = 3;
        Vector3 velocity;
        bool isGrounded;

        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;

        Animator anim;

        float turnSmoothVelocity;
        public float turnSmoothTime = 0.1f;

        void Start()
        {
            anim = GetComponent<Animator>();
        }
        // Update is called once per frame
        void Update()
        {
            //jump
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            //gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            //walk
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle - 90, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }

            if (Input.GetMouseButtonDown(0))
            {
                anim.SetInteger("attack", 1);
                if (colliding == true)
                {
                    SceneManager.LoadScene(3);
                }

            }
            else
            {
                anim.SetInteger("attack", 0);
            }


        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "ToArena")
            {
                SceneManager.LoadScene(2);
            }
            if (collision.gameObject.tag == "Enemy")
            {
                colliding = true;
            }
        }
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                colliding = false;
            }
        }
    }
}
