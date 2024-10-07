using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleItemController : MonoBehaviour
{
    [Header("General")]
    public PlayerController playerController;
    public bool ordered, book;
    public PillarController[] activePillars;
    public PillarController[] inactivePillars;
    public int globalControllerID;

    [Header("Display")]
    public GameObject sprite;

    private bool _activated, _pickedUp;
    private int _step;

    private void Start()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            _activated = GlobalController.Instance.puzzleItemData[globalControllerID].activated;
            _pickedUp = GlobalController.Instance.puzzleItemData[globalControllerID].pickedUp;
            _step = GlobalController.Instance.puzzleItemData[globalControllerID].step;
            if (_activated && !_pickedUp)
            {
                ShowPuzzleItem();
            }
        } 
        else
        {
            _activated = false;
            _pickedUp = false;
            _step = 0;
        }
        SavePuzzleItem();
    }

    void Update()
    {
        if (!_activated && !book)
        {
            if (!ordered)
            {
                bool correct = true;

                for (int i = 0; i < activePillars.Length; i++)
                {
                    if (!activePillars[i].Activated())
                    {
                        correct = false;
                    }
                }

                for (int i = 0; i < activePillars.Length; i++)
                {
                    if (inactivePillars[i].Activated())
                    {
                        correct = false;
                    }
                }

                if (correct)
                {
                    PuzzleCompleted();
                }
            }
            else
            {
                if ((_step == 0 && !activePillars[0].Activated() && !activePillars[1].Activated() && !activePillars[2].Activated() && !activePillars[3].Activated())
                    || (_step == 1 && activePillars[0].Activated() && !activePillars[1].Activated() && !activePillars[2].Activated() && !activePillars[3].Activated())
                    || (_step == 2 && activePillars[0].Activated() && activePillars[1].Activated() && !activePillars[2].Activated() && !activePillars[3].Activated())
                    || (_step == 3 && activePillars[0].Activated() && activePillars[1].Activated() && activePillars[2].Activated() && !activePillars[3].Activated())
                    || (_step == 4 && activePillars[0].Activated() && activePillars[1].Activated() && activePillars[2].Activated() && activePillars[3].Activated()))
                {
                    _step++;
                }

                if (_step == activePillars.Length + 1)
                {
                    PuzzleCompleted();
                }
            }
        }
    }

    private void OnDestroy()
    {
        SavePuzzleItem();
    }

    public void PuzzleCompleted()
    {
        _activated = true;
        ShowPuzzleItem();
        SavePuzzleItem();

        for (int i = 0; i < activePillars.Length; i++)
        {
            activePillars[i].Inactivate();
        }

        if (!ordered)
        {
            for (int i = 0; i < inactivePillars.Length; i++)
            {
                inactivePillars[i].Inactivate();
            }
        }
    }

    void ShowPuzzleItem()
    {
        sprite.SetActive(true);
        this.GetComponent<PolygonCollider2D>().enabled = true;
        this.GetComponent<BoxCollider2D>().enabled = true;

        if (!ordered)
        {
            this.GetComponent<DialogueController>().enabled = true;
        }
        else
        {
            this.GetComponent<ItemController>().enabled = true;
        }
    }

    private void SavePuzzleItem()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.puzzleItemData[globalControllerID].activated = _activated;
            GlobalController.Instance.puzzleItemData[globalControllerID].pickedUp = _pickedUp;
            GlobalController.Instance.puzzleItemData[globalControllerID].step = _step;
        }
    }

    public void PickUp()
    {
        _pickedUp = true;
        SavePuzzleItem();
    }
}
