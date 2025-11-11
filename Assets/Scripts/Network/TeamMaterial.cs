using Fusion;
using UnityEngine;

public class PlayerTeamMaterial : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer bodyRenderer; // el mesh principal del jugador
    [SerializeField] private Renderer weaponPart1; // el mesh principal del jugador
    [SerializeField] private Renderer weaponPart2; // el mesh principal del jugador

    [SerializeField] private Renderer weaponPart3; // el mesh principal del jugador


    [Header("Materials")]
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blueMaterial;

    private PlayerTeam _teamComponent;

    public override void Spawned()
    {
        _teamComponent = GetComponent<PlayerTeam>();

        // 🔥 Cuando el jugador aparece en la red, actualizamos el material
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if (_teamComponent == null || bodyRenderer == null)
            return;

        switch (_teamComponent.Team)
        {
            case Team.Red:
                bodyRenderer.material = weaponPart1.material = weaponPart2.material = weaponPart3.material = redMaterial;

                break;

            case Team.Blue:
                bodyRenderer.material = weaponPart1.material = weaponPart2.material = weaponPart3.material = blueMaterial;
                break;

            default:
                break;
        }
    }
}
