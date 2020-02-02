using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

public class SoldierAgent : EntityBehaviour<ISoldier>
{
    public override void Attached()
    {
        if (!entity.IsOwner)
        {
            GetComponent<Soldier>().enabled = false;
        }

        state.SetTransforms(state.trans, transform);

        Transform animatorTransform = transform.Find("Animator/TmpSprite");
        if (animatorTransform != null)
        {
            var animator = animatorTransform.gameObject.GetComponent<Animator>();
            state.SetAnimator(animator);
        }
    }
}
