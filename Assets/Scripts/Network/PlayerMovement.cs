using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    private CharacterController _controller;
    private Vector3 _velocity;

    [SerializeField] private Transform playerOrient; // referencia desde el inspector



    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }


    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData data))
            return;

        // 🔹 usa la orientación del cuerpo (que rota con la cámara)
        Vector3 moveDir = (playerOrient.forward * data.move + playerOrient.right * data.strafe);

        if (moveDir.sqrMagnitude > 1)
            moveDir.Normalize();

        Vector3 move = moveDir * moveSpeed * Runner.DeltaTime;

        // Gravedad y salto igual que antes...
        if (_controller.isGrounded)
        {
            _velocity.y = -1f;
            if (data.buttons.IsSet(NetworkInputData.JUMP))
                _velocity.y = jumpForce;
        }
        else
        {
            _velocity.y += gravity * Runner.DeltaTime;
        }

        _controller.Move(move + _velocity * Runner.DeltaTime);
    }


}
