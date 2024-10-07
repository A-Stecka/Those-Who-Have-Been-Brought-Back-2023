using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderController : MonoBehaviour
{
    public string hint;
    public PlayerController playerController;
    public int globalControllerID;
    private bool _active;

    private void Awake()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene && !GlobalController.Instance.activeColliders[globalControllerID])
        {
            _active = false;
            this.gameObject.SetActive(_active);
        }
        else
        {
            _active = true;
        }
        SaveCollider();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_active && hint != "")
        {
            playerController.ChangeHint(hint);
            Disable();
        }
    }

    private void OnDestroy()
    {
        SaveCollider();
    }

    public void Disable()
    {
        _active = false;
        SaveCollider();
    }

    private void SaveCollider()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.activeColliders[globalControllerID] = _active;
        }
    }
}
