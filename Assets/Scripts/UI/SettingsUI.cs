using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : BasePanel
{
    public static SettingsUI Instance { get; private set; }
    
    [Header("References")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button musicVolumeButton;
    [SerializeField] private Button sfxVolumeButton;
    [SerializeField] private Button moveLeftRebindButton;
    [SerializeField] private Button moveRightRebindButton;
    [SerializeField] private Button fastFallRebindButton;
    [SerializeField] private Button jumpRebindButton;
    [SerializeField] private Button pauseRebindButton;

    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);

        SnapHidden();
    }

    public override void Show(System.Action onClose = null)
    {
        base.Show(onClose);

        if (GameInput.Instance != null && PlayerController.Instance != null)
        {
            GameInput.Instance.InputActions.Player.Disable();
        }
    }

    public override void Hide()
    {
        if (GameInput.Instance != null && PlayerController.Instance != null)
        {
            GameInput.Instance.InputActions.Player.Enable();
        }

        base.Hide();
    }
}
