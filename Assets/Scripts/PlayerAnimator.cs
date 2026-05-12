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
///
/// Death is handled via a trigger parameter "Hit" so it interrupts any state.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int StateParam = Animator.StringToHash("State");
    private static readonly int HitTrigger = Animator.StringToHash("Hit");

    private Animator animator;

    // =========================================================================
    // Unity Lifecycle
    // =========================================================================

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PlayerController.Instance.OnDied += PlayerController_OnDied;
        PlayerController.Instance.OnRespawned += PlayerController_OnRespawned;
    }

    private void Update()
    {
        if (PlayerController.Instance.IsDead) return;

        if (animator.GetInteger(StateParam) != (int)PlayerController.Instance.CurrentPlayerState)
        {
            animator.SetInteger(StateParam, (int)PlayerController.Instance.CurrentPlayerState);
        }
        
    }

    private void OnDestroy()
    {
        PlayerController.Instance.OnDied -= PlayerController_OnDied;
        PlayerController.Instance.OnRespawned -= PlayerController_OnRespawned;
    }

    // =========================================================================
    // Death / Respawn
    // =========================================================================

    private void PlayerController_OnDied(object sender, EventArgs e)
    {
        // Trigger interrupts any current state and plays the Hit clip
        animator.SetTrigger(HitTrigger);
    }

    private void PlayerController_OnRespawned(object sender, EventArgs e)
    {
        // Reset trigger in case it was queued but not consumed
        animator.ResetTrigger(HitTrigger);

        // Force back to Idle so the animator doesn't resume mid-death-clip
        animator.SetInteger(StateParam, (int)PlayerController.PlayerState.Idle);
    }
}