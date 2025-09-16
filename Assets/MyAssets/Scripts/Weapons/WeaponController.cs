
using UnityEngine;

namespace MyAssets.Scripts.Weapons
{
    /// <summary>
    /// Controlador de un arma. Gestiona el disparo, el fire rate y delega
    /// en el comportamiento de disparo configurado en el ScriptableObject.
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        [Header("Configuración del arma")]
        [SerializeField] private WeaponData data;
        [SerializeField] private Transform firePoint;
        

        /// <summary>
        /// Intenta disparar en la dirección dada. 
        /// Devuelve true si el disparo fue exitoso, false si está en cooldown o mal configurado.
        /// </summary>
        public bool TryShoot(Vector2 direction)
        {
            

            // Ejecutar el disparo
            Debug.Log($"Disparando desde: {firePoint.position}, dirección: {direction}");
            data.shootBehavior.Shoot(data, firePoint, direction);

            // Reiniciar cooldown

            return true;
        }
    }
}