using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoint : MonoBehaviour
{
    public event EventHandler OnActivated;

    public bool IsActive { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsActive) return;
        if (!other.TryGetComponent(out PlayerController _)) return;

        CheckpointManager.Instance.SetLastCheckpoint(this);
    }

    /// <summary>Called by CheckpointManager to activate this checkpoint.</summary>
    public void Activate()
    {
        IsActive = true;
        OnActivated?.Invoke(this, EventArgs.Empty);
    }
}