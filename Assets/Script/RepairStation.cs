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
    public string[] WeaponRepairCommand = new string[] {"X","Z","Z","X" };
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
