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
        ServerBaseMass = GetComponent<Rigidbody2D>().mass;
        state.SetTransforms(state.trans, transform);

        Transform animatorTransform = transform.Find("Animator/TmpSprite");
        if (animatorTransform != null)
        {
            animator = animatorTransform.gameObject.GetComponent<Animator>();
            state.SetAnimator(animator);
        }
      

        state.AddCallback("Equiping", () =>
        {
            UpdateAnimation();
            if (isEntityOwner)
            {               
                CalculateStats();
            }
        }
        );
        UpdateAnimation();

        if (entity.IsOwner)
        {
            StartCoroutine(waitAndEquip());
        }
    }
    IEnumerator waitAndEquip()
    {
        yield return new WaitForSeconds(0.1f);
        ServerSetupInitialEquipment();
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
                //Initialize KnockBackCoolDownConter
                serverKnockBackCoolDownPass = 0.0f;
                isItemDropped = false;
                serverSoldierState = SoldierStateEnum.HitBack;
                break;
            case UnitTypeEnum.Item:
                var itemEntity = hit.GetComponent<BoltEntity>();
                ServerSetEquipment(itemEntity);
                break;
        }
    }


    const string equipmentsGroup = "Equipments";

    [BoxGroup(equipmentsGroup)]
    public GameObject[] InitialEquipments;
    [BoxGroup(equipmentsGroup)]
    [ShowInInspector]
    public BoltEntity Weapon
    {
        get
        {
            if (entity.IsAttached)
                return state.Equiping.Weapon;
            return null;
        }
    }
    [BoxGroup(equipmentsGroup)]
    [ShowInInspector]
    public BoltEntity Armor
    {
        get
        {
            if (entity.IsAttached)
                return state.Equiping.Armor;
            return null;
        }
    }

    void ServerSetupInitialEquipment()
    {
        foreach (var quipment in InitialEquipments)
        {
            var entity = BoltNetwork.Instantiate(quipment);
            ServerSetEquipment(entity);
        }
    }
    [BoxGroup(equipmentsGroup)]
    [Button]
    public void ServerSetEquipment(BoltEntity equipmentEntity)
    {
        var itemAgent = equipmentEntity.GetComponent<ItemAgent>();
        if (itemAgent == null) { return; }
        if (itemAgent.itemState != ItemStateEnum.New) { return; }
        if (itemAgent.state.Holder.HoldBy != null) { return; }

        switch (itemAgent.itemType)
        {
            case ItemTypeEnum.Armor:
                if (state.Equiping.Armor == null)
                {
                    state.Equiping.Armor = equipmentEntity;
                    itemAgent.ServerSetHolder(entity, Vector3.zero);
                    itemAgent.ServerSetIsRenderer(false);
                }
                break;
            case ItemTypeEnum.Weapon:
                if (state.Equiping.Weapon == null)
                {
                    state.Equiping.Weapon = equipmentEntity;
                    itemAgent.ServerSetHolder(entity, Vector3.zero);
                    itemAgent.ServerSetIsRenderer(false);
                }
                break;
        }

    }
    [BoxGroup(equipmentsGroup)]
    [Button]
    public void ServerDropEquipment(ItemTypeEnum itemType)
    {
        switch (itemType)
        {
            case ItemTypeEnum.Armor:
                if (state.Equiping.Armor != null)
                {
                    var itemAgent = state.Equiping.Armor.GetComponent<ItemAgent>();
                    itemAgent.ServerSetItemState(ItemStateEnum.Garbage);
                    itemAgent.ServerSetHolder(null, Vector3.zero);
                    itemAgent.ServerSetIsRenderer(true);
                    state.Equiping.Armor = null;
                }
                break;
            case ItemTypeEnum.Weapon:
                if (state.Equiping.Weapon != null)
                {
                    var itemAgent = state.Equiping.Weapon.GetComponent<ItemAgent>();
                    itemAgent.ServerSetItemState(ItemStateEnum.Garbage);
                    itemAgent.ServerSetHolder(null, Vector3.zero);
                    itemAgent.ServerSetIsRenderer(true);
                    state.Equiping.Weapon = null;
                }
                break;
        }
    }
    [BoxGroup(equipmentsGroup)]
    [Button]
    public void ServerDropRandomEquipment()
    {
        if (state.Equiping.Armor != null && state.Equiping.Weapon != null)        
            ServerDropEquipment((ItemTypeEnum)Random.Range(1, 3));
        else if(state.Equiping.Armor != null)
            ServerDropEquipment(ItemTypeEnum.Armor);
        else if (state.Equiping.Weapon != null)
            ServerDropEquipment(ItemTypeEnum.Weapon);
    }

    const string attackAndDefenceGroup = "Attack and Defense";
    const string equipmentModSubGroup = attackAndDefenceGroup + "/Runtime Calculated Stats";
    /// <summary>
    /// Knock back force apply to opponent
    /// </summary>
    [BoxGroup(attackAndDefenceGroup)]
    public Vector2 ServerBaseAttKnockBackForce = new Vector2(5, 3);
    [BoxGroup(attackAndDefenceGroup)]
    public float serverKnockBackCooldown = 1.5f;
    public float serverKnockBackDropItemDelay = 0.5f;

    [BoxGroup(equipmentModSubGroup)]
    public Vector2 ServerModAttKnockBackForce = Vector2.zero;
    [BoxGroup(equipmentModSubGroup)]
    [ShowInInspector]
    public Vector2 FinalKnockBackForce { get { return ServerBaseAttKnockBackForce + ServerModAttKnockBackForce; } }
    /// <summary>
    /// Same as the initial value of rigid body
    /// </summary>
    [BoxGroup(equipmentModSubGroup)]
    [ReadOnly]
    public float ServerBaseMass;
    /// <summary>
    /// Defense
    /// </summary>
    [BoxGroup(equipmentModSubGroup)]
    [ShowInInspector]
    public float ServerFinalMass { get { return GetComponent<Rigidbody2D>().mass; } }

    void ServerKnockback(Rigidbody2D rigid)
    {
        float dir = side == SideEnem.Left ? 1.0f : -1.0f;

        Vector2 newKnockbackForce = new Vector2(
                                    dir * FinalKnockBackForce.x,
                                    FinalKnockBackForce.y);

        Debug.Log("Added knock back force " + newKnockbackForce, rigid);
        rigid.AddForce(newKnockbackForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Calculate stats according to equipped equipments
    /// </summary>
    [Button]
    [BoxGroup(equipmentModSubGroup)]
    public void CalculateStats()
    {
        Vector2 attackMod = Vector3.zero;
        float defenceMod = 0.0f;
        if (state.Equiping.Armor != null)
        {
            var itemStats = state.Equiping.Armor.GetComponent<ItemAgent>().GetItemStats();
            attackMod += itemStats.Attack;
            defenceMod += itemStats.Defence;
        }
        if (state.Equiping.Weapon != null)
        {
            var itemStats = state.Equiping.Weapon.GetComponent<ItemAgent>().GetItemStats();
            attackMod += itemStats.Attack;
            defenceMod += itemStats.Defence;
        }
        ServerModAttKnockBackForce = attackMod;
        //Use mass to represent defense stats
        GetComponent<Rigidbody2D>().mass = ServerBaseMass + defenceMod;
    }

    [ReadOnly]
    [BoxGroup(attackAndDefenceGroup)]
    public float serverKnockBackCoolDownPass = 0.0f;
    bool isItemDropped = false;
    void ServerKnockBackCooldownCounter()
    {
        serverKnockBackCoolDownPass += Time.deltaTime;
        if (serverKnockBackCoolDownPass > serverKnockBackDropItemDelay && isItemDropped == false)
        {
            isItemDropped = true;
            //Drop item
            ServerDropRandomEquipment();           
        }
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

    const string animationGroup = "Animations";
    Animator animator;
    [BoxGroup(animationGroup)]
    public string
        HaveWeaponAndEquipmentAnimName = "All",
        JustWeaponAnimName = "swordani",
        JustEquipmentAnimName = "Armorani",
        NoItemAnimName = "Peopleani";

    void UpdateAnimation()
    {
        if (state.Equiping.Weapon != null && state.Equiping.Armor != null)
        {
            animator.Play(HaveWeaponAndEquipmentAnimName);
        }
        else if (state.Equiping.Weapon == null && state.Equiping.Armor == null)
        {
            animator.Play(NoItemAnimName);
        }
        else if (state.Equiping.Weapon != null && state.Equiping.Armor == null)
        {
            animator.Play(JustWeaponAnimName);
        }
        else if (state.Equiping.Weapon == null && state.Equiping.Armor != null)
        {
            animator.Play(JustEquipmentAnimName);
        }
    }

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
