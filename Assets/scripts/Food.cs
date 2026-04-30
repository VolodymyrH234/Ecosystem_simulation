using UnityEngine;

public class Food : MonoBehaviour
{
    public float nutrition = 25f;
    public AnimalType foodType;
    void Start()
    {
        if (foodType == AnimalType.Carnivore)
        {
            Destroy(gameObject, 30f); // М’ясо зникає через 20 секунд
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Animal animal = other.GetComponent<Animal>();
        if (animal != null && IsEdibleBy(animal) && animal.IsHungry())
        {
            animal.Eat(nutrition);
            Destroy(gameObject);
        }
    }


    private bool IsEdibleBy(Animal animal)
    {
        if (animal.type == AnimalType.Herbivore && this.foodType == AnimalType.Herbivore) return true;
        if (animal.type == AnimalType.Carnivore && this.foodType == AnimalType.Carnivore) return true;
        if (animal.type == AnimalType.Omnivore) return true;
        return false;
    }
}
