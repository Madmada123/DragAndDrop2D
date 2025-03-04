using UnityEngine;

public class DragAndDropWithCamera : MonoBehaviour
{
    private bool isDragging = false;
    private bool isPlaced = false; // ✅ Флаг, что предмет уже установлен
    private Vector3 offset;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Transform nearestPlacement = null; // Ближайшая точка установки

    [SerializeField] private CameraController cameraController; // Ссылка на контроллер камеры

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Отключаем гравитацию
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
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            if (IsTouchingObject(mousePosition))
            {
                isDragging = true;
                rb.gravityScale = 0;
                rb.bodyType = RigidbodyType2D.Dynamic; // ✅ Объект снова становится динамическим
                offset = transform.position - mousePosition;
                spriteRenderer.sortingOrder = 100;
                cameraController.enabled = false; // Отключаем движение камеры
                isPlaced = false; // ✅ Позволяем предмету снова двигаться
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition + offset;
        }

        if (Input.GetMouseButtonUp(0))
        {
            HandleRelease(); // Вызываем обработку отпускания
            cameraController.enabled = true; // Включаем движение камеры обратно
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsTouchingObject(touchPosition))
                    {
                        isDragging = true;
                        rb.gravityScale = 0;
                        rb.bodyType = RigidbodyType2D.Dynamic;
                        offset = transform.position - touchPosition;
                        spriteRenderer.sortingOrder = 100;
                        cameraController.enabled = false;
                        isPlaced = false;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        transform.position = touchPosition + offset;
                    }
                    break;

                case TouchPhase.Ended:
                    HandleRelease();
                    cameraController.enabled = true;
                    break;
            }
        }
    }

    bool IsTouchingObject(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position);
        return hit != null && hit.gameObject == gameObject;
    }

    void HandleRelease()
    {
        isDragging = false;

        if (nearestPlacement != null) // Если есть место — ставим предмет туда
        {
            transform.position = nearestPlacement.position;
            rb.velocity = Vector2.zero; // Останавливаем движение
            rb.gravityScale = 0; // Отключаем гравитацию
            rb.bodyType = RigidbodyType2D.Static; // Делаем объект статичным
            isPlaced = true; // ✅ Запоминаем, что предмет установлен
        }
        else
        {
            if (!isPlaced) // ✅ Если предмет уже установлен, не включаем гравитацию
            {
                rb.gravityScale = 0.3f;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlacementPoint") && !isPlaced) // ✅ Если уже стоит, не меняем
        {
            nearestPlacement = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlacementPoint") && other.transform == nearestPlacement && !isPlaced)
        {
            nearestPlacement = null;
        }
    }
}
