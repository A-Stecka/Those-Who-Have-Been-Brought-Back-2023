using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TMPro;

public class MenuController : MonoBehaviour
{
    public GameObject mainCanvas, continueCanvas, newGameCanvas, helpCanvas, creditsCanvas;
    public AudioSource audioSource;
    public TextMeshProUGUI continueLevelSlot1, continueLevelSlot2, continueLevelSlot3, newLevelSlot1, newLevelSlot2, newLevelSlot3;

    private void Awake()
    {
        if (GlobalController.Instance != null && GlobalController.Instance.nextScene == 6)
        {
            ShowCredits();
        }
    }

    public void OnContinue()
    {
        audioSource.Play();
        mainCanvas.SetActive(false);
        continueCanvas.SetActive(true);
        LoadSaves(true);
    }

    public void OnContinueGameInSlot(int slot)
    {
        audioSource.Play();

        if (File.Exists(Application.persistentDataPath + "/savefile_" + slot + ".dat"))
        {
            GlobalController.Instance.saveSlot = slot;
            GlobalController.Instance.LoadGame();

            switch (GlobalController.Instance.nextScene)
            {
                case 0:
                    SceneManager.LoadScene("Area_1");
                    break;
                case 5:
                    SceneManager.LoadScene("Area_5_" + GlobalController.Instance.ending);
                    break;
                case 6:
                    break;
                default:
                    SceneManager.LoadScene("Area_" + GlobalController.Instance.nextScene);
                    break;
            }
        }
    }

    public void OnNewGame()
    {
        audioSource.Play();
        mainCanvas.SetActive(false);
        newGameCanvas.SetActive(true);
        LoadSaves(false);
    }

    public void OnNewGameInSlot(int slot)
    {
        audioSource.Play();
        GlobalController.Instance.saveSlot = slot;
        SceneManager.LoadScene("Area_1");
    }

    public void OnHelp()
    {
        audioSource.Play();
        mainCanvas.SetActive(false);
        helpCanvas.SetActive(true);
    }

    public void OnExit()
    {
        audioSource.Play();
        Debug.LogWarning("exit doesnt work in unity, will work in build ((i am sent from MenuController))");
        Application.Quit();
    }

    public void OnReturnToMain()
    {
        audioSource.Play();
        continueCanvas.SetActive(false);
        newGameCanvas.SetActive(false);
        helpCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }

    public void ShowCredits()
    {
        continueCanvas.SetActive(false);
        newGameCanvas.SetActive(false);
        helpCanvas.SetActive(false);
        mainCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(90);
        OnExit();
    }

    private void LoadSaves(bool continueGame)
    {
        BinaryFormatter bf = new BinaryFormatter();

        if (File.Exists(Application.persistentDataPath + "/savefile_1.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savefile_1.dat", FileMode.Open);
            GlobalStats data = (GlobalStats)bf.Deserialize(file);
            file.Close();

            if (continueGame)
            {
                continueLevelSlot1.text = data.nextScene switch
                {
                    0 => "Poziom: poziom 1",
                    6 => "Poziom: koniec",
                    _ => "Poziom: poziom " + data.nextScene,
                };
            }
            else
            {
                newLevelSlot1.text = data.nextScene switch
                {
                    0 => "Poziom: poziom 1",
                    6 => "Poziom: koniec",
                    _ => "Poziom: poziom " + data.nextScene,
                };
            }
        }

        if (File.Exists(Application.persistentDataPath + "/savefile_2.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savefile_2.dat", FileMode.Open);
            GlobalStats data = (GlobalStats)bf.Deserialize(file);
            file.Close();

            if (continueGame)
            {
                continueLevelSlot2.text = data.nextScene switch
                {
                    0 => "Poziom: poziom 1",
                    6 => "Poziom: koniec",
                    _ => "Poziom: poziom " + data.nextScene,
                };
            }
            else
            {
                newLevelSlot2.text = data.nextScene switch
                {
                    0 => "Poziom: poziom 1",
                    6 => "Poziom: koniec",
                    _ => "Poziom: poziom " + data.nextScene,
                };
            }
        }

        if (File.Exists(Application.persistentDataPath + "/savefile_3.dat"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/savefile_3.dat", FileMode.Open);
            GlobalStats data = (GlobalStats)bf.Deserialize(file);
            file.Close();

            if (continueGame)
            {
                continueLevelSlot3.text = data.nextScene switch
                {
                    0 => "Poziom: poziom 1",
                    6 => "Poziom: koniec",
                    _ => "Poziom: poziom " + data.nextScene,
                };
            }
            else
            {
                newLevelSlot3.text = data.nextScene switch
                {
                    0 => "Poziom: poziom 1",
                    6 => "Poziom: koniec",
                    _ => "Poziom: poziom " + data.nextScene,
                };
            }
        }
    }
}
