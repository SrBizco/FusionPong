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
        // Sobrescribir con flags de línea de comando si existen
        if (HasArg("-dedicated")) startAsDedicatedServer = true;
        if (HasArg("-auto")) startAsAutoHostClient = true;

        // Buscar primero un NetworkRunner existente en el mismo GameObject
        _runner = GetComponent<NetworkRunner>();
        if (_runner == null)
            _runner = gameObject.AddComponent<NetworkRunner>();

        // El servidor dedicado no provee input
        _runner.ProvideInput = !startAsDedicatedServer;

        var sceneMgr = gameObject.AddComponent<NetworkSceneManagerDefault>();

        // La escena del juego (pong)
        SceneRef sceneRef = SceneRef.FromIndex(1);

        GameMode mode;
        if (startAsDedicatedServer)
            mode = GameMode.Server;
        else if (startAsAutoHostClient)
            mode = GameMode.AutoHostOrClient;
        else
            mode = GameMode.Client;

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = sceneRef,
            SceneManager = sceneMgr
        });

        if (!result.Ok)
        {
            Debug.LogError($"❌ Falló StartGame: {result.ShutdownReason}");
        }
        else
        {
            Debug.Log($"✅ Runner iniciado como {mode} | Sesión: {sessionName}");
        }
    }

    private static bool HasArg(string flag)
    {
        var args = System.Environment.GetCommandLineArgs();
        foreach (var arg in args)
            if (arg.Equals(flag, System.StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
}
