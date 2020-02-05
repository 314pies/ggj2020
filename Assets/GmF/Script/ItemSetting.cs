using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete]
[System.Serializable]
public class ItemSetting
{
    public ItemTypeEnum ItemType = ItemTypeEnum.None;
    public ItemStateEnum ItemState = ItemStateEnum.New;
    public float addPushSpeed;
    public float addPushTime;
    public string ResourcePath = "ItemSample";

    public void Clone(ItemSetting sampleItem)
    {
        ItemType = sampleItem.ItemType;
        ItemState = sampleItem.ItemState;
        addPushSpeed = sampleItem.addPushSpeed;
        addPushTime = sampleItem.addPushTime;
        ResourcePath = sampleItem.ResourcePath;
    }
}
