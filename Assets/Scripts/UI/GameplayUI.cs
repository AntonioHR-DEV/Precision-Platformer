using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        timerText.text = LevelTimer.Instance.GetFormattedTime();
    }
}
