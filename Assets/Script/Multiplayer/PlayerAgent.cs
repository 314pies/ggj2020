﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using CliffLeeCL;
public class PlayerAgent : EntityBehaviour<IPlayerState>
{
    public override void Attached()
    {
        state.SetTransforms(state.trans, transform);
        state.SetAnimator(GetComponent<Animator>());

        state.AddCallback("localScale", ()=> {
            transform.localScale = state.localScale;
        });
    }

    public override void ControlGained()
    {
     
    }

    public override void ControlLost()
    {
      
    }

    public override void SimulateController()
    {
        ICustomPlayerCommandInput input = CustomPlayerCommand.Create();

        input.Position = transform.position;
        input.Velocity = GetComponent<Rigidbody2D>().velocity;
        input.Rotation = transform.rotation;
        input.Scale = transform.localScale;
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
                transform.localScale = cmd.Input.Scale;
                state.localScale = transform.localScale;
            }
            cmd.Result.Position = transform.position;
        }
    }
}
