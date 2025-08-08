using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class MatchTimer : NetworkBehaviour
{
    [Header("Config")]
    [SerializeField] private int matchSeconds = 180;

    [Header("UI (cliente)")]
    [SerializeField] private TMP_Text timerText;    
    [SerializeField] private GameObject endPanel;    
    [SerializeField] private TMP_Text endText;       

    [Networked] public int SecondsLeft { get; set; }
    [Networked] public NetworkBool Running { get; set; }
    [Networked] public NetworkBool Finished { get; set; }

    private float _accum;

    public override void Spawned()
    {
        UpdateTimerUI();
        if (endPanel) endPanel.SetActive(false);
    }

    public void TryStartIfReady()
    {
        if (!Object.HasStateAuthority || Running || Finished) return;

        int required = GameSettings.RequiredPlayers;               
        if (Runner.ActivePlayers.Count() >= required)
        {
            SecondsLeft = matchSeconds;
            Running = true;
            _accum = 0f;
        }
    }


    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority || !Running || Finished) return;

        _accum += Runner.DeltaTime;
        if (_accum >= 1f)
        {
            _accum -= 1f;
            SecondsLeft = Mathf.Max(SecondsLeft - 1, 0);

            if (SecondsLeft == 0)
                EndMatch();
        }
    }

    void EndMatch()
    {
        if (!Object.HasStateAuthority) return;
        Running = false;
        Finished = true;

        var ball = FindFirstObjectByType<BallController>();
        if (ball && ball.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }


    public override void Render()
    {
        UpdateTimerUI();

        if (Finished && endPanel)
        {
            endPanel.SetActive(true);

            var score = FindFirstObjectByType<ScoreManager>();
            if (score && endText)
            {
                string winner =
                    (score.LeftScore > score.RightScore) ? "Victoria del lado Izquierdo" :
                    (score.RightScore > score.LeftScore) ? "Victoria del lado Derecho" :
                    "Empate";

                endText.text = $"{winner}\n{score.LeftScore} — {score.RightScore}";
            }
        }
    }

    void UpdateTimerUI()
    {
        if (!timerText) return;
        int mins = SecondsLeft / 60;
        int secs = SecondsLeft % 60;
        timerText.text = $"{mins:0}:{secs:00}";
    }
}
