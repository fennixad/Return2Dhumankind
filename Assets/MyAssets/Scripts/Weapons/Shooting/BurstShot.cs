using System.Collections;
using MyAssets.Scripts.Bullets;
using Unity.VisualScripting;
using UnityEngine;

namespace MyAssets.Scripts.Weapons.Shooting
{
    /// <summary>
    /// Dispara r�fagas de balas con un peque�o delay entre ellas.
    /// </summary>
    [CreateAssetMenu(menuName = "Weapons/Shooting/Burst Shot")]
    public class BurstShot : ScriptableShootBehavior
    {
        [Header("Burst Settings")]
        [SerializeField] private int burstCount = 3;

        [SerializeField] private float burstDelay = 0.1f;

        public override void Shoot(WeaponData data, Transform origin, Vector2 direction)
        {
            CoroutineRunner.instance.StartCoroutine(BurstRoutine(data, origin, direction));
        }

        private IEnumerator BurstRoutine(WeaponData data, Transform origin, Vector2 direction)
        {
            for (int i = 0; i < burstCount; i++)
            {
                BulletController bullet = Instantiate(
                    data.bulletPrefab,
                    origin.position,
                    Quaternion.LookRotation(Vector3.forward, direction) // 🔑
                );

                bullet.Initialize(data.bulletData, direction);

                yield return new WaitForSeconds(burstDelay);
            }
        }
    }
}
