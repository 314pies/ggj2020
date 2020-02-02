using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static void CreateItem(ItemSetting itemSetting, Vector2 pos, ItemStateEnum itemState)
    {
        try
        {
            print("Dropping Item" + itemSetting);
            print("Dropping Item resource path" + itemSetting.ResourcePath);

            //GameObject newItem = Instantiate(Resources.Load(itemSetting.ResourcePath)) as GameObject;
            GameObject newItem = BoltNetwork.Instantiate(BoltPrefabs.ItemSample, pos,Quaternion.identity);
            newItem.transform.position = pos;
            ItemBase newItemBase = newItem.GetComponent<ItemBase>();
            newItemBase.itemSetting.Clone(itemSetting);
            newItemBase.itemSetting.ItemState = itemState;
        }
        catch(System.Exception e)
        {
            Debug.LogError(e);
        }
    }
}
