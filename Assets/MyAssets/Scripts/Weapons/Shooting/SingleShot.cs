using MyAssets.Scripts.Bullets;
using UnityEngine;

namespace MyAssets.Scripts.Weapons.Shooting
{
    [CreateAssetMenu(menuName = "Weapons/Shooting/Single Shot")]
    public class SingleShot : ScriptableShootBehavior
    {
        public override void Shoot(WeaponData data, Transform origin, Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            BulletController bullet = Instantiate(
                data.bulletPrefab, 
                origin.position, 
                rotation
            );
            
            bullet.Initialize(data.bulletData, direction);
        }
    }
}
