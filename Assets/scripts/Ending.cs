using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ending : MonoBehaviour
{
    public Text winText;
    public int winThreshold = 50;
    public float checkInterval = 2f;

    private bool gameEnded = false;

    void Start()
    {
        if (winText != null)
            winText.gameObject.SetActive(false);

        StartCoroutine(CheckWinCondition());
    }

    IEnumerator CheckWinCondition()
    {
        while (!gameEnded)
        {
            yield return new WaitForSeconds(checkInterval);

            Animal[] allAnimals = FindObjectsOfType<Animal>();

            int herbivores = 0, carnivores = 0, omnivores = 0;

            foreach (Animal a in allAnimals)
            {
                if (a == null) continue;

                switch (a.type)
                {
                    case AnimalType.Herbivore: herbivores++; break;
                    case AnimalType.Carnivore: carnivores++; break;
                    case AnimalType.Omnivore: omnivores++; break;
                }
            }

            if (herbivores >= winThreshold)
                EndGame("Herbivores win!");
            else if (carnivores >= winThreshold)
                EndGame("Carnivores win!");
            else if (omnivores >= winThreshold)
                EndGame("Omnivores win!");
        }
    }

    void EndGame(string message)
    {
        gameEnded = true;

        if (winText != null)
        {
            winText.text = message;
            winText.gameObject.SetActive(true);
        }

    }
}
