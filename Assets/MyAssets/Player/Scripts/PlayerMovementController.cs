using UnityEngine;
public class PlayerMovementController : MonoBehaviour
{
    [Header("Configuración del Planeta")]
    [Tooltip("Referencia al Transform del centro del planeta.")]
    public Transform planetCenter;
    [Tooltip("La velocidad a la que el jugador se mueve tangencialmente sobre el planeta.")]
    public float moveSpeed = 5f;
    [Tooltip("La fuerza ascendente aplicada cuando el jugador salta.")]
    public float jumpForce = 10f; // <-- Reincorporado
    [Tooltip("La intensidad de la fuerza gravitacional hacia el centro del planeta.")]
    public float gravityStrength = 9.81f;
    [Tooltip("Referencia al script PlanetRotator en el objeto del planeta.")]
    public PlanetRotator planetRotator;

    // Eliminadas: Configuración del Jetpack y Fuel

    [Header("Límites de Movimiento")]
    [Tooltip("La máxima 'distancia' que el jugador puede moverse a izquierda o derecha a lo largo de la superficie del planeta desde su punto de inicio.")]
    public float movementLimit = 5f;

    // --- NUEVAS VARIABLES PARA EL RAYCAST DE SUELO ---
    [Header("Detección de Suelo (Raycast)")]
    [Tooltip("La distancia desde el centro del jugador donde se lanzará el Raycast para detectar el suelo.")]
    public float groundCheckDistance = 0.4f; // Ajusta esto según el tamaño de tu jugador y su collider.
    [Tooltip("La capa (Layer) que se considera 'suelo'.")]
    public LayerMask groundLayer;

    [Header("Estado Interno (Depuración)")]
    [SerializeField, Tooltip("Distancia de movimiento acumulada actual a lo largo de la superficie del planeta.")]
    private float currentMovementDistance = 0f;
    [SerializeField, Tooltip("¿Está el jugador actualmente tocando el suelo según el Raycast?")]
    private bool isGrounded;
    // Eliminado: currentFuel

    // --- Referencias y Caché ---
    private Rigidbody2D rb;
    private Vector2 lastPosition;
    private Vector2 _toCenter;
    private Vector2 _tangent;
    private float _planetRadius;

    // --- Variables para el Input ---
    private float _horizontalInput;
    private bool _jumpButtonDown; // <-- Reincorporado
    // Eliminada: _jumpButtonHeld
    private bool _jumpInputBuffer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Controlamos la gravedad manualmente.
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.linearVelocity = Vector2.zero; // Aseguramos que no haya velocidad inicial residual.
        rb.angularVelocity = 0f;
    }

    void Start()
    {
        // isGrounded se inicializará por el Raycast en FixedUpdate
        lastPosition = transform.position;
        // Eliminada: currentFuel = maxFuel;

        // Calcula el radio inicial del planeta asumiendo que el jugador empieza en la superficie.
        _planetRadius = Vector2.Distance(transform.position, planetCenter.position);
    }

    void Update()
    {
        GatherInput(); // Recoge el input aquí
        UpdateDirectionVectors();
        CalculateTangentialDistance();
        // Eliminado: HandleFuel();

        HandlePlanetRotation(); // Rotación visual del planeta, puede ir en Update
        AlignPlayerOrientation(); // Orientación visual del jugador, puede ir en Update
    }

    private void FixedUpdate()
    {
        CheckIsGrounded(); // ¡Detecta el suelo en FixedUpdate para estar sincronizado con la física!
        HandleMovement();
        HandleJump(); // Llama a la lógica de salto en FixedUpdate
        // Eliminado: HandleJetpack();
        ApplyGravity(); // Aplica la gravedad en FixedUpdate
        _jumpInputBuffer = false; // Resetea el buffer de salto después de procesar el input
    }

    /// <summary>
    /// Centraliza toda la lectura de input en un solo lugar.
    /// </summary>
    private void GatherInput()
    {
        /*
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _jumpButtonDown = Input.GetButtonDown("Jump"); // Solo capturamos el evento de pulsación
        */
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        // Si el botón de salto se presiona en este frame de Update,
        // establece _jumpInputBuffer en true. Se mantendrá true hasta el FixedUpdate
        if (Input.GetButtonDown("Jump"))
        {
            _jumpInputBuffer = true;
        }
        // Nota: NO resetees _jumpInputBuffer aquí. Se reseteará en FixedUpdate.
    }

    private void UpdateDirectionVectors()
    {
        _toCenter = (planetCenter.position - transform.position).normalized;
        _tangent = new Vector2(-_toCenter.y, _toCenter.x);
    }

    private void CalculateTangentialDistance()
    {
        Vector2 currentDelta = (Vector2)transform.position - lastPosition;
        float tangentialMovement = Vector2.Dot(currentDelta, _tangent);
        currentMovementDistance += tangentialMovement;
        lastPosition = transform.position;
    }

    private void HandlePlanetRotation()
    {
        if (planetRotator != null) // Añadir comprobación de nulidad para seguridad
        {
            planetRotator.RotateWithPlayer(_horizontalInput);
        }
    }

    private void HandleMovement()
    {
        float limitedInput = _horizontalInput;
        if (_horizontalInput > 0 && currentMovementDistance >= movementLimit)
        {
            limitedInput = 0;
        }
        else if (_horizontalInput < 0 && currentMovementDistance <= -movementLimit)
        {
            limitedInput = 0;
        }

        Vector2 desiredTangentialVelocity = _tangent * limitedInput * moveSpeed;
        float radialVelocity = Vector2.Dot(rb.linearVelocity, _toCenter); // Velocidad radial actual
        rb.linearVelocity = desiredTangentialVelocity + radialVelocity * _toCenter; // Mantiene la velocidad radial existente
    }

    /// <summary>
    /// Maneja la lógica de salto del jugador.
    /// </summary>
    private void HandleJump()
    {
        // Solo permite saltar si el botón de salto fue presionado y el jugador está en el suelo.
        if (_jumpInputBuffer && isGrounded)
        {
            // Aplica una fuerza instantánea opuesta a la gravedad (hacia "arriba" del planeta).
            // ForceMode2D.Impulse es ideal para saltos, ya que aplica una fuerza al instante.
            rb.AddForce(-_toCenter * jumpForce, ForceMode2D.Impulse);
            // Si quieres un "salto con fricción cero" mientras sube, puedes añadir un chequeo
            // isGrounded = false; (aunque CheckIsGrounded lo hará en el siguiente FixedUpdate)
        }
    }

    // Eliminado: private void HandleJetpack() { ... }

    // Eliminado: private void HandleFuel() { ... }

    private void ApplyGravity()
    {
        // La gravedad solo se aplica si el jugador NO está en el suelo.
        // Si está en el suelo, el Raycast detectará el contacto.
        
        if (!isGrounded)
        {
            // Aplica una fuerza continua hacia el centro del planeta.
            // ForceMode2D.Force es para fuerzas continuas, como la gravedad.
            // Dibuja un rayo azul desde el jugador hacia donde se aplica la gravedad.
            // Debería apuntar consistentemente hacia el centro del planeta.
            Debug.DrawRay(transform.position, _toCenter * 0.2f, Color.blue);
            rb.AddForce(_toCenter * gravityStrength, ForceMode2D.Force);
        }
    }

    private void AlignPlayerOrientation()
    {
        // Asegura que el "arriba" del jugador siempre apunte lejos del centro del planeta.
        transform.up = -_toCenter;
    }

    /// <summary>
    /// Comprueba si el jugador está en el suelo usando un Raycast2D.
    /// Lanza un rayo hacia el centro del planeta desde el jugador.
    /// </summary>
    private void CheckIsGrounded()
    {
        // Origen del rayo: la posición del jugador.
        Vector2 rayOrigin = transform.position;

        // Dirección del rayo: hacia el centro del planeta.
        // _toCenter ya está normalizado y apunta del jugador al centro.
        Vector2 rayDirection = _toCenter;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, groundCheckDistance, groundLayer);

        // Se dibuja verde si golpea el suelo, rojo si no.
        //Debug.DrawRay(rayOrigin, rayDirection * groundCheckDistance, hit.collider != null ? Color.green : Color.red);

        // Actualiza la variable isGrounded.
        isGrounded = hit.collider != null;
    }
    // Eliminados: Los métodos OnCollisionEnter2D, OnCollisionStay2D, OnCollisionExit2D
}