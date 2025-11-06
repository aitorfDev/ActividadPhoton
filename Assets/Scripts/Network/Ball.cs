using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [SerializeField] private float BulletSpeed = 20f;
    [Networked] private TickTimer life { get; set; }
    
    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        Debug.Log("Ball spawned at position: " + transform.position.ToString());
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);
        else
            transform.position += BulletSpeed * transform.forward * Runner.DeltaTime;
    }

    public void OnCollisionEnter()
    {
        Runner.Despawn(Object);
    }

    public void OnTriggerEnter()
    {
        Runner.Despawn(Object);
    }
}