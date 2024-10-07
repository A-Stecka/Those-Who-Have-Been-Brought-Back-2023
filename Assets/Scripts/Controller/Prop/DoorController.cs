using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject gateClosed, gateOpened;
    public int globalControllerID;

    private void Awake()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene && GlobalController.Instance.openDoors[globalControllerID])
        {
            gateClosed.SetActive(false);
            gateOpened.SetActive(true);
        }
        SaveDoor();
    }

    private void OnDestroy()
    {
        SaveDoor();
    }

    public void SaveDoor()
    {
        if (gateOpened.activeSelf && GlobalController.Instance != null)
        {
            GlobalController.Instance.openDoors[globalControllerID] = true;
        }
    }
}
