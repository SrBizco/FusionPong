using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel; // arrastrá el panel acá

    public void Play1v1()
    {
        GameSettings.SelectedMode = GameSettings.Mode.OneVOne;
        SceneManager.LoadScene("Boot");
    }

    public void Play2v2()
    {
        GameSettings.SelectedMode = GameSettings.Mode.TwoVTwo;
        SceneManager.LoadScene("Boot");
    }

    public void OpenCredits()
    {
        if (creditsPanel) creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
