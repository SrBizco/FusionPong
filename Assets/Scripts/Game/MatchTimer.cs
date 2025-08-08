using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class MatchTimer : NetworkBehaviour
{
    [Header("Config")]
    [SerializeField] private int matchSeconds = 180; // 3 minutos

    [Header("UI (cliente)")]
    [SerializeField] private TMP_Text timerText;     // arrastrar en los clientes
    [SerializeField] private GameObject endPanel;    // panel final (oculto al inicio)
    [SerializeField] private TMP_Text endText;       // "Gan� Left/Right 3�2"

    [Networked] public int SecondsLeft { get; set; }
    [Networked] public NetworkBool Running { get; set; }
    [Networked] public NetworkBool Finished { get; set; }

    private float _accum;

    public override void Spawned()
    {
        // UI inicial en cada peer
        UpdateTimerUI();
        if (endPanel) endPanel.SetActive(false);
    }

    // Llamado por el server cuando haya 2 jugadores
    public void TryStartIfReady()
    {
        if (!Object.HasStateAuthority || Running || Finished) return;

        if (Runner.ActivePlayers.Count() >= 2)
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

        // Congelar pelota
        var ball = FindFirstObjectByType<BallController>();
        if (ball && ball.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }


    public override void Render()
    {
        // Actualizar UI en clientes cada frame visual
        UpdateTimerUI();

        if (Finished && endPanel)
        {
            endPanel.SetActive(true);

            // Determinar ganador con el ScoreManager replicado
            var score = FindFirstObjectByType<ScoreManager>();
            if (score && endText)
            {
                string winner =
                    (score.LeftScore > score.RightScore) ? "Jugador Izquierdo" :
                    (score.RightScore > score.LeftScore) ? "Jugador Derecho" :
                    "Empate";

                endText.text = $"{winner}\n{score.LeftScore} � {score.RightScore}";
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
