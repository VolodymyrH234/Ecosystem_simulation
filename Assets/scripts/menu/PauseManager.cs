using UnityEngine;

public class PauseManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}
