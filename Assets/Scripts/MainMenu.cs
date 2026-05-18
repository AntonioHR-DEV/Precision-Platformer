using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to: SceneManager GameObject in the MainMenu scene.
/// Requires:  SceneLoader singleton present in scene.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    // Settings panel — assign in inspector, implement later
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        // Make sure settings panel is hidden on start
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void OnPlayClicked()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.Scene.LevelSelect);
    }

    private void OnSettingsClicked()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}