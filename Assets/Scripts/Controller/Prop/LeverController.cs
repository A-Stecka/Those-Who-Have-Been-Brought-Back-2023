using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public GameObject hint;
    public bool active, activated;
    public int globalControllerID;
    public AudioSource audioSource;

    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();

        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            active = GlobalController.Instance.leverData[globalControllerID].active;
            activated = GlobalController.Instance.leverData[globalControllerID].activated;
            if (activated)
            {
                _animator.SetTrigger("Load");
            }
        }
        else
        {
            activated = false;
        }
        SaveLever();
    }

    private void OnDestroy()
    {
        SaveLever();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && active)
        {
            hint.SetActive(true);
            PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
        }
    }

    void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PlayerController.inputActions.SinglePlayer.Interact.performed -= Interact;
        activated = true;
        active = false;
        hint.SetActive(false);
        _animator.SetTrigger("Lever");
        audioSource.Play();
        SaveLever();
    }

    public void Enable()
    {
        active = true;
        SaveLever();
    }

    private void SaveLever()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.leverData[globalControllerID].active = active;
            GlobalController.Instance.leverData[globalControllerID].activated = activated;
        }
    }
}
