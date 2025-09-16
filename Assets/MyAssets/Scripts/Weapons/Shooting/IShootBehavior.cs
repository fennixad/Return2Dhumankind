using UnityEngine;

namespace MyAssets.Scripts.Weapons.Shooting
{
    /// <summary>
    /// Estrategia de disparo (una bala, ráfaga, escopeta, etc).
    /// </summary>
    public interface IShootBehavior
    {
        void Shoot(WeaponData data, Transform origin, Vector2 direction);
    }
}
