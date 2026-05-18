using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenshotTaker : MonoBehaviour
{
    public static ScreenshotTaker Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private Key screenshotKey = Key.F12;
    [SerializeField] private int superSize = 1; // 1 = normal, 2 = 2x resolution, 4 = 4x

    private string screenshotFolder;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        screenshotFolder = Path.Combine(Application.dataPath, "../Screenshots");

        if (!Directory.Exists(screenshotFolder))
            Directory.CreateDirectory(screenshotFolder);

        Debug.Log($"Screenshots will be saved to: {Path.GetFullPath(screenshotFolder)}");
    }

    private void Update()
    {
        if (Keyboard.current[screenshotKey].wasPressedThisFrame)
            TakeScreenshot();
    }

    private void TakeScreenshot()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"Screenshot_{timestamp}.png";
        string fullPath = Path.Combine(screenshotFolder, fileName);

        ScreenCapture.CaptureScreenshot(fullPath, superSize);
        Debug.Log($"Screenshot saved: {fullPath}");
    }
}