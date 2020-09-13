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
    public event Action StartThrowing = delegate { };

    [SerializeField] CharacterController _controller;
    [SerializeField] Transform _cam;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask boxMask;

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

    Transform lookTransform;
    [SerializeField] Transform cubeTarget;

    private void Start()
    {
        Idle?.Invoke();
        lookTransform = this.transform;
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
        } else
        {
            CheckIfAirborne();
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
            CheckIfAirborne();
        }

        //gravity update
        velocity.y += gravity * Time.deltaTime;

        _controller.Move(velocity * Time.deltaTime);

        //look at cube when summoned
        if(isGrounded && _isMoving == false && Input.GetKeyDown(KeyCode.Mouse0))
        {
            lookTransform.LookAt(new Vector3(cubeTarget.position.x, lookTransform.position.y, cubeTarget.position.z));
            StartThrowing?.Invoke();
        }
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
            speed = sprintSpeed;
        }

        if(isGrounded == true && Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartSprinting?.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StartRunning.Invoke();
        }
    }

    private void CheckIfAirborne()
    {
        if(_isAirborne == true)
        {
            StartFalling?.Invoke();
            //Debug.Log("Airborne: " + _isAirborne);
        }
        _isAirborne = false;
    }

    private void CheckIfGrounded()
    {
        if (_isAirborne == false && _isMoving == false)
        {
			StartLanding?.Invoke();
            //Debug.Log("Airborne: " + _isAirborne);
        }
        if (_isAirborne == false && _isMoving == true)
        {
            StartRunning?.Invoke();
        }
        if (_isAirborne == false && _isMoving == true && Input.GetKey(KeyCode.LeftShift))
        {
            StartSprinting?.Invoke();
        }
        _isAirborne = true;
    }
}
