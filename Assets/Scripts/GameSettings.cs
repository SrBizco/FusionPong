public static class GameSettings
{
    public enum Mode { OneVOne, TwoVTwo }
    public static Mode SelectedMode = Mode.OneVOne;

    public static int RequiredPlayers =>
        SelectedMode == Mode.OneVOne ? 2 : 4;

    public static string SessionName =>
        SelectedMode == Mode.OneVOne ? "FusionPong-1v1" : "FusionPong-2v2";
}
