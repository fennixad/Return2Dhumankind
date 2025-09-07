using UnityEngine;

/// <summary>
/// Controlador de disparo del jugador.
/// </summary>
public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private WeaponController weapon;
    [SerializeField] private MouseAim aim; // referencia a la estrategia de apuntado

    private void Update()
    {
        Vector2 direction = aim.GetDirection(weapon.transform);

        if (Input.GetButton("Fire1"))
        {
            weapon.TryShoot(direction);
        }
    }
}
