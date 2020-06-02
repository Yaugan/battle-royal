using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /* public CharacterController controller;

     public float speed = 13f;
     public float gravity = -9.8f;
     public float jumpHeight = 3f;

     public Transform groundCheck;
     public float groundDistance = 0.4f;
     public LayerMask groundMask;

     Vector3 velocity;
     bool isGrounded;

     // Update is called once per frame
     void Update()
     {
         isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

         if (isGrounded && velocity.y < 0)
         {
             velocity.y = -2f;
         }

         float x = SimpleInput.GetAxis("Horizontal");
         float z = SimpleInput.GetAxis("Vertical");

         Vector3 move = transform.right * x + transform.forward * z;

         controller.Move(move * speed * Time.deltaTime);

         if (Input.GetButtonDown("Jump") && isGrounded)
         {
             velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
         }

         velocity.y += gravity * Time.deltaTime;

         controller.Move(velocity * Time.deltaTime);

     }*/


    public Transform cam;
    public CharacterController characterController;
    public float turnSmoothTime = 0.1f;



    [SerializeField]
    private float speed = 6f;

    private float turnSmoothVelocity;

    private void Start()
    {

    }
    private void Update()
    {
        float horizontal = SimpleInput.GetAxisRaw("Horizontal");
        float vertical = SimpleInput.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }
    }
}
