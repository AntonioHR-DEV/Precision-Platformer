using System;
using UnityEngine;

public class StartCheckpoint : MonoBehaviour
{
    private static readonly int MOVE_TRIGGER_HASH = Animator.StringToHash("Move");

    public static event EventHandler OnStarted;

    [SerializeField] private Animator animator;
    private bool isActivated = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActivated && collision.gameObject.TryGetComponent<PlayerController>(out _))
        {
            isActivated = true;
            animator.SetTrigger(MOVE_TRIGGER_HASH);
            OnStarted?.Invoke(this, EventArgs.Empty);
        }
    }
}
