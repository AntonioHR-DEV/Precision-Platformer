using UnityEngine;

public class EndCheckpoint : MonoBehaviour
{
    private static readonly int PRESS_TRIGGER_HASH = Animator.StringToHash("Press");

    [SerializeField] private Animator animator;
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivated && collision.gameObject.TryGetComponent<PlayerController>(out _))
        {
            isActivated = true;
            animator.SetTrigger(PRESS_TRIGGER_HASH);
        }
    }
}
