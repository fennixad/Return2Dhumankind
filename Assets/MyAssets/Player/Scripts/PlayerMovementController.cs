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


    [Header("Configuración del Jetpack")]
    [Tooltip("La fuerza MÁXIMA de empuje del jetpack (a nivel del suelo).")]
    public float jetpackForce = 15f;
    [Tooltip("La altura sobre la superficie a la que el jetpack pierde toda su fuerza extra.")]
    public float jetpackMaxHeight = 10f; // <-- NUEVO: Techo de altura para el jetpack.
    [Tooltip("La cantidad máxima de combustible del jetpack.")]
    public float maxFuel = 100f;
    [Tooltip("La cantidad de combustible que se gasta por segundo.")]
    public float fuelConsumptionRate = 30f;
    [Tooltip("La cantidad de combustible que se regenera por segundo cuando está en el suelo.")]
    public float fuelRegenerationRate = 40f;


    [Header("Límites de Movimiento")]
    [Tooltip("La máxima 'distancia' que el jugador puede moverse a izquierda o derecha a lo largo de la superficie del planeta desde su punto de inicio.")]
    public float movementLimit = 5f;


    [Header("Estado Interno (Depuración)")]
    [SerializeField, Tooltip("Distancia de movimiento acumulada actual a lo largo de la superficie del planeta.")]
    private float currentMovementDistance = 0f;
    [SerializeField, Tooltip("¿Está el jugador actualmente tocando un collider con la etiqueta 'Ground'?")]
    private bool isGrounded;
    [SerializeField, Tooltip("Combustible actual del jetpack.")]
    private float currentFuel;


    // --- Referencias y Caché ---
    private Rigidbody2D rb;
    private Vector2 lastPosition;
    private Vector2 _toCenter;
    private Vector2 _tangent;
    private float _planetRadius; // <-- NUEVO: Para calcular la altura sobre la superficie.

    // --- Variables para el Input ---
    // NUEVO: Almacenamos el input para usarlo de forma segura en FixedUpdate
    private float _horizontalInput;
    private bool _jumpButtonDown;
    private bool _jumpButtonHeld;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Start()
    {
        isGrounded = false;
        lastPosition = transform.position;
        currentFuel = maxFuel;
        
        // NUEVO: Calculamos el radio del planeta al inicio para saber la altura real.
        _planetRadius = Vector2.Distance(transform.position, planetCenter.position);
    }

    // Update se usa para leer Inputs y lógicas que no son de física.
    void Update()
    {
        GatherInput();
        UpdateDirectionVectors();
        CalculateTangentialDistance();
        //HandleFuel(); // La lógica de consumir/regenerar combustible es por tiempo, puede ir aquí.

        // Estas dos funciones son visuales, no aplican fuerzas, por lo que están bien en Update.
        HandlePlanetRotation();
        AlignPlayerOrientation();
    }

    // FixedUpdate se usa para TODAS las operaciones de física (fuerzas, velocidad).
    private void FixedUpdate()
    {
        // CAMBIADO: Todas las funciones que usan el Rigidbody se llaman desde aquí.
        HandleMovement();
        HandleJump();
        //HandleJetpack();
        ApplyGravity();
    }

    /// <summary>
    /// NUEVO: Centraliza toda la lectura de input en un solo lugar.
    /// </summary>
    private void GatherInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _jumpButtonDown = Input.GetButtonDown("Jump");
        //_jumpButtonHeld = Input.GetButton("Jump");
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
    
    // CAMBIADO: Ya no necesita recibir el input como parámetro.
    private void HandlePlanetRotation()
    {
        planetRotator.RotateWithPlayer(_horizontalInput);
    }

    // CAMBIADO: Esta función ahora se llama HandleMovement y usa el input cacheado.
    private void HandleMovement()
    {
        // Limitamos el movimiento
        float limitedInput = _horizontalInput;
        if (_horizontalInput > 0 && currentMovementDistance >= movementLimit)
        {
            limitedInput = 0;
        }
        else if (_horizontalInput < 0 && currentMovementDistance <= -movementLimit)
        {
            limitedInput = 0;
        }

        // Aplicamos la velocidad
        Vector2 desiredTangentialVelocity = _tangent * limitedInput * moveSpeed;
        float radialVelocity = Vector2.Dot(rb.linearVelocity, _toCenter);
        rb.linearVelocity = desiredTangentialVelocity + radialVelocity * _toCenter;
    }
    
    // CAMBIADO: Usa la variable de input cacheada.
    private void HandleJump()
    {
        if (_jumpButtonDown && isGrounded)
        {
            rb.AddForce(-_toCenter * jumpForce, ForceMode2D.Impulse);
        }
    }

    // CAMBIADO: La lógica del jetpack ahora está separada de la del combustible.
    private void HandleJetpack()
    {
        if (_jumpButtonHeld && !isGrounded && currentFuel > 0)
        {
            // --- NUEVO: Lógica de reducción de fuerza con la altura ---
            
            // 1. Calcular la altura actual sobre la superficie.
            float distanceToCenter = Vector2.Distance(transform.position, planetCenter.position);
            float currentAltitude = distanceToCenter - _planetRadius;

            // 2. Calcular qué porcentaje de la altura máxima hemos alcanzado (de 0.0 a 1.0).
            float heightFactor = Mathf.Clamp01(currentAltitude / jetpackMaxHeight);

            // 3. Interpolar la fuerza. A altura 0, la fuerza es 'jetpackForce'. A altura máxima, la fuerza es 0.
            float dynamicJetpackForce = Mathf.Lerp(jetpackForce, 0f, heightFactor);

            // 4. Aplicar la fuerza dinámica calculada.
            rb.AddForce(-_toCenter * dynamicJetpackForce, ForceMode2D.Force);
        }
    }
    
    // NUEVO: La lógica del combustible ahora está separada para mayor claridad.
    private void HandleFuel()
    {
        // Consumir combustible si se usa el jetpack
        if (_jumpButtonHeld && !isGrounded && currentFuel > 0)
        {
            currentFuel -= fuelConsumptionRate * Time.deltaTime;
        }
        // Regenerar combustible en el suelo
        else if (isGrounded && currentFuel < maxFuel)
        {
            currentFuel += fuelRegenerationRate * Time.deltaTime;
        }

        // Asegurarse de que el combustible nunca se pase de los límites
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
    }
    
    private void ApplyGravity()
    {
        // Para que el jetpack no anule completamente la gravedad, nos aseguramos de aplicarla siempre.
        if (!isGrounded)
        {
            rb.AddForce(_toCenter * gravityStrength);
        }
    }

    private void AlignPlayerOrientation()
    {
        transform.up = -_toCenter;
    }

    // --- Detección de Colisiones ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Añadido por seguridad, para asegurar que isGrounded sea true si estamos en contacto continuo.
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


