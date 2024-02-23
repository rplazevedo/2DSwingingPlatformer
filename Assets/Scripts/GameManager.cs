using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int currentScore = 0;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
        GameUI.instance.UpdateScoreText(currentScore);
    }

    public void AddScore(int score)
    {
        currentScore += score;
        GameUI.instance.UpdateScoreText(currentScore);
    }
}
