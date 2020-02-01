using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using CliffLeeCL;
public class PlayerAgent : EntityBehaviour<IPlayerState>
{
    public override void Attached()
    {
        state.SetTransforms(state.Position, transform);
        state.SetAnimator(GetComponent<Animator>());
        GetComponent<PlayerInteraction>().enabled = false;
        GetComponent<PlayerController2D>().enabled = false;
        GetComponent<PlayerAnimator>().enabled = false;
        state.AddCallback("localScaleX", ()=> {
            transform.localScale = new Vector3(state.localScaleX, transform.localScale.y, transform.localScale.z);
        });

        //GetComponent<Rigidbody2D>().isKinematic = true;
    }

    private void Update()
    {
        if (entity.IsOwner)
        {
            state.localScaleX = transform.localScale.x;
        }
    }

    public override void ControlGained()
    {
        GetComponent<PlayerInteraction>().enabled = true;
        GetComponent<PlayerController2D>().enabled = true;
        GetComponent<PlayerAnimator>().enabled = true;
        //GetComponent<Rigidbody2D>().isKinematic = false;
    }

    public override void ControlLost()
    {
        GetComponent<PlayerInteraction>().enabled = false;
        GetComponent<PlayerController2D>().enabled = false;
        GetComponent<PlayerAnimator>().enabled = false;

        //GetComponent<Rigidbody2D>().isKinematic = true;
    }

    //public override void SimulateController()
    //{
    //    ICustomPlayerCommandInput input = CustomPlayerCommand.Create();

    //    input.Position = transform.position;
    //    input.Velocity = GetComponent<Rigidbody2D>().velocity;

    //    entity.QueueInput(input);
    //}

    //public override void ExecuteCommand(Command command, bool resetState)
    //{
    //    CustomPlayerCommand cmd = (CustomPlayerCommand)command;

    //    if (resetState)
    //    {
    //        // we got a correction from the server, reset (this only runs on the client)
    //       // _motor.SetState(cmd.Result.Position, cmd.Result.Velocity, cmd.Result.IsGrounded, cmd.Result.JumpFrames);
    //    }
    //    else//Execute on server and controller
    //    {
    //        if (entity.IsOwner)
    //        {
    //            transform.position = cmd.Input.Position;
    //            GetComponent<Rigidbody2D>().velocity = cmd.Input.Velocity;
    //        }
    //        cmd.Result.Position = transform.position;
    //    }
    //}
}
