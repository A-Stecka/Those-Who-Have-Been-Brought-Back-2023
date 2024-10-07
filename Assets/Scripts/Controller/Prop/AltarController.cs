using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarController : MonoBehaviour
{
    [Header("General")]
    public float speed;
    public List<SpriteRenderer> runes;
    public bool activated;
    public int globalControllerID;

    private Color currentColor, targetColor;

    void Start()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            activated = GlobalController.Instance.activeAltars[globalControllerID];
        }
        else
        {
            activated = false;
        }
        SaveAltar();
    }

    void Update()
    {
        currentColor = Color.Lerp(currentColor, targetColor, speed * Time.deltaTime);

        foreach (SpriteRenderer rune in runes)
        {
            rune.color = currentColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        activated = true;
        SaveAltar();
        targetColor = new Color(1, 1, 1, 1);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        activated = false;
        SaveAltar();
        targetColor = new Color(1, 1, 1, 0);
    }

    private void OnDestroy()
    {
        SaveAltar();
    }

    private void SaveAltar()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.activeAltars[globalControllerID] = activated;
        }
    }
}
