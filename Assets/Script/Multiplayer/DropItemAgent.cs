using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
public class DropItemAgent : EntityBehaviour<IDroppingItem>
{
    public override void Attached()
    {
        state.SetTransforms(state.trans, transform);
    }
}
