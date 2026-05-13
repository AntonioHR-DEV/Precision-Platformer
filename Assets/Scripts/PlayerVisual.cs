using UnityEngine;
using System;

/// <summary>
/// Reads PlayerController's public state every frame and drives the Animator.
/// No logic lives here — this is purely a translator between controller state
/// and animation state.
///
/// Requires:  Animator component with an int parameter named "State"
///
/// Animator "State" parameter values match PlayerController.PlayerState enum:
///   0 = Idle
///   1 = Running
///   2 = Jumping
///   3 = DoubleJumping
///   4 = Falling
///   5 = WallSliding
///   6 = Dead
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerVisual : MonoBehaviour
{
    private static readonly int STATE_PARAM_HASH = Animator.StringToHash("State");
    private static readonly int DOUBLE_JUMP_ANIMATION_HASH = Animator.StringToHash("DoubleJump");

    private Animator animator;
    private AnimatorStateInfo animatorStateInfo;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PlayerController.Instance.OnRespawned += PlayerController_OnRespawned;
    }

    private void Update()
    {
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!PlayerController.Instance.IsDead && animatorStateInfo.shortNameHash == DOUBLE_JUMP_ANIMATION_HASH && animatorStateInfo.normalizedTime < 1.0f)
        {
            // In DoubleJump animation, skipping state update to avoid interrupting the animation.
            return;
        }

        if (animator.GetInteger(STATE_PARAM_HASH) != (int)PlayerController.Instance.CurrentPlayerState)
        {
            animator.SetInteger(STATE_PARAM_HASH, (int)PlayerController.Instance.CurrentPlayerState);
        }
    }

    private void OnDestroy()
    {
        PlayerController.Instance.OnRespawned -= PlayerController_OnRespawned;
    }

    private void PlayerController_OnRespawned(object sender, EventArgs e)
    {
        // Force back to Idle so the animator doesn't resume mid-death-clip
        animator.SetInteger(STATE_PARAM_HASH, (int)PlayerController.PlayerState.Idle);
    }
}