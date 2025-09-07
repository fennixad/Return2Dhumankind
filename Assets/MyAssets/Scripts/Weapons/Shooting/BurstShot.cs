using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Dispara ráfagas de balas con un pequeño delay entre ellas.
/// </summary>
[CreateAssetMenu(menuName = "Weapons/Shoot Behaviors/Burst Shot")]
public class BurstShot : ScriptableShootBehavior
{
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
            BulletController bullet = Instantiate(data.bulletPrefab, origin.position, Quaternion.identity);
            bullet.Initialize(data.bulletData, direction);
            yield return new WaitForSeconds(burstDelay);
        }
    }
}
