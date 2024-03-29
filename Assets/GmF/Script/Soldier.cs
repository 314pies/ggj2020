﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public SideEnem side = SideEnem.None;
    public float moveSpeed = 2;
    float hitBackSpeed = 4;
    SoldierStateEnum soldierState = SoldierStateEnum.Init;
    public float hitBackTime = 1;
    float hitBackOverTime = 0;
    public float PushBaseSpeed = 3;
    public float PushBaseTime = 1;
    public Vector2 knockbackForce = Vector2.right;

    public ItemSetting WeaponItem = null;
    public ItemSetting EquipmentItem = null;

    public int DropMax = 2;
    public int DropRangMax = 1;
    public AudioClip DropSound;
    public AudioClip Get;
    AudioSource audioSource;

    /// <summary>
    /// Define where the checker should be relatively to player.
    /// </summary>
    [Header("Ground check")]
    public Vector3 checkerPositionOffset = Vector3.zero;
    /// <summary>
    /// Define how large the checker is.
    /// </summary>
    public float checkerRadius = 1.0f;
    /// <summary>
    /// Define how many layers the checker will check.
    /// </summary>
    public LayerMask checkLayer;
    /// <summary>
    /// Is true when player is on the ground.
    /// </summary>
    bool isGrounded = true;

    Animator animator = null;
    public string HaveWeaponAndEquipmentAnimName = "All";
    public string JustWeaponAnimName = "swordani";
    public string JustEquipmentAnimName = "Armorani";
    public string NoItemAnimName = "Peopleani";

    public float GetAtkPushSpeed()
    {
        float totalPushSpeed = PushBaseSpeed + (WeaponItem != null ? WeaponItem.addPushSpeed : 0) + (EquipmentItem != null ? EquipmentItem.addPushSpeed : 0);
        return totalPushSpeed;
    }

    public float GetAtkPushTime()
    {
        float totalPushTime = PushBaseTime + (WeaponItem != null ? WeaponItem.addPushTime : 0) + (EquipmentItem != null ? EquipmentItem.addPushTime : 0);
        return totalPushTime;
    }

    private void Awake()
    {
        side = GetComponent<Side>().side;
        soldierState = SoldierStateEnum.Init;
        audioSource = GetComponent<AudioSource>();
        Transform animatorTransform = transform.Find("Animator/TmpSprite");
        if(animatorTransform != null)
        {
            animator = animatorTransform.gameObject.GetComponent<Animator>();
        }
        InitAnim();
    }

    public void InitAnim()
    {
        if(WeaponItem != null && EquipmentItem != null)
        {
            animator.Play(HaveWeaponAndEquipmentAnimName);
        }
        else if(WeaponItem == null && EquipmentItem == null)
        {
            animator.Play(NoItemAnimName);
        }
        else if(WeaponItem != null && EquipmentItem == null)
        {
            animator.Play(JustWeaponAnimName);
        }
        else if (WeaponItem == null && EquipmentItem != null)
        {
            animator.Play(JustEquipmentAnimName);
        }
    }

    public void Start()
    {
        soldierState = SoldierStateEnum.Move;
    }

    public void Update()
    {
        switch (soldierState)
        {
            case SoldierStateEnum.Move:
                if (isGrounded)
                    Move();
                break;
            case SoldierStateEnum.HitBack:
                HitBack();
                break;
        }
        UpdateIsGrounded();
    }

    /// <summary>
    /// Check whether the player is grounded.
    /// </summary>
    private void UpdateIsGrounded()
    {
        if (Physics2D.OverlapCircle(transform.position + checkerPositionOffset, checkerRadius, checkLayer))
            isGrounded = true;
        else
            isGrounded = false;
    }

    private void OnHit(GameObject hit)
    {
        if (hit.CompareTag("Soldier"))
        {
            
            Rigidbody2D rigid = hit.GetComponent<Rigidbody2D>();
            if (rigid)
                Knockback(rigid);
                
        }

        Unit unit = hit.GetComponent<Unit>();
        if (unit != null)
        {
            switch (unit.UnitType)
            {
                case UnitTypeEnum.Soldier:
                    audioSource.PlayOneShot(DropSound, 0.5f);
                    Soldier other_Soldier = hit.GetComponent<Soldier>();
                    if (other_Soldier != null)
                    {
                        if (other_Soldier.side != side && soldierState == SoldierStateEnum.Move)
                        {
                            soldierState = SoldierStateEnum.HitBack;
                            hitBackOverTime = 0;
                            hitBackSpeed = other_Soldier.GetAtkPushSpeed();
                            hitBackTime = other_Soldier.GetAtkPushTime();

                            Drop();
                        }
                    }
                    break;
                case UnitTypeEnum.Item:
                    
                    ItemBase other_item = hit.GetComponent<ItemBase>();
                    if (other_item != null && other_item.itemSetting.ItemState == ItemStateEnum.New)
                    {
                        if (WeaponItem == null && other_item.itemSetting.ItemType == ItemTypeEnum.Weapon)
                        {
                            audioSource.PlayOneShot(Get, 0.5f);
                            WeaponItem = new ItemSetting();
                            WeaponItem.Clone(other_item.itemSetting);
                            Destroy(other_item.gameObject);
                            InitAnim();
                        }
                        if (EquipmentItem == null && other_item.itemSetting.ItemType == ItemTypeEnum.Equipment)
                        {
                            audioSource.PlayOneShot(Get, 0.5f);
                            EquipmentItem = new ItemSetting();
                            EquipmentItem.Clone(other_item.itemSetting);
                            Destroy(other_item.gameObject);
                            InitAnim();
                        }
                    }
                    break;
            }
        }
    }

    private void Drop()
    {
        int rndInt = Random.Range(0, DropMax);
        bool drop = rndInt <= DropRangMax;
        if (!drop)
        {
            return;
        }

        if (WeaponItem != null && EquipmentItem != null)
        {
            if (Random.Range(0, 2) == 0)
            {
                ItemManager.CreateItem(WeaponItem, transform.position, ItemStateEnum.Garbage);
                WeaponItem = null;
                InitAnim();
            }
            else
            {
                ItemManager.CreateItem(EquipmentItem, transform.position, ItemStateEnum.Garbage);
                EquipmentItem = null;
                InitAnim();
            }
        }
        else if (WeaponItem != null)
        {
            ItemManager.CreateItem(WeaponItem, transform.position, ItemStateEnum.Garbage);
            WeaponItem = null;
            InitAnim();
        }
        else if (EquipmentItem != null)
        {
            ItemManager.CreateItem(EquipmentItem, transform.position, ItemStateEnum.Garbage);
            EquipmentItem = null;
            InitAnim();
        }
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit == null || hit.gameObject == null)
        {
            return;
        }
        OnHit(hit.gameObject);
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        if(hit == null || hit.gameObject == null)
        {
            return;
        }
        OnHit(hit.gameObject);
    }

    private void Move()
    {
        Vector2 force = side == SideEnem.Left ? Vector2.right : Vector2.left;
        transform.Translate(force * moveSpeed * Time.deltaTime);
    }

    private void HitBack()
    {
        if(hitBackOverTime <= hitBackTime)
        {
            /*Vector2 force = side == SideEnem.Left ? Vector2.left : Vector2.right;
            force = force * Time.deltaTime;
            transform.Translate(force * (hitBackSpeed * (hitBackOverTime / hitBackTime)));*/
        }
        else
        {
            soldierState = SoldierStateEnum.Move;
        }
        hitBackOverTime += Time.deltaTime;
    }

    void Knockback(Rigidbody2D rigid)
    {
        float dir = side == SideEnem.Left ? 1.0f : -1.0f;
        Vector2 newKnockbackForce = new Vector2(dir * knockbackForce.x, knockbackForce.y);

        rigid.AddForce(newKnockbackForce * GetAtkPushSpeed(), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Implement this OnDrawGizmosSelected if you want to draw gizmos only if the object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + checkerPositionOffset, checkerRadius);
    }
}
