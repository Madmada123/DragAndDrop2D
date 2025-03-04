using UnityEngine;

public class SortingOrderAndScaleChanger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;

    [Header("Настройки масштаба")]
    public float minScale = 0.7f; // Минимальный размер (дальше)
    public float maxScale = 1.2f; // Максимальный размер (ближе)

    [Header("Границы движения по Y")]
    public float minY = -5f; // Нижняя граница (ближний объект)
    public float maxY = 5f; // Верхняя граница (дальний объект)

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Проверяем, есть ли компонент SpriteRenderer
        if (spriteRenderer == null)
        {
            Debug.LogError($"[SortingOrderAndScaleChanger]: У объекта {gameObject.name} нет SpriteRenderer!");
            return;
        }

        originalScale = transform.localScale;
    }

    void Update()
    {
        if (spriteRenderer == null) return; // Не выполнять код, если нет SpriteRenderer

        // Чем ниже объект, тем он ближе и выше по sortingOrder
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);

        // Изменяем масштаб в зависимости от Y-позиции
        float scaleMultiplier = Mathf.Lerp(maxScale, minScale, Mathf.InverseLerp(minY, maxY, transform.position.y));
        transform.localScale = originalScale * scaleMultiplier;
    }
}
