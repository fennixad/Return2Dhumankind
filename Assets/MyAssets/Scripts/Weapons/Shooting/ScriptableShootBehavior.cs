using UnityEngine;

/// <summary>
/// Clase base para estrategias de disparo.
/// Implementa IShootBehavior pero como ScriptableObject,
/// lo que permite configurar distintas formas de disparar desde el inspector.
/// </summary>
public abstract class ScriptableShootBehavior : ScriptableObject
{
    public abstract void Shoot(WeaponData data, Transform origin, Vector2 direction);
}
