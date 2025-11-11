using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : NetworkBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 100f;
    public float sensY = 100f;

    [Header("References")]
    public Transform headOrientation;
    public Transform playerOrient;
    public Camera cam;

    private float xRotation;
    private float yRotation;

    [Networked] public Quaternion HeadRot { get; set; }

    public override void Spawned()
    {
        if (cam == null)
            TryGetComponent(out cam);

        if (Object.HasInputAuthority)
        {
            cam.enabled = true;
            var audio = cam.GetComponent<AudioListener>();
            if (audio != null) audio.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            if (cam != null) cam.enabled = false;
            var audio = cam.GetComponent<AudioListener>();
            if (audio != null) audio.enabled = false;
        }
    }

    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            Vector2 mouseDelta = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

            yRotation += mouseDelta.x * sensX * Time.deltaTime;
            xRotation -= mouseDelta.y * sensY * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            Quaternion newRot = Quaternion.Euler(xRotation, yRotation, 0);

            headOrientation.rotation = newRot;
            playerOrient.rotation = Quaternion.Euler(0, yRotation, 0);

            HeadRot = newRot; // actualizar en red
        }
        else
        {
            // Interpolación suave de cabeza para otros jugadores
            headOrientation.rotation = Quaternion.Slerp(
                headOrientation.rotation,
                HeadRot,
                15f * Time.deltaTime
            );
        }
    }
}
