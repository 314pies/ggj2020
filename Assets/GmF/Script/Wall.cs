using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    bool isFinish = false;
    SideEnem side = SideEnem.None;
    public CliffLeeCL.GameOverMenu _GameOverMenu = null;

    private void Awake()
    {
        side = gameObject.GetComponent<Side>().side;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckFinish(collision.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckFinish(collision.gameObject);
    }

    private void CheckFinish(GameObject hitGameObject)
    {
        if (isFinish)
        {
            return;
        }

        if (hitGameObject == null)
        {
            return;
        }

        Unit unit = hitGameObject.GetComponent<Unit>();
        if (unit == null)
        {
            return;
        }

        if (unit.UnitType != UnitTypeEnum.Soldier)
        {
            return;
        }

        Side hitSide = hitGameObject.GetComponent<Side>();

        if (hitSide == null)
        {
            return;
        }

        if (side == hitSide.side)
        {
            return;
        }

        _GameOverMenu.OnGameOver(side != SideEnem.Left);
    }
}
