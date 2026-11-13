using UnityEngine;

public class IngredientPopup : MonoBehaviour
{
    public float moveSpeed = 1.0f;      // 上に上がる速度
    public float fadeDuration = 1.0f;   // フェードアウトにかかる時間

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 上に移動
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // フェードアウト
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
