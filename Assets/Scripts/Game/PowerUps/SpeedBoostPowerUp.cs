using Fusion;
using UnityEngine;

public class SpeedBoostPowerUp : NetworkBehaviour
{
    [SerializeField] private float multiplier = 1.5f;
    [SerializeField] private float duration = 10f;
    bool active;

    public override void Spawned() => active = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority || !active) return;
        if (!other.TryGetComponent<BallController>(out var ball)) return;

        var hitter = ball.LastPaddleHit;
        if (hitter != null)
            hitter.RpcApplySpeedBoost(multiplier, duration);

        active = false;
        Runner.Despawn(Object);
    }
}
