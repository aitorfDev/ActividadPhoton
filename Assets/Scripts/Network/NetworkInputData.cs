using Fusion;
using UnityEngine;
public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1;

    public NetworkButtons buttons;
    public float move; // adelante / atrás (W/S)
    public float turn; // rotación (A/D)
}

