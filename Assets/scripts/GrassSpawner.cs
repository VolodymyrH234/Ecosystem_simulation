using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    public GameObject grassPrefab;
    public float spawnInterval = 10f;
    public int maxGrassCount = 20;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    public LayerMask groundLayer; // ✅ Шар землі

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && CountGrass() < maxGrassCount)
        {
            TrySpawnGrass();
            timer = 0f;
        }
    }

    void TrySpawnGrass()
    {
        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        // 🔽 Пускаємо промінь вниз, щоб перевірити землю
        RaycastHit2D hit = Physics2D.Raycast(spawnPosition + Vector2.up * 2f, Vector2.down, 5f, groundLayer);

        if (hit.collider != null)
        {
            // ✅ Є хіт по землі, спавнимо траву
            Vector2 groundPos = new Vector2(spawnPosition.x, hit.point.y + 0.1f); // трохи над землею
            Instantiate(grassPrefab, groundPos, Quaternion.identity);
        }
    }

    int CountGrass()
    {
        return GameObject.FindGameObjectsWithTag("Grass").Length;
    }
}
