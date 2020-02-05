using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Sirenix.OdinInspector;

public class SoldierAgent : EntityBehaviour<ISoldier>
{

    bool isEntityOwner
    {
        get
        {
#if UNITY_EDITOR
            if (SetIsOwnerToTrue)
                return true;
#endif
            return entity.IsOwner;
        }
    }

    public override void Attached()
    {
        state.SetTransforms(state.trans, transform);

        Transform animatorTransform = transform.Find("Animator/TmpSprite");
        if (animatorTransform != null)
        {
            var animator = animatorTransform.gameObject.GetComponent<Animator>();
            state.SetAnimator(animator);
        }
    }



    void Update()
    {
        //If is owner
        if (isEntityOwner)
            ServerUpdate();
    }

  

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit == null || hit.gameObject == null)
        {
            return;
        }
        if (isEntityOwner)
            ServerOnHit(hit.gameObject);
    }
    void OnCollisionEnter2D(Collision2D hit)
    {
        if (hit == null || hit.gameObject == null)
        {
            return;
        }

        if (isEntityOwner)
            ServerOnHit(hit.gameObject);
    }

    #region Server Methods

    [Header("Server")]
    public float moveSpeed = 2;
    public SideEnem side = SideEnem.None;

    [ShowInInspector]
    [ReadOnly]
    SoldierStateEnum serverSoldierState = SoldierStateEnum.Move;

    private void ServerUpdate()
    {
        switch (serverSoldierState)
        {
            case SoldierStateEnum.Move:
                ServerMove();
                break;
            case SoldierStateEnum.HitBack:
                ServerKnockBackCooldownCounter();
                break;
        }       
    }

    void ServerMove()
    {
        if (IsGround)
        {
            Vector2 force = side == SideEnem.Left ? Vector2.right : Vector2.left;
            transform.Translate(force * moveSpeed * Time.deltaTime);
        }
    }

    void ServerOnHit(GameObject hit)
    {
        var unit = hit.GetComponent<Unit>();

        if (unit == null) { return; }
        switch (unit.UnitType)
        {
            case UnitTypeEnum.Soldier:
                Rigidbody2D rigid = hit.GetComponent<Rigidbody2D>();
                if (rigid)
                    ServerKnockback(rigid);

                //Change state
                //Initialize KnockBackCoolDown
                serverKnockBackCoolDownPass = 0.0f;
                serverSoldierState = SoldierStateEnum.HitBack;
                break;
        }
    }

    public Vector2 KnockBackBaseForce = Vector2.right;
    public float serverKnockBackCooldown = 1.5f;

    void ServerKnockback(Rigidbody2D rigid)
    {
       
        float dir = side == SideEnem.Left ? 1.0f : -1.0f;
        Vector2 newKnockbackForce = new Vector2(dir * KnockBackBaseForce.x, KnockBackBaseForce.y);
        Debug.Log("hit" + newKnockbackForce);
        rigid.AddForce(newKnockbackForce, ForceMode2D.Impulse);
    }

   
    [ReadOnly]
    public float serverKnockBackCoolDownPass = 0.0f;
    void ServerKnockBackCooldownCounter()
    {
        serverKnockBackCoolDownPass += Time.deltaTime;
        if (serverKnockBackCoolDownPass > serverKnockBackCooldown)
            serverSoldierState = SoldierStateEnum.Move;
    }

    #region Ground Check
    const string groundCheckGroup = "Ground check";
    /// <summary>
    /// Is player grounded
    /// </summary>
    [BoxGroup(groundCheckGroup)]
    [ShowInInspector]
    public bool IsGround { get { return Physics2D.OverlapCircle(transform.position + checkerPositionOffset, checkerRadius, checkLayer); } }
    /// <summary>
    /// Define where the checker should be relatively to player.
    /// </summary>
    [BoxGroup(groundCheckGroup)]
    public Vector3 checkerPositionOffset = Vector3.zero;
    /// <summary>
    /// Define how large the checker is.
    /// </summary>  
    [BoxGroup(groundCheckGroup)]
    public float checkerRadius = 1.0f;
    /// <summary>
    /// Define how many layers the checker will check.
    /// </summary>
    [BoxGroup(groundCheckGroup)]
    public LayerMask checkLayer;
    #endregion
    #endregion


#if UNITY_EDITOR
    const string testGroup = "Local Test";
    /// <summary>
    /// Enable to active local test
    /// </summary>
    [BoxGroup(testGroup)]
    public bool SetIsOwnerToTrue = false;
#endif
    void OnDrawGizmosSelected()
    {
#region Ground Check
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + checkerPositionOffset, checkerRadius);
#endregion
    }
}
