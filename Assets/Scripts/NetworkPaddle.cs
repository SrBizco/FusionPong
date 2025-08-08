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

        // ✅ Fusion 2
        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out NetworkInputData data))
        {
            paddle.SetNetworkInput(data.MoveY);
        }
        else
        {
            // sin input para este tick: frenar
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

}
