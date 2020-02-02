﻿using System;
using System.Collections;
using System.Collections.Generic;
using Bolt.Matchmaking;
using UdpKit;
using UnityEngine;

public class CustomBoltLauncher : Bolt.GlobalEventListener
{
	void OnGUI()
	{
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

	public override void BoltStartDone()
	{
		if (BoltNetwork.IsServer)
		{
			string matchName = Guid.NewGuid().ToString();

			BoltMatchmaking.CreateSession(
				sessionID: matchName
			);

            var _player = BoltNetwork.Instantiate(BoltPrefabs.Player);
            _player.TakeControl();

        }
	}


    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        foreach (var session in sessionList)
        {
            UdpSession photonSession = session.Value as UdpSession;

            if (photonSession.Source == UdpSessionSource.Photon)
            {
                BoltNetwork.Connect(photonSession);
            }
        }
    }

    public Dictionary<uint, GameObject> serverPlayersDictionary = new Dictionary<uint, GameObject>();
    public override void Connected(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {
            var _player = BoltNetwork.Instantiate(BoltPrefabs.Player);
            serverPlayersDictionary.Add(connection.ConnectionId, _player.gameObject);
            _player.AssignControl(connection);
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
}