using UnityEngine;

/// <summary>
/// Estrategia de apuntado hacia un objetivo fijo (ej: enemigo que dispara al player).
/// </summary>
public class EnemyAim : MonoBehaviour, IAimStrategy
{
    [SerializeField] private Transform target;

    public Vector2 GetDirection(Transform shooter, Transform targetOverride = null)
    {
        Transform effectiveTarget = targetOverride ?? target;
        if (effectiveTarget == null) return Vector2.right;

        return (effectiveTarget.position - shooter.position).normalized;
    }
}
