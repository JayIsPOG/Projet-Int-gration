using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistanceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    private GameData gameData;
    private List<IDataPersistance> dataPersistancesObjects;
    private FileDataHandler dataHandler;
    public static DataPersistanceManager instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Persistance Manager in the scene");
            //Destroy(instance);
        }
        instance = this;
    }
    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistancesObjects = FindAllDataPersistanceObjects();
        LoadGame();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        this.dataPersistancesObjects = FindAllDataPersistanceObjects();
        LoadGame();
        //Debug.Log("Loaded");
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        if(this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        foreach (IDataPersistance dataPersistanceObj in dataPersistancesObjects)
        {
            dataPersistanceObj.LoadData(gameData);
        }
    }
    public void SaveGame()
    {
        this.dataPersistancesObjects = FindAllDataPersistanceObjects();
        foreach (IDataPersistance dataPersistanceObj in dataPersistancesObjects)
        {
            dataPersistanceObj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }
    //private void OnApplicationQuit()
    //{
    //    SaveGame();
    //}

    private List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();
        
        return new List<IDataPersistance>(dataPersistanceObjects);
    }
}
