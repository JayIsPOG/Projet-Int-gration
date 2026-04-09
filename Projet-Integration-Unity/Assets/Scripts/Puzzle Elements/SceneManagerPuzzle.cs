using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManagerPuzzle : MonoBehaviour
{
    public CanvasGroup menuCanvas;
    public bool sceneOpen;
    private string lvlName;
    private bool saving;
    void Start()
    {
        menuCanvas.alpha = 1f;
        OpenScene();
    }

    // Update is called once per frame
    void Update()
    {
        float step = 1 * Time.deltaTime;
        if(sceneOpen)
            menuCanvas.alpha = Mathf.MoveTowards(menuCanvas.alpha, 0f, step);
        else
        {
            if(saving == false)
            {
                saving = true;
                try{
                    FindObjectOfType<DataPersistanceManager>().SaveGame();
                }catch{
                    Debug.LogWarning("No Data Persistance Manager found in this scene");
                }
            }
            menuCanvas.alpha = Mathf.MoveTowards(menuCanvas.alpha, 1f, step);
            if(menuCanvas.alpha == 1f)
                SceneManager.LoadScene(lvlName);
        }
    }

    public void CloseScene(string lvl)
    {
        sceneOpen = false;
        lvlName = lvl;
    }

    public void OpenScene()
    {
        sceneOpen = true;
    }
}
