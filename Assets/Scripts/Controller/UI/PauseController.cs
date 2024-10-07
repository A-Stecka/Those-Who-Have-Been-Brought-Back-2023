using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject mainCanvas, helpCanvas;
    public AudioSource audioSource;

    void OnEnable()
    {
        PlayerController.inputActions.UI.Enable();
        PlayerController.inputActions.SinglePlayer.Disable();
    }

    private void OnDisable()
    {
        PlayerController.inputActions.UI.Enable();
        PlayerController.inputActions.SinglePlayer.Enable();
    }

    public void OnClose()
    {
        audioSource.Play();
        playerController.PlayerMovement(true);
        this.gameObject.SetActive(false);
    }

    public void OnHelp()
    {
        audioSource.Play();
        mainCanvas.SetActive(false);
        helpCanvas.SetActive(true);
    }

    public void OnBack()
    {
        audioSource.Play();
        mainCanvas.SetActive(true);
        helpCanvas.SetActive(false);
    }

    public void OnSaveAndExit()
    {
        audioSource.Play();
        GlobalController.Instance.SaveGame();
        Debug.LogWarning("exit doesnt work in unity, will work in build ((i am sent from PauseController))");
        Application.Quit();
    }

}
