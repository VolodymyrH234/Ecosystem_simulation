using UnityEngine;

public class CameraPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    [Header("World Bounds")]
    public float minX = -100f;
    public float maxX = 100f;
    public float minY = -100f;
    public float maxY = 100f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        ClampPosition();
    }

    void HandleMovement()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += Vector3.up;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.down;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;

        transform.position += direction * (moveSpeed * Time.deltaTime);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    void ClampPosition()
    {
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = cam.aspect * camHalfHeight;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX + camHalfWidth, maxX - camHalfWidth);
        pos.y = Mathf.Clamp(pos.y, minY + camHalfHeight, maxY - camHalfHeight);

        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}
