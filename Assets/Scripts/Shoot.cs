using Fusion;
using UnityEngine;
public class Shoot : NetworkBehaviour
{
    [Networked] private TickTimer delay { get; set; }

    [SerializeField] private float attackSpeed = 0.5f;
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private Transform shootingPos; // Objeto que define hacia dónde se dispara
    [SerializeField] private float shootingOffset = 0.3f; // Objeto que define hacia dónde se dispara

    private void Awake()
    {

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
          
            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, attackSpeed);
                    Vector3 spawnPos = shootingPos.position + shootingPos.forward * shootingOffset;
                    Runner.Spawn(
                        _prefabBall, // Prefab a spawnear
                        spawnPos, // Posicion desde donde se dispara
                        Quaternion.LookRotation(shootingPos.forward),  // Marcar la flechita como direccion
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