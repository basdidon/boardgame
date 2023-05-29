using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.Events;

public class NetworkSpawner : SerializedMonoBehaviour, INetworkRunnerCallbacks
{
    static NetworkSpawner instance;
    public static NetworkSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("NetworkSpawner");
                instance = go.AddComponent<NetworkSpawner>(); 
            }
            return instance;
        }
    }

    public NetworkRunner Runner { get; private set; }

    [OdinSerialize] public List<SessionInfo> sessionInfoList;

    // event
    public UnityAction OnSessionListUpdatedAction;
    public delegate List<SessionInfo> OnSessionListUpdatedDelegate();

    private void Awake() 
    {
        if(instance == null) instance = this;
        Runner = gameObject.AddComponent<NetworkRunner>();

        OnSessionListUpdatedAction += () => Debug.Log("NetworkSpawner from");
    }

    private void Start()
    {
        sessionInfoList = new List<SessionInfo>();
    }

    public async void StartHost(string roomName)
    {
        var result = await Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            CustomLobbyName = roomName,
        });

        if (result.Ok)
        {
            Debug.Log($"started Host: {roomName}");
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public async void JoinLobby()
    {
        var result = await Runner.JoinSessionLobby(SessionLobby.ClientServer, "MainLobby");

        if (result.Ok)
        {
            UiDocControls.Instance.ActiveUiDoc(UiMenus.Lobby);
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        // _runner = gameObject.AddComponent<NetworkRunner>();
        Runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await Runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        Debug.Log($"player {player.PlayerId} has joined.");
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) {
        Debug.Log("OnConnectedToServer()");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner) {}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        Debug.Log("OnSessionListUpdated");
        sessionInfoList = sessionList;
        // call action here !!!!!!
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }


}
