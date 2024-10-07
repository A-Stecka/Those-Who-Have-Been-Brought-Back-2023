using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronDoorController : MonoBehaviour
{
    public bool opensWithAltars;
    public AltarController[] altars;
    public LeverController lever;
    public int globalControllerID;

    private Animator _animator;
    private bool _alreadyUp;

    private void Start()
    {
        _animator = this.GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene && GlobalController.Instance.openDoors[globalControllerID])
        {
            this.GetComponentInChildren<Animator>().SetTrigger("Load");
            _alreadyUp = true;
            SaveAnimation(true);
        }
        else
        {
            _alreadyUp = false;
            SaveAnimation(false);
        }
    }

    void Update()
    {
        if (opensWithAltars)
        {
            bool allActivated = true;

            for (int i = 0; i < altars.Length; i++)
            {
                if (!altars[i].activated)
                {
                    allActivated = false;
                }
            }

            if (!_alreadyUp)
            {
                if (allActivated)
                {
                    _animator.ResetTrigger("Down");
                    _animator.SetTrigger("Up");
                    _alreadyUp = true;
                    SaveAnimation(true);
                }
                else
                {
                    _animator.ResetTrigger("Up");
                    _animator.SetTrigger("Down");
                    _alreadyUp = false;
                    SaveAnimation(false);
                }
            }
        }
        else
        {
            if (lever.activated && !_alreadyUp)
            {
                _animator.ResetTrigger("Down");
                _animator.SetTrigger("Up");
                _alreadyUp = true;
                SaveAnimation(true);
            }
        }
    }

    private void SaveAnimation(bool value)
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.openDoors[globalControllerID] = value;
        }
    }
}
