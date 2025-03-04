using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dragSpeed = 0.5f;  // Скорость перемещения
    public Vector2 minPosition;     // Минимальная граница движения (X, Y)
    public Vector2 maxPosition;     // Максимальная граница движения (X, Y)

    private Vector3 dragOrigin;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Application.isMobilePlatform)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
        ClampCameraPosition(); // Ограничение позиции камеры
    }

    // 📌 Управление мышью (ПК)
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += difference;
        }
    }

    // 📌 Управление пальцами (смартфоны)
    void HandleTouchInput()
    {
        if (Input.touchCount == 1) // Перемещение одним пальцем
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = cam.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(touch.position);
                transform.position += difference;
            }
        }
    }

    // 📌 Ограничение позиции камеры
    void ClampCameraPosition()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minPosition.x, maxPosition.x),
            Mathf.Clamp(transform.position.y, minPosition.y, maxPosition.y),
            transform.position.z
        );
    }
}
