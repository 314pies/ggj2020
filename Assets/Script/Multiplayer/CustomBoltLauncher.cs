using System;
using System.Collections;
using System.Collections.Generic;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine;
using CliffLeeCL;
using Sirenix.OdinInspector;
using TMPro;
using Bolt;
using UdpKit.Platform.Photon;

public class CustomBoltLauncher : Bolt.GlobalEventListener
{
    public GameObject UI;
    public Vector3 PlayerSpawnPoint;

    public bool IsSinglePlayer = false;
    public bool EnableDebugOnGUI = false;

    void Start()
    {
        if (IsSinglePlayer)
        {
            StartServer();
            UI.SetActive(false);
        }
    }

    void OnGUI()
    {
        if (!EnableDebugOnGUI) return;
        if (BoltNetwork.IsRunning) { return; }
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

        if (GUILayout.Button("Start Server", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            // START SERVER
            BoltLauncher.StartServer();
        }

        if (GUILayout.Button("Start Client", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            // START CLIENT
            BoltLauncher.StartClient();
        }

        GUILayout.EndArea();
    }


    public void StartServer()
    {
        BoltLauncher.StartServer();
        NotificationManager.ShowNotification("Creating Server...", true);
    }

    public enum ClientConnectMode { QuickMatch, JoinRoom }
    ClientConnectMode clientConnectMode;
    public void StartClient()
    {
        BoltLauncher.StartClient();
    }

    public void StartQuickMatch()
    {
        clientConnectMode = ClientConnectMode.QuickMatch;
        NotificationManager.ShowNotification("Searching...", true);
        StartClient();
    }

    public TMP_InputField roomIDInput;
    public void StartJoinRoom()
    {
        if (string.IsNullOrEmpty(roomIDInput.text)) { return; }
        clientConnectMode = ClientConnectMode.JoinRoom;
        NotificationManager.ShowNotification("Joining " + roomIDInput.text + "...", true);
        StartClient();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            string matchName = "";
            for (int i = 0; i < 5; i++)
            {
                matchName += UnityEngine.Random.Range(0, 10);
            }

            BoltMatchmaking.CreateSession(
                sessionID: matchName
            );

            var _player = BoltNetwork.Instantiate(BoltPrefabs.Player, PlayerSpawnPoint, Quaternion.identity);
            _player.TakeControl();

            ServerAssignPlayerID(_player);

            BoltNetwork.Instantiate(BoltPrefabs.GameStatusManager);
            NotificationManager.CloseNotification();
            UI.SetActive(false);
        }
        if (BoltNetwork.IsClient)
        {
            if (clientConnectMode == ClientConnectMode.JoinRoom)
            {
                Debug.Log("Joining Session " + roomIDInput.text);
                //BoltMatchmaking.JoinSession(roomIDInput.text);
                //BoltMatchmaking.JoinSession(PhotonSession.Build(roomIDInput.text));
                BoltNetwork.Connect(PhotonSession.Build(roomIDInput.text));
            }
        }
    }

    public override void SessionConnected(UdpSession session, IProtocolToken token)
    {
        Debug.Log("SessionConnected. " + session);
    }

    public override void SessionConnectFailed(UdpSession session, IProtocolToken token)
    {
        Debug.Log("SessionConnectFailed. " + session);
        NotificationManager.ShowNotification("Failed to connect to " + session.HostName, false, true);
        if (BoltNetwork.IsRunning)
            BoltNetwork.Shutdown();
    }


    public override void BoltStartFailed()
    {
        Debug.Log("BoltStartFailed.");
        NotificationManager.ShowNotification("Multilayer Start Failed.", false, true);
        UI.SetActive(true);
    }

    public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
    {
        Debug.Log("ConnectFailed. endpoint: " + endpoint);

        NotificationManager.ShowNotification("Failed to connect to server. ", false, true);
        UI.SetActive(true);
    }


    int playerId = 0;
    void ServerAssignPlayerID(BoltEntity player)
    {
        //var playerList = GameObject.FindGameObjectsWithTag("Player");
        playerId++;
        player.GetComponent<PlayerAgent>().ServerSetPlayerID(playerId);
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);
        if (clientConnectMode == ClientConnectMode.QuickMatch)
        {
            NotificationManager.ShowNotification("Searching... " + sessionList.Count + "games found", true);
            foreach (var session in sessionList)
            {
                UdpSession photonSession = session.Value as UdpSession;

                if (photonSession.Source == UdpSessionSource.Photon)
                {
                    BoltNetwork.Connect(photonSession);
                }
            }
        }
    }

    public Dictionary<uint, GameObject> serverPlayersDictionary = new Dictionary<uint, GameObject>();
    public override void Connected(BoltConnection connection)
    {
        Debug.Log("Connected. " + connection);
        if (BoltNetwork.IsServer)
        {
            var _player = BoltNetwork.Instantiate(BoltPrefabs.Player, PlayerSpawnPoint, Quaternion.identity);
            serverPlayersDictionary.Add(connection.ConnectionId, _player.gameObject);
            _player.AssignControl(connection);
            ServerAssignPlayerID(_player);
        }
        NotificationManager.CloseNotification();
    }

    public override void Disconnected(BoltConnection connection)
    {
        Debug.Log("Disconnected. " + connection);
        if (BoltNetwork.IsServer)
        {
            if (serverPlayersDictionary.ContainsKey(connection.ConnectionId))
            {
                var _p = serverPlayersDictionary[connection.ConnectionId];
                serverPlayersDictionary.Remove(connection.ConnectionId);
                BoltNetwork.Destroy(_p);
            }
        }
        if (BoltNetwork.IsClient)
        {
            NotificationManager.ShowNotification("Disconnected from server.", false, true);
            BoltNetwork.Shutdown();
            UI.SetActive(true);
        }
    }


    //Equip Item
    public override void OnEvent(PlayerPickItemReq evnt)
    {
        Debug.Log("Receive PlayerPickDropItemReq " + evnt.Player);
        evnt.Player.GetComponent<PlayerInteraction>().ServerSetCarryingItem(evnt.Item);
    }

    public override void OnEvent(LaunchCarryingItem evnt)
    {
        evnt.Player.GetComponent<PlayerInteraction>().ServerLaunchCarryingItem(evnt.ArrowRotation);
    }

    public override void OnEvent(RepairItemReq evnt)
    {
        if (evnt.Item != null)
            evnt.Item.GetComponent<ItemAgent>().ServerSetItemState(ItemStateEnum.New);
    }

    [Button]
    private void SpawnBoltPrefab(GameObject item, Vector3 position)
    {
        BoltNetwork.Instantiate(item, position, item.transform.rotation);
    }

    [Button]
    private void SpawnArmor()
    {
        BoltNetwork.Instantiate(BoltPrefabs.Armor, Vector3.zero, Quaternion.identity);
    }

    [Button]
    private void SpawnWeapon()
    {
        BoltNetwork.Instantiate(BoltPrefabs.Sword, Vector3.zero, Quaternion.identity);
    }
}
