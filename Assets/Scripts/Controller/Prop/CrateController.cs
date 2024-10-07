using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateController : MonoBehaviour
{
    public int waitTime;
    public int globalControllerID;
    private Vector3 startingPosition;

    void Awake()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            this.transform.position = GlobalController.Instance.cratePositions[globalControllerID];
        }
        startingPosition = this.transform.position;
        SaveCrate();
    }

    private void Update()
    {
        SaveCrate();
    }

    private void OnDestroy()
    {
        SaveCrate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Untagged"))
        {
            StartCoroutine(Wait(collision.collider));
        }
    }

    IEnumerator Wait(Collider2D collider)
    {
        yield return new WaitForSeconds(waitTime);
        if (GetComponent<BoxCollider2D>().IsTouching(collider))
        {
            this.transform.position = startingPosition;
        }
    }

    private void SaveCrate()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.cratePositions[globalControllerID] = this.transform.position;
        }
    }
}
