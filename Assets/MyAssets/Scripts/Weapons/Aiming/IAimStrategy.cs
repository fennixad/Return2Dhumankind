using UnityEngine;

namespace MyAssets.Scripts.Weapons.Aiming
{
    /// <summary>
    /// Interfaz para diferentes formas de calcular la dirección de disparo.
    /// Esto permite que el jugador apunte con el rat�n,
    /// mientras que un enemigo podr�a apuntar hacia el Player.
    /// </summary>
    public interface IAimStrategy
    {
        /// <param name="shooter">Transform del que dispara (arma, player o enemigo).</param>
        /// <param name="target">Transform del objetivo (opcional, puede ser null).</param>
        /// <returns>Vector2 con la direcci�n normalizada de disparo.</returns>
        Vector2 GetDirection(Transform shooter, Transform target = null);
    }
}
