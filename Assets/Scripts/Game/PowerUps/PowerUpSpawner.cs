using Fusion;
using UnityEngine;

public class PowerUpSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef[] powerUpPrefabs;
    [SerializeField] private Vector2 halfFieldSize = new(8f, 4f);
    [SerializeField] private float spawnInterval = 30f;
    [SerializeField] private bool onlyOneAtATime = true;

    float _t;
    NetworkObject _lastSpawned;

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;

        _t += Runner.DeltaTime;
        if (_t < spawnInterval) return;
        _t = 0f;

        if (onlyOneAtATime && _lastSpawned && _lastSpawned) return;

        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        var idx = Random.Range(0, powerUpPrefabs.Length);
        var pos = new Vector3(
            Random.Range(-halfFieldSize.x, halfFieldSize.x),
            Random.Range(-halfFieldSize.y, halfFieldSize.y),
            0f);

        _lastSpawned = Runner.Spawn(powerUpPrefabs[idx], pos, Quaternion.identity);
    }
}
