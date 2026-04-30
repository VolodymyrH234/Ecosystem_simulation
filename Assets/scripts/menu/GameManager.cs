using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // зберігати при переходах між сценами
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Ecosystem");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
