using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public int globalControllerID;

    [Header("Special Slime only")]
    public bool special;
    public GameObject hint;
    public PlayerController playerController;
    public string triggerName;
    public string loadTriggerName;
    public LeverController lever;
    public string playerHint;

    [Header("Dialogue display")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueLineText;
    public string dialogueLine, wrongRequiredItem;
    public Sprite requiredItem;

    private Animator _animator;
    private bool _active, _activated;

    void Awake()
    {
        _animator = GetComponent<Animator>();

        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            _active = GlobalController.Instance.slimeData[globalControllerID].active;
            _activated = GlobalController.Instance.slimeData[globalControllerID].activated;
            if (_activated)
            {
                _animator.SetTrigger(loadTriggerName);
            }
        }
        else
        {
            _active = true;
            _activated = false;
        }
        SaveSlime();
    }

    private void OnDestroy()
    {
        SaveSlime();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            _animator.SetBool("Angry", true);

            if (special && _active && playerController.HasInBackpack(requiredItem))
            {
                hint.SetActive(true);
                PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        _animator.SetBool("Angry", false);

        if (special)
        {
            hint.SetActive(false);
            PlayerController.inputActions.SinglePlayer.Interact.performed -= Interact;
        }

    }

    void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.PlayerMovement(false);
        PlayerController.inputActions.SinglePlayer.Interact.performed -= Interact;
        hint.SetActive(false);

        nameText.text = playerController.characterName;

        if (requiredItem == playerController.SelectedItem())
        {
            _active = false;
            _activated = true;
            playerController.RemoveFromBackpack(requiredItem);
            dialogueLineText.text = dialogueLine;

            if (playerHint != "")
            {
                playerController.ChangeHint(playerHint);
            }

            _animator.SetBool("Angry", false);
            _animator.SetTrigger(triggerName);

            if (lever != null)
            {
                lever.Enable();
            }
        }
        else
        {
            dialogueLineText.text = wrongRequiredItem;
        }

        dialogueCanvas.SetActive(true);
        PlayerController.inputActions.SinglePlayer.Interact.performed += Close;
        SaveSlime();
    }

    void Close(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        dialogueCanvas.SetActive(false);
        playerController.PlayerMovement(true);
        PlayerController.inputActions.SinglePlayer.Interact.performed -= Close;
        PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
    }

    private void SaveSlime()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.slimeData[globalControllerID].active = _active;
            GlobalController.Instance.slimeData[globalControllerID].activated = _activated;
        }
    }
}
