using UnityEngine;
using Oculus;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.0f;          // Velocidad de movimiento
    public float moveSmooth = 5.0f;         // Suavidad del movimiento
    public float rotationSpeed = 90.0f;     // Velocidad de giro en grados por segundo
    public float rotationSmooth = 5.0f;     // Suavidad del giro

    private Vector3 targetDirection;        // Dirección objetivo para movimiento
    private Quaternion targetRotation;      // Rotación objetivo para giro suave

    void Update()
    {
        // Movimiento - Controlado solo por el joystick izquierdo
        Vector2 primaryAxisLeft = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 moveDirection = new Vector3(primaryAxisLeft.x, 0, primaryAxisLeft.y);

        // Define la dirección del movimiento en base a la orientación de la cámara
        targetDirection = Camera.main.transform.TransformDirection(moveDirection);
        targetDirection.y = 0; // Mantener el movimiento solo en el plano XZ

        // Movimiento suave
        Vector3 smoothMove = Vector3.Lerp(transform.position, transform.position + targetDirection * moveSpeed * Time.deltaTime, moveSmooth * Time.deltaTime);
        transform.position = smoothMove;

        // Giro - Controlado solo por el joystick derecho
        Vector2 primaryAxisRight = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        if (primaryAxisRight.x != 0)
        {
            // Define el ángulo de rotación en función del valor del joystick derecho
            float rotationAmount = primaryAxisRight.x * rotationSpeed * Time.deltaTime;
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + rotationAmount, 0);

            // Aplica una interpolación suave (Slerp) para el giro
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmooth * Time.deltaTime);
        }
    }
}
