using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

class EndingItemController : MonoBehaviour
{
    [Header("Display")]
    public GameObject sprite;
    public string ending;

    private void Start()
    {
        if (GlobalController.Instance != null && GlobalController.Instance.ending == ending)
        {
            ShowItem();
        }
    }

    public void ShowItem()
    {
        sprite.SetActive(true);
        this.GetComponent<PolygonCollider2D>().enabled = true;
        this.GetComponent<BoxCollider2D>().enabled = true;
        this.GetComponent<ItemController>().enabled = true;
    }

}