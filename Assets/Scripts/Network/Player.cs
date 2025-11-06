using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController _cc;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 120f;

    [Networked] private TickTimer delay { get; set; }

    [SerializeField] private Ball _prefabBall;
    [SerializeField] private Transform characterForward; // Objeto que define hacia dónde se dispara

    private Vector3 _forward = Vector3.forward;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();

        // Si no se asigna manualmente, usar el propio transform como fallback
        if (characterForward == null)
            characterForward = transform;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // ROTACIÓN (A/D)
            if (Mathf.Abs(data.turn) > 0f)
            {
                transform.Rotate(Vector3.up, data.turn * turnSpeed * Runner.DeltaTime);
            }

            // MOVIMIENTO (W/S)
            if (Mathf.Abs(data.move) > 0f)
            {
                Vector3 forward = transform.forward * data.move * moveSpeed * Runner.DeltaTime;
                _cc.Move(forward);
            }

            // DIRECCIÓN DE MOVIMIENTO
            Vector3 moveDir = transform.forward * data.move + transform.right * data.turn;
            if (moveDir.sqrMagnitude > 0f)
            {
                _forward = moveDir.normalized;
            }

            // DISPARO
            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    // Usar la dirección del objeto "characterForward"
                    Vector3 shootDir = characterForward.forward;
                    Vector3 spawnPos = characterForward.position + shootDir * 1.0f; // offset opcional

                    Debug.Log(spawnPos.ToString());

                    Runner.Spawn(
                        _prefabBall,
                        spawnPos,
                        Quaternion.LookRotation(shootDir),
                        Object.StateAuthority,
                        (runner, o) =>
                        {
                            o.GetComponent<Ball>().Init();
                        }
                    );
                }
            }
        }
    }
}
