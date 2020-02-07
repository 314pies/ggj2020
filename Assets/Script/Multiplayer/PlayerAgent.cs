using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using CliffLeeCL;
public class PlayerAgent : EntityBehaviour<IPlayerState>
{
    PlayerInteraction playerInteraction;
    public override void Attached()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
        GetComponent<PlayerController2D>().enabled = false;
        GetComponent<PlayerInteraction>().enabled = false;

        state.SetTransforms(state.trans, transform);
        state.SetAnimator(GetComponent<Animator>());
        state.SetTransforms(state.Arrow.Rotation, GetComponent<PlayerInteraction>().Arrow.transform);

        state.AddCallback("Flip", () =>
        {
            GetComponent<SpriteRenderer>().flipX = state.Flip;
        });

        state.AddCallback("CarryingItem", () =>
        {
            //playerInteraction.CurrendHoldingItem = state.HoldingItem;
        });

        state.AddCallback("PlayerID", () =>
        {
            Debug.Log("Player ID: " + state.PlayerID, this);
            pointer.color = state.PlayerID % 2 == 1 ? LeftTeamColor : RightTeamColor;
            playerIDText.color = state.PlayerID % 2 == 1 ? LeftTeamColor : RightTeamColor;

            if (entity.HasControl)
            {
                playerIDText.text = "You";
                //playerIDText.color = Color.white;
            }
            else
            {
                playerIDText.text = "P " + state.PlayerID;
            }
        });

        state.AddCallback("Arrow.IsActive", () =>
        {
            GetComponent<PlayerInteraction>().Arrow.SetActive(state.Arrow.IsActive);
        });
    }

    const string playerIDTagGroup = "Player ID Tag";
    public TMPro.TMP_Text playerIDText;
    public SpriteRenderer pointer;
    public Color LeftTeamColor = Color.blue, RightTeamColor = Color.red;

    public void ServerSetPlayerID(int playerID)
    {
        state.PlayerID = playerID;
    }

    void Update()
    {
        if (entity.IsOwner)
            state.Flip = GetComponent<SpriteRenderer>().flipX;
    }

    public override void ControlGained()
    {
        GetComponent<PlayerController2D>().enabled = true;
        GetComponent<PlayerInteraction>().enabled = true;
    }

    public override void ControlLost()
    {
        GetComponent<PlayerController2D>().enabled = false;
        GetComponent<PlayerInteraction>().enabled = false;
    }

    public override void SimulateController()
    {
        ICustomPlayerCommandInput input = CustomPlayerCommand.Create();

        input.Position = transform.position;
        input.Velocity = GetComponent<Rigidbody2D>().velocity;
        input.Rotation = transform.rotation;
        input.Flip = GetComponent<SpriteRenderer>().flipX;
        input.ArrorRotation = GetComponent<PlayerInteraction>().Arrow.transform.localEulerAngles.z;
        input.ArrowActive = GetComponent<PlayerInteraction>().Arrow.activeSelf;
        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        CustomPlayerCommand cmd = (CustomPlayerCommand)command;


        if (resetState)
        {
            // we got a correction from the server, reset (this only runs on the client)
            // _motor.SetState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.IsGrounded, cmd.Result.JumpFrames);
        }
        else//Execute on server and controller
        {
            if (entity.IsOwner)
            {
                transform.position = cmd.Input.Position;
                GetComponent<Rigidbody2D>().velocity = cmd.Input.Velocity;
                transform.rotation = cmd.Input.Rotation;
                //GetComponent<SpriteRenderer>().flipX = cmd.Input.Flip;
                state.Flip = cmd.Input.Flip;

                GetComponent<PlayerInteraction>().Arrow.transform.localEulerAngles = new Vector3(0, 0, cmd.Input.ArrorRotation);
                //GetComponent<PlayerInteraction>().Arrow.SetActive(cmd.Input.ArrowActive);
                state.Arrow.IsActive = cmd.Input.ArrowActive;
            }

            GetComponent<PlayerAnimator>().UpdateAnimator();
            //Keep in this cicle
            cmd.Result.Position = transform.position;
        }
    }
}
