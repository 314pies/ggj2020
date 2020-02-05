using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Sirenix.OdinInspector;

public class ItemAgent : EntityBehaviour<IItem>
{

    public SpriteRenderer Renderer;
    public Sprite NewSprite, BrokenSprite;

    public ItemTypeEnum itemType;

    [ReadOnly]
    public ItemStateEnum itemState;
    public override void Attached()
    {
        state.AddCallback("Holder", () =>
        {
            if (state.Holder.HoldBy != null)
            {
                //Disable rigid body so it will follow parent transform
                GetComponent<Rigidbody2D>().isKinematic = true;
                transform.parent = state.Holder.HoldBy.transform;
                transform.localPosition = state.Holder.HoldingPosition;
                //Stop syncing position
                state.SetTransforms(state.trnas, null);
            }
            else
            {
                transform.parent = null;
                //Start syncing position
                state.SetTransforms(state.trnas, transform);
                GetComponent<Rigidbody2D>().isKinematic = false;
            }
        });

        state.AddCallback("ItemState", () =>
        {
            itemState = (ItemStateEnum)state.ItemState;
            switch (itemState)
            {
                case ItemStateEnum.New:
                    Renderer.sprite = NewSprite;
                    break;

                case ItemStateEnum.Garbage:
                    Renderer.sprite = BrokenSprite;
                    break;
            }

        });

        state.AddCallback("IsRender", () =>
            {
                Renderer.enabled = state.IsRender;
            }
        );
    }

    [Button]
    public void ServerSetHolder(BoltEntity parent, Vector3 HoldingPosition)
    {
        state.Holder.HoldBy = parent;
        state.Holder.HoldingPosition = HoldingPosition;
    }

    [Button]
    public void ServerSetItemState(ItemStateEnum item)
    {
        state.ItemState = (int)item;
    }

    [Button]
    public void ServerSetIsRenderer(bool isRenderer)
    {
        state.IsRender = isRenderer;
    }
}
