using UnityEngine;

public class CuverPlayerController : MonoBehaviour
{
    public Transform planetCenter;
    public float moveSpeed;
    public float jumpForce;
    public float gravityStrength;
    public PlanetRotator planetRotator;

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
    void Start()
    {
        gravityStrength = 9.81f;
        isGrounded = false;
    }

    void Update()
    {
        Vector2 toCenter = (planetCenter.position - transform.position).normalized;
        Vector2 tangent = new Vector2(-toCenter.y, toCenter.x);

        // Movimiento tangencial
        float input = Input.GetAxis("Horizontal");
        rb.linearVelocity = tangent * input * moveSpeed + Vector2.Dot(rb.linearVelocity, toCenter) * toCenter;

        // Rotar el planeta seg�n el movimiento
        planetRotator.RotateWithPlayer(input);

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(-toCenter * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }

        // Gravedad hacia el centro
        rb.AddForce(toCenter * gravityStrength);

        // Alinear orientaci�n del jugador
        transform.up = -toCenter;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
