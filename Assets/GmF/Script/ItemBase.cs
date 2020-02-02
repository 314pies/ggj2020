using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemBase : MonoBehaviour
{
    public Sprite Weapons;
    public Sprite Equip;
    public ItemSetting itemSetting = new ItemSetting();
    void Start()
    {
        SpriteRenderer mySpr;
        mySpr =transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
        if (itemSetting.ItemType == ItemTypeEnum.Weapon)
        {
            mySpr.sprite = Weapons;
        }
        else
        {
            mySpr.sprite = Equip;
        }
        
    }

}
