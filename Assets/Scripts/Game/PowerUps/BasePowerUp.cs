using Fusion;
using UnityEngine;

public abstract class BasePowerUp : NetworkBehaviour
{
    [SerializeField] protected float duration = 10f;

    bool _active;

    public override void Spawned() => _active = true;

    protected abstract void ApplyTo(NetworkPaddle target);

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority || !_active) return;

        if (other.TryGetComponent<BallController>(out var ball))
        {
            var last = ball.LastPaddleHit;          // quién pegó último
            if (last != null)
            {
                ApplyTo(last);
                _active = false;
                Runner.Despawn(Object);             // desaparece al usarse
            }
        }
    }
}
