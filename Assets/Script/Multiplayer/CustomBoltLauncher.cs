﻿using System;
using System.Collections;
using System.Collections.Generic;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine;
using CliffLeeCL;
using Sirenix.OdinInspector;
using TMPro;
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
    }

    public enum ClientConnectMode { QuickMatch,JoinRoom }
    ClientConnectMode clientConnectMode;
    public void StartClient()
    {
        BoltLauncher.StartClient();
    }

    public void StartQuickMatch()
    {
        clientConnectMode = ClientConnectMode.QuickMatch;
        StartClient();
    }

    public TMP_InputField roomIDInput;
    public void StartJoinRoom()
    {
        clientConnectMode = ClientConnectMode.JoinRoom;
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
        }
        UI.SetActive(false);
    }

    void ServerAssignPlayerID(BoltEntity player)
    {
        var playerList = GameObject.FindGameObjectsWithTag("Player");
        player.GetComponent<PlayerAgent>().ServerSetPlayerID(playerList.Length);
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);
        if(clientConnectMode == ClientConnectMode.QuickMatch)
        {
            foreach (var session in sessionList)
            {
                UdpSession photonSession = session.Value as UdpSession;

                if (photonSession.Source == UdpSessionSource.Photon)
                {
                    BoltNetwork.Connect(photonSession);
                }
            }
        }
        if(clientConnectMode == ClientConnectMode.JoinRoom)
        {
            BoltMatchmaking.JoinSession(roomIDInput.text);
        }
        
    }

    public Dictionary<uint, GameObject> serverPlayersDictionary = new Dictionary<uint, GameObject>();
    public override void Connected(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {
            var _player = BoltNetwork.Instantiate(BoltPrefabs.Player, PlayerSpawnPoint, Quaternion.identity);
            serverPlayersDictionary.Add(connection.ConnectionId, _player.gameObject);
            _player.AssignControl(connection);
            ServerAssignPlayerID(_player);
        }
    }

    public override void Disconnected(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {
            if (serverPlayersDictionary.ContainsKey(connection.ConnectionId))
            {
                var _p = serverPlayersDictionary[connection.ConnectionId];
                serverPlayersDictionary.Remove(connection.ConnectionId);
                BoltNetwork.Destroy(_p);
            }
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
}
