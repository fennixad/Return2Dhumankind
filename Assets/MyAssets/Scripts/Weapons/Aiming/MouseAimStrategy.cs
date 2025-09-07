using UnityEngine;

/// <summary>
/// Estrategia de apuntado hacia el cursor del ratón (para el jugador).
/// </summary>
public class MouseAim : MonoBehaviour, IAimStrategy
{
    [SerializeField] private Transform originBullet;
    [SerializeField] private Transform player;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public Vector2 GetDirection(Transform shooter, Transform target = null)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mouseWorldPos - shooter.position;
        dir.z = 0f;
        dir.Normalize();

        // Rotar el arma para que siga al ratón
        float angle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);
        shooter.rotation = Quaternion.Euler(Vector3.forward * angle);

        // Flip visual del arma según lado del ratón
        bool flipY = player.position.x > mouseWorldPos.x;
        spriteRenderer.flipY = flipY;

        return dir;
    }
}