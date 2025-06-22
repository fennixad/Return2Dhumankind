using UnityEngine;

public class PlanetRotator : MonoBehaviour
{
    public float planetRotationSpeed;

    public void RotateWithPlayer (float input)
    {
        transform.Rotate(0f, 0f, + input * planetRotationSpeed * Time.deltaTime, Space.Self);
    }
}
