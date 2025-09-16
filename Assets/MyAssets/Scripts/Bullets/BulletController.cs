using UnityEngine;

namespace MyAssets.Scripts.Bullets
{
    /// <summary>
    /// Comportamiento de la bala instanciada.
    /// Configurado al inicializar con datos de BulletData.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class BulletController : MonoBehaviour
    {
        private BulletData data;
        private Rigidbody2D rb;
        private SpriteRenderer sr;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }
    
        /// <summary>
        /// Inicializa la bala con datos y dirección.
        /// </summary>
        public void Initialize(BulletData bulletData, Vector2 direction)
        {
            data = bulletData;
            sr.sprite = data.sprite;
            rb.linearVelocity = direction.normalized * data.speed;
            
            // Rotar sprite de la bala hacia la dirección
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            Destroy(gameObject, data.lifeTime);
        }

        private void FixedUpdate()
        {
            // Aplicar gravedad personalizada si existe
            if (data is not null && data.gravity != 0f)
            {
                rb.linearVelocity += Vector2.down * (data.gravity * Time.fixedDeltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log($"{data.bulletName} hit {collision.name} for {data.damage} damage");
            Destroy(gameObject);
        }
    }
}
