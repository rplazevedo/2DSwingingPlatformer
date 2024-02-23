using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI scoreText;

    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }
    public void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
