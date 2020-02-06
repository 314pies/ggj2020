using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Bolt;
using TMPro;
public class GameStatusManager : EntityBehaviour<IGameStatus>
{
    public enum GameStatus { WaitingForPlayer, Playing, Result };
    public GameStatus gameStatus { get { return (GameStatus)state.Status; } }

    const string UIGroup = "Game Status UI";
    [BoxGroup(UIGroup)]
    public GameObject WaitingForPlayer;

    const string gameResultGroup = UIGroup + "/Game Result";
    [BoxGroup(gameResultGroup)]
    public GameObject GameResult;
    [BoxGroup(gameResultGroup)]
    public TMP_Text GameOverTitle;
    [BoxGroup(gameResultGroup)]
    public string leftWinText = "Team Left Wins!", rightWinText = "Team Right Wins!";

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            state.Status = (int)GameStatus.Playing;
        }

        state.AddCallback("Status", OnGameStatusUpdated);

        state.AddCallback("GameResult", () =>
        {
            var winningSide = (SideEnem)state.GameResult;
            if(winningSide == SideEnem.Left)
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
        if (gameStatus == GameStatus.Result)        
            GameResult.SetActive(true);
        else
            GameResult.SetActive(false);
    }
}
