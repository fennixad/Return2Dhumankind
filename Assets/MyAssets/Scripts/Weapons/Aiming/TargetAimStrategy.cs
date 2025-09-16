using UnityEngine;

namespace MyAssets.Scripts.Weapons.Aiming
{
    /// <summary>
    /// Estrategia de apuntado hacia un objetivo fijo.
    /// Ejemplo: torretas o enemigos que disparan al jugador.
    /// </summary>
    public class TargetAimStrategy : MonoBehaviour, IAimStrategy
    {
        [SerializeField] private Transform target;

        public Vector2 GetDirection(Transform shooter, Transform targetOverride = null)
        {
            Transform effectiveTarget = targetOverride ?? target;
            if (effectiveTarget == null) return Vector2.right;

            return (effectiveTarget.position - shooter.position).normalized;
        }
    }
}
