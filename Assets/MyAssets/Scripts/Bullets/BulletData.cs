using UnityEngine;

namespace MyAssets.Scripts.Bullets
{
    /// <summary>
    /// Configuración de una bala. 
    /// Cada tipo de bala se define como un ScriptableObject.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBulletData", menuName = "Weapons/Bullets/Data")]
    public class BulletData : ScriptableObject
    {
        [Header("General")]
        public string bulletName;
        public Sprite sprite;

        [Header("Stats")]
        public int damage = 10;
        public float speed = 20f;
        public float lifeTime = 2f;
        public float gravity = 0f; // Si quieres balas en parábola
    }
}
