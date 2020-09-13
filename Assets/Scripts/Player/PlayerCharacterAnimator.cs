using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCharacterAnimator : MonoBehaviour
{
    [SerializeField] ThirdPersonMovement _thirdPersonMovement = null;
    private float transitionDuration = .2f;

    //these names align with the naming in our Animator node
    const string IdleState = "Idle";
    const string RunState = "Run";
    const string SprintState = "Sprint";
    const string JumpState = "Jumping";
    const string FallState = "Falling";
    const string LandState = "Landing";
    const string ThrowState = "Throwing";

    Animator _animator = null;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnIdle()
    {
        _animator.CrossFadeInFixedTime(IdleState, transitionDuration);
    }

    private void OnStartRunning()
    {
        _animator.CrossFadeInFixedTime(RunState, transitionDuration);
    }

    private void OnStartSprinting()
    {
        _animator.CrossFadeInFixedTime(SprintState, transitionDuration);
    }

    private void OnStartJumping()
    {
        _animator.CrossFadeInFixedTime(JumpState, transitionDuration);
    }

    private void OnStartFalling()
    {
        _animator.CrossFadeInFixedTime(FallState, transitionDuration);
    }

    private void OnStartLanding()
    {
        _animator.CrossFadeInFixedTime(LandState, transitionDuration);
    }

    private void OnStartThrowing()
    {
        _animator.CrossFadeInFixedTime(ThrowState, transitionDuration);
    }

    private void OnEnable()
    {
        _thirdPersonMovement.Idle += OnIdle;
        _thirdPersonMovement.StartRunning += OnStartRunning;
        _thirdPersonMovement.StartSprinting += OnStartSprinting;
        _thirdPersonMovement.StartJumping += OnStartJumping;
        _thirdPersonMovement.StartFalling += OnStartFalling;
        _thirdPersonMovement.StartLanding += OnStartLanding;
        _thirdPersonMovement.StartThrowing += OnStartThrowing;
    }

    private void OnDisable()
    {
        _thirdPersonMovement.Idle -= OnIdle;
        _thirdPersonMovement.StartRunning -= OnStartRunning;
        _thirdPersonMovement.StartSprinting -= OnStartSprinting;
        _thirdPersonMovement.StartJumping -= OnStartJumping;
        _thirdPersonMovement.StartFalling -= OnStartFalling;
        _thirdPersonMovement.StartLanding -= OnStartLanding;
        _thirdPersonMovement.StartThrowing -= OnStartThrowing;
    }
}
