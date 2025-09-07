using UnityEngine;

/// <summary>
/// Interfaz para diferentes formas de calcular la dirección de disparo.
/// Esto permite que el jugador apunte con el ratón,
/// mientras que un enemigo podría apuntar hacia el Player.
/// </summary>
public interface IAimStrategy
{
    /// <param name="shooter">Transform del que dispara (arma, player o enemigo).</param>
    /// <param name="target">Transform del objetivo (opcional, puede ser null).</param>
    /// <returns>Vector2 con la dirección normalizada de disparo.</returns>
    Vector2 GetDirection(Transform shooter, Transform target = null);
}
