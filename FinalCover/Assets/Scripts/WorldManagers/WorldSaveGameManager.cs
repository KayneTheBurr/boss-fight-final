using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;
    [SerializeField] public PlayerManager player;

    [Header("Save/Load")] //used in inspector for now 
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;

    [Header("World Scene Index")]
    [SerializeField] public int worldSceneIndex = 1;

    [Header("Save Data Writer")]
    //private WriteSaveData saveDataWriter;

    [Header("Current Character Data")]
    //public CharacterSlot currentCharacterSlotUsed;
    public CharacterSaveData currentCharacterData;
    private string fileName;

    [Header("CharacterSlots")]
    public CharacterSaveData characterSlot01, characterSlot02, characterSlot03, characterSlot04, characterSlot05,
                                characterSlot06, characterSlot07, characterSlot08, characterSlot09, characterSlot10;
    private void Awake()
    {
        //one at a time 
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllCharacterProfiles();
    }
    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }
        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }
    //public string DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot slot)
    //{
    //    string fileName = "";
    //    switch (slot)
    //    {
    //        case CharacterSlot.CharacterSlot_01:
    //            fileName = "characterSlot_01";
    //            break;
    //        case CharacterSlot.CharacterSlot_02:
    //            fileName = "characterSlot_02";
    //            break;
    //        case CharacterSlot.CharacterSlot_03:
    //            fileName = "characterSlot_03";
    //            break;
    //        case CharacterSlot.CharacterSlot_04:
    //            fileName = "characterSlot_04";
    //            break;
    //        case CharacterSlot.CharacterSlot_05:
    //            fileName = "characterSlot_05";
    //            break;
    //        case CharacterSlot.CharacterSlot_06:
    //            fileName = "characterSlot_06";
    //            break;
    //        case CharacterSlot.CharacterSlot_07:
    //            fileName = "characterSlot_07";
    //            break;
    //        case CharacterSlot.CharacterSlot_08:
    //            fileName = "characterSlot_08";
    //            break;
    //        case CharacterSlot.CharacterSlot_09:
    //            fileName = "characterSlot_09";
    //            break;
    //        case CharacterSlot.CharacterSlot_10:
    //            fileName = "characterSlot_10";
    //            break;
    //    }
    //    return fileName;

    //}
    public void AttemptToCreateNewGame()
    {
        //saveDataWriter = new WriteSaveData();
        //saveDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        ////check to see if we can make a new save file in this character slot (for each slot)
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_01);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_01;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_02);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_02;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_03);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_03;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_04);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_04;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_05);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_05;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_06);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_06;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_07);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_07;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_08);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_08;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_09);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_09;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}
        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_10);
        //if (!saveDataWriter.CheckToSeeIfFileExists())
        //{
        //    currentCharacterSlotUsed = CharacterSlot.CharacterSlot_10;
        //    currentCharacterData = new CharacterSaveData();
        //    NewGame();
        //    return;
        //}

        ////if there are no free slots, player must delete a character first 
        //TitleScreenManager.instance.DisplayNoFreeSlotsPopUp();

    }

    private void NewGame()
    {
        ////temporary until choosing a base character stats system exists
        //player.playerStatsManager.heart.SetInt(10);
        //player.playerStatsManager.strength.SetInt(10);

        //SaveGame();
        //StartCoroutine(LoadWorldScene());
    }
    public void LoadGame()
    {
        ////load a previous game with a file depending on which slot we are using
        //fileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(currentCharacterSlotUsed);

        //saveDataWriter = new WriteSaveData();

        ////generally works on multiple machine types 
        //saveDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        //saveDataWriter.saveFileName = fileName;
        //currentCharacterData = saveDataWriter.LoadSaveFile();

        //StartCoroutine(LoadWorldScene());
    }
    public void SaveGame()
    {
        //fileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(currentCharacterSlotUsed);

        //saveDataWriter = new WriteSaveData();
        ////generally works on multiple machine types 
        //saveDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        //saveDataWriter.saveFileName = fileName;

        ////pass player info from game to the save file
        //player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        ////write that info onto a json file saved on the (this) machine
        //saveDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }

    //load all character profiles on device when starting game 
    private void LoadAllCharacterProfiles()
    {
        //saveDataWriter = new WriteSaveData();
        //saveDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_01);
        //characterSlot01 = saveDataWriter.LoadSaveFile();

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_02);
        //characterSlot02 = saveDataWriter.LoadSaveFile();

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_03);
        //characterSlot03 = saveDataWriter.LoadSaveFile();

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_04);
        //characterSlot04 = saveDataWriter.LoadSaveFile();

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_05);
        //characterSlot05 = saveDataWriter.LoadSaveFile();

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_06);
        //characterSlot06 = saveDataWriter.LoadSaveFile();

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_07);
        //characterSlot07 = saveDataWriter.LoadSaveFile();

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_08);
        //characterSlot08 = saveDataWriter.LoadSaveFile();

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_09);
        //characterSlot09 = saveDataWriter.LoadSaveFile();

        //saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(CharacterSlot.CharacterSlot_10);
        //characterSlot10 = saveDataWriter.LoadSaveFile();

    }

    //public void DeleteCharacterSlot(CharacterSlot slot)
    //{
    //    // choose a fileName to delete based on sent file
    //    saveDataWriter = new WriteSaveData();
    //    saveDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
    //    saveDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotUsed(slot);
    //    saveDataWriter.DeleteSaveFile();
    //}
    public IEnumerator LoadWorldScene()
    {
        ////If you only want one world scene, use this
        //AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        ////if you wnat your world to have multiple scenes, use this 
        ////AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndexNumber);

        //player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

        yield return null;

    }
}
