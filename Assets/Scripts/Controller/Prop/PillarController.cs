using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarController : MonoBehaviour
{
    public GameObject hint, glow;
    public int globalControllerID;
    public AudioSource audioSource;
    
    private bool _active;

    void Awake()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            _active = GlobalController.Instance.pillarData[globalControllerID].active;
            glow.SetActive(GlobalController.Instance.pillarData[globalControllerID].activated);
        }
        else
        {
            _active = true;
        }
        SavePillar();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && _active)
        {
            hint.SetActive(true);
            PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        hint.SetActive(false);
        PlayerController.inputActions.SinglePlayer.Interact.performed -= Interact;
    }

    private void OnDestroy()
    {
        SavePillar();
    }

    void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        glow.SetActive(!glow.activeSelf);
        audioSource.Play();
        SavePillar();
    }

    public bool Activated()
    {
        return glow.activeSelf;
    }

    public void Inactivate()
    {
        _active = false;
        SavePillar();
    }

    private void SavePillar()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.pillarData[globalControllerID].active = _active;
            GlobalController.Instance.pillarData[globalControllerID].activated = glow.activeSelf;
        }
    }
}
