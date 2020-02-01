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

    public ItemSetting WeaponItem = null;
    public ItemSetting EquipmentItem = null;

    
    public float GetAtkPushSpeed()
    {
        float totalPushSpeed = PushBaseSpeed + (WeaponItem != null ? WeaponItem.addPushSpeed : 0) + (EquipmentItem != null ? EquipmentItem.addPushSpeed : 0);
        return totalPushSpeed;
    }

    public float GetAtkPushTime()
    {
        float totalPushTime = PushBaseTime + (WeaponItem != null ? WeaponItem.addPushTime : 0) + (EquipmentItem != null ? EquipmentItem.addPushTime : 0);
        return PushBaseTime;
    }

    private void Awake()
    {
        WeaponItem = null;
        EquipmentItem = null;
        side = GetComponent<Side>().side;
        soldierState = SoldierStateEnum.Init;
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
                Move();
                break;
            case SoldierStateEnum.HitBack:
                HitBack();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();
        if(unit != null)
        {
            switch (unit.UnitType)
            {
                case UnitTypeEnum.Soldier:
                    Soldier other_Soldier = other.GetComponent<Soldier>();
                    if (other_Soldier != null)
                    {
                        if (other_Soldier.side != side && soldierState == SoldierStateEnum.Move)
                        {
                            soldierState = SoldierStateEnum.HitBack;
                            hitBackOverTime = 0;
                            hitBackSpeed = other_Soldier.GetAtkPushSpeed();
                            hitBackTime = other_Soldier.GetAtkPushTime();

                            if(WeaponItem != null && EquipmentItem != null)
                            {
                                if (Random.Range(0, 1) == 0)
                                {
                                    ItemManager.CreateItem(WeaponItem, transform.position, ItemStateEnum.Garbage);
                                    WeaponItem = null;
                                }
                                else 
                                {
                                    ItemManager.CreateItem(EquipmentItem, transform.position, ItemStateEnum.Garbage);
                                    EquipmentItem = null;
                                }
                            }
                           else if(WeaponItem != null)
                            {
                                ItemManager.CreateItem(WeaponItem, transform.position, ItemStateEnum.Garbage);
                                WeaponItem = null;
                            }
                            else if(EquipmentItem != null)
                            {
                                ItemManager.CreateItem(EquipmentItem, transform.position, ItemStateEnum.Garbage);
                                EquipmentItem = null;
                            }
                        }
                    }
                    break;
                case UnitTypeEnum.Item:
                    ItemBase other_item = other.GetComponent<ItemBase>();
                    if(other_item != null && other_item.itemSetting.ItemState == ItemStateEnum.New)
                    {
                        if(WeaponItem == null && other_item.itemSetting.ItemType == ItemTypeEnum.Weapon)
                        {
                            WeaponItem = new ItemSetting();
                            WeaponItem.Clone(other_item.itemSetting);
                            Destroy(other_item.gameObject);
                        }
                        if(EquipmentItem == null && other_item.itemSetting.ItemType == ItemTypeEnum.Equipment)
                        {
                            EquipmentItem = new ItemSetting();
                            EquipmentItem.Clone(other_item.itemSetting);
                            Destroy(other_item.gameObject);
                        }
                    }
                    break;
            }
        }
       
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
            Vector2 force = side == SideEnem.Left ? Vector2.left : Vector2.right;
            force = force * Time.deltaTime;
            transform.Translate(force * (hitBackSpeed * (hitBackOverTime / hitBackTime)));
        }
        else
        {
            soldierState = SoldierStateEnum.Move;
        }
        hitBackOverTime += Time.deltaTime;
    }
}