using UnityEngine;

public class Aim2D : MonoBehaviour
{
    public Transform player;

    public Transform originBullet;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);

        Vector3 a = transform.position;
        Vector3 b = ray.origin + ray.direction * 10f;

        Debug.DrawLine(a, b);

        Vector3 dir = b - a;
        dir.z = 0f;
        dir.Normalize();

        float _angulos = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);
        transform.rotation = Quaternion.Euler(Vector3.forward * _angulos);

        Debug.DrawRay(originBullet.position, b - originBullet.position, Color.yellow);

        bool _flipY = player.position.x > b.x;
        spriteRenderer.flipY = _flipY;
    }
}
