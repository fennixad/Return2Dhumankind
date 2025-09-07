using UnityEngine;


public class PlanetRotator : MonoBehaviour
{
    [Header("Configuracion de Rotacion")]
    [Tooltip("Velocidad de rotación del planeta en grados por segundo")]
    public float planetRotationSpeed;

    public void RotateWithPlayer (float input)
    {
        transform.Rotate(0f, 0f, + input * planetRotationSpeed * Time.deltaTime, Space.Self);
    }
}

