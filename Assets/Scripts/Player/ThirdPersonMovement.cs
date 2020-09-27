using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThirdPersonMovement : MonoBehaviour
{
    //Character Animations
    public event Action Idle = delegate { };
    public event Action StartRunning = delegate { };
    public event Action StartSprinting = delegate { };
    public event Action StartJumping = delegate { };
    public event Action StartFalling = delegate { };
    public event Action StartLanding = delegate { };
    public event Action StartThrowing = delegate { };
    public event Action Damaged = delegate { };
    public event Action Dying = delegate { };

    [Header("Materials")]
    [SerializeField] Material flashColor;
    [SerializeField] Material normalColor;
    [SerializeField] Renderer _renderer; //needed for changing materials
    [SerializeField] float flashTime = 0.1f;
    [SerializeField] float flashDuration = 5f;

    [Header("VFX")]
    [SerializeField] GameObject damageParticle;
    [SerializeField] GameObject chargeParticle;
    [SerializeField] GameObject landParticle;

    [Header("SFX")]
    [SerializeField] AudioClip _damageSound;
    [SerializeField] AudioClip _deathSound;
    [SerializeField] AudioClip _jumpSound;
    [SerializeField] AudioClip _landingSound;
    [SerializeField] AudioClip _chargeSound;

    [Header("Camera")]
    [SerializeField] CharacterController _controller;
    [SerializeField] Transform _cam;

    [Header("Gravity Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;
    //[SerializeField] LayerMask boxMask;

    [Header("Movement")]
    private float speed;
    [SerializeField] float walkSpeed = 10f;
    [SerializeField] float sprintSpeed = 20f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float turnSmoothTime = 0.1f;

    [Header("Abilities")]
    [SerializeField] Transform cubeTarget;
    [SerializeField] float chargeTime = 0.7f;
    [SerializeField] float reloadTime = 0.3f;
    [SerializeField] PlayerAbility _playerAbility;
    [SerializeField] BoxToss projectile;

    //Movement
    Transform lookTransform;
    float turnSmoothVelocity;
    Vector3 velocity;

    //State booleans
    bool _isMoving = false;
    bool _isAirborne = true;
    bool _isDamaged = false;
    bool _isDead = false;
    bool _isCharging = false;
    bool isGrounded;
    bool reloaded = true;
    

    private void Awake()
    {
        _renderer.enabled = true;
        _renderer.sharedMaterial = normalColor;
    }

    private void Start()
    {
        Idle?.Invoke();
        lookTransform = this.transform;
        Cursor.visible = false;
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
        if(_isDead == false)
        {
            if(_isDamaged == false)
            {
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
                if (isGrounded && Input.GetButtonDown("Jump"))
                {
                    StartJumping?.Invoke();
                    AudioHelper.PlayClip2D(_jumpSound, 1f);
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    CheckIfAirborne();
                }

                //gravity update
                velocity.y += gravity * Time.deltaTime;

                _controller.Move(velocity * Time.deltaTime);


                if (isGrounded && _isMoving == false && 
                    Input.GetKeyDown(KeyCode.Mouse0) && reloaded == true)
                {
                    lookTransform.LookAt(new Vector3(cubeTarget.position.x, lookTransform.position.y, cubeTarget.position.z));
                    _playerAbility.UseMain();
                    StartThrowing?.Invoke();
                    StartCoroutine(Reload());
                    StartCoroutine(Charge());
                }
                if (isGrounded && _isMoving == false && _isCharging == true &&
                    Input.GetKey(KeyCode.Mouse0) && reloaded == true)
                {
                    lookTransform.LookAt(new Vector3(cubeTarget.position.x, lookTransform.position.y, cubeTarget.position.z));
                    Instantiate(chargeParticle, cubeTarget.position, cubeTarget.rotation);
                    StartThrowing?.Invoke();
                    //Debug.Log("Charging...");
                }

                if(isGrounded && _isMoving == false && _isCharging == true &&
                    Input.GetKeyUp(KeyCode.Mouse0))
                {
                    _isCharging = false;
                    _playerAbility.UseSub();
                    StartThrowing?.Invoke();
                    //Debug.Log("Release!");
                }
            }
        }  
    }

    IEnumerator Charge ()
    {
        _isCharging = false;
        reloaded = false;
        yield return new WaitForSeconds(chargeTime);
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _isCharging = true;
            reloaded = true;
            AudioHelper.PlayClip2D(_chargeSound, 1f);
        }
    }

    IEnumerator Reload()
    {
        reloaded = false;
        yield return new WaitForSeconds(reloadTime);
        reloaded = true;
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
            Landing();
            StartLanding?.Invoke();
            //Debug.Log("Airborne: " + _isAirborne);
        }
        else if (_isAirborne == false && _isMoving == true)
        {
            Landing();
            StartRunning?.Invoke();
        }
        else if (_isAirborne == false && _isMoving == true && Input.GetKey(KeyCode.LeftShift))
        {
            Landing();
            StartSprinting?.Invoke();
        }
        _isAirborne = true;
    }

    private void Landing ()
    {
        AudioHelper.PlayClip2D(_landingSound, 1f);
        Instantiate(landParticle, transform.position, transform.rotation);
    }

    public void CheckIfDamaged()
    {
        _isDamaged = true;
        Damaged?.Invoke();
        AudioHelper.PlayClip2D(_damageSound, 1f);
        Instantiate(damageParticle, transform.position, transform.rotation);
        StartCoroutine(Flash());
        StartCoroutine(Stun());
    }

    IEnumerator Flash ()
    {
        for (int i = 0; i < flashDuration; i++)
        {
            _renderer.material = flashColor;
            yield return new WaitForSeconds(flashTime);
            _renderer.material = normalColor;
            yield return new WaitForSeconds(flashTime);

        }
    }

    IEnumerator Stun ()
    {
        for (int i = 0; i < 2; i++)
        {
            transform.Translate(Vector3.back * Time.deltaTime * 5);
            yield return new WaitForSeconds(1f);
            _isDamaged = false;
        }
    }

    public void CheckIfDead()
    {
        _isDead = true;
        AudioHelper.PlayClip2D(_deathSound, 1f);
        Dying?.Invoke();
        //Debug.Log("Dead");
    }
}
