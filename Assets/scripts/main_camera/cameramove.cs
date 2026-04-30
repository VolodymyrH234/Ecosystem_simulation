using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    [Header("Camera Movement Bounds")]
    public float minX = -100f;
    public float maxX = 100f;
    public float minY = -100f;
    public float maxY = 100f;

    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            direction += Vector3.up;       // Y+
        if (Input.GetKey(KeyCode.S))
            direction += Vector3.down;     // Y-
        if (Input.GetKey(KeyCode.A))
            direction += Vector3.left;     // X-
        if (Input.GetKey(KeyCode.D))
            direction += Vector3.right;    // X+

        transform.position += direction * (moveSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX + camHalfWidth, maxX - camHalfWidth);
        pos.y = Mathf.Clamp(pos.y, minY + camHalfHeight, maxY - camHalfHeight);

        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

}
