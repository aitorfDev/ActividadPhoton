using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Player Prefab")]
    public NetworkPrefabRef playerPrefab;

    [Header("Spawn Points")]
    public Transform redSpawn;
    public Transform blueSpawn;

    private NetworkRunner runner;

    // Contadores por equipo
    private int redCount = 0;
    private int blueCount = 0;

    // Diccionario para rastrear los objetos spawneados
    private Dictionary<PlayerRef, NetworkObject> spawned = new Dictionary<PlayerRef, NetworkObject>();

    // -------------------
    // UI de conexión
    // -------------------
    private void OnGUI()
    {
        if (runner == null || !runner.IsRunning)
        {
            if (GUI.Button(new Rect(10, 10, 200, 40), "Host"))
                StartGame(GameMode.Host);

            if (GUI.Button(new Rect(10, 60, 200, 40), "Client"))
                StartGame(GameMode.Client);

            if (GUI.Button(new Rect(10, 110, 200, 40), "Server"))
                StartGame(GameMode.Server);
        }
    }

    // -------------------
    // StartGame
    // -------------------
    private async void StartGame(GameMode mode)
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;


        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    // -------------------
    // Spawn de jugadores
    // -------------------
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) return; // solo host spawnea

        Team team;
        Transform spawnPoint;

        // Asignamos equipo con menos jugadores
        if (redCount <= blueCount)
        {
            team = Team.Red;
            spawnPoint = redSpawn;
            redCount++;
        }
        else
        {
            team = Team.Blue;
            spawnPoint = blueSpawn;
            blueCount++;
        }

        // Spawn del jugador con InputAuthority del player correspondiente
        NetworkObject playerObj = runner.Spawn(
            playerPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            player
        );

        // Asignamos el equipo al PlayerTeam
        if (playerObj.TryGetComponent<PlayerTeam>(out var teamComp))
            teamComp.Team = team;

        spawned[player] = playerObj;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawned.TryGetValue(player, out var obj))
        {
            if (obj.TryGetComponent<PlayerTeam>(out var teamComp))
            {
                if (teamComp.Team == Team.Red) redCount--;
                else if (teamComp.Team == Team.Blue) blueCount--;
            }

            runner.Despawn(obj);
            spawned.Remove(player);
        }
    }

    // -------------------
    // Input
    // -------------------
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed) data.move += 1;
        if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed) data.move -= 1;
        if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed) data.strafe -= 1;
        if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed) data.strafe += 1;
        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            data.buttons.Set(NetworkInputData.JUMP, true);
        if (UnityEngine.InputSystem.Mouse.current.leftButton.isPressed)
            data.buttons.Set(NetworkInputData.MOUSEBUTTON0, true);

        input.Set(data);
    }

    // -------------------
    // Callbacks vacíos
    // -------------------
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
