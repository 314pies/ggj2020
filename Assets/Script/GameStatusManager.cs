using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Bolt;
using TMPro;
public class GameStatusManager : EntityBehaviour<IGameStatus>
{
    public enum GameStatus { WaitingForPlayer = 1, Playing, Result };
    public GameStatus gameStatus { get { return (GameStatus)state.Status; } }

    const string UIGroup = "Game Status UI";

    [BoxGroup(UIGroup)]
    public GameObject UIRoot;

    const string waitingForPlayerGroup = UIGroup + "/Wait for Players";
    [BoxGroup(waitingForPlayerGroup)]
    public GameObject WaitingForPlayer;
    [BoxGroup(waitingForPlayerGroup)]
    public GameObject StartButton;

    const string gameResultGroup = UIGroup + "/Game Result";
    [BoxGroup(gameResultGroup)]
    public GameObject GameResult, RestartButton;
    [BoxGroup(gameResultGroup)]
    public TMP_Text GameOverTitle;
    [BoxGroup(gameResultGroup)]
    public string leftWinText = "Team Left Wins!", rightWinText = "Team Right Wins!";

    public override void Attached()
    {
        state.AddCallback("Status", OnGameStatusUpdated);
        if (entity.IsOwner)
        {
            state.Status = (int)GameStatus.WaitingForPlayer;
        }
        state.AddCallback("GameResult", () =>
        {
            var winningSide = (SideEnem)state.GameResult;
            if (winningSide == SideEnem.Left)
                GameOverTitle.text = leftWinText;
            else if (winningSide == SideEnem.Right)
                GameOverTitle.text = rightWinText;
        });
    }

    public void ServerOnWallTouched(SideEnem winningSide)
    {
        if (gameStatus == GameStatus.Playing)
        {
            state.Status = (int)GameStatus.Result;
            state.GameResult = (int)winningSide;
        }
    }

    void OnGameStatusUpdated()
    {
        if (gameStatus == GameStatus.WaitingForPlayer)
        {
            WaitingForPlayer.SetActive(true);
            StartButton.SetActive(entity.IsOwner);
        }
        else
        {
            WaitingForPlayer.SetActive(false);
        }


        if (gameStatus == GameStatus.Result)
        {
            GameResult.SetActive(true);
            RestartButton.SetActive(entity.IsOwner);
        }
        else
            GameResult.SetActive(false);
    }


    [Button]
    void ServerCleanUp()
    {
        CleanUpTag[] itemsToCleanUp = FindObjectsOfType(typeof(CleanUpTag)) as CleanUpTag[];
        for (int i = 0; i < itemsToCleanUp.Length; i++)
        {
            if (itemsToCleanUp[i] != null)
                BoltNetwork.Destroy(itemsToCleanUp[i].gameObject);
        }
    }

    [Button]
    public void ServerStartNewGame()
    {
        state.Status = (int)GameStatus.Playing;
        if (entity.IsOwner)
        {
            ServerCleanUp();
            ServerSpawnSoldiers();
        }
    }

    public GameObject[] PlayersList
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("Player");
        }
    }
    const string soldierSpawnGroup = "Spawn Soldier";
    [BoxGroup(soldierSpawnGroup)]
    public int SoldierBaseCount = 3;
    [BoxGroup(soldierSpawnGroup)]
    public Vector3 LeftSolderSpawnPoint, RightSoldierSolderSpawnPoint;
    [BoxGroup(soldierSpawnGroup)]
    public float SpawnGap = 0.15f;
    [Button]
    [BoxGroup(soldierSpawnGroup)]
    void ServerSpawnSoldiers()
    {
        int leftTeamPlayerCount = 0, rightTeamPlayerCount = 0; ;
        var _playerList = PlayersList;
        foreach (var player in _playerList)
        {
            if (player.GetComponent<PlayerAgent>().state.PlayerID % 2 == 1)
            {
                leftTeamPlayerCount++;
            }
            else
            {
                rightTeamPlayerCount++;
            }
        }

        int leftTeamSoldiersCount = SoldierBaseCount;
        if (leftTeamPlayerCount > SoldierBaseCount)
            leftTeamSoldiersCount = leftTeamPlayerCount;

        int rightTeamSoldiersCount = SoldierBaseCount;
        if (rightTeamSoldiersCount > SoldierBaseCount)
            rightTeamSoldiersCount = rightTeamPlayerCount;

        //Balance
        if (leftTeamPlayerCount > rightTeamPlayerCount)
            rightTeamSoldiersCount += (leftTeamPlayerCount - rightTeamPlayerCount);
        else if (leftTeamPlayerCount < rightTeamPlayerCount)
            leftTeamSoldiersCount += (rightTeamPlayerCount - leftTeamPlayerCount);

        Debug.Log("Spawning " + leftTeamSoldiersCount + " soldiers for left team.");
        Vector3 spawnPos = LeftSolderSpawnPoint;       
        for (int i=0;i< leftTeamSoldiersCount; i++)
        {
            BoltNetwork.Instantiate(BoltPrefabs.SoldierL, spawnPos, Quaternion.identity);
            spawnPos.x += (-SpawnGap);
        }

        Debug.Log("Spawning " + rightTeamSoldiersCount + " soldiers for right team.");
        spawnPos = RightSoldierSolderSpawnPoint;
        for (int i = 0; i < rightTeamSoldiersCount; i++)
        {
            BoltNetwork.Instantiate(BoltPrefabs.SoldierR, spawnPos, Quaternion.identity);
            spawnPos.x += (SpawnGap);
        }
    }
}
