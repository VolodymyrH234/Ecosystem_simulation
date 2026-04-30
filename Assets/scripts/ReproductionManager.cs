using System.Collections;
using UnityEngine;

public class ReproductionManager : MonoBehaviour
{
    public float reproductionDistance = 1.5f;

    void Update()
    {
        Animal[] animals = FindObjectsOfType<Animal>();

        foreach (var a1 in animals)
        {
            if (!a1.CanReproduce() || a1.isMovingToMate) continue;

            foreach (var a2 in animals)
            {
                if (a1 == a2 || !a2.CanReproduce() || a2.isMovingToMate) continue;
                if (a1.type != a2.type) continue;
                if (a1.gender == a2.gender) continue;

                float dist = Vector2.Distance(a1.transform.position, a2.transform.position);
                if (dist > reproductionDistance * 4f) continue; // Обмежимо максимальну відстань

                StartCoroutine(HandleReproduction(a1, a2));
                return;
            }
        }
    }

    IEnumerator HandleReproduction(Animal a1, Animal a2)
    {
        a1.isMovingToMate = true;
        a2.isMovingToMate = true;

        AnimalAI ai1 = a1.GetComponent<AnimalAI>();
        AnimalAI ai2 = a2.GetComponent<AnimalAI>();

        if (ai1 != null) ai1.enabled = false;
        if (ai2 != null) ai2.enabled = false;

        // Біг один до одного
        while (Vector2.Distance(a1.transform.position, a2.transform.position) > reproductionDistance)
        {
            MoveTowards(a1, a2.transform.position);
            MoveTowards(a2, a1.transform.position);
            yield return null;
        }

        // Зупинилися
        Rigidbody2D rb1 = a1.GetComponent<Rigidbody2D>();
        Rigidbody2D rb2 = a2.GetComponent<Rigidbody2D>();

        if (rb1 != null) rb1.linearVelocity = Vector2.zero;
        if (rb2 != null) rb2.linearVelocity = Vector2.zero;

        a1.GetComponent<Animator>().SetFloat("Speed", 0f);
        a2.GetComponent<Animator>().SetFloat("Speed", 0f);

        yield return new WaitForSeconds(2f); // Затримка перед створенням дитинча

        // Створення дитинча
        Vector2 spawnPos = (a1.transform.position + a2.transform.position) / 2f;
        if (a1.childPrefab != null)
        {
            Instantiate(a1.childPrefab, spawnPos, Quaternion.identity);
        }


        a1.hunger -= 20f;
        a2.hunger -= 20f;

        a1.SetReproductionCooldown(5f);
        a2.SetReproductionCooldown(5f);

        a1.isMovingToMate = false;
        a2.isMovingToMate = false;

        // Включаємо AI знову
        if (ai1 != null) ai1.enabled = true;
        if (ai2 != null) ai2.enabled = true;
    }


    void MoveTowards(Animal animal, Vector2 target)
    {
        Vector2 dir = (target - (Vector2)animal.transform.position).normalized;
        Rigidbody2D rb = animal.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.MovePosition((Vector2)animal.transform.position + dir * animal.runSpeed * Time.deltaTime);

        animal.GetComponent<Animator>().SetFloat("Speed", animal.runSpeed);
        animal.FlipSprite(dir.x);
    }
}
