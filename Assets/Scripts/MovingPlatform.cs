using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private Transform pointATransform;
    [SerializeField] private Transform pointBTransform;
    private Vector3 pointA;
    private Vector3 pointB;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float pauseDuration = 0.5f;

    // ── State ─────────────────────────────────────────────────────────────────

    private Vector3 target;
    private bool isPaused;

    // ── Public readonly properties ────────────────────────────────────────────
    public bool IsMoving => !isPaused;
    public Vector3 PointA => pointATransform.position;
    public Vector3 PointB => pointBTransform.position;

    // =========================================================================
    // Unity Lifecycle
    // =========================================================================

    private void Start()
    {
        pointA = pointATransform.position;
        pointB = pointBTransform.position;

        // Start at Point A heading toward Point B
        transform.position = pointA;
        target = pointB;
    }

    private void Update()
    {
        if (isPaused) return;

        // Move toward the current target at constant speed
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Reached the target — pause then swap
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            StartCoroutine(PauseAtEndpoint());
        }
    }

    private IEnumerator PauseAtEndpoint()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseDuration);

        // Swap target
        target = target == pointA ? pointB : pointA;
        isPaused = false;
    }

    // =========================================================================
    // Player Sticking
    // Parent the player to the platform on contact so they inherit movement.
    // Unparent when they leave so normal physics resumes.
    // =========================================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out PlayerController _)) return;

        // Only stick if player is landing on top, not hitting the side
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                collision.transform.SetParent(transform);
                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out PlayerController _)) return;

        collision.transform.SetParent(null);
    }

    // =========================================================================
    // Gizmos — visualize the two points in the Scene view
    // =========================================================================

    private void OnDrawGizmos()
    {
        if (pointATransform == null || pointBTransform == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pointATransform.position, 0.15f);
        Gizmos.DrawSphere(pointBTransform.position, 0.15f);
        Gizmos.DrawLine(pointATransform.position, pointBTransform.position);
    }
}