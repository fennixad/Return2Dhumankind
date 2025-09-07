using UnityEngine;

/// <summary>
/// Controlador de disparo de enemigos.
/// </summary>
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private WeaponController weapon;
    [SerializeField] private Transform target; // El jugador

    private IAimStrategy aimStrategy;

    private void Awake()
    {
        // En este caso asignamos TargetAim
        aimStrategy = GetComponent<IAimStrategy>();
        if (aimStrategy == null)
            Debug.LogError("Falta una estrategia de apuntado (ej: TargetAim)");
    }

    private void Update()
    {
        // Ejemplo: disparo automático
        Vector2 dir = aimStrategy.GetDirection(weapon.transform, target);
        weapon.Shoot(dir);
    }
}
