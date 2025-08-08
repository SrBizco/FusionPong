using System;           
using System.Linq;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
public class GameNetwork : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Prefabs (Network Prefab Ref)")]
    [SerializeField] private NetworkPrefabRef paddleLeftPrefab;
    [SerializeField] private NetworkPrefabRef paddleRightPrefab;
    [SerializeField] private NetworkPrefabRef ballPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform leftSpawn;
    [SerializeField] private Transform rightSpawn;
    [SerializeField] private Transform ballSpawn;

    private NetworkRunner _runner;
    private readonly Dictionary<PlayerRef, NetworkObject> _playerPaddle = new();

    void Awake()
    {
        // Unity 6: FindFirstObjectByType en vez de FindObjectOfType<T>()
        _runner = FindFirstObjectByType<NetworkRunner>();
        if (_runner != null)
            _runner.AddCallbacks(this);
    }

    // ===================== INPUT =====================
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        float y = 0f;
        // Para test local con 2 teclados simples
        if (Input.GetKey(KeyCode.W)) y += 1f;
        if (Input.GetKey(KeyCode.S)) y -= 1f;
        if (Input.GetKey(KeyCode.UpArrow)) y += 1f;
        if (Input.GetKey(KeyCode.DownArrow)) y -= 1f;

        input.Set(new NetworkInputData { MoveY = Mathf.Clamp(y, -1f, 1f) });
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    // ===================== JOIN / LEAVE =====================
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        bool isLeft = (runner.ActivePlayers.Count() % 2) == 1;
        var prefab = isLeft ? paddleLeftPrefab : paddleRightPrefab;
        var spawn = isLeft ? leftSpawn : rightSpawn;

        var paddle = runner.Spawn(prefab, spawn.position, spawn.rotation, player);
        _playerPaddle[player] = paddle;

        var ball = FindFirstObjectByType<BallController>();
        if (ball == null)
        {
            ball = runner.Spawn(ballPrefab, ballSpawn.position, ballSpawn.rotation, null)
                         .GetComponent<BallController>();
            ball.Freeze(true);
        }

        // ← Cambiá esta línea
        if (runner.ActivePlayers.Count() >= GameSettings.RequiredPlayers)
            ball.ResetAndServe(3f);

        var timer = FindFirstObjectByType<MatchTimer>();
        if (timer) timer.TryStartIfReady(); // el timer ya valida RequiredPlayers internamente
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return;

        if (_playerPaddle.TryGetValue(player, out var paddle))
        {
            runner.Despawn(paddle);
            _playerPaddle.Remove(player);
        }

        // Si quedamos por debajo del mínimo, pausamos
        if (runner.ActivePlayers.Count() < GameSettings.RequiredPlayers)
        {
            var ball = FindFirstObjectByType<BallController>();
            if (ball) ball.Freeze(true);

            var timer = FindFirstObjectByType<MatchTimer>();
            if (timer) timer.Running = false;
        }
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        int maxPlayers = GameSettings.RequiredPlayers; // 2 o 4 según el menú
        int current = runner.ActivePlayers.Count();

        if (current >= maxPlayers)
            request.Refuse();
        else
            request.Accept();
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
