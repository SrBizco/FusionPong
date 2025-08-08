using Fusion;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class ScoreManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    [Networked] public int LeftScore { get; set; }
    [Networked] public int RightScore { get; set; }

    private int _prevL, _prevR;

    public override void Spawned()
    {
        _prevL = LeftScore;
        _prevR = RightScore;
        UpdateUI();
    }

    public override void Render()
    {
        if (_prevL != LeftScore || _prevR != RightScore)
        {
            _prevL = LeftScore;
            _prevR = RightScore;
            UpdateUI();
        }
    }

    public void AddLeft()
    {
        if (!Object.HasStateAuthority) return;
        LeftScore++;
    }

    public void AddRight()
    {
        if (!Object.HasStateAuthority) return;
        RightScore++;
    }

    public void ResetScores()
    {
        if (!Object.HasStateAuthority) return;
        LeftScore = 0;
        RightScore = 0;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"{LeftScore} — {RightScore}";
    }
}
