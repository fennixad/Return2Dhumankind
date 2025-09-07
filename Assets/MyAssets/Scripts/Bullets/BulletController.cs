using UnityEngine;

/// <summary>
/// Comportamiento de la bala instanciada.
/// </summary>
public class BulletController : MonoBehaviour
{
    [SerializeField] private BulletData data;
    private Rigidbody2D rb;

    public BulletData Data => data;

    /// <summary>
    /// Inicializa la bala con datos y dirección.
    /// </summary>
    public void Initialize(BulletData bulletData, Vector2 direction)
    {
        data = bulletData;
        rb = GetComponent<Rigidbody2D>();

        GetComponent<SpriteRenderer>().sprite = data.sprite;
        rb.linearVelocity = direction.normalized * data.speed;

        Destroy(gameObject, data.lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"{data.bulletName} hit {collision.name} for {data.damage} damage");
        Destroy(gameObject);
    }
}
