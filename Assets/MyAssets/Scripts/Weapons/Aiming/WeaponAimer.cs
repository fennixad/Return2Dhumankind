using UnityEngine;

namespace MyAssets.Scripts.Weapons.Aiming
{
    /// <summary>
    /// Se encarga de rotar visualmente el arma siguiendo la dirección calculada
    /// por la estrategia de apuntado asignada.
    /// </summary>
    public class WeaponAimer : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private MonoBehaviour aimStrategyComponent; // Debe implementar IAimStrategy
        [SerializeField] private Transform player; // Para flipY (opcional)
        [SerializeField] private SpriteRenderer spriteRenderer; // Opcional, para flipY

        private IAimStrategy aimStrategy;

        public Vector2 CurrentDirection { get; private set; }

        private void Awake()
        {
            if (aimStrategyComponent == null)
                aimStrategyComponent = GetComponent<IAimStrategy>() as MonoBehaviour;

            aimStrategy = aimStrategyComponent as IAimStrategy;
            if (aimStrategy == null)
                Debug.LogError("El componente asignado no implementa IAimStrategy", this);
        }

        private void Update()
        {
            if (aimStrategy == null) return;

            // Calcula la dirección de apuntado
            CurrentDirection = aimStrategy.GetDirection(transform);

            // Rota el arma
            float angle = Vector3.SignedAngle(Vector3.right, CurrentDirection, Vector3.forward);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // FlipY si el ratón está a la izquierda del jugador (opcional)
            if (spriteRenderer != null && player != null)
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                spriteRenderer.flipY = player.position.x > mouseWorld.x;
            }
        }
    }
}
