using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Shoot Behaviors/Spread Shot")]
public class SpreadShot : ScriptableShootBehavior
{
    [SerializeField] private int bulletsPerShot = 5;
    [SerializeField] private float spreadAngle = 10f;

    public override void Shoot(WeaponData data, Transform origin, Vector2 direction)
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            float angle = (i - (bulletsPerShot - 1) / 2f) * spreadAngle;
            Vector2 spreadDir = Quaternion.Euler(0, 0, angle) * direction;

            BulletController bullet = Instantiate(data.bulletPrefab, origin.position, Quaternion.identity);
            bullet.Initialize(data.bulletData, spreadDir);
        }
    }
}