using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler OnJumpStarted;
    public event EventHandler OnPauseStarted;

    private InputActions inputActions;

    // -- Properties -----------------------------------------------------------
    public Vector2 MoveInput { get; private set; }
    public InputActions InputActions => inputActions;

    // =========================================================================
    // Unity Lifecycle
    // =========================================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        inputActions = new InputActions();

        inputActions.Player.Jump.started += _ => OnJumpStarted?.Invoke(this, EventArgs.Empty);
        inputActions.Player.Pause.started += _ => OnPauseStarted?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        // Poll move each frame so MoveInput is always current
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();
    }

    private void OnDestroy()
    {
        inputActions.Player.Disable();
        inputActions.Dispose();
    }

    /// <summary>
    /// Call this from a Level Manager or Player Controller when a gameplay level loads.
    /// </summary>
    public void EnablePlayerInput()
    {
        inputActions?.Player.Enable();
    }

    public void DisablePlayerInput()
    {
        inputActions?.Player.Disable();
    }
}