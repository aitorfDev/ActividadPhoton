using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    private CharacterController controller;
    private Vector3 velocity;

    [SerializeField] private Transform playerOrient;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;
        if (!GetInput(out NetworkInputData data)) return;

        Vector3 moveDir = (playerOrient.forward * data.move + playerOrient.right * data.strafe);
        if (moveDir.sqrMagnitude > 1) moveDir.Normalize();

        Vector3 move = moveDir * moveSpeed * Runner.DeltaTime;

        if (controller.isGrounded)
        {
            velocity.y = -1f;
            if (data.buttons.IsSet(NetworkInputData.JUMP))
                velocity.y = jumpForce;
        }
        else
        {
            velocity.y += gravity * Runner.DeltaTime;
        }

        controller.Move(move + velocity * Runner.DeltaTime);
    }
}
