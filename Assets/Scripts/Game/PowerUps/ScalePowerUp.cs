using Fusion;
using UnityEngine;

public class ScalePowerUp : BasePowerUp
{
    [SerializeField] private float scaleMultiplier = 1.5f;

    protected override void ApplyTo(NetworkPaddle target)
    {
        target.RpcApplyScaleBoost(scaleMultiplier, duration);
    }
}
