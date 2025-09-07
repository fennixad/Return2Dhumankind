using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Shoot Behaviors/Single Shot")]
public class SingleShot : ScriptableShootBehavior
{
    public override void Shoot(WeaponData data, Transform origin, Vector2 direction)
    {
        BulletController bullet = Instantiate(data.bulletPrefab, origin.position, Quaternion.identity);
        bullet.Initialize(data.bulletData, direction);
    }
}
