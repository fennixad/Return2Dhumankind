using MyAssets.Scripts.Weapons;
using UnityEngine;

namespace MyAssets.Scripts.Player
{
    /// <summary>
    /// Controlador de disparo del jugador.
    /// Usa un arma (WeaponController) y una estrategia de apuntado.
    /// </summary>
    public class PlayerShooter : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private WeaponController weapon;
        [SerializeField] private Aim2D aimer; // O Aim2D, según lo que uses

        private void Awake()
        {
            if (weapon == null)
                Debug.LogWarning("WeaponController no asignado en PlayerShooter", this);

            if (aimer == null)
                Debug.LogWarning("Aimer no asignado en PlayerShooter", this);
        }

        private void Update()
        {
            if (weapon == null) return;
            // La dirección debe venir del firePoint, que es el que se rota
            Vector2 direction = weapon.transform.right;

            if (Input.GetButton("Fire1"))
            {
                bool shot = weapon.TryShoot(direction);

                if (shot)
                    Debug.Log("💥 Disparo exitoso");
                else
                    Debug.Log("🔒 No se pudo disparar (cooldown o configuración)");
            }
        }
    }
}

