using UnityEngine;

namespace MyAssets.Scripts.Weapons.Aiming
{
    /// <summary>
    /// Estrategia de apuntado controlada por la entrada del jugador.
    /// Puede ser ratón (PC) o stick derecho (mando).
    /// </summary>
    public class PlayerInputAimStrategy : MonoBehaviour, IAimStrategy
    {
        public Vector2 GetDirection(Transform shooter, Transform target = null)
        {
            // Caso PC: apuntar con ratón
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = shooter.position.z; // Asegura que ambos estén en el mismo plano
            Vector3 dir = mouseWorldPos - shooter.position;
            dir.z = 0f;

            // TODO: extender aquí para soportar mando (input del stick derecho)
            return dir.normalized;
        }
    }
}