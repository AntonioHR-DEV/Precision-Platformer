using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler OnJumpStarted;

    private InputActions inputActions;

    // -- Properties -----------------------------------------------------------
    public Vector2 MoveInput { get; private set; }

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

        inputActions.Player.Enable();
    }

    private void Update()
    {
        // Poll move each frame so MoveInput is always current
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // Reload active scene when R is pressed (for testing)
        if (UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnDestroy()
    {
        inputActions.Player.Disable();
        inputActions.Dispose();
    }
}