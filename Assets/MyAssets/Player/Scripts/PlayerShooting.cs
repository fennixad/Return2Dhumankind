using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private Camera cam;
    public Transform player; // Referencia al Transform del GameObject del jugador

    [Header("Ajuste de Rotación")]
    [Tooltip("Ángulo de ajuste si el sprite de tu arma no apunta 'hacia la derecha' por defecto. Ajusta en el Inspector.")]
    public float rotationOffset = 0f; // Lo inicializamos en 0, pero lo ajustarás en el Inspector.

    void Start()
    {
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("PlayerShooting: No se encontró una cámara principal. Asegúrate de que tu cámara principal tenga el tag 'MainCamera'.");
            enabled = false;
            return;
        }

        if (player == null)
        {
            Debug.LogError("PlayerShooting: ¡La referencia al jugador no está asignada! Asigna el GameObject del jugador en el Inspector.");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // 1. Mover el arma a la posición del jugador más el offset.
        Vector3 offsetPosition = player.position;
        transform.position = offsetPosition;

        // 2. Posición del mouse en coordenadas de mundo
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.z));

        // 3. Calcular la dirección desde la POSICIÓN ACTUAL DEL ARMA hacia el cursor
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        // 4. Calcular el ángulo de rotación
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 5. Aplicar la rotación al transform GLOBAL del arma
        // ¡CAMBIO CLAVE AQUÍ! Sumamos el rotationOffset
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }
}
