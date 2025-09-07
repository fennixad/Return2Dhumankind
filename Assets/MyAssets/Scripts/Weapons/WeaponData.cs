using UnityEngine;

/// <summary>
/// Configuración del arma, editable desde el inspector.
/// </summary>
[CreateAssetMenu(menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("General")]
    public string weaponName;
    public Sprite sprite;

    [Header("Stats")]
    public float fireRate = 0.2f;
    public float reloadTime = 1f;

    [Header("Ammo")]
    public BulletController bulletPrefab;
    public BulletData bulletData;

    [Header("Shooting Behavior")]
    public ScriptableShootBehavior shootBehavior;
}
