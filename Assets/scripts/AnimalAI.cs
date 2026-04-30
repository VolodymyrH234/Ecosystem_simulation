using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animal), typeof(Animator))]
public class AnimalAI : MonoBehaviour
{
    private Animal animal;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform target;

    public float detectionRange = 150f;
    public float walkSpeed = 20f;
    public float runSpeed = 50f;

    private Vector2 wanderDirection;
    private float wanderTimer = 0f;

    private Animal targetAnimal;
    private bool isAttacking = false;




    void Start()
    {
        animal = GetComponent<Animal>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        ChooseRandomDirection();
    }

    private float attackCooldown = 2f;
    private float attackTimer = 0f;

    void Update()
    {
        if (animal.health <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        // Дитинчата не атакують, але повинні рухатись
        if (animal.gender == AnimalGender.Child)
        {
            Wander(); // Тільки блукання
            return;
        }

        // Якщо тварина їсть або в стані розмноження — стоїть
        if (animal.IsEating || animal.isReproducing)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        attackTimer -= Time.deltaTime;

        // Якщо м'ясоїдний і голодний — атакує інших
        if (animal.type == AnimalType.Carnivore && animal.hunger < 70 && !isAttacking)
        {
            FindAttackTarget();
            if (targetAnimal != null)
            {
                ChaseAndAttack();
            }
            else
            {
                Wander();
            }
            return;
        }

        // Якщо голодний — шукає їжу
        if (animal.hunger <= 75f)
        {
            FindTarget();
            if (target != null)
            {
                MoveToTarget();
                return;
            }
        }

        // Якщо не атакує і не їсть — просто гуляє
        Wander();
    }



    private float minX = -355f;
    private float maxX = 355f;
    private float minY = -190f;
    private float maxY = 200f;

    Vector2 ClampPosition(Vector2 position)
    {
        float clampedX = Mathf.Clamp(position.x, minX, maxX);
        float clampedY = Mathf.Clamp(position.y, minY, maxY);
        return new Vector2(clampedX, clampedY);
    }





    void Flip(float horizontal)
    {
        if (spriteRenderer == null) return;

        if (horizontal > 0.05f)
            spriteRenderer.flipX = false;
        else if (horizontal < -0.05f)
            spriteRenderer.flipX = true;
    }

    void FindTarget()
    {
        if (animal.hunger > 75f) // Тварина не шукає їжу, якщо не дуже голодна
        {
            target = null;
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var hit in hits)
        {
            Food food = hit.GetComponent<Food>();
            if (food && CanEat(food))
            {
                target = food.transform;
                return;
            }
        }

        target = null;
    }


    bool CanEat(Food food)
    {
        if (animal.hunger > 75f) return false;

        if (animal.type == AnimalType.Herbivore && food.foodType == AnimalType.Herbivore) return true;
        if (animal.type == AnimalType.Carnivore && food.foodType == AnimalType.Carnivore) return true;
        if (animal.type == AnimalType.Omnivore) return true;
        return false;
    }

    void MoveToTarget()
    {
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        float speed = runSpeed;

        Vector2 newPos = rb.position + direction * speed * Time.deltaTime;
        rb.MovePosition(ClampPosition(newPos));

        animator.SetFloat("Speed", speed);
        Flip(direction.x);
    }

    void FindAttackTarget()
    {
        float closestDistance = float.MaxValue;
        Animal closestAnimal = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) continue; // не атакує себе

            Animal other = hit.GetComponent<Animal>();
            if (other != null && other.health > 0 && other.type != animal.type)
            {
                float dist = Vector2.Distance(transform.position, other.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestAnimal = other;
                }
            }
        }

        targetAnimal = closestAnimal;
    }


    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            ChooseRandomDirection();
        }

        float speed = walkSpeed;
        Vector2 newPos = rb.position + wanderDirection * speed * Time.deltaTime;
        rb.MovePosition(ClampPosition(newPos));

        animator.SetFloat("Speed", speed);
        Flip(wanderDirection.x);
    }



    void ChooseRandomDirection()
    {
        wanderDirection = Random.insideUnitCircle.normalized;
        wanderTimer = Random.Range(2f, 5f);
    }



    void TryAttackNearestAnimal()
    {
        if (animal.gender == AnimalGender.Child) return;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        Animal nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            Animal other = hit.GetComponent<Animal>();
            if (other != null && other != animal && other.type != animal.type && other.health > 0)
            {
                float dist = Vector2.Distance(transform.position, other.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = other;
                }
            }
        }

        if (nearest != null && attackTimer <= 0f)
        {
            Attack(nearest);
            attackTimer = attackCooldown;
        }

    }

    void Attack(Animal targetAnimal)
    {
        if (animal.gender == AnimalGender.Child) return;
        animator.SetTrigger("Attack");

        targetAnimal.TakeDamage(25f); // Напад м'ясоїдного

        // Випадкова відповідь
        float chance = Random.value;
        bool willFightBack = false;

        if (targetAnimal.type == AnimalType.Herbivore)
            willFightBack = chance < 0.5f;
        else if (targetAnimal.type == AnimalType.Omnivore)
            willFightBack = chance < 0.75f;

        if (willFightBack)
        {
            StartCoroutine(DelayedCounterAttack(targetAnimal));
        }
        else
        {
            // Втеча: активуй анімацію бігу
            Animator targetAnim = targetAnimal.GetComponent<Animator>();
            if (targetAnim) targetAnim.SetFloat("Speed", targetAnimal.runSpeed);
            Vector2 away = (targetAnimal.transform.position - transform.position).normalized;
            Rigidbody2D targetRb = targetAnimal.GetComponent<Rigidbody2D>();
            if (targetRb) targetRb.MovePosition(targetRb.position + away * targetAnimal.runSpeed * Time.deltaTime);
        }
    }

    IEnumerator DelayedCounterAttack(Animal target)
    {
        yield return new WaitForSeconds(0.5f);

        if (target == null) yield break;                      // 🔒 Перевірка існування
        if (target.health <= 0) yield break;                  // 🔒 Перевірка чи не мертвий
        if (target.gameObject == null) yield break;           // 🔒 Перевірка на знищення

        Animator anim = target.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Attack");

        float damage = target.type == AnimalType.Herbivore ? 15f : 25f;
        animal.TakeDamage(damage);
    }


    void ChaseAndAttack()
    {
        if (animal.gender == AnimalGender.Child) return;
        if (targetAnimal == null || targetAnimal.health <= 0)
        {
            isAttacking = false;
            animator.SetFloat("Speed", 0);
            return;
        }

        float distance = Vector2.Distance(transform.position, targetAnimal.transform.position);

        if (distance > 1.5f) // Ще далеко — підбігає
        {
            isAttacking = false;
            Vector2 direction = ((Vector2)targetAnimal.transform.position - rb.position).normalized;
            Vector2 newPos = rb.position + direction * runSpeed * Time.deltaTime;
            rb.MovePosition(ClampPosition(newPos));
            animator.SetFloat("Speed", runSpeed);
            Flip(direction.x);
        }
        else
        {
            if (!isAttacking)
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
                StartCoroutine(AttackTarget());
            }
        }
    }


    IEnumerator AttackTarget()
    {

        isAttacking = true;

        if (animal.gender == AnimalGender.Child)
        {
            isAttacking = false;
            yield break;
        }

        if (targetAnimal == null || targetAnimal.health <= 0)
        {
            isAttacking = false;
            animator.SetFloat("Speed", 0);
            yield break;
        }

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f); // Почекати анімацію

        if (targetAnimal != null && targetAnimal.health > 0)
        {
            targetAnimal.TakeDamage(25);

            // Відповідь жертви
            float chance = Random.value;
            if (targetAnimal.type == AnimalType.Herbivore)
            {
                if (chance < 0.5f)
                    targetAnimal.GetComponent<AnimalAI>()?.RunAwayFrom(transform.position);
                else
                    targetAnimal.GetComponent<AnimalAI>()?.CounterAttack(this.animal);
            }
            else if (targetAnimal.type == AnimalType.Omnivore)
            {
                if (chance < 0.75f)
                    targetAnimal.GetComponent<AnimalAI>()?.RunAwayFrom(transform.position);
                else
                    targetAnimal.GetComponent<AnimalAI>()?.CounterAttack(this.animal);
            }
        }

        yield return new WaitForSeconds(1f); // Затримка до нової атаки

        isAttacking = false;
    }

    public void RunAwayFrom(Vector3 threatPosition)
    {
        Vector2 dir = (rb.position - (Vector2)threatPosition).normalized;
        Vector2 newPos = rb.position + dir * runSpeed * Time.deltaTime;
        rb.MovePosition(ClampPosition(newPos));

        animator.SetFloat("Speed", runSpeed);
        animator.SetTrigger("Run");
        Flip(dir.x);
    }

    public void CounterAttack(Animal attacker)
    {
        if (animal.health <= 0 || attacker == null || animal.gender == AnimalGender.Child) return;

        animator.SetTrigger("Attack");

        float damage = 0;
        if (animal.type == AnimalType.Herbivore)
            damage = 15f;
        else if (animal.type == AnimalType.Omnivore)
            damage = 25f;

        attacker.TakeDamage(damage);
    }




}