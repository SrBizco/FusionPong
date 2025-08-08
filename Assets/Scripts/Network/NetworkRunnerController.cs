using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerController : MonoBehaviour
{
    [Header("Opciones de inicio")]
    [SerializeField] private bool startAsDedicatedServer = false;
    [SerializeField] private bool startAsAutoHostClient = false;
    [SerializeField] private string sessionName = "FusionPong"; // se sobrescribe con GameSettings

    private NetworkRunner _runner;

    private async void Start()
    {
        // Flags CLI
        if (HasArg("-dedicated")) startAsDedicatedServer = true;
        if (HasArg("-auto")) startAsAutoHostClient = true;

        // Runner (reusar si ya existe)
        _runner = GetComponent<NetworkRunner>();
        if (_runner == null) _runner = gameObject.AddComponent<NetworkRunner>();

        // Dedicated no provee input
        _runner.ProvideInput = !startAsDedicatedServer;

        // Scene manager
        var sceneMgr = gameObject.AddComponent<NetworkSceneManagerDefault>();

        // Escena de juego (Game) → Build index 2: [0]=MainMenu, [1]=Boot, [2]=Game
        SceneRef sceneRef = SceneRef.FromIndex(2);

        // Modo de inicio
        GameMode mode =
            startAsDedicatedServer ? GameMode.Server :
            startAsAutoHostClient ? GameMode.AutoHostOrClient :
            GameMode.Client;

        // Nombre de sesión según 1v1 / 2v2 elegido en el menú
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
