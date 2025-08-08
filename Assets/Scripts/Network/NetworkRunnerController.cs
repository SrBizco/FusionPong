using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerController : MonoBehaviour
{
    [Header("Opciones de inicio")]
    [SerializeField] private bool startAsDedicatedServer = false;
    [SerializeField] private bool startAsAutoHostClient = false;
    [SerializeField] private string sessionName = "FusionPong";

    private NetworkRunner _runner;

    private async void Start()
    {
        if (HasArg("-dedicated")) startAsDedicatedServer = true;
        if (HasArg("-auto")) startAsAutoHostClient = true;

        _runner = GetComponent<NetworkRunner>();
        if (_runner == null) _runner = gameObject.AddComponent<NetworkRunner>();

        _runner.ProvideInput = !startAsDedicatedServer;

        var sceneMgr = gameObject.AddComponent<NetworkSceneManagerDefault>();

        SceneRef sceneRef = SceneRef.FromIndex(2);

        GameMode mode =
            startAsDedicatedServer ? GameMode.Server :
            startAsAutoHostClient ? GameMode.AutoHostOrClient :
            GameMode.Client;

        string session = GameSettings.SessionName;

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = session,
            Scene = sceneRef,
            SceneManager = sceneMgr
        });

        if (!result.Ok)
            Debug.LogError($"❌ Falló StartGame: {result.ShutdownReason}");
        else
            Debug.Log($"✅ Runner iniciado como {mode} | Sesión: {session}");
    }

    private static bool HasArg(string flag)
    {
        var args = System.Environment.GetCommandLineArgs();
        foreach (var a in args)
            if (a.Equals(flag, System.StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
}
