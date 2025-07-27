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

        _jumpInputBuffer = false; // Limpiamos el input buffer tras procesarlo
    }

    private void GatherInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        _jumpButtonHeld = Input.GetButton("Jump");

        // Buffer de pulsación
        if (Input.GetButtonDown("Jump"))
        {
            _jumpInputBuffer = true;
        }

        // Si mantienes espacio y estás en el aire, recuerda que quieres saltar al tocar suelo
        if (_jumpButtonHeld && !isGrounded)
        {
            jumpRequestedWhileAirborne = true;
        }
    }

    private void UpdateDirectionVectors()
    {
        _toCenter = (planetCenter.position - transform.position).normalized;
        _tangent = new Vector2(-_toCenter.y, _toCenter.x);
    }

    private void AlignPlayerOrientation()
    {
        transform.up = -_toCenter;
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
        rb.linearVelocity = desiredTangentialVelocity + radialVelocity * _toCenter;
    }

    private void HandleJump()
    {
        // Caso 1: salto normal con buffer
        if (_jumpInputBuffer && isGrounded)
        {
            PerformJump();
        }
        // Caso 2: estabas cayendo y tocaste el suelo mientras mantenías espacio
        else if (jumpRequestedWhileAirborne && isGrounded)
        {
            PerformJump();
        }
    }
    private void PerformJump()
    {
        rb.AddForce(-_toCenter * jumpForce, ForceMode2D.Impulse);
        _jumpInputBuffer = false;
        jumpRequestedWhileAirborne = false;
    }
    private void HandleJetpack()
    {
        bool usingJetpackThisFrame = _jumpButtonHeld && !isGrounded;

        if (!usingJetpackThisFrame)
        {
            wasUsingJetpack = false;
            return;
        }

        float distanceToCenter = Vector2.Distance(transform.position, planetCenter.position);
        if (distanceToCenter >= maxHeight)
        {
            float upwardVelocity = Vector2.Dot(rb.linearVelocity, -_toCenter);
            if (upwardVelocity > 0f)
            {
                Vector2 tangentVel = Vector2.Dot(rb.linearVelocity, _tangent) * _tangent;
                rb.linearVelocity = tangentVel; // Cancelamos la subida
            }

            wasUsingJetpack = false;
            return;
        }

        float radialSpeed = Vector2.Dot(rb.linearVelocity, -_toCenter); // Hacia arriba
        float fallSpeed = Vector2.Dot(rb.linearVelocity, _toCenter);    // Hacia abajo

        float force = jetpackForce;

        if (!wasUsingJetpack && fallSpeed > minFallSpeedForBoost)
            force *= jetpackBoostMultiplier;

        if (radialSpeed < maxJetpackSpeed)
            rb.AddForce(-_toCenter * force, ForceMode2D.Force);

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