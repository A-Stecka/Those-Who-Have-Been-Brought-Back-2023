using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("General")]
    public GameObject hint;
    public PlayerController playerController;
    public string playerHint;
    public Sprite itemImage;
    public GameObject sprite;
    public AudioSource audioSource;
    public int globalControllerID;

    [Header("Dialogue display")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueLineText;
    public string dialogueLine;

    private bool _active, _open;
    private Animator _animator;

    void Awake()
    {
        _animator = sprite.GetComponent<Animator>();

        if (GlobalController.Instance != null)
        {
            _active = GlobalController.Instance.ending == "B";
            _open = GlobalController.Instance.openChests[globalControllerID];

            if (_active)
            {
                ShowChest();
            }
        }
        else
        {
            _active = false;
            _open = false;
        }
    }

    private void OnDestroy()
    {
        SaveChest();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_active && !_open && collider.CompareTag("Player"))
        {
            hint.SetActive(true);
            PlayerController.inputActions.SinglePlayer.Interact.performed += OpenChest;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        hint.SetActive(false);
        PlayerController.inputActions.SinglePlayer.Interact.performed -= OpenChest;
    }

    void OpenChest(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PlayerController.inputActions.SinglePlayer.Interact.performed -= OpenChest;

        playerController.PlayerMovement(false);
        nameText.text = playerController.characterName;
        dialogueLineText.text = dialogueLine;
        dialogueCanvas.SetActive(true);

        _open = true;
        _animator.SetTrigger("Open");
        audioSource.Play();
        playerController.AddToBackpack(itemImage);

        SaveChest();

        PlayerController.inputActions.SinglePlayer.Interact.performed += Close;
    }

    void CloseDisplay()
    {
        dialogueCanvas.SetActive(false);
        playerController.PlayerMovement(true);
    }

    void Close(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PlayerController.inputActions.SinglePlayer.Interact.performed -= Close;
        hint.SetActive(false);
        CloseDisplay();
    }

    void ShowChest()
    {
        sprite.SetActive(true);

        BoxCollider2D[] colliders = this.GetComponents<BoxCollider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = true;
        }

        if (_open)
        {
            _animator.SetTrigger("Load");
        }
    }

    private void SaveChest()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.openChests[globalControllerID] = _open;
        }
    }
}
