using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerController : MonoBehaviour
{
    [SerializeField] private bool autoStart = true;
    [SerializeField] private string sessionName = "FusionPong";

    private NetworkRunner _runner;

    private async void Start()
    {
        // Flags de línea de comando
        bool dedicated = HasArg("-dedicated");   // modo servidor dedicado
        bool auto = HasArg("-auto");        // opcional: AutoHostOrClient para pruebas rápidas

        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = !dedicated; // el server no provee input

        var sceneMgr = gameObject.AddComponent<NetworkSceneManagerDefault>();

        var active = SceneManager.GetActiveScene();
        SceneRef sceneRef = SceneRef.FromIndex(active.buildIndex);

        var mode = dedicated
            ? GameMode.Server
            : (auto ? GameMode.AutoHostOrClient : GameMode.Client);

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = sceneRef,
            SceneManager = sceneMgr,
            PlayerCount = 4
        });

        if (!result.Ok)
        {
            Debug.LogError($"Runner StartGame failed: {result.ShutdownReason}");
        }
        else
        {
            Debug.Log($"Runner started as {mode} | session: {sessionName}");
        }
    }

    private static bool HasArg(string flag)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
            if (args[i].Equals(flag, System.StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
}
