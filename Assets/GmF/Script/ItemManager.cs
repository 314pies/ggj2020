using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static void CreateItem(ItemSetting itemSetting, Vector2 pos, ItemStateEnum itemState)
    {
        GameObject newItem = Instantiate(Resources.Load(itemSetting.ResourcePath)) as GameObject;
        newItem.transform.position = pos;
        ItemBase newItemBase = newItem.GetComponent<ItemBase>();
        newItemBase.itemSetting.ItemState = itemState;
    }
}
