using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : NetworkBehaviour
{
    public float sensX = 100f;
    public float sensY = 100f;

    public Transform orientation;
    public Transform headOrientation;
    public Transform playerOrient;
    float xRotation;
    float yRotation;

    private void Start()
    {
        // Solo la cámara local debe estar activa y controlar el ratón
        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Desactiva la cámara del resto de jugadores
            if (TryGetComponent<Camera>(out var cam))
                cam.enabled = false;
        }
    }

    private void Update()
    {
        // Solo el jugador local controla su cámara
        if (!Object.HasInputAuthority)
            return;

        // Nuevo Input System
        float mouseX = 0f;
        float mouseY = 0f;

        if (Mouse.current != null)
        {
            mouseX = Mouse.current.delta.x.ReadValue() * sensX * Time.deltaTime;
            mouseY = Mouse.current.delta.y.ReadValue() * sensY * Time.deltaTime;
        }

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotar cámara y orientación
        transform.rotation = headOrientation.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        playerOrient.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
