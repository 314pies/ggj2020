using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using CliffLeeCL;
using UnityEngine.UI;

public class RepairStation : MonoBehaviour
{
    public enum Status { Idle, CapturingInput }
    public string[] WeaponRepairCommand = new string[] { "X", "Z", "Z", "X" };
    public string[] ArmorRepairCommand = new string[] { "C", "X", "Z", "C" };

    [ReadOnly]
    public Status status = Status.Idle;
    public GameObject CurrentCapturingPlayer;
    public string[] command;
    public GameObject UIRoot;

    public GameObject[] InputKeysUI;
    Text InputKeyText(GameObject inputUIRoot) { return inputUIRoot.GetComponentInChildren<Text>(); }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (status == Status.Idle)
        {
            var playerAgent = hit.gameObject.GetComponent<PlayerAgent>();
            if (playerAgent == null) { return; }
            if (!playerAgent.entity.HasControl) { return; }
            //InitialCapturing(playerAgent.gameObject); return;
            var carryingItem = playerAgent.state.CarryingItem;
            if (carryingItem == null) { return; }
            var itemAgent = carryingItem.GetComponent<ItemAgent>();
            if (itemAgent.itemState == ItemStateEnum.Garbage)
            {
                string[] command = null;
                if (itemAgent.itemType == ItemTypeEnum.Weapon)
                    command = WeaponRepairCommand;
                else if (itemAgent.itemType == ItemTypeEnum.Armor)
                    command = ArmorRepairCommand;

                InitialCapturing(playerAgent.gameObject, command);
            }
        }
    }

    void Update()
    {
        if (status == Status.CapturingInput)
        {
            CapturingInput();
        }
        SetFixingAnimationStatus();
    }

    public GameObject FixingAnimation;
    public Vector3 FixingAnimationTriggerAdjust = new Vector3(0.6f, 0.6f, 0.6f);
    List<Collider2D> playerList = new List<Collider2D>();
    void SetFixingAnimationStatus()
    {
        bool activeFixingAnim = false;
        ContactFilter2D Filter = new ContactFilter2D();
        //var pos = new Vector3(transform.position.x + GetComponent<BoxCollider2D>().offset.x, transform.position.y + GetComponent<BoxCollider2D>().offset.y);
        //Physics2D.OverlapBox(pos, GetComponent<BoxCollider2D>().size + new Vector2(0.4f,0.4f), 0, Filter, playerList);
        var col = GetComponent<BoxCollider2D>();
        Physics2D.OverlapBox(col.bounds.center, col.bounds.extents + FixingAnimationTriggerAdjust, col.transform.rotation.x, Filter, playerList);

        if (playerList.Count > 0)
        {
            foreach (var p in playerList)
            {
                if (p.gameObject.tag == "Player")
                {
                    if (p.GetComponent<PlayerAgent>() != null)
                    {
                        if (p.GetComponent<PlayerAgent>().state.CarryingItem != null)
                        {
                            if (p.GetComponent<PlayerAgent>().state.CarryingItem.GetComponent<ItemAgent>() != null)
                            {
                                if (ItemStateEnum.Garbage == (ItemStateEnum)p.GetComponent<PlayerAgent>().state.CarryingItem.GetComponent<ItemAgent>().itemState)
                                {
                                    activeFixingAnim = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        FixingAnimation.SetActive(activeFixingAnim);
    }

    void InitialCapturing(GameObject player, string[] command = null)
    {
        CurrentCapturingPlayer = player;
        CurrentCapturingPlayer.GetComponent<PlayerController2D>().enabled = false;
        status = Status.CapturingInput;
        ClearInput();
        if (command != null)
            this.command = command;
        UIRoot.SetActive(true);
    }

    void CloseCapturing()
    {
        if (CurrentCapturingPlayer != null)
            CurrentCapturingPlayer.GetComponent<PlayerController2D>().enabled = true;
        UIRoot.SetActive(false);
    }

    void ClearInput()
    {
        currentIndex = 0;
        foreach (var uiKey in InputKeysUI)
        {
            InputKeyText(uiKey).text = "";
        }
    }

    public int currentIndex = 0;
    void CapturingInput()
    {
        var commandKey = command[currentIndex];
        var keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), commandKey);

        if (Input.GetKeyDown(keyCode))
        {
            InputKeyText(InputKeysUI[currentIndex]).text = commandKey;
            currentIndex++;
        }
        else
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(vKey) && keyCode != vKey)
                {
                    ClearInput();
                }
            }
        }

        if (currentIndex >= command.Length)
        {
            status = Status.Idle;
            CloseCapturing();
            //Repair success
            //Send repair request
            var repairReq = RepairItemReq.Create(Bolt.GlobalTargets.OnlyServer);
            repairReq.Item = CurrentCapturingPlayer.GetComponent<PlayerAgent>().state.CarryingItem; ;
            repairReq.Send();
        }
    }
}
