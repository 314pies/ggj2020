using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NotificationManager : MonoBehaviour
{
    public GameObject root;
    public TMP_Text message;
    public GameObject loading,closeButton;

    static NotificationManager singleton;
    void Awake()
    {
        singleton = this;
    }

    public static void ShowNotification(string message, bool showLoading, bool showCloseButton = false)
    {
        singleton.root.SetActive(true);
        singleton.message.text = message;
        singleton.loading.SetActive(showLoading);
        singleton.closeButton.SetActive(showCloseButton);        
    }

    public static void CloseNotification()
    {
        singleton.root.SetActive(false);
    }
}
