using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLoader : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            if(SceneManager.GetActiveScene().name.Contains("Puzzle"))
            {
                FindObjectsByType<SceneManagerPuzzle>(FindObjectsSortMode.None)[0].CloseScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        if ((Input.GetKey(KeyCode.LeftShift)))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) SceneManager.LoadScene("Main_Menu");
            if (Input.GetKey(KeyCode.P))
            {
                if (Input.GetKey(KeyCode.Alpha1)) SceneManager.LoadScene("Puzzle 1");
                if (Input.GetKey(KeyCode.Alpha2)) SceneManager.LoadScene("Puzzle 2");
                if (Input.GetKey(KeyCode.Alpha3)) SceneManager.LoadScene("Puzzle 3");
                if (Input.GetKey(KeyCode.Alpha4)) SceneManager.LoadScene("Puzzle 4");
                if (Input.GetKey(KeyCode.Alpha5)) SceneManager.LoadScene("Puzzle 5");
                if (Input.GetKey(KeyCode.Alpha6)) SceneManager.LoadScene("Puzzle End");
            }
            if (Input.GetKey(KeyCode.F))
            {
                if (Input.GetKey(KeyCode.Alpha1)) SceneManager.LoadScene("Fight_1");
                if (Input.GetKey(KeyCode.Alpha2)) SceneManager.LoadScene("Fight_2");
                if (Input.GetKey(KeyCode.Alpha3)) SceneManager.LoadScene("Fight_3");
                if (Input.GetKey(KeyCode.Alpha4)) SceneManager.LoadScene("Fight End");
            }
            if (Input.GetKey(KeyCode.B))
            {
                if (Input.GetKey(KeyCode.Alpha1)) SceneManager.LoadScene("backgammon");
                if (Input.GetKey(KeyCode.Alpha2)) SceneManager.LoadScene("Back End");
            }
            
        }
        if (Input.GetKey(KeyCode.F4))
        {
            Application.Quit();
        }
    }
}
