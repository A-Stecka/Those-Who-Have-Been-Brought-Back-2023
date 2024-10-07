using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class GlobalController : MonoBehaviour
{
    public static GlobalController Instance;

    [Header("Object counts")]
    public int itemCount;
    public int crateCount;
    public int doorCount;
    public int dialogueCount;
    public int colliderCount;
    public int slimeCount;
    public int leverCount;
    public int altarCount;
    public int pillarCount;
    public int puzzleItemCount;
    public int chestCount;

    [Header("Transition info")]
    public int currentScene;
    public int nextScene;
    public bool firstLoadOfScene;

    [Header("Game object info")]
    public bool[] firstLoad;
    public bool[] activeItems;
    public bool[] openDoors;
    public bool[] activeColliders;
    public bool[] activeAltars;
    public bool[] openChests;
    public Vector3[] cratePositions;
    public PlayerStats playerData;
    public DialogueStats[] dialogueData;
    public BasicStats[] slimeData;
    public BasicStats[] leverData;
    public BasicStats[] pillarData;
    public PuzzleItemStats[] puzzleItemData;
    public float musicVolume;
    public float soundEffectsVolume;

    [Header("Placeholder info")]
    public bool enableGreenGhost;
    public bool enableBlueGhost;
    public bool enableBlueGrave;
    public bool enableVioletGrave;
    public bool enableRedGrave;
    public bool enableGrave;

    [Header("Ending choice info")]
    public string ending;
    public int step;
    public Sprite sprite;

    [Header("Save info")]
    public int saveSlot;
    public bool loadingFromSave;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            InitialiseFields();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitialiseFields()
    {
        firstLoadOfScene = true;
        firstLoad = new bool[5];
        firstLoad[0] = false;
        for (int i = 1; i < firstLoad.Length; i++)
        {
            firstLoad[i] = true;
        }
        activeItems = new bool[itemCount];
        openDoors = new bool[doorCount];
        activeColliders = new bool[colliderCount];
        activeAltars = new bool[altarCount];
        openChests = new bool[chestCount];
        cratePositions = new Vector3[crateCount];
        for (int i = 0; i < crateCount; i++)
        {
            cratePositions[i] = new Vector3();
        }
        dialogueData = new DialogueStats[dialogueCount];
        for (int i = 0; i < dialogueCount; i++)
        {
            dialogueData[i] = new DialogueStats();
        }
        slimeData = new BasicStats[slimeCount];
        for (int i = 0; i < slimeCount; i++)
        {
            slimeData[i] = new BasicStats();
        }
        leverData = new BasicStats[leverCount];
        for (int i = 0; i < leverCount; i++)
        {
            leverData[i] = new BasicStats();
        }
        pillarData = new BasicStats[pillarCount];
        for (int i = 0; i < pillarCount; i++)
        {
            pillarData[i] = new BasicStats();
        }
        puzzleItemData = new PuzzleItemStats[puzzleItemCount];
        for (int i = 0; i < puzzleItemCount; i++)
        {
            puzzleItemData[i] = new PuzzleItemStats();
        }
        playerData = new PlayerStats
        {
            backpack = new List<Sprite>(),
        };
        musicVolume = 0.5f;
        soundEffectsVolume = 1.0f;
        enableGreenGhost = false;
        enableBlueGhost = false;
        enableBlueGrave = false;
        enableVioletGrave = false;
        enableRedGrave = false;
        enableGrave = false;
        ending = "";
        step = 0;
        loadingFromSave = false;
    }

    public void ChooseEnding(string ending)
    {
        DialogueController guardianSpecial = GameObject.Find("CHARACTER Guardian Special").GetComponent<DialogueController>();
        DialogueController ghostRed = GameObject.Find("CHARACTER Red Ghost").GetComponent<DialogueController>();

        if (guardianSpecial != null && ghostRed != null)
        {
            this.ending = ending;
            if (ending == "A")
            {
                guardianSpecial.DisableCutscene();
            }
            else
            {
                ghostRed.DisableCutscene();
            }
        }
    }

    public void NextStep()
    {
        step++;

        if (step == 4)
        {
            switch (ending)
            {
                case "A":
                    enableGrave = true;
                    PlayerController playerController = GameObject.Find("PLAYER").GetComponent<PlayerController>();
                    playerController.AddToBackpack(sprite);
                    playerController.DisplayCutscene();
                    break;
                case "B":
                    enableGrave = true;
                    PuzzleItemController languageBook = GameObject.Find("Language Book").GetComponent<PuzzleItemController>();
                    languageBook.PuzzleCompleted();
                    break;
            }
            
        }
    }

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savefile_" + saveSlot + ".dat");

        GlobalStats globalStats = new GlobalStats
        {
            currentScene = currentScene,
            nextScene = nextScene,
            firstLoadOfScene = false,
            firstLoad = firstLoad,
            activeItems = activeItems,
            openDoors = openDoors,
            activeColliders = activeColliders,
            activeAltars = activeAltars,
            cratePositions = new float[cratePositions.Length][],
            playerData = new PlayerStatsSerializable
            {
                x = playerData.x,
                y = playerData.y,
                hasBackpack = playerData.hasBackpack,
                understandsGuardians = playerData.understandsGuardians,
                canReadGravestone = playerData.canReadGravestone,
                backpack = new List<string>(),
                hint = playerData.hint,
                backpackIndex = playerData.backpackIndex,
                speed = playerData.speed
            },
            dialogueData = dialogueData,
            slimeData = slimeData,
            leverData = leverData,
            pillarData = pillarData,
            openChests = openChests,
            puzzleItemData = puzzleItemData,
            musicVolume = musicVolume,
            soundEffectsVolume = soundEffectsVolume,
            enableGreenGhost = enableGreenGhost,
            enableBlueGhost = enableBlueGhost,
            enableBlueGrave = enableBlueGrave,
            enableVioletGrave = enableVioletGrave,
            enableRedGrave = enableRedGrave,
            enableGrave = enableGrave,
            ending = ending,
            step = step,
            sprite = sprite.texture.name
        };

        for (int i = 0; i < cratePositions.Length; i++)
        {
            globalStats.cratePositions[i] = new float[3];
            globalStats.cratePositions[i][0] = cratePositions[i].x;
            globalStats.cratePositions[i][1] = cratePositions[i].y;
            globalStats.cratePositions[i][2] = cratePositions[i].z;
        }

        for (int i = 0; i < playerData.backpack.Count; i++)
        {
            globalStats.playerData.backpack.Add(playerData.backpack[i].texture.name);
        }

        bf.Serialize(file, globalStats);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/savefile_" + saveSlot + ".dat"))
        {
            loadingFromSave = true;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savefile_" + saveSlot + ".dat", FileMode.Open);
            GlobalStats data = (GlobalStats) bf.Deserialize(file);
            file.Close();

            currentScene = data.currentScene;
            nextScene = data.nextScene;
            firstLoadOfScene = data.firstLoadOfScene;
            firstLoad = data.firstLoad;
            activeItems = data.activeItems;
            openDoors = data.openDoors;
            activeColliders = data.activeColliders;
            activeAltars = data.activeAltars;
            cratePositions = new Vector3[data.cratePositions.Length];
            playerData = new PlayerStats 
            {
                x = data.playerData.x,
                y = data.playerData.y,
                hasBackpack = data.playerData.hasBackpack,
                understandsGuardians = data.playerData.understandsGuardians,
                canReadGravestone = data.playerData.canReadGravestone,
                backpack = new List<Sprite>(),
                hint = data.playerData.hint,
                backpackIndex = data.playerData.backpackIndex,
                speed = data.playerData.speed
            };
            dialogueData = data.dialogueData;
            slimeData = data.slimeData;
            leverData = data.leverData;
            pillarData = data.pillarData;
            openChests = data.openChests;
            puzzleItemData = data.puzzleItemData;
            musicVolume = data.musicVolume;
            soundEffectsVolume = data.soundEffectsVolume;
            enableGreenGhost = data.enableGreenGhost;
            enableBlueGhost = data.enableBlueGhost;
            enableBlueGrave = data.enableBlueGrave;
            enableVioletGrave = data.enableVioletGrave;
            enableRedGrave = data.enableRedGrave;
            enableGrave = data.enableGrave;
            ending = data.ending;
            step = data.step;
            sprite = Resources.Load<Sprite>(data.sprite);

            for (int i = 0; i < cratePositions.Length; i++)
            {
                cratePositions[i] = new Vector3(data.cratePositions[i][0], data.cratePositions[i][1], data.cratePositions[i][2]);
            }

            for (int i = 0; i < data.playerData.backpack.Count; i++)
            {
                playerData.backpack.Add(Resources.Load<Sprite>(data.playerData.backpack[i]));
            }
        }
        else
        {
            Debug.LogError("savefile not found ((i am sent from GlobalController))");
        }
    }

}
