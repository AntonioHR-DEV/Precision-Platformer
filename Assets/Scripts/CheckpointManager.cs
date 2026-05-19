using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Tooltip("The position the player spawns at when no checkpoint has been activated yet.")]
    [SerializeField] private Transform levelStartPosition;

    private Checkpoint lastCheckpoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Called by a Checkpoint when the player touches it.
    /// Deactivates the previous checkpoint and activates the new one.
    /// </summary>
    public void SetLastCheckpoint(Checkpoint checkpoint)
    {
        if (lastCheckpoint == checkpoint) return;

        lastCheckpoint = checkpoint;
        lastCheckpoint.Activate();
    }

    /// <summary>
    /// Returns the respawn position — active checkpoint if one exists,
    /// otherwise the level start position.
    /// </summary>
    public Vector3 GetRespawnPosition()
    {
        if (lastCheckpoint != null)
            return lastCheckpoint.transform.position;

        if (levelStartPosition != null)
            return levelStartPosition.position;

        Debug.LogWarning("CheckpointManager: No active checkpoint and no level start position set.");
        return Vector3.zero;
    }

    /// <summary>Resets lastCheckpoint — call this when restarting the level.</summary>
    public void ResetLastCheckpoint()
    {
        lastCheckpoint = null;
    }
}