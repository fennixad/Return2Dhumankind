using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Planeta")]
    public Transform planetCenter;
    public PlanetRotator planetRotator;

    [Header("Movimiento")]
    public float moveSpeed = 1f;
    public float movementLimit = 5f;

    [Header("Salto y Jetpack")]
    public float jumpForce = 5f;
    public float gravityStrength = 9.81f;
    public float jetpackForce = 15f;
    public float maxJetpackSpeed = 5f;
    public float jetpackBoostMultiplier = 25f;
    public float minFallSpeedForBoost = 7f;
    public float maxHeight = 10f;


    [Header("Fuel del Jetpack")]
    public float maxFuel = 100f;
    public float fuelConsumptionRate = 20f;   // unidades por segundo
    public float fuelRegenRate = 15f;         // unidades por segundo cuando estás en el suelo
    [SerializeField] private float currentFuel;
    [SerializeField] private bool isOutOfFuel = false;

    [Header("Detección de suelo")]
    public float groundCheckDistance = 0.4f;
    public LayerMask groundLayer;

    // Estado
    private float currentMovementDistance = 0f;
    private bool isGrounded;
    private bool wasUsingJetpack = false;
    private bool jumpRequestedWhileAirborne = false;


    // Input
    private float _horizontalInput;
    private bool _jumpButtonHeld;
    private bool _jumpInputBuffer;

    // Física y orientación
    private Rigidbody2D rb;
    private Vector2 lastPosition;
    private Vector2 _toCenter;
    private Vector2 _tangent;
    private float _planetRadius;

    public Transform playerGraphicsTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start()
    {
        lastPosition = transform.position;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        _planetRadius = Vector2.Distance(transform.position, planetCenter.position);
        currentFuel = maxFuel; // inicializamos con el fuel lleno
    }

    private void Update()
    {
        GatherInput();
        CalculateTangentialDistance();
        HandlePlanetRotation();
        AlignPlayerOrientation();
    }

    private void FixedUpdate()
    {
        CheckIsGrounded();
        UpdateDirectionVectors();
        HandleMovement();
        HandleJump();
        HandleJetpack();
        ApplyGravity();
        // Regenera fuel si estás en el suelo
        if (isGrounded && currentFuel < maxFuel)
        {
            currentFuel += fuelRegenRate * Time.fixedDeltaTime;
            currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        }
        _jumpInputBuffer = false; // Limpiamos el input buffer tras procesarlo
    }

    private void GatherInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _jumpButtonHeld = Input.GetButton("Jump");

        // Registrar buffer de pulsación inicial
        if (Input.GetButtonDown("Jump"))
        {
            _jumpInputBuffer = true;
        }

        // Si estamos en el aire y se está manteniendo espacio, registramos intención de salto
        if (!isGrounded && _jumpButtonHeld)
        {
            jumpRequestedWhileAirborne = true;
        }

        // Si se suelta el salto, se cancela esa intención
        if (!Input.GetButton("Jump"))
        {
            jumpRequestedWhileAirborne = false;
        }
    }

    private void UpdateDirectionVectors()
    {
        _toCenter = (planetCenter.position - transform.position).normalized;
        _tangent = new Vector2(-_toCenter.y, _toCenter.x);
    }
    /*
    private void AlignPlayerOrientation()
    {
        transform.up = -_toCenter;
    }
    */
    private void AlignPlayerOrientation()
    {
        if (playerGraphicsTransform != null)
        {
            playerGraphicsTransform.up = -_toCenter; // Rotar solo el GFX del jugador
        }
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
        if (planetRotator != null)
            planetRotator.RotateWithPlayer(_horizontalInput);
    }

    private void HandleMovement()
    {
        float limitedInput = _horizontalInput;

        if (_horizontalInput > 0 && currentMovementDistance >= movementLimit)
            limitedInput = 0;
        else if (_horizontalInput < 0 && currentMovementDistance <= -movementLimit)
            limitedInput = 0;

        Vector2 desiredTangentialVelocity = _tangent * limitedInput * moveSpeed;

        float radialVelocity = Vector2.Dot(rb.linearVelocity, _toCenter);

        // ✅ Si estás en el suelo y no estás intentando saltar ni usar jetpack, limpiamos posibles rebotes.
        bool groundedAndIdle = isGrounded && !_jumpInputBuffer && !_jumpButtonHeld && !jumpRequestedWhileAirborne;

        if (groundedAndIdle && radialVelocity < 0f)
        {
            radialVelocity = 0f;
        }

        rb.linearVelocity = desiredTangentialVelocity + radialVelocity * _toCenter;
    }

    private void HandleJump()
    {
        // Si acabas de pulsar espacio y estás en el suelo
        if (_jumpInputBuffer && isGrounded)
        {
            PerformJump();
            return;
        }

        // Si venías cayendo, tocaste suelo y mantenías espacio
        if (jumpRequestedWhileAirborne && isGrounded && _jumpButtonHeld)
        {
            PerformJump();
            return;
        }
    }
    
    private void PerformJump()
    {
        // Cancelamos cualquier velocidad descendente
        Vector2 tangentVelocity = Vector2.Dot(rb.linearVelocity, _tangent) * _tangent;
        rb.linearVelocity = tangentVelocity;

        // Impulso hacia fuera del planeta
        rb.AddForce(-_toCenter * jumpForce, ForceMode2D.Impulse);

        // Limpiamos los flags
        _jumpInputBuffer = false;
        jumpRequestedWhileAirborne = false;
    }
   
    /*
    private void PerformJump()
    {
        rb.linearVelocityY = 0f; // Reseteamos la velocidad vertical antes de saltar
        float _force = Mathf.Sqrt(2f * jumpForce * gravityStrength); // Calculamos la fuerza de salto basada en la gravedad
        rb.AddForce(Vector2.up * _force, ForceMode2D.Impulse);
        _jumpInputBuffer = false;
        jumpRequestedWhileAirborne = false;
    }
    */
    private void HandleJetpack()
    {
        bool usingJetpackThisFrame = _jumpButtonHeld && !isGrounded;

        if (!usingJetpackThisFrame)
        {
            wasUsingJetpack = false;
            isOutOfFuel = false;
            return;
        }

        // No hacemos nada si no hay fuel
        if (currentFuel <= 0f)
        {
            isOutOfFuel = true;
            return;
        }

        float distanceToCenter = Vector2.Distance(transform.position, planetCenter.position);
        if (distanceToCenter >= maxHeight)
        {
            float upwardVelocity = Vector2.Dot(rb.linearVelocity, -_toCenter);
            if (upwardVelocity > 0f)
            {
                Vector2 tangentVel = Vector2.Dot(rb.linearVelocity, _tangent) * _tangent;
                rb.linearVelocity = tangentVel;
            }

            wasUsingJetpack = false;
            return;
        }

        float radialSpeed = Vector2.Dot(rb.linearVelocity, -_toCenter);
        float fallSpeed = Vector2.Dot(rb.linearVelocity, _toCenter);
        float force = jetpackForce;

        if (!wasUsingJetpack && fallSpeed > minFallSpeedForBoost)
            force *= jetpackBoostMultiplier;

        if (radialSpeed < maxJetpackSpeed)
        {
            rb.AddForce(-_toCenter * force, ForceMode2D.Force);
            currentFuel -= fuelConsumptionRate * Time.fixedDeltaTime;
            currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);
        }

        wasUsingJetpack = true;
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
            rb.AddForce(_toCenter * gravityStrength, ForceMode2D.Force);
    }

    private void CheckIsGrounded()
    {
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, _toCenter, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
    }
}