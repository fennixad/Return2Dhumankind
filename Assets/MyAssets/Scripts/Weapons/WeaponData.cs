using MyAssets.Scripts.Bullets;
using MyAssets.Scripts.Weapons.Shooting;
using UnityEngine;

namespace MyAssets.Scripts.Weapons
{
    /// <summary>
    /// Configuración de un arma. Editable desde el inspector.
    /// </summary>
    [CreateAssetMenu(menuName = "Weapons/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("General")]
        public string weaponName;
        public Sprite sprite; // Sprite para mostrar en el arma o en la UI

        [Header("Stats")]
        public float fireRate = 0.2f;
        public float reloadTime = 1f;

        [Header("Ammo")]
        public BulletController bulletPrefab; // Prefab genérico de bala
        public BulletData bulletData;         // Datos de la bala (tipo específico)

        [Header("Shooting Behavior")]
        public ScriptableShootBehavior shootBehavior; // Strategy Pattern
    }
}
