using UnityEngine;

/// <summary>
/// Representa un arma genérica.
/// Se le asigna un tipo de disparo y datos de configuración.
/// </summary>
public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData data;
    [SerializeField] private Transform firePoint;

    private float nextFireTime;
    public void TryShoot(Vector2 direction)
    {
        if (Time.time < nextFireTime) return;
        if (data == null || data.shootBehavior == null) return;

        data.shootBehavior.Shoot(data, firePoint, direction);
        nextFireTime = Time.time + data.fireRate;
    }
}