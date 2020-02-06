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

        state.AddCallback("Flip", () =>
        {
            GetComponent<SpriteRenderer>().flipX = state.Flip;
        });

        state.AddCallback("CarryingItem", () =>
        {
            //playerInteraction.CurrendHoldingItem = state.HoldingItem;
        });
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
            }

            GetComponent<PlayerAnimator>().UpdateAnimator();
            //Keep in this cicle
            cmd.Result.Position = transform.position;
        }
    }
}
