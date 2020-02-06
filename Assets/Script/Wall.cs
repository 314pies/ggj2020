using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameStatusManager gameStatusManager;
    public SideEnem side = SideEnem.None;

    private void Awake()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameStatusManager.entity.IsOwner)
            ServerCheckFinish(collision.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameStatusManager.entity.IsOwner)
            ServerCheckFinish(collision.gameObject);
    }

    private void ServerCheckFinish(GameObject hitGameObject)
    {
        Debug.Log("ServerCheckFinish " + side, hitGameObject);
        Unit unit = hitGameObject.GetComponent<Unit>();
        if (unit == null)
            return;
        if (unit.UnitType != UnitTypeEnum.Soldier)
            return;
        if (side == SideEnem.Right)
        {
            if (hitGameObject.GetComponent<SoldierAgent>().side == SideEnem.Left)
            {
                //Left win
                gameStatusManager.ServerOnWallTouched(SideEnem.Left);
            }
        }

        if (side == SideEnem.Left)
        {
            if (hitGameObject.GetComponent<SoldierAgent>().side == SideEnem.Right)
            {
                //Right win
                gameStatusManager.ServerOnWallTouched(SideEnem.Right);
            }
        }
    }
}
