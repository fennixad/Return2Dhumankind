using UnityEngine;

/// <summary>
/// Configuración de las balas.
/// </summary>
[CreateAssetMenu(fileName = "NewBulletData", menuName = "Weapons/Bullet Data")]
public class BulletData : ScriptableObject
{
    public string bulletName;
    public Sprite sprite;
    public int damage = 10;
    public float speed = 20f;
    public float lifeTime = 2f;
    public float gravity = 0f;
}
