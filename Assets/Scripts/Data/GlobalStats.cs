using System;

[Serializable]
class GlobalStats
{
    public int currentScene;
    public int nextScene;
    public bool firstLoadOfScene;
    public bool[] firstLoad;
    public bool[] activeItems;
    public bool[] openDoors;
    public bool[] activeColliders;
    public bool[] activeAltars;
    public bool[] openChests;
    public float[][] cratePositions;
    public PlayerStatsSerializable playerData = new PlayerStatsSerializable();
    public DialogueStats[] dialogueData;
    public BasicStats[] slimeData;
    public BasicStats[] leverData;
    public BasicStats[] pillarData;
    public PuzzleItemStats[] puzzleItemData;
    public float musicVolume;
    public float soundEffectsVolume;
    public bool enableGreenGhost;
    public bool enableBlueGhost;
    public bool enableBlueGrave;
    public bool enableVioletGrave;
    public bool enableRedGrave;
    public bool enableGrave;
    public string ending;
    public int step;
    public string sprite;
}
