using System.Collections;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(PaddleController))]
public class NetworkPaddle : NetworkBehaviour
{
    [SerializeField] private PaddleController paddle;

    void Reset() => paddle = GetComponent<PaddleController>();

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out NetworkInputData data))
        {
            paddle.SetNetworkInput(data.MoveY);
        }
        else
        {
            paddle.SetNetworkInput(0f);
        }
    }
    public override void Spawned()
    {
        if (paddle == null)
            paddle = GetComponent<PaddleController>();

        var top = GameObject.Find("TopWall")?.GetComponent<Collider>();
        var bottom = GameObject.Find("BottomWall")?.GetComponent<Collider>();

        if (top != null && bottom != null)
            paddle.SetWalls(top, bottom);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcApplyScaleBoost(float multiplier, float seconds)
    {
        StopCoroutine(nameof(ScaleBoostRoutine));
        StartCoroutine(ScaleBoostRoutine(multiplier, seconds));
    }

    private IEnumerator ScaleBoostRoutine(float multiplier, float seconds)
    {
        var tr = transform;
        var original = tr.localScale;

        tr.localScale = new Vector3(original.x, original.y * multiplier, original.z);
        yield return new WaitForSeconds(seconds);
        tr.localScale = original;
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcApplySpeedBoost(float multiplier, float seconds)
    {
        StopCoroutine(nameof(SpeedBoostRoutine));
        StartCoroutine(SpeedBoostRoutine(multiplier, seconds));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float seconds)
    {
        float original = paddle.Speed;
        paddle.Speed = original * multiplier;
        yield return new WaitForSeconds(seconds);
        paddle.Speed = original;
    }
    public static NetworkPaddle GetOpponent(NetworkPaddle me)
    {
        var paddles = FindObjectsByType<NetworkPaddle>(FindObjectsSortMode.None);
        if (paddles == null || paddles.Length < 2)
            return null;

        return paddles[0] == me ? paddles[1] : paddles[0];
    }
}
