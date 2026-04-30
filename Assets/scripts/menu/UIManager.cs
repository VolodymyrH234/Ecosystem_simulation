using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button newGameButton;
    public Button exitButton;

    void Start()
    {
        newGameButton.onClick.AddListener(() => GameManager.Instance.StartNewGame());
        exitButton.onClick.AddListener(() => GameManager.Instance.ExitGame());
    }
}
