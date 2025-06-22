using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    [Header("Configuración del Planeta")]
    [Tooltip("Referencia al Transform del centro del planeta.")]
    public Transform planetCenter;
    [Tooltip("La velocidad a la que el jugador se mueve tangencialmente sobre el planeta.")]
    public float moveSpeed = 5f; 
    [Tooltip("La fuerza ascendente aplicada cuando el jugador salta.")]
    public float jumpForce = 10f; 
    [Tooltip("La intensidad de la fuerza gravitacional hacia el centro del planeta.")]
    public float gravityStrength = 9.81f; 

    [Tooltip("Referencia al script PlanetRotator en el objeto del planeta.")]
    public PlanetRotator planetRotator;

    // --- Límites de Movimiento ---
    [Header("Límites de Movimiento")]
    [Tooltip("La máxima 'distancia' que el jugador puede moverse a izquierda o derecha a lo largo de la superficie del planeta desde su punto de inicio.")]
    public float movementLimit = 5f;

    // --- Estado Interno (Depuración) ---
    [Header("Estado Interno (Depuración)")]
    [SerializeField, Tooltip("Distancia de movimiento acumulada actual a lo largo de la superficie del planeta.")]
    private float currentMovementDistance = 0f; // Rastrea la distancia total movida
    [SerializeField, Tooltip("¿Está el jugador actualmente tocando un collider con la etiqueta 'Ground'?")]
    private bool isGrounded;

    // --- Referencias Privadas ---
    private Rigidbody2D rb;
    private Vector2 lastPosition; // Para calcular la distancia movida entre frames

    // --- Direcciones Almacenadas en Caché ---
    // Se actualizan en cada frame para mayor eficiencia
    private Vector2 _toCenter;
    private Vector2 _tangent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Deshabilita la gravedad predeterminada de Unity, nosotros manejaremos una gravedad personalizada
        rb.freezeRotation = true; // Congela la rotación para que no gire el jugador con las colisiones
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Suaviza el movimiento en bajas velocidades de frame
    }

    void Start()
    {
        isGrounded = false;
        lastPosition = transform.position; // Inicializa lastPosition para el cálculo de distancia
    }

    void Update()
    {
        UpdateDirectionVectors();
        CalculateTangentialDistance();

        float horizontalInput = Input.GetAxisRaw("Horizontal");

        HandlePlanetRotation(horizontalInput);

        float playerMovementInput = LimitPlayerMovement(horizontalInput);
        HandlePlayerTangentialMovement(playerMovementInput);

        HandleJump();
        ApplyGravity();
        AlignPlayerOrientation();
    }

    /// <summary>
    /// Actualiza los vectores de dirección (hacia el centro y tangente) basándose en la posición actual del jugador relativa al centro del planeta.
    /// </summary>
    private void UpdateDirectionVectors()
    {
        _toCenter = (planetCenter.position - transform.position).normalized;
        _tangent = new Vector2(-_toCenter.y, _toCenter.x);
    }

    /// <summary>
    /// Calcula la distancia tangencial recorrida por el jugador desde el último frame
    /// y actualiza la distancia total 'currentMovementDistance'.
    /// </summary>
    private void CalculateTangentialDistance()
    {
        Vector2 currentDelta = (Vector2)transform.position - lastPosition;
        float tangentialMovement = Vector2.Dot(currentDelta, _tangent);
        currentMovementDistance += tangentialMovement;
        lastPosition = transform.position;
    }

    /// <summary>
    /// Rota el planeta basándose en la entrada horizontal cruda del jugador.
    /// </summary>
    /// <param name="input">La entrada horizontal cruda del jugador (ej. -1, 0, 1).</param>
    private void HandlePlanetRotation(float input)
    {
        // La rotación del planeta ahora es controlada directamente por el input, con parada en seco si input es 0
        planetRotator.RotateWithPlayer(input);
    }

    /// <summary>
    /// Limita el movimiento horizontal del jugador basándose en el 'movementLimit' definido.
    /// El planeta seguirá rotando basándose en la entrada cruda, pero la
    /// traslación del jugador se detendrá si se alcanza el límite.
    /// </summary>
    /// <param name="rawInput">La entrada horizontal cruda del jugador.</param>
    /// <returns>El valor de entrada modificado (0 si se alcanza el límite, de lo contrario rawInput).</returns>
    private float LimitPlayerMovement(float rawInput)
    {
        float limitedInput = rawInput;
        if (rawInput > 0 && currentMovementDistance >= movementLimit)
        {
            limitedInput = 0; // Detiene al jugador de moverse a la derecha
        }
        else if (rawInput < 0 && currentMovementDistance <= -movementLimit)
        {
            limitedInput = 0; // Detiene al jugador de moverse a la izquierda
        }
        return limitedInput;
    }

    /// <summary>
    /// Aplica velocidad tangencial al Rigidbody2D del jugador basándose en la entrada procesada.
    /// Preserva la velocidad radial existente para evitar que se "pegue" al planeta.
    /// </summary>
    /// <param name="input">La entrada horizontal (potencialmente limitada) para el movimiento del jugador.</param>
    private void HandlePlayerTangentialMovement(float input)
    {
        // Calcula la velocidad tangencial deseada.
        // Multiplicamos por `moveSpeed` para establecer la velocidad directamente.
        Vector2 desiredTangentialVelocity = _tangent * input * moveSpeed;

        // Mantenemos la componente de velocidad actual hacia/desde el centro para evitar 'adherencias'
        float radialVelocity = Vector2.Dot(rb.linearVelocity, _toCenter);

        // Establecemos la velocidad lineal del Rigidbody directamente.
        // Esto crea el efecto de "parada en seco" si el input es 0,
        // y movimiento instantáneo a 'moveSpeed' si hay input.
        rb.linearVelocity = desiredTangentialVelocity + radialVelocity * _toCenter;
    }

    /// <summary>
    /// Maneja la acción de salto del jugador si se presiona el botón de salto y el jugador está en el suelo.
    /// Aplica una fuerza de impulso alejándose del centro del planeta.
    /// </summary>
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(-_toCenter * jumpForce, ForceMode2D.Impulse);
            isGrounded = false; // El jugador ya no está en el suelo después de saltar
        }
    }

    /// <summary>
    /// Aplica la fuerza gravitacional hacia el centro del planeta usando la 'gravityStrength' definida.
    /// </summary>
    private void ApplyGravity()
    {
        rb.AddForce(_toCenter * gravityStrength);
    }

    /// <summary>
    /// Alinea la dirección 'arriba' del jugador con la inversa del vector hacia el centro del planeta,
    /// asegurando que el jugador siempre esté erguido con respecto a la superficie del planeta.
    /// </summary>
    private void AlignPlayerOrientation()
    {
        transform.up = -_toCenter;
    }

    /// <summary>
    /// Llamado cuando otro collider (2D) entra en el collider de este objeto.
    /// Se usa para detectar cuándo el jugador aterriza en el "Ground".
    /// </summary>
    /// <param name="collision">Los datos de Collision2D asociados con esta colisión.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    /// <summary>
    /// Llamado cuando otro collider (2D) ha salido del collider de este objeto.
    /// Se usa para detectar cuándo el jugador deja el "Ground" (ej. después de saltar o caerse de un borde).
    /// </summary>
    /// <param name="collision">Los datos de Collision2D asociados con esta colisión.</param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}


