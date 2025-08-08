using Fusion;
using UnityEngine;

public class SlowOpponentPowerUp : NetworkBehaviour
{
    [SerializeField] private float multiplier = 0.66f; // más lento
    [SerializeField] private float duration = 10f;
    bool active;

    public override void Spawned() => active = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority || !active) return;
        if (!other.TryGetComponent<BallController>(out var ball)) return;

        var hitter = ball.LastPaddleHit;
        if (hitter != null)
        {
            var rival = GetOpponent(hitter);
            if (rival != null)
                rival.RpcApplySpeedBoost(multiplier, duration);
        }

        active = false;
        Runner.Despawn(Object);
    }

    static NetworkPaddle GetOpponent(NetworkPaddle me)
    {
        var paddles = FindObjectsByType<NetworkPaddle>(FindObjectsSortMode.None);
        if (paddles == null || paddles.Length < 2) return null;
        return paddles[0] == me ? paddles[1] : paddles[0];
    }
}
