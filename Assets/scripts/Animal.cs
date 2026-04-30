using UnityEngine;
using System.Collections;

public enum AnimalType { Carnivore, Herbivore, Omnivore }
public enum AnimalGender { Male, Female, Child }

public class Animal : MonoBehaviour
{
    [Header("Animal Settings")]
    public AnimalType type;
    public AnimalGender gender;

    [Header("Drop Settings")]
    public GameObject meatPrefab;

    [Header("Child Prefab")]
    public GameObject childPrefab;

    [Header("Adult Prefabs")]
    public GameObject adultMalePrefab;
    public GameObject adultFemalePrefab;

    public float hunger = 100f;
    public float health = 100f;

    public float hungerForReproduction = 80f;
    public float hungerForSurvival = 30f;

    public float speed = 2f;
    public float runSpeed = 4f;
    public float eatAmount = 25f;

    private float hungerDecreaseRate = 0.2f; // Зменшено темп спаду голоду
    private float healthDecreaseRate = 2f;

    [HideInInspector] public bool isReproducing = false;

    private Animator anim;
    private bool isInWater = false;
    private bool isEating = false;

    public bool IsEating => isEating;



    void Start()
    {
        anim = GetComponent<Animator>();
        if (gender == AnimalGender.Child)
            StartCoroutine(GrowUp());
    }

    void Update()
    {
        HandleNeeds();
        Animate();
    }

    private bool canReproduceNow = true;

    public bool CanReproduce()
    {
        return hunger >= hungerForReproduction && gender != AnimalGender.Child && canReproduceNow;
    }

    public void SetReproductionCooldown(float delay)
    {
        StartCoroutine(ReproductionCooldown(delay));
    }

    private IEnumerator ReproductionCooldown(float delay)
    {
        canReproduceNow = false;
        yield return new WaitForSeconds(delay);
        canReproduceNow = true;
    }








    void HandleNeeds()
    {
        hunger = Mathf.Max(0, hunger - hungerDecreaseRate * Time.deltaTime);

        if (hunger < hungerForSurvival)
            health = Mathf.Max(0, health - healthDecreaseRate * Time.deltaTime);

        if (health <= 0)
            Die();
    }

    public void Eat(float amount)
    {
        if (!isEating && IsHungry()) // додаємо перевірку
            StartCoroutine(EatRoutine(amount));
    }

    private IEnumerator EatRoutine(float amount)
    {
        isEating = true;
        anim.SetTrigger("Eat");

        yield return new WaitForSeconds(2f);

        hunger = Mathf.Min(hunger + amount, 100f);
        health = Mathf.Min(health + amount * 0.5f, 100f);

        isEating = false;
    }


    public bool IsHungry() => hunger < 80f; // Ситість перевіряється перед пошуком їжі

    public void TakeDamage(float amount)
    {
        health = Mathf.Max(0, health - amount);
        anim.SetTrigger("Hurt");

        if (health <= 0)
            Die();
    }

    void Die()
    {
        anim.SetTrigger("Die");

        if (meatPrefab)
            Instantiate(meatPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject, 1f);
    }



    IEnumerator GrowUp()
    {
        yield return new WaitForSeconds(30f);

        GameObject newAdult = null;

        if (Random.value < 0.5f && adultMalePrefab != null)
        {
            newAdult = Instantiate(adultMalePrefab, transform.position, Quaternion.identity);
        }
        else if (adultFemalePrefab != null)
        {
            newAdult = Instantiate(adultFemalePrefab, transform.position, Quaternion.identity);
        }

        if (newAdult != null)
        {
            Destroy(gameObject); // Знищити дитинча
        }
        else
        {
            Debug.LogWarning("Не вказано дорослі префаби для " + name);
        }
    }


    void Animate()
    {
        anim.SetFloat("Health", health);
        anim.SetFloat("Hunger", hunger);
        anim.SetBool("Swimming", isInWater);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
            isInWater = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
            isInWater = false;
    }






    [HideInInspector] public bool isMovingToMate = false;

    public void FlipSprite(float horizontal)
    {
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;

        if (horizontal > 0.05f) sr.flipX = false;
        else if (horizontal < -0.05f) sr.flipX = true;
    }




}
