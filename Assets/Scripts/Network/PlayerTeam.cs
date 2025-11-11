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

   
}
