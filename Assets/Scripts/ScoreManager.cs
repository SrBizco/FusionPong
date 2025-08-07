using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private int leftScore, rightScore;

    private void Start()
    {
        UpdateUI();
    }

    public void AddLeft()
    {
        leftScore++;
        UpdateUI();
    }

    public void AddRight()
    {
        rightScore++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"{leftScore} — {rightScore}";
    }
}
