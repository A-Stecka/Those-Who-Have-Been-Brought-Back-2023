using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [Header("General")]
    public GameObject hint;
    public PlayerController playerController;
    public bool interactable, loop, active;
    public string[] playerHints;
    public int globalControllerID;

    [Header("Dialogue display")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueLineText;
    public string characterName, wrongItem;
    public string[] names, dialogueLines;
    public int[] cutsceneLenghts;
    public Sprite[] requiredItems;

    [Header("Game progression")]
    public GameObject[] collidersToDisable;
    public DialogueController[] cutscenesToEnable;
    public GameObject[] objectsToEnable;
    public Sprite[] cutsceneGifts;

    [Header("Guardians only")]
    public bool guardian;
    public bool loopGuardian;
    public string[] guardianLines;

    [Header("Gates only")]
    public bool gate;
    public GameObject gateClosed, gateOpened;
    public AudioSource audioSource;

    [Header("Scroll only")]
    public bool scroll;

    [Header("Book only")]
    public bool book;

    [Header("Bench only")]
    public bool bench;
    public Sprite itemForExtraHint;

    [Header("Weirdo & Orange Ghost only")]
    public bool weirdoOrOrange;
    public string triggerName;
    public int lastCutsceneIndex;

    [Header("Placeholder only")]
    public string placeholderFor;

    [Header("Gravestone only")]
    public bool gravestone;
    public bool ourGravestone;
    public string[] linesEndingA;
    public string[] linesEndingB;

    [Header("Final statue only")]
    public bool finalStatue;
    public bool nextToStatue;
    public GameObject sprite;
    private bool _activated;

    [Header("Ending")]
    public string ending;

    private int _index, _cutsceneIndex, _nextEnabled;

    void Awake()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            active = GlobalController.Instance.dialogueData[globalControllerID].active;
            loop = GlobalController.Instance.dialogueData[globalControllerID].loop;
            _nextEnabled = GlobalController.Instance.dialogueData[globalControllerID].nextEnabled;
            _index = GlobalController.Instance.dialogueData[globalControllerID].index;
            _cutsceneIndex = GlobalController.Instance.dialogueData[globalControllerID].cutsceneIndex;

            if (nextToStatue)
            {
                _activated = GlobalController.Instance.dialogueData[globalControllerID].activated;
                sprite.SetActive(_activated);
            }

            if (GlobalController.Instance.dialogueData[globalControllerID].switchAnimation)
            {
                this.GetComponentInChildren<Animator>().SetTrigger("Load");
            }

            if (GlobalController.Instance.enableGreenGhost && this.gameObject.name == "CHARACTER Green Ghost")
            {
                GlobalController.Instance.enableGreenGhost = false;
                EnableNextCutscene();
            }

            if (GlobalController.Instance.enableBlueGhost && this.gameObject.name == "CHARACTER Blue Ghost")
            {
                GlobalController.Instance.enableBlueGhost = false;
                EnableNextCutscene();
            }

            if (GlobalController.Instance.enableBlueGrave && this.gameObject.name == "PROP Blue Grave")
            {
                GlobalController.Instance.enableBlueGrave = false;
                EnableNextCutscene();
            }

            if (GlobalController.Instance.enableVioletGrave && this.gameObject.name == "PROP Violet Grave")
            {
                GlobalController.Instance.enableVioletGrave = false;
                EnableNextCutscene();
            }

            if (GlobalController.Instance.enableRedGrave && this.gameObject.name == "PROP Red Grave")
            {
                GlobalController.Instance.enableRedGrave = false;
                EnableNextCutscene();
            }
        }
        else
        {
            _index = 0;
            _cutsceneIndex = 0;
            _nextEnabled = 0;

            if (nextToStatue)
            {
                _activated = false;
            }
        }

        if (ourGravestone)
        {
            string[] newLines;
            int j = 0;
            switch (GlobalController.Instance.ending)
            {
                case "A":
                    newLines = new string[dialogueLines.Length + linesEndingA.Length];
                    cutsceneLenghts[1] = newLines.Length;
                    for (int i = 0; i < dialogueLines.Length; i++)
                    {
                        newLines[j] = dialogueLines[i];
                        j++;
                    }
                    for (int i = 0; i < linesEndingA.Length; i++)
                    {
                        newLines[j] = linesEndingA[i];
                        j++;
                    }
                    dialogueLines = newLines;
                    break;
                case "B":
                    newLines = new string[dialogueLines.Length + linesEndingB.Length];
                    cutsceneLenghts[1] = newLines.Length;
                    for (int i = 0; i < dialogueLines.Length; i++)
                    {
                        newLines[j] = dialogueLines[i];
                        j++;
                    }
                    for (int i = 0; i < linesEndingB.Length; i++)
                    {
                        newLines[j] = linesEndingB[i];
                        j++;
                    }
                    dialogueLines = newLines;
                    break;
            }
            names = new string[dialogueLines.Length];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = "Duch";
            }
        }

        SaveNPC();
    }

    private void Update()
    {
        if (GlobalController.Instance.enableGrave && this.gameObject.name == "PROP Our Grave")
        {
            GlobalController.Instance.enableGrave = false;
            active = false;
            EnableNextCutscene();
        }
    }

    private void OnDestroy()
    {
        SaveNPC();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && active)
        {
            if (loop)
            {
                _index = 0;
                SaveNPC();
            }

            if (interactable && (!nextToStatue || !_activated))
            {
                hint.SetActive(true);
                PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
            }
            else
            {
                playerController.PlayerMovement(false);

                nameText.text = names[_index];
                dialogueLineText.text = dialogueLines[_index];
                dialogueCanvas.SetActive(true);

                PlayerController.inputActions.SinglePlayer.Interact.performed += NextText;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (interactable)
        {
            hint.SetActive(false);
            PlayerController.inputActions.SinglePlayer.Interact.performed -= Interact;
        }
        else
        {
            PlayerController.inputActions.SinglePlayer.Interact.performed -= NextText;
        }
    }

    void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.PlayerMovement(false);
        PlayerController.inputActions.SinglePlayer.Interact.performed -= Interact;

        if (loop)
        {
            _index = 0;
            SaveNPC();
        }

        if (bench)
        {
            playerController.AnimateForward();
            hint.SetActive(false);
            StartCoroutine(Wait());
        }
        else
        {
            DisplayText();
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        hint.SetActive(true);
        DisplayText();
    }

    public void DisplayText()
    {
        if (requiredItems.Length == 0 || requiredItems[_cutsceneIndex] == null || (ourGravestone && playerController.canReadGravestone)
            || requiredItems[_cutsceneIndex] == playerController.SelectedItem())
        {
            if (requiredItems.Length != 0 && requiredItems[_cutsceneIndex] != null && (!ourGravestone || !playerController.canReadGravestone)
                && requiredItems[_cutsceneIndex] == playerController.SelectedItem()
                && (requiredItems[_cutsceneIndex].name != "Scroll" || (gameObject.name != "CHARACTER Red Ghost" && gameObject.name != "CHARACTER Guardian Special")))
            {
                playerController.RemoveFromBackpack(requiredItems[_cutsceneIndex]);
            }

            if (ending != "" && _cutsceneIndex == cutsceneLenghts.Length - 1)
            {
                GlobalController.Instance.ChooseEnding(ending);
            }

            if (gate)
            {
                audioSource.Play();
                gateClosed.SetActive(false);
                gateOpened.SetActive(true);
                this.gameObject.GetComponent<DoorController>().SaveDoor();
            }

            nameText.text = names[_index];
            if (playerController.understandsGuardians || !guardian)
            {
                dialogueLineText.text = dialogueLines[_index];
            }
            else
            {
                dialogueLineText.text = guardianLines[_index];
            }

            PlayerController.inputActions.SinglePlayer.Interact.performed += NextText;
        }
        else
        {
            nameText.text = characterName;
            dialogueLineText.text = wrongItem;
            PlayerController.inputActions.SinglePlayer.Interact.performed += Close;
        }

        dialogueCanvas.SetActive(true);
        SaveNPC();
    }

    void Close(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        dialogueCanvas.SetActive(false);
        playerController.PlayerMovement(true);

        PlayerController.inputActions.SinglePlayer.Interact.performed -= Close;
        PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
    }

    void NextText(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PlayerController.inputActions.SinglePlayer.Interact.performed -= NextText;

        _index++;

        if (_index < cutsceneLenghts[_cutsceneIndex] && (playerController.understandsGuardians || !guardian)
            || (bench && playerController.HasInBackpack(itemForExtraHint) && _index < cutsceneLenghts[_cutsceneIndex] + 1))
        {
            nameText.text = names[_index];
            dialogueLineText.text = dialogueLines[_index];
            PlayerController.inputActions.SinglePlayer.Interact.performed += NextText;
        }
        else
        {
            dialogueCanvas.SetActive(false);
            playerController.PlayerMovement(true);

            if (scroll)
            {
                playerController.understandsGuardians = true;
                this.gameObject.GetComponent<PuzzleItemController>().PickUp();
                this.gameObject.SetActive(false);
            }

            if (book)
            {
                playerController.canReadGravestone = true;
                this.gameObject.GetComponent<PuzzleItemController>().PickUp();
                this.gameObject.SetActive(false);
            }

            if (!loop || (guardian && !loopGuardian && playerController.understandsGuardians))
            {
                DisableCutscene();
            }
            else
            {
                if (interactable)
                {
                    PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
                }
            }
        }
        SaveNPC();
    }

    public void DisableCutscene()
    {
        if (nextToStatue)
        {
            _activated = true;
            sprite.SetActive(_activated);
            GlobalController.Instance.NextStep();
        }

        if (gravestone && _cutsceneIndex == cutsceneLenghts.Length - 1)
        {
            GlobalController.Instance.NextStep();
        }

        if (ourGravestone)
        {
            playerController.PlayerMovement(false);
            switch (GlobalController.Instance.ending)
            {
                case "A":
                    playerController.SetAnimation(-1);
                    break;
                case "B":
                    playerController.SetAnimation(-2);
                    break;
            }
            playerController.End();
        }

        if (collidersToDisable.Length > _cutsceneIndex && collidersToDisable[_cutsceneIndex] != null)
        {
            ColliderController colliderController = collidersToDisable[_cutsceneIndex].GetComponent<ColliderController>();
            if (colliderController != null)
            {
                colliderController.Disable();
            }
            else
            {
                ColliderController[] colliderControllers = collidersToDisable[_cutsceneIndex].GetComponentsInChildren<ColliderController>();
                for (int i = 0; i < colliderControllers.Length; i++)
                {
                    colliderControllers[i].Disable();
                }
            }
            collidersToDisable[_cutsceneIndex].SetActive(false);
        }

        if (finalStatue)
        {
            for (int i = 0; i < cutscenesToEnable.Length; i++)
            {
                cutscenesToEnable[i].EnableNextCutscene();
            }
        }
        else
        {
            if (cutscenesToEnable.Length > _cutsceneIndex && cutscenesToEnable[_cutsceneIndex] != null)
            {
                cutscenesToEnable[_cutsceneIndex].EnableNextCutscene();
            }
        }
   
        if (objectsToEnable.Length > _cutsceneIndex && objectsToEnable[_cutsceneIndex] != null)
        {
            objectsToEnable[_cutsceneIndex].GetComponent<ItemController>().Enable();
        }

        if (cutsceneGifts.Length > _cutsceneIndex && cutsceneGifts[_cutsceneIndex] != null)
        {
            playerController.AddToBackpack(cutsceneGifts[_cutsceneIndex]);
        }

        if (playerHints.Length != 0 && playerHints[_cutsceneIndex] != "")
        {
            playerController.ChangeHint(playerHints[_cutsceneIndex]);
        }

        if (weirdoOrOrange && _cutsceneIndex == lastCutsceneIndex)
        {
            this.GetComponentInChildren<Animator>().SetTrigger(triggerName);
            SaveAnimation();
        }

        if (_nextEnabled == 0 && (gameObject.name != "CHARACTER Red Ghost" || _cutsceneIndex != cutsceneLenghts.Length - 2))
        {
            active = false;

            if (hint != null)
            {
                hint.SetActive(false);
            }

            if (interactable)
            {
                PlayerController.inputActions.SinglePlayer.Interact.performed -= Interact;
            }
        }
        else
        {
            _cutsceneIndex++;
            PlayerController.inputActions.SinglePlayer.Interact.performed += Interact;
            if (_nextEnabled != 0)
            {
                _nextEnabled--;
            }
        }

        SaveNPC();
    }

    public void EnableNextCutscene()
    {
        if (placeholderFor != "")
        {
            if (placeholderFor == "CHARACTER Blue Ghost")
            {
                GlobalController.Instance.enableBlueGhost = true;
            }
            if (placeholderFor == "CHARACTER Green Ghost")
            {
                GlobalController.Instance.enableGreenGhost = true;
            }
            if (placeholderFor == "PROP Blue Grave")
            {
                GlobalController.Instance.enableBlueGrave = true;
            }
            if (placeholderFor == "PROP Violet Grave")
            {
                GlobalController.Instance.enableVioletGrave = true;
            }
            if (placeholderFor == "PROP Red Grave")
            {
                GlobalController.Instance.enableRedGrave = true;
            }
        }
        else
        {
            if (gate)
            {
                active = true;
            }
            else
            {
                if (!active || loop)
                {
                    active = true;
                    if (!nextToStatue)
                    {
                        _cutsceneIndex++;
                    }
                }
                else
                {
                    _nextEnabled++;
                }
            }
        }
        loop = false;
        SaveNPC();
    }

    private void SaveNPC()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.dialogueData[globalControllerID].active = active;
            GlobalController.Instance.dialogueData[globalControllerID].loop = loop;
            GlobalController.Instance.dialogueData[globalControllerID].nextEnabled = _nextEnabled;
            GlobalController.Instance.dialogueData[globalControllerID].index = _index;
            GlobalController.Instance.dialogueData[globalControllerID].cutsceneIndex = _cutsceneIndex;

            if (nextToStatue)
            {
                GlobalController.Instance.dialogueData[globalControllerID].activated = _activated;
            }
        }
    }

    private void SaveAnimation()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.dialogueData[globalControllerID].switchAnimation = true;
        }
    }

}
