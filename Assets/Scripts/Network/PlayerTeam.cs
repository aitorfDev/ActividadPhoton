using Fusion;
using UnityEngine;

public enum Team
{
    Red,
    Blue
}

public class PlayerTeam : NetworkBehaviour
{
    [Networked] public Team Team { get; set; }

    [Header("Materials")]
    public Material redMaterial;
    public Material blueMaterial;

    private Renderer playerRenderer;

    private void Awake()
    {
        playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer == null)
            Debug.LogWarning("No Renderer found for PlayerTeam.");
    }

    public override void Spawned()
    {
        ApplyTeamColor();
    }

    private void Update()
    {
        // Aplicar color continuamente según la variable Networked
        ApplyTeamColor();
    }

    private void ApplyTeamColor()
    {
        if (playerRenderer == null) return;

        switch (Team)
        {
            case Team.Red:
                playerRenderer.material = redMaterial;
                break;
            case Team.Blue:
                playerRenderer.material = blueMaterial;
                break;
        }
    }
}
