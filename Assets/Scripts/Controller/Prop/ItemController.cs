using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [Header("General")]
    public GameObject hint;
    public PlayerController playerController;
    public bool emptyItem;
    public string playerHint;
    public Sprite itemImage;
    public DialogueController cutsceneToEnable;
    public GameObject colliderToDisable;
    public GameObject[] alternativeHidingSpots;
    public Sprite requiredItem;
    public int globalControllerID;

    [Header("Dialogue display")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueLineText;
    public string dialogueLine, wrongRequiredItem;

    [Header("Hidden object only")]
    public GameObject sprite;

    [Header("Backpack")]
    public bool isBackpack;
    public AudioSource audioSource;

    private bool _active;

    void Awake()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            _active = GlobalController.Instance.activeItems[globalControllerID];

            if (!emptyItem)
            {
                this.gameObject.SetActive(_active);
            }
            else
            {
                BoxCollider2D[] colliders = this.GetComponents<BoxCollider2D>();
                for (int j = 0; j < colliders.Length; j++)
                {
                    if (colliders[j].isTrigger)
                    {
                        colliders[j].enabled = _active;
                    }
                }
            }
        }
        else
        {
            _active = true;
        }
        SaveItem();
    }

    private void OnDestroy()
    {
        SaveItem();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_active && collider.CompareTag("Player") && (isBackpack || playerController.hasBackpack))
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

    void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PlayerController.inputActions.SinglePlayer.Interact.performed -= Interact;

        playerController.PlayerMovement(false);
        nameText.text = playerController.characterName;

        if (requiredItem == null || requiredItem == playerController.SelectedItem())
        {
            if (!emptyItem)
            {
                dialogueLineText.text = dialogueLine;
            }
            else
            {
                dialogueLineText.text = "Nic tutaj nie ma...";
            }
            PlayerController.inputActions.SinglePlayer.Interact.performed += PickUpItem;
        }
        else
        {
            dialogueLineText.text = wrongRequiredItem;
            PlayerController.inputActions.SinglePlayer.Interact.performed += Close;
        }

        dialogueCanvas.SetActive(true);
    }

    void CloseDisplay()
    {
        dialogueCanvas.SetActive(false);
        playerController.PlayerMovement(true);
    }

    void Close(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PlayerController.inputActions.SinglePlayer.Interact.performed -= Close;
        PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
        CloseDisplay();
    }

    void PickUpItem(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PlayerController.inputActions.SinglePlayer.Interact.performed -= PickUpItem;
        CloseDisplay(); 
        _active = false;

        if (!emptyItem)
        {
            this.gameObject.SetActive(false);

            if (colliderToDisable != null)
            {
                ColliderController colliderController = colliderToDisable.GetComponent<ColliderController>();
                if (colliderController != null)
                {
                    colliderController.Disable();

                }
                else
                {
                    ColliderController[] colliderControllers = colliderToDisable.GetComponentsInChildren<ColliderController>();
                    for (int i = 0; i < colliderControllers.Length; i++)
                    {
                        colliderControllers[i].Disable();
                    }
                }
                colliderToDisable.SetActive(false);
            }

            if (alternativeHidingSpots.Length != 0)
            {
                for (int i = 0; i < alternativeHidingSpots.Length; i++)
                {
                    BoxCollider2D[] colliders = alternativeHidingSpots[i].GetComponents<BoxCollider2D>();
                    for (int j = 0; j < colliders.Length; j++)
                    {
                        if (colliders[j].isTrigger)
                        {
                            colliders[j].enabled = false;
                        }
                    }
                }
            }

            if (isBackpack)
            {
                audioSource.Play();
                playerController.EnableBackpack();
            }
            else

            {
                playerController.AddToBackpack(itemImage);
                if (cutsceneToEnable != null)
                {
                    cutsceneToEnable.EnableNextCutscene();
                }
            }

            if (playerHint != "")
            {
                playerController.ChangeHint(playerHint);
            }
        }
        else
        {
            PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
        }
        SaveItem();
    }

    private void SaveItem()
    {
        if (!emptyItem && GlobalController.Instance != null)
        {
            GlobalController.Instance.activeItems[globalControllerID] = _active;

            if (alternativeHidingSpots.Length != 0)
            {
                for (int i = 0; i < alternativeHidingSpots.Length; i++)
                {
                    GlobalController.Instance.activeItems[alternativeHidingSpots[i].GetComponent<ItemController>().globalControllerID] = _active;
                }
            }
        }
    }

    public void Enable()
    {
        sprite.SetActive(true);
        this.GetComponent<PolygonCollider2D>().enabled = true;
        this.GetComponent<BoxCollider2D>().enabled = true;      
        this.GetComponent<ItemController>().enabled = true;
        SaveItem();
    }

}