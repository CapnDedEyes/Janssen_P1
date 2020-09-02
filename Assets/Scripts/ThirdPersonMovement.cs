using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThirdPersonMovement : MonoBehaviour
{
    public event Action Idle = delegate { };
    public event Action StartRunning = delegate { };
    public event Action StartSprinting = delegate { };
    public event Action StartJumping = delegate { };
    public event Action StartFalling = delegate { };
    public event Action StartLanding = delegate { };

    [SerializeField] CharacterController _controller;
    [SerializeField] Transform _cam;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    private float speed;

    [SerializeField] float walkSpeed = 10f;
    [SerializeField] float sprintSpeed = 20f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 3f;

    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    bool _isMoving = false;
    bool _isAirborne = true;

    Vector3 velocity;
    bool isGrounded;

    private void Start()
    {
        Idle?.Invoke();
    }

    // Update is called once per frame
    private void Update()
    {
        //grounded check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Debug.Log("Ground Distance: " + velocity.y);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            CheckIfGrounded();
        }

        //basic movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        //running player update
        if (direction.magnitude >= 0.1f)
        {
            speed = walkSpeed;

            CheckIfStartedMoving();
            CheckIfSprinting();
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle
                (transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        else
        {
            CheckIfStoppedMoving();
        }

        //jump input
        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            StartJumping?.Invoke();
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            //Fall animation never functions properly due to the presence of other animations
            CheckIfAirborne();
        }

        //gravity update
        velocity.y += gravity * Time.deltaTime;

        _controller.Move(velocity * Time.deltaTime);
    }

    private void CheckIfStartedMoving()
    {
        if (_isMoving == false && isGrounded == true)
        {
            //our velocity says we're moving but we previously were not
            //this means we've started moving!
            StartRunning?.Invoke();
            //Debug.Log("Started");
        }
        _isMoving = true;
    }

    private void CheckIfStoppedMoving()
    {
        if (_isMoving == true && isGrounded == true)
        {
            //our veloctiy says we're not moving but we previously were
            //this means we've stopped!
            Idle?.Invoke();
            //Debug.Log("Stopped");
        }
        _isMoving = false;
    }

    private void CheckIfSprinting()
    {
        if(isGrounded == true && Input.GetKey(KeyCode.LeftShift))
        {
            //Recently added since I realized I needed a sprint animation
            speed = sprintSpeed;
        } 
        if(speed == sprintSpeed)
        {
            StartSprinting?.Invoke();
        }
        else
        {
            CheckIfStartedMoving();
        }
    }

    private void CheckIfAirborne()
    {
        if(_isAirborne == true)
        {
            StartFalling?.Invoke();
            Debug.Log("Airborne: " + _isAirborne);
        }
        _isAirborne = false;
    }

    private void CheckIfGrounded()
    {
        if (_isAirborne == false)
        {
            //Recently added a landing animation!
			//Landing animation kind of works since it's called on line 53
            //but Landing cannot transition to Idle or Running
			StartLanding?.Invoke();
            Debug.Log("Airborne: " + _isAirborne);
            //CheckIfStoppedMoving();
        }
        _isAirborne = true;

    }
}
//If all else fails, I'm willing to utilize Mechanim to integrate all remaining animations
//I wanted to try hard coding everything if possible cuz I was told it may be easier to debug
//I apologize for the mess between lines  84 & 159

//known issues: 
//Jumping, Falling, and Sprinting animations never play properly
//Landing cannot transition to Idle
//Landing or Running can transition from Falling, but Falling currently doesn't work
