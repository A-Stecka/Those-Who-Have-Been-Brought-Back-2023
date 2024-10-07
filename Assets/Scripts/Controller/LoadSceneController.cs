using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
    public bool active;
    public int sceneToLoad, currentScene;
    public bool firstLoad;

    private void Awake()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            firstLoad = GlobalController.Instance.firstLoad[sceneToLoad - 1];

            if (sceneToLoad == 5 && GlobalController.Instance.ending != "")
            {
                active = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (active && collider.CompareTag("Player"))
        {
            if (GlobalController.Instance != null)
            {
                GlobalController.Instance.currentScene = currentScene;
                GlobalController.Instance.nextScene = sceneToLoad;
                GlobalController.Instance.firstLoadOfScene = firstLoad;
                GlobalController.Instance.firstLoad[sceneToLoad - 1] = false;
                if (sceneToLoad == 5)
                {
                    SceneManager.LoadScene("Area_5_" + GlobalController.Instance.ending);
                } 
                else
                {
                    SceneManager.LoadScene("Area_" + sceneToLoad);
                }
            }
        }
    }
}
