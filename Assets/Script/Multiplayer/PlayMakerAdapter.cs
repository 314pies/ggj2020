using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMakerAdapter : MonoBehaviour
{

    public bool IsLocal;

    public void UpdateLocal()
    {
        IsLocal = GetComponent<BoltEntity>().HasControl;
    }
}
