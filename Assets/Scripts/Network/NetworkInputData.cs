using Fusion;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 0;
    public const byte JUMP = 1;

    public float move;    // Adelante / atrás (W/S)
    public float strafe;  // Izquierda / derecha (A/D)
    public NetworkButtons buttons; // Botones (salto, disparo, etc.)
}
