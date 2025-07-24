using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// DESCRIPCION:
/// 
/// </summary>

public class Movement : MonoBehaviour
{
    // ***********************************************
    #region 1) Definicion de variables
    [SerializeField] InputAction thrust;
    [SerializeField] InputAction rotation;
    [SerializeField] float thrustStrength;
    [SerializeField] float rotationStrength;
    [SerializeField] AudioClip motor;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem leftParticles;
    [SerializeField] ParticleSystem rightParticles;

    Rigidbody rb;
    AudioSource audioSource;
    #endregion
    // ***********************************************
    #region 2) Funciones de Unity

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource= GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
         thrust.Enable();
        rotation.Enable();
    }

    private void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
    }


    #endregion
    // ***********************************************
    #region 3) Funciones originales
    private void ProcessThrust()
    {
        if (thrust.IsPressed())
        {
            
            rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
            if (!audioSource.isPlaying) 
            {
                audioSource.PlayOneShot(motor);
                thrustParticles.Play();
            }


        }
        else
        {
            audioSource.Stop();
            thrustParticles.Stop(); 
        }

    }
    void ProcessRotation()
    {
        if (rotation.IsPressed())
        {
            
            float rotationInput = rotation.ReadValue<float>();
            if (rotationInput > 0)
            {
                ApplyRotation(rotationStrength);
                leftParticles.Play();
            }
            else if (rotationInput < 0)
            {
                ApplyRotation(-rotationStrength);
                rightParticles.Play();
            }
           
        }
        else 
        {
            
            rightParticles.Stop();
            leftParticles.Stop();
        }
    }

    private void ApplyRotation(float rotationThisFrame)
    {
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotationThisFrame * Time.fixedDeltaTime);
        rb.freezeRotation = false;
    }
    #endregion
    // ***********************************************
}
