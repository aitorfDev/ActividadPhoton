using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    private int loggedPlayers;

    [SerializeField] private Transform[] spawnpoints;
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private bool _mouseButton0;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private void Update()
    {
        _mouseButton0 |= Mouse.current.leftButton.isPressed;

        if (Mouse.current.leftButton.isPressed)
        {
            Debug.Log("Mouse Button 0 pressed");
        }
    }

    private async void StartGame(GameMode mode)
    {
        // Crear el Fusion runner y activar el envío de input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Crear información de la escena actual
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();

        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Iniciar o unirse a una sesión (según el modo seleccionado)
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }

            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }

            if (GUI.Button(new Rect(0, 80, 200, 40), "Server"))
            {
                StartGame(GameMode.Server);
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        loggedPlayers++;

        if (runner.IsServer)
        {
            // Seleccionar punto de aparición
            Vector3 spawnPosition = spawnpoints[loggedPlayers / 2].position;

            NetworkObject networkPlayerObject = runner.Spawn(
                _playerPrefab,
                spawnPosition,
                Quaternion.identity,
                player
            );

            // Guardar referencia del jugador
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        // Movimiento básico
        if (Keyboard.current.wKey.isPressed) data.move += 1;  // adelante
        if (Keyboard.current.sKey.isPressed) data.move -= 1;  // atrás
        if (Keyboard.current.aKey.isPressed) data.turn -= 1;  // gira izquierda
        if (Keyboard.current.dKey.isPressed) data.turn += 1;  // gira derecha

        // Disparo con el botón izquierdo del ratón
        if (Mouse.current.leftButton.isPressed)
        {
            Debug.Log("Mouse Button 0 pressed");
            data.buttons.Set(NetworkInputData.MOUSEBUTTON0, true);
        }

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
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
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
